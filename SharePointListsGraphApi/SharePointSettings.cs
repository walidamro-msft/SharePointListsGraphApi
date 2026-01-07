namespace SharePointListsGraphApi
{
    internal class SharePointSettings
    {
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string SiteUrl { get; set; } = string.Empty;
        public string ListName { get; set; } = string.Empty;
        public string Columns { get; set; } = "*";
        public bool EnableDebugLogging { get; set; } = false;
    }
}
