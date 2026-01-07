using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace SharePointListsGraphApi.Services
{
    internal class TestDataGeneratorService
    {
        private readonly GraphServiceClient _graphClient;
        private readonly Random _random = new();

        public TestDataGeneratorService(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<int> GenerateTestDataAsync(string siteId, string listId, int numberOfRows, int parallelTasks = 1)
        {
            // Set console encoding to UTF-8 to support progress bar characters
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine($"\nGenerating {numberOfRows} test rows{(parallelTasks > 1 ? $" using {parallelTasks} parallel tasks" : "")}...\n");

            // Get list columns to know what fields to populate
            var columns = await GetListColumnsAsync(siteId, listId);
            
            int successCount = 0;
            int failureCount = 0;
            object lockObj = new object();

            if (parallelTasks > 1)
            {
                // Parallel processing
                var semaphore = new SemaphoreSlim(parallelTasks);
                var tasks = new List<Task>();
                int completedCount = 0;

                for (int i = 0; i < numberOfRows; i++)
                {
                    var rowIndex = i;
                    await semaphore.WaitAsync();

                    tasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            var testData = GenerateRandomData(columns, rowIndex + 1);
                            
                            var listItem = new ListItem
                            {
                                Fields = new FieldValueSet
                                {
                                    AdditionalData = testData
                                }
                            };

                            await _graphClient.Sites[siteId].Lists[listId].Items.PostAsync(listItem);
                            
                            lock (lockObj)
                            {
                                successCount++;
                                completedCount++;
                                var progress = completedCount * 100 / numberOfRows;
                                var progressBarLength = progress / 5;
                                Console.Write($"\r[{new string('#', progressBarLength)}{new string('-', 20 - progressBarLength)}] {progress}% - Created {successCount} of {numberOfRows} rows...{new string(' ', 10)}");
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (lockObj)
                            {
                                failureCount++;
                                completedCount++;
                                var progress = completedCount * 100 / numberOfRows;
                                var progressBarLength = progress / 5;
                                Console.Write($"\r[{new string('#', progressBarLength)}{new string('-', 20 - progressBarLength)}] {progress}% - Created {successCount} of {numberOfRows} rows...{new string(' ', 10)}");
                                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.Length < 100)
                                {
                                    Console.WriteLine($"\nX Failed to create row {rowIndex + 1}: {ex.Message}");
                                }
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }

                await Task.WhenAll(tasks);
            }
            else
            {
                // Sequential processing
                for (int i = 0; i < numberOfRows; i++)
                {
                    var progress = (i + 1) * 100 / numberOfRows;
                    Console.Write($"\r[{new string('#', progress / 5)}{new string('-', 20 - progress / 5)}] {progress}% - Creating row {i + 1} of {numberOfRows}...");

                    try
                    {
                        var testData = GenerateRandomData(columns, i + 1);
                        
                        var listItem = new ListItem
                        {
                            Fields = new FieldValueSet
                            {
                                AdditionalData = testData
                            }
                        };

                        await _graphClient.Sites[siteId].Lists[listId].Items.PostAsync(listItem);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nX Failed to create row {i + 1}: {ex.Message}");
                        failureCount++;
                    }
                }
            }

            Console.WriteLine($"\n\n+ Successfully created {successCount} rows");
            if (failureCount > 0)
            {
                Console.WriteLine($"X Failed to create {failureCount} rows");
            }

            return successCount;
        }

        private async Task<List<ColumnDefinition>> GetListColumnsAsync(string siteId, string listId)
        {
            var columnsResponse = await _graphClient.Sites[siteId].Lists[listId].Columns.GetAsync();
            return columnsResponse?.Value ?? new List<ColumnDefinition>();
        }

        private Dictionary<string, object> GenerateRandomData(List<ColumnDefinition> columns, int rowNumber)
        {
            var data = new Dictionary<string, object>();

            foreach (var column in columns)
            {
                // Skip system columns
                if (column.ReadOnly == true || column.Name?.StartsWith("_") == true)
                    continue;

                var value = GenerateRandomValueForColumn(column, rowNumber);
                if (value != null && !string.IsNullOrEmpty(column.Name))
                {
                    data[column.Name] = value;
                }
            }

            return data;
        }

        private object? GenerateRandomValueForColumn(ColumnDefinition column, int rowNumber)
        {
            try
            {
                if (column.Text != null)
                {
                    if (column.Text.AllowMultipleLines == true)
                    {
                        return $"Test multiline text for row {rowNumber}\nLine 2\nLine 3";
                    }
                    return $"Test {column.Name} {rowNumber}";
                }

                if (column.Number != null)
                {
                    return _random.Next(1, 1000);
                }

                if (column.DateTime != null)
                {
                    var randomDays = _random.Next(-365, 365);
                    return DateTime.Now.AddDays(randomDays).ToString("yyyy-MM-ddTHH:mm:ssZ");
                }

                if (column.Choice != null)
                {
                    if (column.Choice.Choices != null && column.Choice.Choices.Count > 0)
                    {
                        var randomIndex = _random.Next(0, column.Choice.Choices.Count);
                        return column.Choice.Choices[randomIndex];
                    }
                    return "Option 1";
                }

                if (column.Boolean != null)
                {
                    return _random.Next(0, 2) == 1;
                }

                if (column.Currency != null)
                {
                    return Math.Round(_random.NextDouble() * 10000, 2);
                }

                if (column.HyperlinkOrPicture != null)
                {
                    return new
                    {
                        Url = $"https://example.com/link{rowNumber}",
                        Description = $"Link {rowNumber}"
                    };
                }

                // Default for unknown types
                return $"Test value {rowNumber}";
            }
            catch
            {
                return null;
            }
        }
    }
}
