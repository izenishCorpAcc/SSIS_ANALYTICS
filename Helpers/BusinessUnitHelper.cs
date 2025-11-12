namespace SSISAnalyticsDashboard.Helpers
{
    public static class BusinessUnitHelper
    {
        public static string GetBusinessUnit(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return "Uncategorized";

            if (packageName.StartsWith("CR", StringComparison.OrdinalIgnoreCase))
                return "ClientRepo";
            
            if (packageName.StartsWith("CN", StringComparison.OrdinalIgnoreCase))
                return "ChartNav";
            
            if (packageName.StartsWith("EDS", StringComparison.OrdinalIgnoreCase))
                return "EDS";
            
            if (packageName.StartsWith("HIM", StringComparison.OrdinalIgnoreCase))
                return "HIM";

            return "Uncategorized";
        }

        public static string GetBusinessUnitWhereClause(string? businessUnit)
        {
            if (string.IsNullOrEmpty(businessUnit) || string.IsNullOrWhiteSpace(businessUnit))
            {
                Console.WriteLine($"[BusinessUnitHelper] No business unit filter applied");
                return "";
            }

            var upperUnit = businessUnit.Trim().ToUpper();
            
            var clause = upperUnit switch
            {
                "CLIENTREPO" => " AND e.package_name LIKE 'CR%'",
                "CHARTNAV" => " AND e.package_name LIKE 'CN%'",
                "EDS" => " AND e.package_name LIKE 'EDS%'",
                "HIM" => " AND e.package_name LIKE 'HIM%'",
                "UNCATEGORIZED" => " AND e.package_name NOT LIKE 'CR%' AND e.package_name NOT LIKE 'CN%' AND e.package_name NOT LIKE 'EDS%' AND e.package_name NOT LIKE 'HIM%'",
                _ => ""
            };
            
            Console.WriteLine($"[BusinessUnitHelper] Input: '{businessUnit}' -> Upper: '{upperUnit}' -> Clause: '{clause}'");
            return clause;
        }
    }
}
