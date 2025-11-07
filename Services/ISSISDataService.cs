using SSISAnalyticsDashboard.Models;

namespace SSISAnalyticsDashboard.Services
{
    public interface ISSISDataService
    {
        Task<ExecutionMetrics> GetMetricsAsync();
        Task<List<ExecutionTrend>> GetTrendsAsync();
        Task<List<ErrorLog>> GetErrorsAsync();
        Task<List<PackageExecution>> GetExecutionsAsync();
    }
}
