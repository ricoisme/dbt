using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using System.Threading.Channels;

namespace DataSyncService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IList<AgentInfo> _agentInfos;
        private readonly Channel<(string, SyncAgent)> _queue;
        private const int _capacity = 1024;
        private const int _delayMs = 1 * 1000;
        internal event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            UnobservedTaskException += Worker_UnobservedTaskException;
            _agentInfos = _configuration.GetSection("AgentInfos")?.Get<List<AgentInfo>>() ?? new List<AgentInfo>();

            var boundedChannelOptions = new BoundedChannelOptions(_capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<(string, SyncAgent)>(boundedChannelOptions);
        }

        private void Worker_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs ex)
        {
            _logger.LogError(ex.Exception, $"UnobservedTaskException:{ex.Exception.Message}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // ALTER DATABASE mytest  SET CHANGE_TRACKING = ON  (CHANGE_RETENTION = 7 DAYS, AUTO_CLEANUP = ON)
            foreach (var agentInfo in _agentInfos)
            {
                var serverProvider = new SqlSyncChangeTrackingProvider(agentInfo.SourceConnectinoString);
                var clientProvider = new SqlSyncChangeTrackingProvider(agentInfo.TargetConnectionString);

                var syncHelper = new Helper();
                syncHelper.SetServerProvider(serverProvider)
                    .SetClientProvider(clientProvider)
                    .SetTables(agentInfo.Tables)
                    .SetVersion(agentInfo.CurrentVersion, agentInfo.NextVersion)
                    .SetFilterVersion("v");

                //await syncHelper.DetermineDropAllAsync();

                var agent = await syncHelper.SyncProcessAsync();
                var scopeInfoClient = await agent.LocalOrchestrator
                .GetScopeInfoClientAsync(string.IsNullOrEmpty(agentInfo.NextVersion) ? agentInfo.CurrentVersion : agentInfo.NextVersion);
                _logger.LogInformation($"Client LastSync:{scopeInfoClient.LastSync}");
                await _queue.Writer.WriteAsync((agentInfo.Name, agent), stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await BackgroundProcessingAsync(stoppingToken);
                await Task.Delay(_delayMs, stoppingToken);
            }
        }

        private async Task BackgroundProcessingAsync(CancellationToken stoppingToken)
        {
            var syncAgent = await _queue.Reader.ReadAsync(stoppingToken);
            var agentInfo = _agentInfos.FirstOrDefault(c => c.Name.Equals(syncAgent.Item1, StringComparison.OrdinalIgnoreCase));
            var nextVersion = agentInfo?.NextVersion ?? "";
            var currentVersion = agentInfo?.CurrentVersion ?? "";
            if (string.IsNullOrEmpty(currentVersion)) return;

            _ = Task.Run(async () =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = await syncAgent.Item2.SynchronizeAsync(string.IsNullOrEmpty(nextVersion) ? currentVersion : nextVersion);
                        _logger.LogInformation("{Name} Result: {result}", syncAgent.Item1, result);
                        await Task.Delay(_delayMs, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during sync for {SyncAgent}.", syncAgent.Item1);
                }
            }, stoppingToken);
        }
    }
}
