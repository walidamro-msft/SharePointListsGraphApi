using Microsoft.Graph;
using System.Text.Json;

namespace SharePointListsGraphApi.Services
{
    internal class ListExportService
    {
        private readonly GraphServiceClient _graphClient;

        public ListExportService(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<List<Dictionary<string, object>>> DownloadAllListItemsAsync(
            string siteId, 
            string listId, 
            string columns,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            // Set console encoding to UTF-8 to support progress bar characters
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            var allItems = new List<Dictionary<string, object>>();
            var pageCount = 0;

            Console.WriteLine("\nDownloading list items...\n");
            
            if (fromDate.HasValue || toDate.HasValue)
            {
                Console.WriteLine($"Applying date filter on Modified field:");
                if (fromDate.HasValue)
                    Console.WriteLine($"  From: {fromDate.Value:yyyy-MM-dd HH:mm:ss}");
                if (toDate.HasValue)
                    Console.WriteLine($"  To: {toDate.Value:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine();
            }
            
            var response = await _graphClient.Sites[siteId].Lists[listId].Items
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = ["fields"];
                    if (columns != "*")
                    {
                        requestConfiguration.QueryParameters.Select = columns.Split(',').Select(c => c.Trim()).ToArray();
                    }
                    
                    // Build filter query for date range
                    var filters = new List<string>();
                    if (fromDate.HasValue)
                    {
                        var fromDateStr = fromDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                        filters.Add($"fields/Modified ge '{fromDateStr}'");
                    }
                    if (toDate.HasValue)
                    {
                        var toDateStr = toDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                        filters.Add($"fields/Modified le '{toDateStr}'");
                    }
                    
                    if (filters.Any())
                    {
                        requestConfiguration.QueryParameters.Filter = string.Join(" and ", filters);
                    }
                });

            while (response != null)
            {
                pageCount++;
                if (response.Value != null)
                {
                    var itemsInPage = response.Value.Count;
                    
                    foreach (var item in response.Value)
                    {
                        if (item.Fields?.AdditionalData != null)
                        {
                            var itemData = new Dictionary<string, object>();
                            
                            if (columns == "*")
                            {
                                foreach (var field in item.Fields.AdditionalData)
                                {
                                    itemData[field.Key] = field.Value ?? string.Empty;
                                }
                            }
                            else
                            {
                                var selectedColumns = columns.Split(',').Select(c => c.Trim());
                                foreach (var column in selectedColumns)
                                {
                                    if (item.Fields.AdditionalData.TryGetValue(column, out var value))
                                    {
                                        itemData[column] = value ?? string.Empty;
                                    }
                                    else
                                    {
                                        itemData[column] = string.Empty;
                                    }
                                }
                            }
                            
                            allItems.Add(itemData);
                        }
                    }
                    
                    // Show animated spinner to indicate active downloading
                    var spinnerChars = new[] { '|', '/', '-', '\\' };
                    var spinner = spinnerChars[pageCount % 4];
                    Console.Write($"\r{spinner} Downloading... Page #{pageCount} ({itemsInPage} items) | Total: {allItems.Count:N0} items{new string(' ', 10)}");
                }

                if (!string.IsNullOrEmpty(response.OdataNextLink))
                {
                    var nextPageRequest = new Microsoft.Graph.Sites.Item.Lists.Item.Items.ItemsRequestBuilder(
                        response.OdataNextLink, 
                        _graphClient.RequestAdapter);
                    
                    response = await nextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine($"\n\n+ Total items downloaded: {allItems.Count}");
            return allItems;
        }

        public async Task SaveToJsonFileAsync(List<Dictionary<string, object>> items, string fileName)
        {
            Console.WriteLine($"Saving to {fileName}...");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(items, options);
            await File.WriteAllTextAsync(fileName, json);
            
            Console.WriteLine($"+ Successfully saved {items.Count} items to {fileName}");
        }
    }
}
