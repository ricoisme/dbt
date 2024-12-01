using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace DataSyncService
{
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

        internal SyncHelper SetServerProvider(SqlSyncChangeTrackingProvider serverProvider)
        {
            _serverProvider = serverProvider;
            return this;
        }

        internal SyncHelper SetClientProvider(SqlSyncChangeTrackingProvider clientProvider)
        {
            _clientProvider = clientProvider;
            return this;
        }

        internal SyncHelper SetTables(IEnumerable<string> tables)
        {
            _tables = tables;
            return this;
        }

        internal SyncHelper SetVersion(string currentVersion, string nextVersion)
        {
            _currentVersion = currentVersion;
            _nextVersion = nextVersion;
            return this;
        }

        internal SyncHelper SetSyncOption(int batchSize, int sqlCommandTimeout)
        {
            _batchSize = batchSize;
            _sqlCommandTimeout = sqlCommandTimeout;
            return this;
        }

        /// <summary>
        /// like '_%key%'
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal SyncHelper SetFilterVersion(string key)
        {
            _key = key;
            return this;
        }

        internal SyncHelper SetLogger(ILogger<Worker> logger)
        {
            _logger = logger;
            return this;
        }


        public async Task<SyncAgent> SyncProcessAsync()
        {
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
                var sopeInfo = await agent.RemoteOrchestrator.GetScopeInfoAsync(_nextVersion);
                var nextScopeInfo = await agent.LocalOrchestrator.ProvisionAsync(sopeInfo);
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

        private async Task DropOldSpsAsync(DbConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = Const.GetDropAllOldSps.Replace("$version$", _key);
            try
            {
                await connection.OpenAsync();
                var dataTable = new DataTable();
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(dataReader);
                }

                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        command.CommandText = dr[0].ToString();
                        command.ExecuteNonQuery();
                    }
                }

                await connection.CloseAsync();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
        }
    }
}
