namespace SSISAnalyticsDashboard.Models
{
    public class ExecutionTrend
    {
        public string Date { get; set; } = string.Empty;
        public int Success { get; set; }
        public int Failed { get; set; }
        public decimal AvgDuration { get; set; }
    }
}
