namespace SharePointListsGraphApi.Helpers
{
    internal static class ValidationHelper
    {
        public static void ValidateSettings(SharePointSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.TenantId))
                throw new ArgumentException("TenantId is required in appsettings.json");
            
            if (string.IsNullOrWhiteSpace(settings.ClientId))
                throw new ArgumentException("ClientId is required in appsettings.json");
            
            if (string.IsNullOrWhiteSpace(settings.ClientSecret))
                throw new ArgumentException("ClientSecret is required in appsettings.json");
            
            if (string.IsNullOrWhiteSpace(settings.SiteUrl))
                throw new ArgumentException("SiteUrl is required in appsettings.json");
            
            if (string.IsNullOrWhiteSpace(settings.ListName))
                throw new ArgumentException("ListName is required in appsettings.json");
        }
    }
}
