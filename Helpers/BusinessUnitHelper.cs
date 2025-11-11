namespace SSISAnalyticsDashboard.Helpers
{
    public static class BusinessUnitHelper
    {
        public static string GetBusinessUnit(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                return "Uncategorized";

            if (projectName.StartsWith("CR_", StringComparison.OrdinalIgnoreCase))
                return "ClientRepo";
            
            if (projectName.StartsWith("CN_", StringComparison.OrdinalIgnoreCase))
                return "ChartNav";
            
            if (projectName.StartsWith("EDS_", StringComparison.OrdinalIgnoreCase))
                return "EDS";
            
            if (projectName.StartsWith("HIM_", StringComparison.OrdinalIgnoreCase))
                return "HIM";

            return "Uncategorized";
        }

        public static string GetBusinessUnitWhereClause(string? businessUnit)
        {
            if (string.IsNullOrEmpty(businessUnit) || string.IsNullOrWhiteSpace(businessUnit))
                return "";

            var upperUnit = businessUnit.Trim().ToUpper();
            
            return upperUnit switch
            {
                "CLIENTREPO" => " AND e.project_name LIKE 'CR[_]%'",
                "CHARTNAV" => " AND e.project_name LIKE 'CN[_]%'",
                "EDS" => " AND e.project_name LIKE 'EDS[_]%'",
                "HIM" => " AND e.project_name LIKE 'HIM[_]%'",
                "UNCATEGORIZED" => " AND e.project_name NOT LIKE 'CR[_]%' AND e.project_name NOT LIKE 'CN[_]%' AND e.project_name NOT LIKE 'EDS[_]%' AND e.project_name NOT LIKE 'HIM[_]%'",
                _ => ""
            };
        }
    }
}
