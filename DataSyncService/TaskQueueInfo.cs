using Dotmim.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncService
{
    /// <summary>
    /// 代表佇列中的同步任務資訊
    /// </summary>
    public sealed class TaskQueueInfo
    {
        /// <summary>
        /// 取得或設定任務的唯一識別碼
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 取得或設定同步代理程式執行個體
        /// </summary>
        public SyncAgent SyncAgent { get; set; }

        /// <summary>
        /// 取得或設定電子郵件地址，預設為空字串
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
