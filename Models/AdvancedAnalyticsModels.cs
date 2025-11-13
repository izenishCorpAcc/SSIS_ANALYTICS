namespace SSISAnalyticsDashboard.Models
{
    public class PackageReliability
    {
        public string PackageName { get; set; } = string.Empty;
        public int TotalExecutions { get; set; }
        public int SuccessfulExecutions { get; set; }
        public int FailedExecutions { get; set; }
        public decimal SuccessRate { get; set; }
        public decimal ReliabilityScore { get; set; }
        public string Trend { get; set; } = string.Empty;
    }

    public class MTBFAnalysis
    {
        public string PackageName { get; set; } = string.Empty;
        public decimal MeanTimeBetweenFailures { get; set; }
        public decimal AvailabilityPercentage { get; set; }
        public int FailureCount { get; set; }
        public DateTime? LastFailure { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ErrorCluster
    {
        public string ErrorCategory { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public int Frequency { get; set; }
        public List<string> AffectedPackages { get; set; } = new();
        public DateTime FirstOccurrence { get; set; }
        public DateTime LastOccurrence { get; set; }
        public string Severity { get; set; } = string.Empty;
    }

    public class SLACompliance
    {
        public string PackageName { get; set; } = string.Empty;
        public decimal SLAThresholdMinutes { get; set; }
        public decimal AvgExecutionMinutes { get; set; }
        public int TotalExecutions { get; set; }
        public int CompiantExecutions { get; set; }
        public decimal CompliancePercentage { get; set; }
        public string ComplianceStatus { get; set; } = string.Empty;
    }

    public class PerformanceTrend
    {
        public DateTime Date { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public decimal AvgExecutionMinutes { get; set; }
        public decimal MinExecutionMinutes { get; set; }
        public decimal MaxExecutionMinutes { get; set; }
        public int ExecutionCount { get; set; }
        public string Trend { get; set; } = string.Empty;
    }

    public class ExecutionHeatmapData
    {
        public int HourOfDay { get; set; }
        public int DayOfWeek { get; set; }
        public int ExecutionCount { get; set; }
        public decimal AvgDurationMinutes { get; set; }
        public int FailureCount { get; set; }
    }

    public class PackageCorrelation
    {
        public string Package1 { get; set; } = string.Empty;
        public string Package2 { get; set; } = string.Empty;
        public int CoExecutionCount { get; set; }
        public decimal CorrelationScore { get; set; }
        public decimal AvgTimeDifferenceMinutes { get; set; }
    }

    public class ResourceUtilization
    {
        public DateTime TimeSlot { get; set; }
        public int ConcurrentExecutions { get; set; }
        public decimal TotalCPUTime { get; set; }
        public decimal PeakMemoryMB { get; set; }
        public string UtilizationLevel { get; set; } = string.Empty;
    }
}
