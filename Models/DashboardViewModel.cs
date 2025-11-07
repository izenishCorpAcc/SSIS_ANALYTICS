namespace SSISAnalyticsDashboard.Models
{
    public class DashboardViewModel
    {
        public ExecutionMetrics Metrics { get; set; } = new();
        public List<ExecutionTrend> Trends { get; set; } = new();
        public List<ErrorLog> RecentErrors { get; set; } = new();
        public List<PackageExecution> RecentExecutions { get; set; } = new();
        public List<PackageExecution> LastExecutedPackages { get; set; } = new();
    }
}
