namespace DataSyncService
{
    public sealed class AgentInfo
    {
        public string Name { get; set; }
        public string SourceConnectinoString { get; set; }
        public string TargetConnectionString { get; set; }
        public List<string> Tables { get; set; }
        public string CurrentVersion { get; set; }
        public string NextVersion { get; set; }
        public int BatchSize { get; set; } = 1500;
        public int SqlCommandTimeoutSec { get; set; } = 30;
    }
}
