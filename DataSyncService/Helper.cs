using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace DataSyncService
{
    internal class Helper
    {
        private SqlSyncChangeTrackingProvider _serverProvider;
        private SqlSyncChangeTrackingProvider _clientProvider;
        private IEnumerable<string> _tables;
        private string _currentVersion;
        private string _nextVersion;
        private string _key;
        private const string _getDropAllOldSps = @"SELECT CONCAT( 'drop proc ',SPECIFIC_SCHEMA,'.',SPECIFIC_NAME,' ;') as [statement]
  FROM INFORMATION_SCHEMA.ROUTINES
 WHERE ROUTINE_TYPE = 'PROCEDURE'
 and (SPECIFIC_NAME like '%_$version$%')";
        private ILogger<Worker> _logger;

        internal Helper SetServerProvider(SqlSyncChangeTrackingProvider serverProvider)
        {
            _serverProvider = serverProvider;
            return this;
        }

        internal Helper SetClientProvider(SqlSyncChangeTrackingProvider clientProvider)
        {
            _clientProvider = clientProvider;
            return this;
        }

        internal Helper SetTables(IEnumerable<string> tables)
        {
            _tables = tables;
            return this;
        }

        internal Helper SetVersion(string currentVersion, string nextVersion)
        {
            _currentVersion = currentVersion;
            _nextVersion = nextVersion;
            return this;
        }

        /// <summary>
        /// like '_%key%'
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal Helper SetFilterVersion(string key)
        {
            _key = key;
            return this;
        }

        internal Helper SetLogger(ILogger<Worker> logger)
        {
            _logger = logger;
            return this;
        }


        public async Task<SyncAgent> SyncProcessAsync()
        {
            var syncOption = new SyncOptions { BatchSize = 1500, DbCommandTimeout = 30 };
            var agent = new SyncAgent(_clientProvider, _serverProvider, syncOption);
            agent.OnApplyChangesConflictOccured(async acfa =>
            {
                var conflict = await acfa.GetSyncConflictAsync();
                acfa.Resolution = ConflictResolution.ServerWins;
            });
            var setup = new SyncSetup(_tables);

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
            Console.WriteLine($"{_serverProvider.GetDatabaseName()} migration done.");

            // client apply new version
            var sopeInfo = await agent.RemoteOrchestrator.GetScopeInfoAsync(_nextVersion);
            var nextScopeInfo = await agent.LocalOrchestrator.ProvisionAsync(sopeInfo);
            Console.WriteLine($"{_clientProvider.GetDatabaseName()} Provision done.");

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

        public async Task DetermineDropAllAsync()
        {
            var agent = new SyncAgent(_clientProvider, _serverProvider);
            var serverOrchestrator = new RemoteOrchestrator(_serverProvider);
            var clientOrchestrator = new LocalOrchestrator(_clientProvider);

            var scopInfos = await agent.RemoteOrchestrator.GetAllScopeInfosAsync();
            if (scopInfos?.Count > 1 && string.IsNullOrEmpty(_nextVersion))
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
            command.CommandText = _getDropAllOldSps.Replace("$version$", _key);
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
