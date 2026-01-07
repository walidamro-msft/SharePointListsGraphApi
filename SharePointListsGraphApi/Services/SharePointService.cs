using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace SharePointListsGraphApi.Services
{
    internal class SharePointService
    {
        private readonly GraphServiceClient _graphClient;

        public SharePointService(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<bool> TestAuthenticationAsync()
        {
            try
            {
                Console.WriteLine("Testing authentication...");
                await _graphClient.Sites.GetAsync();
                Console.WriteLine("? Authentication successful!");
                Console.WriteLine();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Authentication failed: {ex.Message}");
                return false;
            }
        }

        public async Task<Site> GetSiteAsync(string siteUrl)
        {
            Console.WriteLine("Retrieving SharePoint site...");
            
            var uri = new Uri(siteUrl);
            var hostname = uri.Host;
            var sitePath = uri.AbsolutePath.TrimStart('/');

            Console.WriteLine($"Attempting to access site: {hostname}:/{sitePath}");

            try
            {
                var site = await _graphClient.Sites[$"{hostname}:/{sitePath}"].GetAsync();

                if (site != null)
                {
                    Console.WriteLine($"? Site ID: {site.Id}");
                    Console.WriteLine($"? Site Name: {site.DisplayName}");
                    return site;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed with format '{hostname}:/{sitePath}': {ex.Message}");
                
                try
                {
                    Console.WriteLine("Trying alternative method: searching by URL...");
                    var searchResult = await _graphClient.Sites
                        .GetAsync(requestConfiguration =>
                        {
                            requestConfiguration.QueryParameters.Search = $"\"{siteUrl}\"";
                        });

                    var site = searchResult?.Value?.FirstOrDefault();
                    if (site != null)
                    {
                        Console.WriteLine($"? Site found via search!");
                        Console.WriteLine($"? Site ID: {site.Id}");
                        Console.WriteLine($"? Site Name: {site.DisplayName}");
                        return site;
                    }
                }
                catch (Exception searchEx)
                {
                    Console.WriteLine($"Search also failed: {searchEx.Message}");
                }

                throw new Exception($"Unable to access site: {siteUrl}\n" +
                    $"Original error: {ex.Message}");
            }

            throw new Exception($"Site not found: {siteUrl}");
        }

        public async Task<List> GetListAsync(string siteId, string listName)
        {
            Console.WriteLine($"Retrieving list '{listName}'...");

            var lists = await _graphClient.Sites[siteId].Lists
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = $"displayName eq '{listName}'";
                });

            var list = lists?.Value?.FirstOrDefault();
            if (list == null)
            {
                throw new Exception($"List not found: {listName}");
            }

            Console.WriteLine($"? List ID: {list.Id}");
            return list;
        }
    }
}
