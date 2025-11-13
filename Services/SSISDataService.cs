using Microsoft.Data.SqlClient;
using SSISAnalyticsDashboard.Models;
using SSISAnalyticsDashboard.Helpers;
using System.Data;
using Microsoft.Extensions.Caching.Memory;

namespace SSISAnalyticsDashboard.Services
{
    public class SSISDataService : ISSISDataService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SSISDataService> _logger;
        private readonly IMemoryCache _cache;
        private const int CACHE_DURATION_SECONDS = 30; // Cache for 30 seconds

        public SSISDataService(IHttpContextAccessor httpContextAccessor, ILogger<SSISDataService> logger, IMemoryCache cache)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _cache = cache;
        }

        private string GetConnectionString()
        {
            // Try to get connection string from session first
            var sessionConnectionString = _httpContextAccessor.HttpContext?.Session?.GetString("SSISDBConnection");
            if (!string.IsNullOrEmpty(sessionConnectionString))
            {
                return sessionConnectionString;
            }
            
            throw new InvalidOperationException("Connection string not found in session. Please configure the server first.");
        }

        public async Task<ExecutionMetrics> GetMetricsAsync(string? businessUnit = null)
        {
            var cacheKey = $"Metrics_{businessUnit ?? "ALL"}";
            
            // Try to get from cache first
            if (_cache.TryGetValue(cacheKey, out ExecutionMetrics? cachedMetrics) && cachedMetrics != null)
            {
                _logger.LogInformation($"✅ Metrics retrieved from cache for: {businessUnit ?? "ALL"}");
                return cachedMetrics;
            }
            
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);
                
                _logger.LogInformation($"GetMetricsAsync - BusinessUnit: {businessUnit ?? "NULL"}, Filter: {businessUnitFilter}");

                var query = $@"
                    SELECT 
                        COUNT(*) as TotalExecutions,
                        SUM(CASE WHEN status = 4 THEN 1 ELSE 0 END) as FailedExecutions,
                        SUM(CASE WHEN status = 7 THEN 1 ELSE 0 END) as SuccessfulExecutions,
                        AVG(DATEDIFF(SECOND, start_time, end_time)) as AvgDuration
                    FROM [SSISDB].[catalog].[executions] e WITH (NOLOCK)
                    WHERE start_time >= DATEADD(day, -30, GETDATE())
                    {businessUnitFilter}";
                    
                _logger.LogInformation($"Executing query: {query}");

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30; // 30 second timeout
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    // Use Convert.ToInt32 for aggregate functions which may return different types
                    var totalExecutions = Convert.ToInt32(reader.GetValue(0));
                    var failedExecutions = Convert.ToInt32(reader.GetValue(1));
                    var successfulExecutions = Convert.ToInt32(reader.GetValue(2));
                    var avgDuration = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3));

                    var successRate = totalExecutions > 0 
                        ? (decimal)successfulExecutions / totalExecutions * 100 
                        : 0;

                    var metrics = new ExecutionMetrics
                    {
                        TotalExecutions = totalExecutions,
                        SuccessfulExecutions = successfulExecutions,
                        FailedExecutions = failedExecutions,
                        SuccessRate = Math.Round(successRate, 2),
                        AvgDuration = avgDuration
                    };
                    
                    // Cache the result
                    _cache.Set(cacheKey, metrics, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                    _logger.LogInformation($"💾 Metrics cached for: {businessUnit ?? "ALL"}");
                    
                    return metrics;
                }

                return new ExecutionMetrics();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching metrics");
                throw;
            }
        }

        public async Task<List<ExecutionTrend>> GetTrendsAsync(string? businessUnit = null)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);

                var query = $@"
                    SELECT 
                        CAST(start_time AS DATE) as Date,
                        SUM(CASE WHEN status = 7 THEN 1 ELSE 0 END) as Success,
                        SUM(CASE WHEN status = 4 THEN 1 ELSE 0 END) as Failed,
                        AVG(DATEDIFF(SECOND, start_time, end_time)) as AvgDuration
                    FROM [SSISDB].[catalog].[executions] e
                    WHERE start_time >= DATEADD(day, -30, GETDATE())
                    {businessUnitFilter}
                    GROUP BY CAST(start_time AS DATE)
                    ORDER BY Date DESC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                var trends = new List<ExecutionTrend>();
                while (await reader.ReadAsync())
                {
                    trends.Add(new ExecutionTrend
                    {
                        Date = reader.GetDateTime(0).ToString("yyyy-MM-dd"),
                        Success = Convert.ToInt32(reader.GetValue(1)),
                        Failed = Convert.ToInt32(reader.GetValue(2)),
                        AvgDuration = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3))
                    });
                }

                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching trends");
                throw;
            }
        }

        public async Task<List<ErrorLog>> GetErrorsAsync(string? businessUnit = null)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);

                var query = $@"
                    SELECT TOP 50
                        e.execution_id,
                        e.package_name,
                        em.message_time,
                        em.event_message_id,
                        em.message
                    FROM [SSISDB].[catalog].[executions] e
                    INNER JOIN [SSISDB].[catalog].[event_messages] em 
                        ON e.execution_id = em.operation_id
                    WHERE em.message_type = 120  -- Error messages
                        AND e.start_time >= DATEADD(day, -30, GETDATE())
                        AND e.status = 4  -- Failed executions
                        {businessUnitFilter}
                    ORDER BY em.message_time DESC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                var errors = new List<ErrorLog>();
                while (await reader.ReadAsync())
                {
                    errors.Add(new ErrorLog
                    {
                        ExecutionId = reader.GetInt64(0),
                        PackageName = reader.GetString(1),
                        ErrorTime = reader.GetDateTimeOffset(2).DateTime,
                        ErrorCode = reader.GetInt32(3),
                        ErrorDescription = reader.GetString(4)
                    });
                }

                return errors;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching errors");
                throw;
            }
        }

        public async Task<List<PackageExecution>> GetExecutionsAsync(string? businessUnit = null)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);

                var query = $@"
                    SELECT TOP 50
                        e.execution_id,
                        e.package_name,
                        e.folder_name,
                        e.project_name,
                        CASE 
                            WHEN e.status = 1 THEN 'Created'
                            WHEN e.status = 2 THEN 'Running'
                            WHEN e.status = 3 THEN 'Canceled'
                            WHEN e.status = 4 THEN 'Failed'
                            WHEN e.status = 5 THEN 'Pending'
                            WHEN e.status = 6 THEN 'Ended Unexpectedly'
                            WHEN e.status = 7 THEN 'Succeeded'
                            WHEN e.status = 8 THEN 'Stopping'
                            WHEN e.status = 9 THEN 'Completed'
                            ELSE 'Unknown'
                        END as Status,
                        e.start_time,
                        e.end_time,
                        DATEDIFF(SECOND, e.start_time, e.end_time) as Duration
                    FROM [SSISDB].[catalog].[executions] e
                    WHERE e.start_time >= DATEADD(day, -30, GETDATE())
                    {businessUnitFilter}
                    ORDER BY e.start_time DESC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                var executions = new List<PackageExecution>();
                while (await reader.ReadAsync())
                {
                    executions.Add(new PackageExecution
                    {
                        ExecutionId = reader.GetInt64(0),
                        PackageName = reader.GetString(1),
                        FolderName = reader.GetString(2),
                        ProjectName = reader.GetString(3),
                        Status = reader.GetString(4),
                        StartTime = reader.GetDateTimeOffset(5).DateTime,
                        EndTime = reader.IsDBNull(6) ? null : reader.GetDateTimeOffset(6).DateTime,
                        Duration = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader.GetValue(7))
                    });
                }

                return executions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching executions");
                throw;
            }
        }

        public async Task<List<PackageExecution>> GetLastExecutedPackagesAsync(int count = 10, string? businessUnit = null)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);

                var query = $@"
                    SELECT TOP {count}
                        e.execution_id,
                        e.package_name,
                        e.folder_name,
                        e.project_name,
                        CASE 
                            WHEN e.status = 1 THEN 'Created'
                            WHEN e.status = 2 THEN 'Running'
                            WHEN e.status = 3 THEN 'Canceled'
                            WHEN e.status = 4 THEN 'Failed'
                            WHEN e.status = 5 THEN 'Pending'
                            WHEN e.status = 6 THEN 'Ended Unexpectedly'
                            WHEN e.status = 7 THEN 'Succeeded'
                            WHEN e.status = 8 THEN 'Stopping'
                            WHEN e.status = 9 THEN 'Completed'
                            ELSE 'Unknown'
                        END as Status,
                        e.start_time,
                        e.end_time,
                        DATEDIFF(SECOND, e.start_time, e.end_time) as Duration
                    FROM [SSISDB].[catalog].[executions] e
                    WHERE 1=1
                    {businessUnitFilter}
                    ORDER BY e.start_time DESC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                var executions = new List<PackageExecution>();
                while (await reader.ReadAsync())
                {
                    executions.Add(new PackageExecution
                    {
                        ExecutionId = reader.GetInt64(0),
                        PackageName = reader.GetString(1),
                        FolderName = reader.GetString(2),
                        ProjectName = reader.GetString(3),
                        Status = reader.GetString(4),
                        StartTime = reader.GetDateTimeOffset(5).DateTime,
                        EndTime = reader.IsDBNull(6) ? null : reader.GetDateTimeOffset(6).DateTime,
                        Duration = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader.GetValue(7))
                    });
                }

                return executions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching last executed packages");
                throw;
            }
        }

        public async Task<List<CurrentExecution>> GetCurrentExecutionsAsync(string? businessUnit = null)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);

                var query = $@"
                    SELECT 
                        e.execution_id,
                        e.package_name,
                        e.start_time,
                        DATEDIFF(SECOND, e.start_time, GETDATE()) as duration_seconds,
                        e.status,
                        CASE e.status
                            WHEN 1 THEN 'Created'
                            WHEN 2 THEN 'Running'
                            WHEN 3 THEN 'Canceled'
                            WHEN 4 THEN 'Failed'
                            WHEN 5 THEN 'Pending'
                            WHEN 6 THEN 'Ended Unexpectedly'
                            WHEN 7 THEN 'Succeeded'
                            WHEN 8 THEN 'Stopping'
                            WHEN 9 THEN 'Completed'
                            ELSE 'Unknown'
                        END as status_description,
                        e.executed_as_name
                    FROM [SSISDB].[catalog].[executions] e
                    WHERE e.status IN (1, 2, 5, 8)  -- Created, Running, Pending, Stopping
                    {businessUnitFilter}
                    ORDER BY e.start_time DESC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                var currentExecutions = new List<CurrentExecution>();
                while (await reader.ReadAsync())
                {
                    var durationSeconds = Convert.ToInt32(reader.GetValue(3));
                    currentExecutions.Add(new CurrentExecution
                    {
                        ExecutionId = reader.GetInt64(0),
                        PackageName = reader.GetString(1),
                        StartTime = reader.GetDateTime(2),
                        DurationSeconds = durationSeconds,
                        Status = reader.GetInt32(4).ToString(),
                        StatusDescription = reader.GetString(5),
                        ExecutedBy = reader.IsDBNull(6) ? "N/A" : reader.GetString(6),
                        IsLongRunning = durationSeconds > 1800 // 30 minutes
                    });
                }

                return currentExecutions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current executions");
                throw;
            }
        }

        public async Task<List<PackagePerformance>> GetPackagePerformanceStatsAsync(int days = 30, string? businessUnit = null)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);

                var query = $@"
                    SELECT 
                        e.package_name,
                        COUNT(*) as total_executions,
                        SUM(CASE WHEN e.status = 7 THEN 1 ELSE 0 END) as successful_executions,
                        SUM(CASE WHEN e.status = 4 THEN 1 ELSE 0 END) as failed_executions,
                        CAST(SUM(CASE WHEN e.status = 7 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) as success_rate,
                        AVG(DATEDIFF(SECOND, e.start_time, e.end_time)) as avg_duration,
                        MIN(DATEDIFF(SECOND, e.start_time, e.end_time)) as min_duration,
                        MAX(DATEDIFF(SECOND, e.start_time, e.end_time)) as max_duration,
                        MAX(e.start_time) as last_execution_time,
                        (SELECT TOP 1 status FROM [SSISDB].[catalog].[executions] 
                         WHERE package_name = e.package_name 
                         ORDER BY start_time DESC) as last_status
                    FROM [SSISDB].[catalog].[executions] e
                    WHERE e.start_time >= DATEADD(day, -@Days, GETDATE())
                    {businessUnitFilter}
                    GROUP BY e.package_name
                    ORDER BY total_executions DESC";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Days", days);
                using var reader = await command.ExecuteReaderAsync();

                var packagePerformance = new List<PackagePerformance>();
                while (await reader.ReadAsync())
                {
                    packagePerformance.Add(new PackagePerformance
                    {
                        PackageName = reader.GetString(0),
                        TotalExecutions = Convert.ToInt32(reader.GetValue(1)),
                        SuccessfulExecutions = Convert.ToInt32(reader.GetValue(2)),
                        FailedExecutions = Convert.ToInt32(reader.GetValue(3)),
                        SuccessRate = reader.GetDecimal(4),
                        AvgDurationSeconds = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                        MinDurationSeconds = reader.IsDBNull(6) ? 0 : Convert.ToInt32(reader.GetValue(6)),
                        MaxDurationSeconds = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader.GetValue(7)),
                        LastExecutionTime = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                        LastExecutionStatus = Convert.ToInt32(reader.GetValue(9)) == 7 ? "Success" : "Failed"
                    });
                }

                return packagePerformance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching package performance stats");
                throw;
            }
        }

        public async Task<List<FailurePattern>> GetFailurePatternsAsync(int days = 30, string? businessUnit = null)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);

                var query = $@"
                    SELECT 
                        e.package_name,
                        COUNT(*) as failure_count,
                        (SELECT TOP 1 em.message 
                         FROM [SSISDB].[catalog].[event_messages] em
                         WHERE em.operation_id = e.execution_id 
                         AND em.message_type = 120
                         ORDER BY em.message_time DESC) as most_common_error,
                        MAX(e.end_time) as last_failure_time,
                        CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM [SSISDB].[catalog].[executions] 
                                                   WHERE package_name = e.package_name 
                                                   AND start_time >= DATEADD(day, -@Days, GETDATE())) AS DECIMAL(5,2)) as failure_rate
                    FROM [SSISDB].[catalog].[executions] e
                    WHERE e.status = 4  -- Failed
                    AND e.start_time >= DATEADD(day, -@Days, GETDATE())
                    {businessUnitFilter}
                    GROUP BY e.package_name
                    HAVING COUNT(*) > 0
                    ORDER BY failure_count DESC";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Days", days);
                using var reader = await command.ExecuteReaderAsync();

                var failurePatterns = new List<FailurePattern>();
                while (await reader.ReadAsync())
                {
                    failurePatterns.Add(new FailurePattern
                    {
                        PackageName = reader.GetString(0),
                        FailureCount = Convert.ToInt32(reader.GetValue(1)),
                        MostCommonError = reader.IsDBNull(2) ? "N/A" : reader.GetString(2),
                        LastFailureTime = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                        FailureRate = reader.GetDecimal(4)
                    });
                }

                return failurePatterns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching failure patterns");
                throw;
            }
        }

        public async Task<List<ExecutionTimeline>> GetExecutionTimelineAsync(int hours = 24, string? businessUnit = null)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var businessUnitFilter = BusinessUnitHelper.GetBusinessUnitWhereClause(businessUnit);

                var query = $@"
                    SELECT 
                        e.execution_id,
                        e.package_name,
                        e.start_time,
                        e.end_time,
                        DATEDIFF(MINUTE, e.start_time, ISNULL(e.end_time, GETDATE())) as duration_minutes,
                        e.status,
                        CASE e.status
                            WHEN 7 THEN 'success'
                            WHEN 4 THEN 'danger'
                            WHEN 2 THEN 'primary'
                            WHEN 3 THEN 'warning'
                            ELSE 'secondary'
                        END as status_color
                    FROM [SSISDB].[catalog].[executions] e
                    WHERE e.start_time >= DATEADD(hour, -@Hours, GETDATE())
                    {businessUnitFilter}
                    ORDER BY e.start_time DESC";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Hours", hours);
                using var reader = await command.ExecuteReaderAsync();

                var timeline = new List<ExecutionTimeline>();
                while (await reader.ReadAsync())
                {
                    var status = reader.GetInt32(5);
                    var statusText = status switch
                    {
                        1 => "Created",
                        2 => "Running",
                        3 => "Canceled",
                        4 => "Failed",
                        5 => "Pending",
                        6 => "Ended Unexpectedly",
                        7 => "Succeeded",
                        8 => "Stopping",
                        9 => "Completed",
                        _ => "Unknown"
                    };

                    timeline.Add(new ExecutionTimeline
                    {
                        ExecutionId = reader.GetInt64(0),
                        PackageName = reader.GetString(1),
                        StartTime = reader.GetDateTime(2),
                        EndTime = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                        DurationMinutes = Convert.ToInt32(reader.GetValue(4)),
                        Status = statusText,
                        StatusColor = reader.GetString(6)
                    });
                }

                return timeline;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching execution timeline");
                throw;
            }
        }

        // ========== Advanced Analytics Methods ==========

        public async Task<List<PackageReliability>> GetPackageReliabilityScoresAsync()
        {
            var cacheKey = "package_reliability";
            if (_cache.TryGetValue(cacheKey, out List<PackageReliability>? cachedData) && cachedData != null)
            {
                Console.WriteLine("✅ Package Reliability retrieved from cache");
                return cachedData;
            }

            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    WITH ReliabilityData AS (
                        SELECT 
                            e.package_name,
                            COUNT(*) as total_executions,
                            SUM(CASE WHEN e.status = 7 THEN 1 ELSE 0 END) as successful_executions,
                            SUM(CASE WHEN e.status = 4 THEN 1 ELSE 0 END) as failed_executions,
                            CAST(SUM(CASE WHEN e.status = 7 THEN 1.0 ELSE 0.0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) as success_rate
                        FROM [SSISDB].[catalog].[executions] e WITH (NOLOCK)
                        WHERE e.start_time >= DATEADD(day, -30, GETDATE())
                        GROUP BY e.package_name
                        HAVING COUNT(*) >= 5
                    ),
                    TrendData AS (
                        SELECT 
                            e.package_name,
                            CAST(SUM(CASE WHEN e.status = 7 AND e.start_time >= DATEADD(day, -7, GETDATE()) THEN 1.0 ELSE 0.0 END) * 100.0 / 
                                 NULLIF(COUNT(CASE WHEN e.start_time >= DATEADD(day, -7, GETDATE()) THEN 1 END), 0) AS DECIMAL(5,2)) as recent_success_rate
                        FROM [SSISDB].[catalog].[executions] e WITH (NOLOCK)
                        WHERE e.start_time >= DATEADD(day, -30, GETDATE())
                        GROUP BY e.package_name
                    )
                    SELECT 
                        r.package_name,
                        r.total_executions,
                        r.successful_executions,
                        r.failed_executions,
                        r.success_rate,
                        CAST((r.success_rate + ISNULL(t.recent_success_rate, 0)) / 2 AS DECIMAL(5,2)) as reliability_score,
                        CASE 
                            WHEN ISNULL(t.recent_success_rate, 0) > r.success_rate THEN 'Improving'
                            WHEN ISNULL(t.recent_success_rate, 0) < r.success_rate THEN 'Declining'
                            ELSE 'Stable'
                        END as trend
                    FROM ReliabilityData r
                    LEFT JOIN TrendData t ON r.package_name = t.package_name
                    ORDER BY reliability_score DESC, total_executions DESC";

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<PackageReliability>();
                while (await reader.ReadAsync())
                {
                    results.Add(new PackageReliability
                    {
                        PackageName = reader.GetString(0),
                        TotalExecutions = Convert.ToInt32(reader.GetValue(1)),
                        SuccessfulExecutions = Convert.ToInt32(reader.GetValue(2)),
                        FailedExecutions = Convert.ToInt32(reader.GetValue(3)),
                        SuccessRate = reader.GetDecimal(4),
                        ReliabilityScore = reader.GetDecimal(5),
                        Trend = reader.GetString(6)
                    });
                }

                _cache.Set(cacheKey, results, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                Console.WriteLine($"💾 Package Reliability cached ({results.Count} packages)");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching package reliability scores");
                throw;
            }
        }

        public async Task<List<MTBFAnalysis>> GetMTBFAnalysisAsync()
        {
            var cacheKey = "mtbf_analysis";
            if (_cache.TryGetValue(cacheKey, out List<MTBFAnalysis>? cachedData) && cachedData != null)
            {
                Console.WriteLine("✅ MTBF Analysis retrieved from cache");
                return cachedData;
            }

            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    WITH FailureData AS (
                        SELECT 
                            package_name,
                            start_time,
                            LAG(start_time) OVER (PARTITION BY package_name ORDER BY start_time) as prev_failure_time
                        FROM [SSISDB].[catalog].[executions] WITH (NOLOCK)
                        WHERE status = 4 AND start_time >= DATEADD(day, -30, GETDATE())
                    ),
                    MTBFCalc AS (
                        SELECT 
                            package_name,
                            COUNT(*) as failure_count,
                            AVG(DATEDIFF(HOUR, prev_failure_time, start_time)) as mtbf_hours,
                            MAX(start_time) as last_failure
                        FROM FailureData
                        WHERE prev_failure_time IS NOT NULL
                        GROUP BY package_name
                    ),
                    TotalUptime AS (
                        SELECT 
                            package_name,
                            COUNT(*) as total_executions
                        FROM [SSISDB].[catalog].[executions] WITH (NOLOCK)
                        WHERE start_time >= DATEADD(day, -30, GETDATE())
                        GROUP BY package_name
                    )
                    SELECT 
                        ISNULL(m.package_name, t.package_name) as package_name,
                        ISNULL(m.mtbf_hours, 720) as mtbf_hours,
                        CAST((CAST(t.total_executions - ISNULL(m.failure_count, 0) AS FLOAT) * 100.0 / t.total_executions) AS DECIMAL(5,2)) as availability_pct,
                        ISNULL(m.failure_count, 0) as failure_count,
                        m.last_failure,
                        CASE 
                            WHEN ISNULL(m.mtbf_hours, 720) >= 168 THEN 'Excellent'
                            WHEN ISNULL(m.mtbf_hours, 720) >= 72 THEN 'Good'
                            WHEN ISNULL(m.mtbf_hours, 720) >= 24 THEN 'Fair'
                            ELSE 'Poor'
                        END as status
                    FROM TotalUptime t
                    LEFT JOIN MTBFCalc m ON t.package_name = m.package_name
                    WHERE t.total_executions >= 5
                    ORDER BY mtbf_hours DESC";

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<MTBFAnalysis>();
                while (await reader.ReadAsync())
                {
                    results.Add(new MTBFAnalysis
                    {
                        PackageName = reader.GetString(0),
                        MeanTimeBetweenFailures = reader.GetDecimal(1),
                        AvailabilityPercentage = reader.GetDecimal(2),
                        FailureCount = Convert.ToInt32(reader.GetValue(3)),
                        LastFailure = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                        Status = reader.GetString(5)
                    });
                }

                _cache.Set(cacheKey, results, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                Console.WriteLine($"💾 MTBF Analysis cached ({results.Count} packages)");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching MTBF analysis");
                throw;
            }
        }

        public async Task<List<ErrorCluster>> GetErrorClustersAsync()
        {
            var cacheKey = "error_clusters";
            if (_cache.TryGetValue(cacheKey, out List<ErrorCluster>? cachedData) && cachedData != null)
            {
                Console.WriteLine("✅ Error Clusters retrieved from cache");
                return cachedData;
            }

            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    WITH ErrorData AS (
                        SELECT 
                            e.execution_id,
                            ex.package_name,
                            e.message,
                            e.event_name,
                            e.message_time,
                            CASE 
                                WHEN e.message LIKE '%timeout%' OR e.message LIKE '%time out%' THEN 'Timeout'
                                WHEN e.message LIKE '%connection%' THEN 'Connection'
                                WHEN e.message LIKE '%permission%' OR e.message LIKE '%access%' THEN 'Permission'
                                WHEN e.message LIKE '%memory%' THEN 'Memory'
                                WHEN e.message LIKE '%validation%' THEN 'Validation'
                                WHEN e.message LIKE '%deadlock%' THEN 'Deadlock'
                                ELSE 'Other'
                            END as error_category
                        FROM [SSISDB].[catalog].[event_messages] e WITH (NOLOCK)
                        JOIN [SSISDB].[catalog].[executions] ex WITH (NOLOCK) ON e.operation_id = ex.execution_id
                        WHERE e.message_type = 120
                          AND e.message_time >= DATEADD(day, -30, GETDATE())
                    )
                    SELECT 
                        error_category,
                        LEFT(message, 200) as error_message,
                        COUNT(*) as frequency,
                        MIN(message_time) as first_occurrence,
                        MAX(message_time) as last_occurrence,
                        CASE 
                            WHEN COUNT(*) > 50 THEN 'Critical'
                            WHEN COUNT(*) > 20 THEN 'High'
                            WHEN COUNT(*) > 10 THEN 'Medium'
                            ELSE 'Low'
                        END as severity
                    FROM ErrorData
                    GROUP BY error_category, LEFT(message, 200)
                    HAVING COUNT(*) >= 3
                    ORDER BY frequency DESC";

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<ErrorCluster>();
                while (await reader.ReadAsync())
                {
                    results.Add(new ErrorCluster
                    {
                        ErrorCategory = reader.GetString(0),
                        ErrorMessage = reader.GetString(1),
                        Frequency = Convert.ToInt32(reader.GetValue(2)),
                        FirstOccurrence = reader.GetDateTime(3),
                        LastOccurrence = reader.GetDateTime(4),
                        Severity = reader.GetString(5),
                        AffectedPackages = new List<string>()
                    });
                }

                _cache.Set(cacheKey, results, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                Console.WriteLine($"💾 Error Clusters cached ({results.Count} clusters)");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching error clusters");
                throw;
            }
        }

        public async Task<List<SLACompliance>> GetSLAComplianceAsync()
        {
            var cacheKey = "sla_compliance";
            if (_cache.TryGetValue(cacheKey, out List<SLACompliance>? cachedData) && cachedData != null)
            {
                Console.WriteLine("✅ SLA Compliance retrieved from cache");
                return cachedData;
            }

            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    WITH PackageSLA AS (
                        SELECT 
                            package_name,
                            PERCENTILE_CONT(0.95) WITHIN GROUP (ORDER BY DATEDIFF(MINUTE, start_time, end_time)) 
                                OVER (PARTITION BY package_name) * 1.2 as sla_threshold
                        FROM [SSISDB].[catalog].[executions] WITH (NOLOCK)
                        WHERE status = 7
                          AND end_time IS NOT NULL
                          AND start_time >= DATEADD(day, -30, GETDATE())
                    ),
                    ComplianceData AS (
                        SELECT 
                            e.package_name,
                            s.sla_threshold,
                            AVG(DATEDIFF(MINUTE, e.start_time, e.end_time)) as avg_execution_minutes,
                            COUNT(*) as total_executions,
                            SUM(CASE WHEN DATEDIFF(MINUTE, e.start_time, e.end_time) <= s.sla_threshold THEN 1 ELSE 0 END) as compliant_executions
                        FROM [SSISDB].[catalog].[executions] e WITH (NOLOCK)
                        JOIN PackageSLA s ON e.package_name = s.package_name
                        WHERE e.status = 7
                          AND e.end_time IS NOT NULL
                          AND e.start_time >= DATEADD(day, -30, GETDATE())
                        GROUP BY e.package_name, s.sla_threshold
                    )
                    SELECT DISTINCT
                        package_name,
                        sla_threshold,
                        avg_execution_minutes,
                        total_executions,
                        compliant_executions,
                        CAST(compliant_executions * 100.0 / total_executions AS DECIMAL(5,2)) as compliance_percentage,
                        CASE 
                            WHEN CAST(compliant_executions * 100.0 / total_executions AS DECIMAL(5,2)) >= 95 THEN 'Excellent'
                            WHEN CAST(compliant_executions * 100.0 / total_executions AS DECIMAL(5,2)) >= 85 THEN 'Good'
                            WHEN CAST(compliant_executions * 100.0 / total_executions AS DECIMAL(5,2)) >= 70 THEN 'Fair'
                            ELSE 'Poor'
                        END as compliance_status
                    FROM ComplianceData
                    WHERE total_executions >= 5
                    ORDER BY compliance_percentage DESC";

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<SLACompliance>();
                while (await reader.ReadAsync())
                {
                    results.Add(new SLACompliance
                    {
                        PackageName = reader.GetString(0),
                        SLAThresholdMinutes = reader.GetDecimal(1),
                        AvgExecutionMinutes = reader.GetDecimal(2),
                        TotalExecutions = Convert.ToInt32(reader.GetValue(3)),
                        CompiantExecutions = Convert.ToInt32(reader.GetValue(4)),
                        CompliancePercentage = reader.GetDecimal(5),
                        ComplianceStatus = reader.GetString(6)
                    });
                }

                _cache.Set(cacheKey, results, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                Console.WriteLine($"💾 SLA Compliance cached ({results.Count} packages)");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching SLA compliance");
                throw;
            }
        }

        public async Task<List<PerformanceTrend>> GetPerformanceTrendsAsync()
        {
            var cacheKey = "performance_trends";
            if (_cache.TryGetValue(cacheKey, out List<PerformanceTrend>? cachedData) && cachedData != null)
            {
                Console.WriteLine("✅ Performance Trends retrieved from cache");
                return cachedData;
            }

            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    WITH DailyPerformance AS (
                        SELECT 
                            CAST(start_time AS DATE) as execution_date,
                            package_name,
                            AVG(DATEDIFF(MINUTE, start_time, end_time)) as avg_minutes,
                            MIN(DATEDIFF(MINUTE, start_time, end_time)) as min_minutes,
                            MAX(DATEDIFF(MINUTE, start_time, end_time)) as max_minutes,
                            COUNT(*) as execution_count,
                            LAG(AVG(DATEDIFF(MINUTE, start_time, end_time))) OVER (PARTITION BY package_name ORDER BY CAST(start_time AS DATE)) as prev_avg
                        FROM [SSISDB].[catalog].[executions] WITH (NOLOCK)
                        WHERE status = 7
                          AND end_time IS NOT NULL
                          AND start_time >= DATEADD(day, -14, GETDATE())
                        GROUP BY CAST(start_time AS DATE), package_name
                    )
                    SELECT 
                        execution_date,
                        package_name,
                        avg_minutes,
                        min_minutes,
                        max_minutes,
                        execution_count,
                        CASE 
                            WHEN prev_avg IS NULL THEN 'Stable'
                            WHEN avg_minutes > prev_avg * 1.1 THEN 'Degrading'
                            WHEN avg_minutes < prev_avg * 0.9 THEN 'Improving'
                            ELSE 'Stable'
                        END as trend
                    FROM DailyPerformance
                    WHERE execution_count >= 2
                    ORDER BY package_name, execution_date DESC";

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<PerformanceTrend>();
                while (await reader.ReadAsync())
                {
                    results.Add(new PerformanceTrend
                    {
                        Date = reader.GetDateTime(0),
                        PackageName = reader.GetString(1),
                        AvgExecutionMinutes = reader.GetDecimal(2),
                        MinExecutionMinutes = reader.GetDecimal(3),
                        MaxExecutionMinutes = reader.GetDecimal(4),
                        ExecutionCount = Convert.ToInt32(reader.GetValue(5)),
                        Trend = reader.GetString(6)
                    });
                }

                _cache.Set(cacheKey, results, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                Console.WriteLine($"💾 Performance Trends cached ({results.Count} data points)");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching performance trends");
                throw;
            }
        }

        public async Task<List<ExecutionHeatmapData>> GetExecutionHeatmapAsync()
        {
            var cacheKey = "execution_heatmap";
            if (_cache.TryGetValue(cacheKey, out List<ExecutionHeatmapData>? cachedData) && cachedData != null)
            {
                Console.WriteLine("✅ Execution Heatmap retrieved from cache");
                return cachedData;
            }

            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        DATEPART(HOUR, start_time) as hour_of_day,
                        DATEPART(WEEKDAY, start_time) as day_of_week,
                        COUNT(*) as execution_count,
                        AVG(DATEDIFF(MINUTE, start_time, ISNULL(end_time, GETDATE()))) as avg_duration_minutes,
                        SUM(CASE WHEN status = 4 THEN 1 ELSE 0 END) as failure_count
                    FROM [SSISDB].[catalog].[executions] WITH (NOLOCK)
                    WHERE start_time >= DATEADD(day, -30, GETDATE())
                    GROUP BY DATEPART(HOUR, start_time), DATEPART(WEEKDAY, start_time)
                    ORDER BY day_of_week, hour_of_day";

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<ExecutionHeatmapData>();
                while (await reader.ReadAsync())
                {
                    results.Add(new ExecutionHeatmapData
                    {
                        HourOfDay = Convert.ToInt32(reader.GetValue(0)),
                        DayOfWeek = Convert.ToInt32(reader.GetValue(1)),
                        ExecutionCount = Convert.ToInt32(reader.GetValue(2)),
                        AvgDurationMinutes = reader.GetDecimal(3),
                        FailureCount = Convert.ToInt32(reader.GetValue(4))
                    });
                }

                _cache.Set(cacheKey, results, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                Console.WriteLine($"💾 Execution Heatmap cached ({results.Count} data points)");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching execution heatmap");
                throw;
            }
        }

        public async Task<List<PackageCorrelation>> GetPackageCorrelationAsync()
        {
            var cacheKey = "package_correlation";
            if (_cache.TryGetValue(cacheKey, out List<PackageCorrelation>? cachedData) && cachedData != null)
            {
                Console.WriteLine("✅ Package Correlation retrieved from cache");
                return cachedData;
            }

            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    WITH PackagePairs AS (
                        SELECT 
                            e1.package_name as package1,
                            e2.package_name as package2,
                            COUNT(*) as co_execution_count,
                            AVG(ABS(DATEDIFF(MINUTE, e1.start_time, e2.start_time))) as avg_time_diff_minutes
                        FROM [SSISDB].[catalog].[executions] e1 WITH (NOLOCK)
                        JOIN [SSISDB].[catalog].[executions] e2 WITH (NOLOCK)
                            ON CAST(e1.start_time AS DATE) = CAST(e2.start_time AS DATE)
                            AND e1.package_name < e2.package_name
                            AND ABS(DATEDIFF(MINUTE, e1.start_time, e2.start_time)) <= 60
                        WHERE e1.start_time >= DATEADD(day, -30, GETDATE())
                        GROUP BY e1.package_name, e2.package_name
                        HAVING COUNT(*) >= 3
                    )
                    SELECT TOP 20
                        package1,
                        package2,
                        co_execution_count,
                        CAST((co_execution_count * 100.0) / (SELECT COUNT(DISTINCT CAST(start_time AS DATE)) FROM [SSISDB].[catalog].[executions] WITH (NOLOCK) WHERE start_time >= DATEADD(day, -30, GETDATE())) AS DECIMAL(5,2)) as correlation_score,
                        avg_time_diff_minutes
                    FROM PackagePairs
                    ORDER BY co_execution_count DESC";

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<PackageCorrelation>();
                while (await reader.ReadAsync())
                {
                    results.Add(new PackageCorrelation
                    {
                        Package1 = reader.GetString(0),
                        Package2 = reader.GetString(1),
                        CoExecutionCount = Convert.ToInt32(reader.GetValue(2)),
                        CorrelationScore = reader.GetDecimal(3),
                        AvgTimeDifferenceMinutes = reader.GetDecimal(4)
                    });
                }

                _cache.Set(cacheKey, results, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                Console.WriteLine($"💾 Package Correlation cached ({results.Count} correlations)");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching package correlation");
                throw;
            }
        }

        public async Task<List<ResourceUtilization>> GetResourceUtilizationAsync()
        {
            var cacheKey = "resource_utilization";
            if (_cache.TryGetValue(cacheKey, out List<ResourceUtilization>? cachedData) && cachedData != null)
            {
                Console.WriteLine("✅ Resource Utilization retrieved from cache");
                return cachedData;
            }

            try
            {
                var connectionString = GetConnectionString();
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    WITH TimeSlots AS (
                        SELECT 
                            DATEADD(HOUR, DATEDIFF(HOUR, 0, start_time), 0) as time_slot,
                            COUNT(*) as concurrent_executions,
                            SUM(DATEDIFF(SECOND, start_time, ISNULL(end_time, GETDATE()))) as total_cpu_time
                        FROM [SSISDB].[catalog].[executions] WITH (NOLOCK)
                        WHERE start_time >= DATEADD(day, -7, GETDATE())
                        GROUP BY DATEADD(HOUR, DATEDIFF(HOUR, 0, start_time), 0)
                    )
                    SELECT 
                        time_slot,
                        concurrent_executions,
                        CAST(total_cpu_time AS DECIMAL(10,2)) as total_cpu_time,
                        CAST(0.0 AS DECIMAL(10,2)) as peak_memory_mb,
                        CASE 
                            WHEN concurrent_executions > 20 THEN 'Critical'
                            WHEN concurrent_executions > 10 THEN 'High'
                            WHEN concurrent_executions > 5 THEN 'Medium'
                            ELSE 'Low'
                        END as utilization_level
                    FROM TimeSlots
                    ORDER BY time_slot DESC";

                using var command = new SqlCommand(query, connection);
                command.CommandTimeout = 30;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<ResourceUtilization>();
                while (await reader.ReadAsync())
                {
                    results.Add(new ResourceUtilization
                    {
                        TimeSlot = reader.GetDateTime(0),
                        ConcurrentExecutions = Convert.ToInt32(reader.GetValue(1)),
                        TotalCPUTime = reader.GetDecimal(2),
                        PeakMemoryMB = reader.GetDecimal(3),
                        UtilizationLevel = reader.GetString(4)
                    });
                }

                _cache.Set(cacheKey, results, TimeSpan.FromSeconds(CACHE_DURATION_SECONDS));
                Console.WriteLine($"💾 Resource Utilization cached ({results.Count} time slots)");

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching resource utilization");
                throw;
            }
        }
    }
}
