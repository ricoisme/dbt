using Dotmim.Sync.SqlServer;

namespace DataSyncService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private const string _mydbConnectionString = "server=127.0.0.1,1533;database=mytest;user id=sa;password=Sa12345678;Encrypt=false;";
        private const string _myReportConnectionString = "server=127.0.0.1,1533;database=mytest_Report;user id=sa;password=Sa12345678;Encrypt=false;";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // ALTER DATABASE mytest  SET CHANGE_TRACKING = ON  (CHANGE_RETENTION = 7 DAYS, AUTO_CLEANUP = ON)
            var serverProvider = new SqlSyncChangeTrackingProvider(_mydbConnectionString);
            var clientProvider = new SqlSyncChangeTrackingProvider(_myReportConnectionString);

            var currentVersion = "v0";
            var nextVersion = "";
            var tables = new[] { "dbo.Employee", "dbo.Products" };

            var syncHelper = new Helper();
            syncHelper.SetServerProvider(serverProvider)
                .SetClientProvider(clientProvider)
                .SetTables(tables)
                .SetVersion(currentVersion, nextVersion)
                .SetFilterVersion("v");
            await syncHelper.DetermineDropAllAsync();

            var agent = await syncHelper.SyncProcessAsync();

            var scopeInfoClient = await agent.LocalOrchestrator
             .GetScopeInfoClientAsync(string.IsNullOrEmpty(nextVersion) ? currentVersion : nextVersion);
            Console.WriteLine($"Client LastSync:{scopeInfoClient.LastSync}");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var result = await agent.SynchronizeAsync(string.IsNullOrEmpty(nextVersion) ? currentVersion : nextVersion);
                Console.WriteLine(result);
                await Task.Delay(1 * 1000, stoppingToken);
            }
        }
    }
}
