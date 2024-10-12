using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchivingService.Entities
{
    public sealed class SourceTable
    {
        public int Id { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public bool Active { get; set; } = true;
        public int DataCopyBatchSize { get; set; } = 1000;
        public string KeyQuery { get; set; }
        public bool Archive { get; set; } = true;
        public bool Purge { get; set; } = true;
    }
}
