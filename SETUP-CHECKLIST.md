# ✅ Setup Checklist - SSIS Analytics Dashboard

## Pre-Launch Checklist

Use this checklist to ensure your dashboard is ready to run.

---

## 1. Prerequisites ✓

- [x] .NET 8 SDK installed (verified: 8.0.202)
- [ ] SQL Server with SSISDB catalog accessible
- [ ] Windows user has read permissions on SSISDB
- [ ] Browser installed (Chrome, Edge, Firefox)

**Verify .NET Installation:**
```bash
dotnet --version
# Should show: 8.0.202 or higher
```

---

## 2. Project Files ✓

### Models (5 files)
- [x] `Models/ExecutionMetrics.cs`
- [x] `Models/ExecutionTrend.cs`
- [x] `Models/ErrorLog.cs`
- [x] `Models/PackageExecution.cs`
- [x] `Models/DashboardViewModel.cs`

### Services (2 files)
- [x] `Services/ISSISDataService.cs`
- [x] `Services/SSISDataService.cs`

### Controllers (1 file)
- [x] `Controllers/DashboardController.cs`

### Views (2 dashboard files)
- [x] `Views/Dashboard/Index.cshtml`
- [x] `Views/Shared/_Layout.cshtml`

### Configuration (3 files)
- [x] `Program.cs` - Dependency injection configured
- [x] `appsettings.json` - Connection string template added
- [x] `SSISAnalyticsDashboard.csproj` - Packages referenced

### Documentation (4 files)
- [x] `README.md` - Complete documentation
- [x] `QUICKSTART.md` - Quick start guide
- [x] `CONVERSION-SUMMARY.md` - Conversion summary
- [x] `SETUP-CHECKLIST.md` - This file

---

## 3. NuGet Packages ✓

- [x] `Microsoft.Data.SqlClient` version 6.1.2
- [x] `Newtonsoft.Json` version 13.0.4

**Verify Packages:**
```bash
dotnet list package
```

---

## 4. Build Verification ✓

- [x] Project builds successfully
- [x] 0 errors
- [x] 0 warnings

**Build Output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:03.19
```

---

## 5. Configuration Required ⚠️

### Update Connection String

**File:** `appsettings.json`

**Current:**
```json
{
  "ConnectionStrings": {
    "SSISDBConnection": "Server=your-server-name;Database=SSISDB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**Action Required:**
- [ ] Replace `your-server-name` with your actual SQL Server instance

**Examples:**
- Local SQL Server: `localhost`
- Named instance: `localhost\\SQLEXPRESS`
- Remote server: `192.168.1.100` or `SERVERNAME`

---

## 6. Database Permissions

Ensure your Windows user has access to SSISDB:

```sql
-- Connect to SSISDB in SQL Server Management Studio
USE SSISDB;
GO

-- Grant read permissions
GRANT SELECT ON [catalog].[executions] TO [DOMAIN\YourUsername];
GRANT SELECT ON [catalog].[event_messages] TO [DOMAIN\YourUsername];
```

**Checklist:**
- [ ] Windows user can connect to SQL Server
- [ ] User has SELECT permission on `[catalog].[executions]`
- [ ] User has SELECT permission on `[catalog].[event_messages]`
- [ ] SSISDB catalog exists and has execution data

---

## 7. Launch Commands

### Option 1: Standard Run
```bash
cd "c:\Users\sagar.chudali\Documents\SSIS analyzer\SSISAnalyticsDashboard"
dotnet run
```

### Option 2: Development Mode (Hot Reload)
```bash
cd "c:\Users\sagar.chudali\Documents\SSIS analyzer\SSISAnalyticsDashboard"
dotnet watch run
```

### Option 3: Custom Port
```bash
dotnet run --urls "https://localhost:5555"
```

**Launch Checklist:**
- [ ] Navigate to project directory
- [ ] Run `dotnet run`
- [ ] Wait for "Application started" message
- [ ] Note the URL (usually https://localhost:5001)

---

## 8. Browser Access

**URLs:**
- Primary: https://localhost:5001
- Alternative: http://localhost:5000

**First Launch:**
- [ ] Open browser
- [ ] Navigate to https://localhost:5001
- [ ] Accept self-signed certificate warning (if prompted)
- [ ] Dashboard loads successfully

---

## 9. Dashboard Verification

### Expected Components:

#### Metrics Cards (Top Row)
- [ ] Total Executions card displays number
- [ ] Successful Executions card displays number and percentage
- [ ] Failed Executions card displays number
- [ ] Average Duration card displays seconds

#### Charts (Middle Row)
- [ ] Success Rate Pie Chart renders
- [ ] Execution Trends Line Chart renders
- [ ] Charts show data (if executions exist)

#### Tables (Bottom Section)
- [ ] Recent Executions table displays
- [ ] Status badges colored correctly (Green=Success, Red=Failed)
- [ ] Recent Errors table displays
- [ ] Error descriptions visible

#### UI Elements
- [ ] Blue navbar at top
- [ ] "SSIS Analytics" branding visible
- [ ] Refresh button works
- [ ] No console errors in browser dev tools (F12)

---

## 10. Troubleshooting Checklist

### Problem: Build Fails
- [ ] Run `dotnet restore`
- [ ] Run `dotnet clean`
- [ ] Run `dotnet build`

### Problem: Connection Error
- [ ] Verify SQL Server is running
- [ ] Check server name in `appsettings.json`
- [ ] Test connection in SQL Server Management Studio
- [ ] Verify SSISDB catalog exists

### Problem: No Data Displayed
- [ ] Check if SSIS packages have been executed in last 30 days
- [ ] Verify Windows user permissions
- [ ] Review browser console for errors (F12)
- [ ] Check application console output for exceptions

### Problem: Port Already in Use
- [ ] Stop other applications using port 5001
- [ ] Use alternate port: `dotnet run --urls "https://localhost:5555"`
- [ ] Check Task Manager for running `SSISAnalyticsDashboard.exe`

---

## 11. Post-Launch Verification

### Functional Tests:
- [ ] Metrics cards show correct numbers
- [ ] Charts render without errors
- [ ] Tables populate with data
- [ ] Refresh button reloads page
- [ ] No errors in console output
- [ ] No errors in browser console

### Performance Tests:
- [ ] Page loads in < 5 seconds
- [ ] Charts render smoothly
- [ ] No lag when scrolling tables

### Browser Compatibility:
- [ ] Works in Chrome/Edge
- [ ] Works in Firefox
- [ ] Responsive on mobile (if needed)

---

## 12. Production Deployment (Optional)

### IIS Deployment:
- [ ] Run `dotnet publish -c Release -o ./publish`
- [ ] Create IIS website pointing to publish folder
- [ ] Set Application Pool to "No Managed Code"
- [ ] Enable Windows Authentication in IIS
- [ ] Test public URL

### Azure App Service:
- [ ] Create Azure App Service (Windows)
- [ ] Configure Windows Authentication
- [ ] Deploy via Visual Studio or Azure CLI
- [ ] Update connection string in Azure Configuration
- [ ] Test Azure URL

---

## 13. Documentation Review

- [ ] Read `README.md` for full documentation
- [ ] Review `QUICKSTART.md` for setup steps
- [ ] Check `CONVERSION-SUMMARY.md` for architecture details
- [ ] Bookmark this checklist for future reference

---

## 14. Optional Enhancements

### Future Improvements:
- [ ] Add user authentication/authorization
- [ ] Implement SignalR for real-time updates
- [ ] Add filtering by package name
- [ ] Add date range picker
- [ ] Export data to Excel/PDF
- [ ] Email notifications for failures
- [ ] Add performance metrics charts
- [ ] Implement caching layer

---

## 15. Support & Resources

### Quick Links:
- Project Folder: `c:\Users\sagar.chudali\Documents\SSIS analyzer\SSISAnalyticsDashboard\`
- README: `README.md`
- Quick Start: `QUICKSTART.md`

### Useful Commands:
```bash
# Check .NET version
dotnet --version

# Build project
dotnet build

# Run application
dotnet run

# Clean build files
dotnet clean

# List installed packages
dotnet list package

# Restore packages
dotnet restore
```

### Common Errors:
| Error | Solution |
|-------|----------|
| Port in use | Use different port or stop other apps |
| Connection failed | Check SQL Server connection string |
| No data | Verify SSIS executions exist |
| Build error | Run `dotnet restore` and rebuild |

---

## ✅ Launch Status

### Current Status:
- ✅ Project created
- ✅ All files generated
- ✅ Build successful (0 errors, 0 warnings)
- ✅ Packages installed
- ⚠️ Connection string needs update
- ⏳ Ready to run after configuration

---

## 🎯 Final Steps

1. **Update connection string** in `appsettings.json`
2. **Run the application**: `dotnet run`
3. **Open browser**: https://localhost:5001
4. **Verify dashboard** loads with data
5. **Celebrate!** 🎉

---

## 📝 Notes

- Default port: 5001 (HTTPS), 5000 (HTTP)
- Data range: Last 30 days
- Auto-refresh: Manual (click Refresh button)
- Authentication: Windows Authentication
- Database: SSISDB catalog views

---

**Checklist Last Updated:** 2025  
**Project Version:** 1.0  
**Framework:** ASP.NET Core 8.0  
**Status:** Ready for Configuration & Launch

---

## ✨ Success Criteria

Your dashboard is ready when:
- ✅ Build completes with 0 errors
- ✅ Application starts without exceptions
- ✅ Dashboard loads in browser
- ✅ Metrics cards display numbers
- ✅ Charts render successfully
- ✅ Tables show execution data

**You're all set! Happy monitoring! 🚀**
