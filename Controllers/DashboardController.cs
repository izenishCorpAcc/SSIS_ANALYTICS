using Microsoft.AspNetCore.Mvc;
using SSISAnalyticsDashboard.Models;
using SSISAnalyticsDashboard.Services;

namespace SSISAnalyticsDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISSISDataService _dataService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ISSISDataService dataService, ILogger<DashboardController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? businessUnit = null)
        {
            _logger.LogInformation($"====== Dashboard Index Called ======");
            _logger.LogInformation($"Business Unit Parameter: '{businessUnit ?? "NULL"}'");
            
            // Check if connection string exists in session
            var connectionString = HttpContext.Session.GetString("SSISDBConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                // Redirect to ServerConfig if not configured
                return RedirectToAction("Index", "ServerConfig");
            }

            // Pass businessUnit to view for maintaining dropdown selection
            ViewBag.SelectedBusinessUnit = businessUnit ?? "";
            
            _logger.LogInformation($"ViewBag.SelectedBusinessUnit set to: '{ViewBag.SelectedBusinessUnit}'");

            try
            {
                _logger.LogInformation($"Loading dashboard with business unit filter: {businessUnit ?? "ALL"}");

                // Load all data in parallel for MUCH faster page load
                var sw = System.Diagnostics.Stopwatch.StartNew();
                
                var metricsTask = _dataService.GetMetricsAsync(businessUnit);
                var trendsTask = _dataService.GetTrendsAsync(businessUnit);
                var recentErrorsTask = _dataService.GetErrorsAsync(businessUnit);
                var recentExecutionsTask = _dataService.GetExecutionsAsync(businessUnit);
                var lastExecutedPackagesTask = _dataService.GetLastExecutedPackagesAsync(10, businessUnit);
                var currentExecutionsTask = _dataService.GetCurrentExecutionsAsync(businessUnit);
                var packagePerformanceTask = _dataService.GetPackagePerformanceStatsAsync(30, businessUnit);
                var failurePatternsTask = _dataService.GetFailurePatternsAsync(30, businessUnit);
                var executionTimelineTask = _dataService.GetExecutionTimelineAsync(24, businessUnit);
                
                // Wait for all tasks to complete
                await Task.WhenAll(
                    metricsTask, trendsTask, recentErrorsTask, recentExecutionsTask,
                    lastExecutedPackagesTask, currentExecutionsTask, packagePerformanceTask,
                    failurePatternsTask, executionTimelineTask
                );
                
                sw.Stop();
                _logger.LogInformation($"⚡ Dashboard data loaded in {sw.ElapsedMilliseconds}ms (parallel execution)");

                var viewModel = new DashboardViewModel
                {
                    Metrics = metricsTask.Result,
                    Trends = trendsTask.Result,
                    RecentErrors = recentErrorsTask.Result,
                    RecentExecutions = recentExecutionsTask.Result,
                    LastExecutedPackages = lastExecutedPackagesTask.Result,
                    CurrentExecutions = currentExecutionsTask.Result,
                    PackagePerformanceStats = packagePerformanceTask.Result,
                    FailurePatterns = failurePatternsTask.Result,
                    ExecutionTimeline = executionTimelineTask.Result
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading dashboard with business unit: {businessUnit ?? "ALL"}");
                ViewBag.ErrorMessage = $"Failed to load dashboard data: {ex.Message}";
                ViewBag.SelectedBusinessUnit = businessUnit ?? "";
                return View(new DashboardViewModel());
            }
        }

        // API endpoints for AJAX refresh
        [HttpGet]
        [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetMetrics()
        {
            // Check if connection string exists in session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SSISDBConnection")))
            {
                return Unauthorized(new { error = "Not configured" });
            }

            try
            {
                var metrics = await _dataService.GetMetricsAsync();
                return Json(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching metrics");
                return StatusCode(500, new { error = "Failed to fetch metrics" });
            }
        }

        [HttpGet]
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> GetTrends()
        {
            // Check if connection string exists in session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SSISDBConnection")))
            {
                return Unauthorized(new { error = "Not configured" });
            }

            try
            {
                var trends = await _dataService.GetTrendsAsync();
                return Json(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching trends");
                return StatusCode(500, new { error = "Failed to fetch trends" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLastExecutedPackages()
        {
            // Check if connection string exists in session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SSISDBConnection")))
            {
                return Unauthorized(new { error = "Not configured" });
            }

            try
            {
                var packages = await _dataService.GetLastExecutedPackagesAsync(10);
                return Json(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching last executed packages");
                return StatusCode(500, new { error = "Failed to fetch last executed packages" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentExecutions()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SSISDBConnection")))
            {
                return Unauthorized(new { error = "Not configured" });
            }

            try
            {
                var currentExecutions = await _dataService.GetCurrentExecutionsAsync();
                return Json(currentExecutions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current executions");
                return StatusCode(500, new { error = "Failed to fetch current executions" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPackagePerformance()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SSISDBConnection")))
            {
                return Unauthorized(new { error = "Not configured" });
            }

            try
            {
                var performance = await _dataService.GetPackagePerformanceStatsAsync(30);
                return Json(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching package performance");
                return StatusCode(500, new { error = "Failed to fetch package performance" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFailurePatterns()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SSISDBConnection")))
            {
                return Unauthorized(new { error = "Not configured" });
            }

            try
            {
                var patterns = await _dataService.GetFailurePatternsAsync(30);
                return Json(patterns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching failure patterns");
                return StatusCode(500, new { error = "Failed to fetch failure patterns" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetExecutionTimeline()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SSISDBConnection")))
            {
                return Unauthorized(new { error = "Not configured" });
            }

            try
            {
                var timeline = await _dataService.GetExecutionTimelineAsync(24);
                return Json(timeline);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching execution timeline");
                return StatusCode(500, new { error = "Failed to fetch execution timeline" });
            }
        }
    }
}
