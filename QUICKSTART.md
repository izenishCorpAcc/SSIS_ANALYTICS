# Quick Start Guide - SSIS Analytics Dashboard

Get your SSIS Analytics Dashboard running in 5 minutes!

## Prerequisites Checklist

- [ ] .NET 8 SDK installed
- [ ] SQL Server with SSISDB catalog
- [ ] Windows user with SSISDB read access

## Step 1: Update Connection String (1 minute)

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SSISDBConnection": "Server=YOUR-SERVER-HERE;Database=SSISDB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**Replace `YOUR-SERVER-HERE` with**:
- Local SQL Server: `localhost` or `(local)`
- Named instance: `localhost\\SQLEXPRESS`
- Remote server: `192.168.1.100` or `SERVERNAME`

## Step 2: Build & Run (2 minutes)

Open PowerShell/Terminal in project folder:

```bash
# Navigate to project folder
cd "c:\Users\sagar.chudali\Documents\SSIS analyzer\SSISAnalyticsDashboard"

# Build the project
dotnet build

# Run the application
dotnet run
```

## Step 3: Open Dashboard (30 seconds)

Open your browser and navigate to:

**https://localhost:5001**

## Expected Output

You should see:

### Metrics Cards
- Total Executions (last 30 days)
- Successful Executions (with percentage)
- Failed Executions
- Average Duration

### Charts
- Success Rate Pie Chart
- Execution Trends Line Graph

### Tables
- Recent Executions (50 most recent)
- Recent Errors (50 most recent)

## Troubleshooting

### Problem: Build Fails

**Solution:**
```bash
dotnet restore
dotnet clean
dotnet build
```

### Problem: Connection Error

**Error**: "Cannot open database 'SSISDB'"

**Solution:**
1. Verify SQL Server is running
2. Check server name in connection string
3. Ensure SSISDB catalog exists:
   - Open SQL Server Management Studio
   - Connect to your server
   - Look for "Integration Services Catalogs" node
   - If missing, right-click and "Create Catalog"

### Problem: No Data Displayed

**Possible Causes:**
1. No SSIS executions in last 30 days
2. Windows user lacks permissions

**Solution:**
```sql
-- Grant read access to your Windows user
USE SSISDB;
GRANT SELECT ON [catalog].[executions] TO [DOMAIN\YourUsername];
GRANT SELECT ON [catalog].[event_messages] TO [DOMAIN\YourUsername];
```

### Problem: Port Already in Use

**Error**: "Failed to bind to address https://localhost:5001"

**Solution:**
```bash
# Run on different port
dotnet run --urls "https://localhost:5555"
```

## Quick Commands Reference

```bash
# Build only
dotnet build

# Run (development mode with hot reload)
dotnet watch run

# Clean build files
dotnet clean

# Restore packages
dotnet restore

# Publish for deployment
dotnet publish -c Release
```

## Configuration Tips

### Change Data Range

Default is 30 days. To change:

1. Open `Services/SSISDataService.cs`
2. Find `DATEADD(day, -30, GETDATE())`
3. Replace `-30` with desired days

### Auto-Refresh Dashboard

Add to `Views/Dashboard/Index.cshtml` (before closing `</div>`):

```javascript
<script>
    // Auto-refresh every 60 seconds
    setTimeout(function() {
        location.reload();
    }, 60000);
</script>
```

### Custom Theme Colors

Edit `Views/Shared/_Layout.cshtml`, change navbar class:

```cshtml
<!-- Blue theme (default) -->
<nav class="navbar ... bg-primary">

<!-- Dark theme -->
<nav class="navbar ... bg-dark">

<!-- Success green -->
<nav class="navbar ... bg-success">
```

## Next Steps

1. ✅ Dashboard running successfully
2. 🔧 Customize date ranges if needed
3. 📊 Add more charts or metrics
4. 🚀 Deploy to production server

## Need Help?

1. Check `README.md` for detailed documentation
2. Review console output for error messages
3. Verify SQL Server permissions
4. Test connection string in SQL Server Management Studio

---

**Happy Monitoring! 🎉**

Your SSIS Analytics Dashboard is ready to help you monitor package executions!
