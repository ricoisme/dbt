using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using System.Threading.Channels;

namespace DataSyncService
{
    /// <summary>
    /// 背景服務工作者，負責執行資料同步作業
    /// </summary>
    /// <remarks>
    /// 使用前請確保資料庫已啟用變更追蹤功能:
    /// <code language="sql">
    /// ALTER DATABASE mytest SET CHANGE_TRACKING = ON (CHANGE_RETENTION = 7 DAYS, AUTO_CLEANUP = ON)
    /// </code>
    /// </remarks>
    public class Worker : BackgroundService
    {
        // ALTER DATABASE mytest  SET CHANGE_TRACKING = ON  (CHANGE_RETENTION = 7 DAYS, AUTO_CLEANUP = ON)         

        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IList<AgentInfo> _agentInfos;
        private readonly Channel<TaskQueueInfo> _queue;
        private readonly SyncHelper _syncHelper;
        private const int _capacity = 100;
        private const int _delayMs = 1 * 1000;

        /// <summary>
        /// 當未觀察到的工作例外發生時觸發的事件
        /// </summary>
        internal event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        /// <summary>
        /// 初始化 <see cref="Worker"/> 類別的新執行個體
        /// </summary>
        /// <param name="logger">記錄器執行個體</param>
        /// <param name="configuration">設定提供者</param>
        public Worker(ILogger<Worker> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _syncHelper = new SyncHelper();
            UnobservedTaskException += Worker_UnobservedTaskException;
            _agentInfos = _configuration.GetSection("AgentInfos")?.Get<List<AgentInfo>>() ?? new List<AgentInfo>();

            var boundedChannelOptions = new BoundedChannelOptions(_capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<TaskQueueInfo>(boundedChannelOptions);
        }

        /// <summary>
        /// 處理未觀察到的工作例外事件
        /// </summary>
        /// <param name="sender">事件發送者</param>
        /// <param name="ex">包含例外資訊的事件參數</param>
        private void Worker_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs ex)
        {
            _logger.LogError(ex.Exception, $"UnobservedTaskException:{ex.Exception.Message}");
        }

        /// <summary>
        /// 執行背景服務的主要邏輯
        /// </summary>
        /// <param name="stoppingToken">用於通知服務停止的取消權杖</param>
        /// <returns>代表非同步作業的工作</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await CreateSyncAgentIntoQueueAsync(stoppingToken);

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

        /// <summary>
        /// 建立同步代理程式並將其加入到處理佇列中
        /// </summary>
        /// <param name="stoppingToken">用於通知服務停止的取消權杖</param>
        /// <returns>代表非同步作業的工作</returns>
        private async Task CreateSyncAgentIntoQueueAsync(CancellationToken stoppingToken)
        {
            foreach (var agentInfo in _agentInfos)
            {
                var serverProvider = new SqlSyncChangeTrackingProvider(agentInfo.SourceConnectinoString);
                var clientProvider = new SqlSyncChangeTrackingProvider(agentInfo.TargetConnectionString);

                _syncHelper.SetDefault()
                    .SetServerProvider(serverProvider)
                    .SetClientProvider(clientProvider)
                    .SetTables(agentInfo.Tables)
                    .SetLogger(_logger)
                    .SetVersion(agentInfo.CurrentVersion, agentInfo.NextVersion)
                    .SetSyncOption(agentInfo.BatchSize, agentInfo.SqlCommandTimeoutSec)
                    .SetFilterVersion("v");

                //await _syncHelper.DetermineDropAllAsync(true);
                //return;
                var agent = await _syncHelper.SyncProcessAsync();
                if (agent == null) continue;

                var scopeInfoClient = await agent.LocalOrchestrator
                .GetScopeInfoClientAsync(string.IsNullOrEmpty(agentInfo.NextVersion) ? agentInfo.CurrentVersion : agentInfo.NextVersion);
                _logger.LogInformation($"Client LastSync:{scopeInfoClient.LastSync}");
                await _queue.Writer.WriteAsync(new TaskQueueInfo { Key = agentInfo.Name, SyncAgent = agent }, stoppingToken);
            }
        }

        /// <summary>
        /// 處理背景同步作業
        /// </summary>
        /// <param name="stoppingToken">用於通知服務停止的取消權杖</param>
        /// <returns>代表非同步作業的工作</returns>
        /// <remarks>
        /// 從佇列讀取任務並在背景執行同步作業，持續執行直到取消權杖被觸發
        /// </remarks>
        private async Task BackgroundProcessingAsync(CancellationToken stoppingToken)
        {
            var taskQueueInfo = await _queue.Reader.ReadAsync(stoppingToken);
            if (taskQueueInfo == null) return;

            var agentInfo = _agentInfos.FirstOrDefault(c => c.Name.Equals(taskQueueInfo.Key, StringComparison.OrdinalIgnoreCase));
            var nextVersion = agentInfo?.NextVersion ?? "";
            var currentVersion = agentInfo?.CurrentVersion ?? "";
            if (agentInfo == null || string.IsNullOrEmpty(currentVersion)) return;

            _ = Task.Run(async () =>
            {
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = await taskQueueInfo.SyncAgent.SynchronizeAsync(string.IsNullOrEmpty(nextVersion) ? currentVersion : nextVersion);
                        _logger.LogInformation("{Name} Result: {result}", taskQueueInfo.Key, result);
                        await Task.Delay(_delayMs, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during sync for {SyncAgent}.", taskQueueInfo.Key);
                }
            }, stoppingToken);
        }
    }
}
