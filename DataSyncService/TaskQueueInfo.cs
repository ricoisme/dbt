using Dotmim.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncService
{
    public sealed class TaskQueueInfo
    {
        public string Key { get; set; }
        public SyncAgent SyncAgent { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
