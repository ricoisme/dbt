namespace ArchivingService.Entities
{
    public sealed class ProcessState
    {
        public Int64 Id { get; set; }
        public int SourceTableId { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int RowsCopied { get; set; }
        public DateTime LastArchivedDate { get; set; }
        public int RowsPurged { get; set; }
        public DateTime LastPurgedDate { get; set; }
        public DateTime? CompleteDate { get; set; }
    }
}
