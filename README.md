# SSIS Analytics Dashboard - ASP.NET Core MVC

A professional web dashboard for monitoring and analyzing SQL Server Integration Services (SSIS) package executions using ASP.NET Core 8, C#, and Windows Authentication.

## Features

- **Real-time SSISDB Monitoring**: Live data from SQL Server SSISDB catalog
- **Execution Metrics**: Total, successful, and failed execution counts with success rates
- **Trend Analysis**: 30-day historical trends with interactive charts
- **Error Tracking**: Recent error logs with detailed descriptions
- **Execution History**: Complete package execution history
- **Windows Authentication**: Seamless integration with SQL Server using Windows Auth
- **Responsive UI**: Bootstrap 5 with modern card layouts and data tables
- **Interactive Charts**: Chart.js visualizations (pie charts and line graphs)

## Technology Stack

- **Framework**: ASP.NET Core 8 MVC
- **Language**: C# 12
- **Database**: Microsoft SQL Server (SSISDB)
- **Data Provider**: Microsoft.Data.SqlClient 6.1.2
- **UI Framework**: Bootstrap 5.3
- **Charts**: Chart.js 4.x
- **Icons**: Bootstrap Icons

## Project Structure

```
SSISAnalyticsDashboard/
├── Controllers/
│   └── DashboardController.cs      # Main controller with API endpoints
├── Models/
│   ├── ExecutionMetrics.cs         # Aggregated metrics model
│   ├── ExecutionTrend.cs           # Daily trend data model
│   ├── ErrorLog.cs                 # Error log model
│   ├── PackageExecution.cs         # Package execution model
│   └── DashboardViewModel.cs       # Combined view model
├── Services/
│   ├── ISSISDataService.cs         # Service interface
│   └── SSISDataService.cs          # Database service implementation
├── Views/
│   ├── Dashboard/
│   │   └── Index.cshtml            # Main dashboard view
│   └── Shared/
│       └── _Layout.cshtml          # Layout template
├── wwwroot/                        # Static files (CSS, JS, images)
├── appsettings.json                # Configuration
└── Program.cs                      # Application entry point
```

## Prerequisites

- **.NET 8 SDK**: [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** with SSISDB catalog enabled
- **Windows Authentication** access to SSISDB
- **Visual Studio 2022** or **VS Code** (recommended)

## Installation & Setup

### 1. Clone or Download the Project

```bash
cd "c:\Users\sagar.chudali\Documents\SSIS analyzer\SSISAnalyticsDashboard"
```

### 2. Update Connection String

Edit `appsettings.json` and replace `your-server-name` with your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "SSISDBConnection": "Server=YOUR-SQL-SERVER-NAME;Database=SSISDB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**Examples:**
- Local SQL Server: `Server=localhost;...`
- Named instance: `Server=localhost\\SQLEXPRESS;...`
- Remote server: `Server=192.168.1.100;...`

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

The dashboard will be available at:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

## Configuration

### Connection String Options

The default connection string uses Windows Authentication:

```
Server=your-server-name;Database=SSISDB;Integrated Security=true;TrustServerCertificate=true;
```

**Connection String Parameters:**
- `Server`: SQL Server instance name
- `Database`: Always "SSISDB"
- `Integrated Security=true`: Use Windows Authentication
- `TrustServerCertificate=true`: Trust self-signed certificates

### Logging

Configure logging levels in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "SSISAnalyticsDashboard": "Debug"
    }
  }
}
```

## Database Queries

The application queries the following SSISDB catalog views:

### 1. Execution Metrics
```sql
SELECT 
    COUNT(*) as TotalExecutions,
    SUM(CASE WHEN status = 4 THEN 1 ELSE 0 END) as FailedExecutions,
    SUM(CASE WHEN status = 7 THEN 1 ELSE 0 END) as SuccessfulExecutions,
    AVG(DATEDIFF(SECOND, start_time, end_time)) as AvgDuration
FROM [SSISDB].[catalog].[executions]
WHERE start_time >= DATEADD(day, -30, GETDATE())
```

### 2. Execution Trends
```sql
SELECT 
    CAST(start_time AS DATE) as Date,
    SUM(CASE WHEN status = 7 THEN 1 ELSE 0 END) as Success,
    SUM(CASE WHEN status = 4 THEN 1 ELSE 0 END) as Failed
FROM [SSISDB].[catalog].[executions]
WHERE start_time >= DATEADD(day, -30, GETDATE())
GROUP BY CAST(start_time AS DATE)
```

### 3. Error Logs
```sql
SELECT TOP 50
    e.execution_id,
    e.package_name,
    em.message_time,
    em.message
FROM [SSISDB].[catalog].[executions] e
INNER JOIN [SSISDB].[catalog].[event_messages] em 
    ON e.execution_id = em.execution_id
WHERE em.message_type = 120  -- Error messages
```

## Dashboard Components

### Metrics Cards (Top Row)
- **Total Executions**: Count of all executions in last 30 days
- **Successful**: Count and percentage of successful executions
- **Failed**: Count of failed executions
- **Avg Duration**: Average execution time in seconds

### Charts (Middle Section)
- **Success Rate Pie Chart**: Visual breakdown of successes vs failures
- **Execution Trends Line Chart**: Daily success/failure trends over 30 days

### Data Tables (Bottom Section)
- **Recent Executions**: Last 50 package executions with status badges
- **Recent Errors**: Last 50 error messages with descriptions

## API Endpoints

The dashboard exposes JSON API endpoints for AJAX refresh:

- `GET /Dashboard/GetMetrics` - Returns execution metrics as JSON
- `GET /Dashboard/GetTrends` - Returns trend data as JSON

## Troubleshooting

### Connection Errors

**Error**: "Cannot open database 'SSISDB'"
- **Solution**: Ensure SSISDB catalog is enabled on your SQL Server
- Enable with: SQL Server Management Studio → Integration Services Catalogs → Create Catalog

**Error**: "Login failed for user"
- **Solution**: Verify Windows user has access to SSISDB
- Grant permissions: `GRANT SELECT ON [SSISDB].[catalog].[executions] TO [DOMAIN\Username]`

### Build Errors

**Error**: "Package Microsoft.Data.SqlClient not found"
```bash
dotnet add package Microsoft.Data.SqlClient
dotnet restore
```

**Error**: "The name 'ISSISDataService' does not exist"
```bash
# Verify all files are created in Services/ folder
dotnet build --no-incremental
```

### Runtime Errors

**Error**: "No data displayed on dashboard"
- Verify SQL Server contains execution data in SSISDB
- Check connection string in appsettings.json
- Review application logs for detailed errors

## Development

### Adding New Features

1. **Create Model**: Add new model class in `Models/`
2. **Update Service**: Add method to `ISSISDataService` interface and implementation
3. **Update Controller**: Add action method in `DashboardController`
4. **Update View**: Add UI components in `Views/Dashboard/Index.cshtml`

### Running in Development Mode

```bash
dotnet watch run
```

Hot reload is enabled - changes to C# and Razor files will auto-refresh.

## Deployment

### Publish to IIS

```bash
dotnet publish -c Release -o ./publish
```

Then configure IIS:
1. Create new website in IIS Manager
2. Point to `./publish` folder
3. Set Application Pool to "No Managed Code"
4. Ensure Windows Authentication is enabled

### Publish to Azure App Service

```bash
dotnet publish -c Release
# Deploy via Azure Portal or Azure CLI
```

## Database Permissions

Minimum permissions required for the Windows user/service account:

```sql
USE SSISDB;
GO

-- Grant read access to catalog views
GRANT SELECT ON [catalog].[executions] TO [DOMAIN\Username];
GRANT SELECT ON [catalog].[event_messages] TO [DOMAIN\Username];
GRANT EXECUTE ON [catalog].[get_execution_statistics] TO [DOMAIN\Username];
```

## Performance Considerations

- **Date Range**: Currently limited to 30 days. Adjust in service queries if needed.
- **Top N**: Error and execution tables show top 50. Increase if required.
- **Connection Pooling**: Enabled by default in SqlConnection
- **Async Operations**: All database operations use async/await

## Customization

### Change Date Range

Edit `SSISDataService.cs`, replace `DATEADD(day, -30, GETDATE())` with desired range:
- Last 7 days: `DATEADD(day, -7, GETDATE())`
- Last 90 days: `DATEADD(day, -90, GETDATE())`

### Add More Charts

Add Chart.js configurations in the `@section Scripts` block of `Index.cshtml`.

### Custom Styling

Edit `wwwroot/css/site.css` to customize colors, fonts, and layouts.

## Support

For issues or questions:
1. Check SQL Server connection and permissions
2. Review application logs in console output
3. Verify SSISDB contains execution data
4. Ensure .NET 8 SDK is installed

## License

This project is provided as-is for monitoring SSIS executions.

---

**Built with ASP.NET Core 8 and Microsoft.Data.SqlClient**
