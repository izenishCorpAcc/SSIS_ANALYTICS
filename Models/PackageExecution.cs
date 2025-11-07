namespace SSISAnalyticsDashboard.Models
{
    public class PackageExecution
    {
        public long ExecutionId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string FolderName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Duration { get; set; }
    }
}
