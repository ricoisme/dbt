namespace DataSyncService
{
    /// <summary>
    /// 代表資料同步代理程式的設定資訊
    /// </summary>
    public sealed class AgentInfo
    {
        /// <summary>
        /// 取得或設定代理程式的名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或設定來源資料庫的連線字串
        /// </summary>
        public string SourceConnectinoString { get; set; }

        /// <summary>
        /// 取得或設定目標資料庫的連線字串
        /// </summary>
        public string TargetConnectionString { get; set; }

        /// <summary>
        /// 取得或設定要同步的資料表清單
        /// </summary>
        public List<string> Tables { get; set; }

        /// <summary>
        /// 取得或設定目前使用的版本號
        /// </summary>
        public string CurrentVersion { get; set; }

        /// <summary>
        /// 取得或設定下一個版本號
        /// </summary>
        public string NextVersion { get; set; }

        /// <summary>
        /// 取得或設定批次處理的大小，預設為 1500
        /// </summary>
        public int BatchSize { get; set; } = 1500;

        /// <summary>
        /// 取得或設定 SQL 指令的逾時時間(秒)，預設為 30 秒
        /// </summary>
        public int SqlCommandTimeoutSec { get; set; } = 30;
    }
}
