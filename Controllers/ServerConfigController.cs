using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SSISAnalyticsDashboard.Models;
using System.Text.Json;

namespace SSISAnalyticsDashboard.Controllers
{
    public class ServerConfigController : Controller
    {
        private readonly ILogger<ServerConfigController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public ServerConfigController(
            ILogger<ServerConfigController> _logger,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            this._logger = _logger;
            _configuration = configuration;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var connectionString = _configuration.GetConnectionString("SSISDBConnection");
            var model = new ServerConfigViewModel
            {
                IsConfigured = !string.IsNullOrEmpty(connectionString) && 
                              !connectionString.Contains("your-server-name"),
                AuthenticationMode = "Windows" // Default to Windows Auth
            };

            if (model.IsConfigured)
            {
                // Extract server name from connection string
                var builder = new SqlConnectionStringBuilder(connectionString);
                model.ServerName = builder.DataSource;
                model.AuthenticationMode = builder.IntegratedSecurity ? "Windows" : "SQL";
                if (!builder.IntegratedSecurity)
                {
                    model.Username = builder.UserID;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ServerConfigViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Only handle Windows Authentication for now
                if (model.AuthenticationMode != "Windows")
                {
                    model.ErrorMessage = "Only Windows Authentication is supported at this time.";
                    return View(model);
                }

                // Build connection string for Windows Authentication
                string connectionString = $"Server={model.ServerName};Database=SSISDB;Integrated Security=true;TrustServerCertificate=true;Encrypt=false;";

                // Update BOTH appsettings.json files (root and bin/Debug)
                var rootAppSettingsPath = Path.Combine(_environment.ContentRootPath, "appsettings.json");
                await UpdateAppSettingsFile(rootAppSettingsPath, connectionString);
                _logger.LogInformation($"Updated root appsettings.json with server: {model.ServerName}");
                
                var binAppSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                await UpdateAppSettingsFile(binAppSettingsPath, connectionString);
                _logger.LogInformation($"Updated bin appsettings.json with server: {model.ServerName}");

                // Reload configuration
                var configRoot = (IConfigurationRoot)_configuration;
                configRoot.Reload();

                // Set session flag to bypass middleware check on next request
                HttpContext.Session.SetString("ConfigJustSaved", "true");

                _logger.LogInformation($"Configuration saved successfully for server: {model.ServerName}");
                
                // Redirect directly to Dashboard
                return Redirect("/Dashboard/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Configuration failed");
                model.ErrorMessage = $"Configuration error: {ex.Message}";
                return View(model);
            }
        }

        private async Task UpdateAppSettingsFile(string filePath, string connectionString)
        {
            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var jsonObj = JsonDocument.Parse(json);
            
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
            {
                writer.WriteStartObject();
                
                foreach (var property in jsonObj.RootElement.EnumerateObject())
                {
                    if (property.Name == "ConnectionStrings")
                    {
                        writer.WriteStartObject("ConnectionStrings");
                        writer.WriteString("SSISDBConnection", connectionString);
                        writer.WriteEndObject();
                    }
                    else
                    {
                        property.WriteTo(writer);
                    }
                }
                
                writer.WriteEndObject();
            }

            await System.IO.File.WriteAllBytesAsync(filePath, stream.ToArray());
        }
    }
}

