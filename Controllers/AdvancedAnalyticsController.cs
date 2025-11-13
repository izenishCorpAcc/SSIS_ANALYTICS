using Microsoft.AspNetCore.Mvc;
using SSIS_ANALYTICS.Services;
using System.Diagnostics;

namespace SSIS_ANALYTICS.Controllers
{
    public class AdvancedAnalyticsController : Controller
    {
        private readonly ISSISDataService _dataService;

        public AdvancedAnalyticsController(ISSISDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> Index()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Parallel execution of all analytics queries
                var reliabilityTask = _dataService.GetPackageReliabilityScoresAsync();
                var mtbfTask = _dataService.GetMTBFAnalysisAsync();
                var errorClustersTask = _dataService.GetErrorClustersAsync();
                var slaTask = _dataService.GetSLAComplianceAsync();
                var performanceTrendsTask = _dataService.GetPerformanceTrendsAsync();
                var executionHeatmapTask = _dataService.GetExecutionHeatmapAsync();
                var correlationTask = _dataService.GetPackageCorrelationAsync();
                var resourceUtilTask = _dataService.GetResourceUtilizationAsync();

                await Task.WhenAll(
                    reliabilityTask,
                    mtbfTask,
                    errorClustersTask,
                    slaTask,
                    performanceTrendsTask,
                    executionHeatmapTask,
                    correlationTask,
                    resourceUtilTask
                );

                ViewBag.ReliabilityScores = await reliabilityTask;
                ViewBag.MTBFAnalysis = await mtbfTask;
                ViewBag.ErrorClusters = await errorClustersTask;
                ViewBag.SLACompliance = await slaTask;
                ViewBag.PerformanceTrends = await performanceTrendsTask;
                ViewBag.ExecutionHeatmap = await executionHeatmapTask;
                ViewBag.PackageCorrelation = await correlationTask;
                ViewBag.ResourceUtilization = await resourceUtilTask;

                stopwatch.Stop();
                Console.WriteLine($"⚡ Advanced Analytics data loaded in {stopwatch.ElapsedMilliseconds}ms");

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading Advanced Analytics: {ex.Message}");
                throw;
            }
        }

        [ResponseCache(Duration = 10)]
        [HttpGet]
        public async Task<IActionResult> GetReliabilityScores()
        {
            var data = await _dataService.GetPackageReliabilityScoresAsync();
            return Json(data);
        }

        [ResponseCache(Duration = 10)]
        [HttpGet]
        public async Task<IActionResult> GetMTBF()
        {
            var data = await _dataService.GetMTBFAnalysisAsync();
            return Json(data);
        }

        [ResponseCache(Duration = 10)]
        [HttpGet]
        public async Task<IActionResult> GetErrorClusters()
        {
            var data = await _dataService.GetErrorClustersAsync();
            return Json(data);
        }

        [ResponseCache(Duration = 10)]
        [HttpGet]
        public async Task<IActionResult> GetSLACompliance()
        {
            var data = await _dataService.GetSLAComplianceAsync();
            return Json(data);
        }

        [ResponseCache(Duration = 10)]
        [HttpGet]
        public async Task<IActionResult> GetPerformanceTrends()
        {
            var data = await _dataService.GetPerformanceTrendsAsync();
            return Json(data);
        }

        [ResponseCache(Duration = 10)]
        [HttpGet]
        public async Task<IActionResult> GetExecutionHeatmap()
        {
            var data = await _dataService.GetExecutionHeatmapAsync();
            return Json(data);
        }

        [ResponseCache(Duration = 10)]
        [HttpGet]
        public async Task<IActionResult> GetPackageCorrelation()
        {
            var data = await _dataService.GetPackageCorrelationAsync();
            return Json(data);
        }

        [ResponseCache(Duration = 10)]
        [HttpGet]
        public async Task<IActionResult> GetResourceUtilization()
        {
            var data = await _dataService.GetResourceUtilizationAsync();
            return Json(data);
        }
    }
}
