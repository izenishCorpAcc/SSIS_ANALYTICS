namespace SSISAnalyticsDashboard.Models
{
    public class ExecutionMetrics
    {
        public int TotalExecutions { get; set; }
        public int SuccessfulExecutions { get; set; }
        public int FailedExecutions { get; set; }
        public decimal SuccessRate { get; set; }
        public decimal AvgDuration { get; set; }
    }
}
