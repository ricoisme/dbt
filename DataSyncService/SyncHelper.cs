using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace DataSyncService
{
    /// <summary>
    /// 提供資料庫同步作業的輔助類別
    /// </summary>
    internal sealed class SyncHelper
    {
        private SqlSyncChangeTrackingProvider _serverProvider;
        private SqlSyncChangeTrackingProvider _clientProvider;
        private IEnumerable<string> _tables;
        private ILogger<Worker> _logger;
        private string _currentVersion;
        private string _nextVersion;
        private string _key;
        private int _batchSize;
        private int _sqlCommandTimeout;

        /// <summary>
        /// 將所有設定重設為預設值
        /// </summary>
        /// <returns>目前的 <see cref="SyncHelper"/> 執行個體，以支援鏈式呼叫</returns>
        internal SyncHelper SetDefault()
        {
            _serverProvider = null;
            _clientProvider = null;
            _tables = null;
            _logger = null;
            _currentVersion = string.Empty;
            _nextVersion = string.Empty;
            _key = string.Empty;
            _batchSize = 1500;
            _sqlCommandTimeout = 60;
            return this;
        }

        /// <summary>
        /// 設定伺服器端的同步提供者
        /// </summary>
        /// <param name="serverProvider">伺服器端的 SQL 同步變更追蹤提供者</param>
        /// <returns>目前的 <see cref="SyncHelper"/> 執行個體，以支援鏈式呼叫</returns>
        internal SyncHelper SetServerProvider(SqlSyncChangeTrackingProvider serverProvider)
        {
            ArgumentNullException.ThrowIfNull(serverProvider, nameof(serverProvider));
            _serverProvider = serverProvider;
            return this;
        }

        /// <summary>
        /// 設定用戶端的同步提供者
        /// </summary>
        /// <param name="clientProvider">用戶端的 SQL 同步變更追蹤提供者</param>
        /// <returns>目前的 <see cref="SyncHelper"/> 執行個體，以支援鏈式呼叫</returns>
        internal SyncHelper SetClientProvider(SqlSyncChangeTrackingProvider clientProvider)
        {
            ArgumentNullException.ThrowIfNull(clientProvider, nameof(clientProvider));
            _clientProvider = clientProvider;
            return this;
        }

        /// <summary>
        /// 設定要同步的資料表集合
        /// </summary>
        /// <param name="tables">資料表名稱的集合</param>
        /// <returns>目前的 <see cref="SyncHelper"/> 執行個體，以支援鏈式呼叫</returns>
        internal SyncHelper SetTables(IEnumerable<string> tables)
        {
            ArgumentNullException.ThrowIfNull(tables, nameof(tables));
            _tables = tables;
            return this;
        }

        /// <summary>
        /// 設定目前版本和下一個版本的識別字串
        /// </summary>
        /// <param name="currentVersion">目前使用的版本</param>
        /// <param name="nextVersion">下一個版本</param>
        /// <returns>目前的 <see cref="SyncHelper"/> 執行個體，以支援鏈式呼叫</returns>
        internal SyncHelper SetVersion(string currentVersion, string nextVersion)
        {
            _currentVersion = currentVersion;
            _nextVersion = nextVersion;
            return this;
        }

        /// <summary>
        /// 設定同步作業的選項
        /// </summary>
        /// <param name="batchSize">批次大小</param>
        /// <param name="sqlCommandTimeout">SQL 指令逾時時間(秒)</param>
        /// <returns>目前的 <see cref="SyncHelper"/> 執行個體，以支援鏈式呼叫</returns>
        internal SyncHelper SetSyncOption(int batchSize, int sqlCommandTimeout)
        {
            _batchSize = batchSize;
            _sqlCommandTimeout = sqlCommandTimeout;
            return this;
        }

        /// <summary>
        /// 設定版本篩選的關鍵字
        /// </summary>
        /// <param name="key">篩選關鍵字，用於模糊比對版本名稱 (例如: '_%<paramref name="key"/>%')</param>
        /// <returns>目前的 <see cref="SyncHelper"/> 執行個體，以支援鏈式呼叫</returns>
        internal SyncHelper SetFilterVersion(string key)
        {
            _key = key;
            return this;
        }

        /// <summary>
        /// 設定記錄器執行個體
        /// </summary>
        /// <param name="logger">記錄器執行個體</param>
        /// <returns>目前的 <see cref="SyncHelper"/> 執行個體，以支援鏈式呼叫</returns>
        internal SyncHelper SetLogger(ILogger<Worker> logger)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            _logger = logger;
            return this;
        }

        /// <summary>
        /// 執行非同步的資料同步程序
        /// </summary>
        /// <returns>包含 <see cref="SyncAgent"/> 的非同步作業。如果發生錯誤，則傳回 <see langword="null"/></returns>
        /// <remarks>
        /// 此方法會設定同步代理程式、處理衝突解決、並在必要時進行版本遷移
        /// </remarks>
        public async Task<SyncAgent> SyncProcessAsync()
        {            
            ArgumentNullException.ThrowIfNull(_serverProvider, nameof(_serverProvider));
            ArgumentNullException.ThrowIfNull(_clientProvider, nameof(_clientProvider));
            ArgumentNullException.ThrowIfNull(_tables, nameof(_tables));
            ArgumentNullException.ThrowIfNull(_logger, nameof(_logger));
            if (string.IsNullOrEmpty(_currentVersion))
                throw new InvalidOperationException("Current version must be set before synchronization.");
            var syncOption = new SyncOptions { BatchSize = _batchSize, DbCommandTimeout = _sqlCommandTimeout };
            var agent = new SyncAgent(_clientProvider, _serverProvider, syncOption);
            agent.OnApplyChangesConflictOccured(async acfa =>
            {
                var conflict = await acfa.GetSyncConflictAsync();
                acfa.Resolution = ConflictResolution.ServerWins;
            });

            try
            {
                var setup = new SyncSetup(_tables);
                foreach (var table in setup.Tables)
                {
                    table.SyncDirection = SyncDirection.DownloadOnly;
                }

                //setup the sync (triggers / tracking tables ...)
                await agent.SynchronizeAsync(_currentVersion, setup);

                //determine if execution is necessary
                if (string.IsNullOrEmpty(_nextVersion)) return agent;
                var scopInfo = await agent.RemoteOrchestrator
                    .GetScopeInfoAsync(_nextVersion);
                if (scopInfo.Schema?.HasRows == true) return agent;

                // server apply new version
                var remoteOrchestrator = new RemoteOrchestrator(_serverProvider);
                await remoteOrchestrator.ProvisionAsync(_nextVersion, setup);
                _logger.LogInformation($"{_serverProvider.GetDatabaseName()} migration done.");

                // client apply new version
                var scopeInfo = await agent.RemoteOrchestrator.GetScopeInfoAsync(_nextVersion);
                var nextScopeInfo = await agent.LocalOrchestrator.ProvisionAsync(scopeInfo);
                _logger.LogInformation($"{_clientProvider.GetDatabaseName()} Provision done.");

                // copy sync information into new version scope from last point
                var nextScopeInfoClient = await agent.LocalOrchestrator
                    .GetScopeInfoClientAsync(_nextVersion);
                var currentScopeInfoClient = await agent.LocalOrchestrator
                    .GetScopeInfoClientAsync(_currentVersion);
                nextScopeInfoClient.ShadowScope(currentScopeInfoClient);
                var saveResult = await agent.LocalOrchestrator
                    .SaveScopeInfoClientAsync(nextScopeInfoClient);
                var result = await agent.SynchronizeAsync(_nextVersion);

                return agent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 判斷並執行清除所有同步相關物件的作業
        /// </summary>
        /// <param name="force">是否強制執行清除作業。預設為 <see langword="false"/></param>
        /// <returns>代表非同步作業的工作</returns>
        /// <remarks>
        /// 此方法會清除同步範圍資訊、預存程序、追蹤資料表和觸發程序等物件。
        /// 當存在多個範圍且沒有指定下一個版本時，或是強制執行時，才會進行清除作業
        /// </remarks>
        public async Task DetermineDropAllAsync(bool force = false)
        {
            var agent = new SyncAgent(_clientProvider, _serverProvider);
            var serverOrchestrator = new RemoteOrchestrator(_serverProvider);
            var clientOrchestrator = new LocalOrchestrator(_clientProvider);

            var scopInfos = await agent.RemoteOrchestrator.GetAllScopeInfosAsync();
            if (force || scopInfos?.Count > 1 && string.IsNullOrEmpty(_nextVersion))
            {
                //await agent.LocalOrchestrator.DropAllAsync();
                //await agent.RemoteOrchestrator.DropAllAsync();

                var syncProvision = SyncProvision.ScopeInfo | SyncProvision.ScopeInfoClient |
                    SyncProvision.StoredProcedures | SyncProvision.TrackingTable |
                    SyncProvision.Triggers;
                await serverOrchestrator.DeprovisionAsync(syncProvision);
                await clientOrchestrator.DeprovisionAsync(syncProvision);

                if (!string.IsNullOrEmpty(_key))
                {
                    await DropOldSpsAsync(_serverProvider.CreateConnection());
                    await DropOldSpsAsync(_clientProvider.CreateConnection());
                }
                _logger.LogInformation("DropAll done.");
            }
        }

        /// <summary>
        /// 非同步刪除舊版本的預存程序
        /// </summary>
        /// <param name="connection">資料庫連線</param>
        /// <returns>代表非同步作業的工作</returns>
        /// <exception cref="SqlException">當 SQL 執行發生錯誤時擲回</exception>
        /// <remarks>
        /// 此方法會查詢並刪除所有符合版本命名模式的預存程序
        /// </remarks>
        private async Task DropOldSpsAsync(DbConnection connection)
        {
            ArgumentNullException.ThrowIfNull(connection, nameof(connection));

            await using (connection)
            {
                await using var command = connection.CreateCommand();
                command.CommandText = Const.GetDropAllOldSps.Replace("$version$", _key);
                
                try
                {
                    await connection.OpenAsync();
                    var dataTable = new DataTable();
                    await using (var dataReader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(dataReader);
                    }

                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            command.CommandText = dr[0].ToString() ?? string.Empty;
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    _logger.LogError(ex, "刪除舊版本預存程序時發生錯誤: {Message}", ex.Message);
                    throw;
                }
            }
        }
    }
}
