using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Azure.Identity;
using SharePointListsGraphApi.Services;
using SharePointListsGraphApi.Helpers;

namespace SharePointListsGraphApi
{
    internal class Program
    {
        private static GraphServiceClient? _graphClient;
        private static SharePointSettings? _settings;
        private static Site? _currentSite;
        private static List? _currentList;

        static async Task Main(string[] args)
        {
            try
            {
                // Load configuration
                await InitializeAsync();

                // Main menu loop
                bool exit = false;
                while (!exit)
                {
                    MenuHelper.DisplayMainMenu();
                    var choice = MenuHelper.GetMenuChoice();

                    switch (choice)
                    {
                        case 1:
                            await AddColumnsFromCsvAsync();
                            break;
                        case 2:
                            await GenerateTestDataAsync();
                            break;
                        case 3:
                            await ExportListToJsonAsync();
                            break;
                        case 4:
                            exit = true;
                            Console.WriteLine("\nGoodbye!");
                            break;
                    }

                    if (!exit)
                    {
                        MenuHelper.PressKeyToContinue();
                    }
                }
            }
            catch (Exception ex)
            {
                MenuHelper.DisplayError($"Fatal error: {ex.Message}");
                Console.WriteLine($"\nStack Trace: {ex.StackTrace}");
                MenuHelper.PressKeyToContinue();
            }
        }

        static async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _settings = configuration.GetSection("SharePointSettings").Get<SharePointSettings>();

            if (_settings == null)
            {
                throw new Exception("Error: Unable to load settings from appsettings.json");
            }

            ValidationHelper.ValidateSettings(_settings);

            // Create Graph client
            var clientSecretCredential = new ClientSecretCredential(
                _settings.TenantId,
                _settings.ClientId,
                _settings.ClientSecret);

            _graphClient = new GraphServiceClient(clientSecretCredential);

            // Initialize services and get site/list info
            var sharePointService = new SharePointService(_graphClient);
            
            MenuHelper.DisplayInfo($"Site: {_settings.SiteUrl}");
            MenuHelper.DisplayInfo($"List: {_settings.ListName}");
            Console.WriteLine();

            await sharePointService.TestAuthenticationAsync();
            
            _currentSite = await sharePointService.GetSiteAsync(_settings.SiteUrl);
            _currentList = await sharePointService.GetListAsync(_currentSite.Id!, _settings.ListName);
            
            Console.WriteLine();
        }

        static async Task AddColumnsFromCsvAsync()
        {
            try
            {
                MenuHelper.DisplayHeader("Add Columns from CSV File");

                var csvPath = MenuHelper.GetFilePathInput("Enter the path to the CSV file: ");

                if (!File.Exists(csvPath))
                {
                    MenuHelper.DisplayError($"File not found: {csvPath}");
                    return;
                }

                var columnService = new ColumnManagementService(_graphClient!);
                var count = await columnService.AddColumnsFromCsvAsync(
                    _currentSite!.Id!, 
                    _currentList!.Id!, 
                    csvPath);

                Console.WriteLine();
                MenuHelper.DisplaySuccess($"Operation completed. {count} columns added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                MenuHelper.DisplayError($"Error adding columns: {ex.Message}");
            }
        }

        static async Task GenerateTestDataAsync()
        {
            try
            {
                MenuHelper.DisplayHeader("Generate Random Test Data");

                var numberOfRows = MenuHelper.GetNumberInput(
                    "Enter the number of test rows to create (1-60000): ", 
                    1, 
                    60000);

                Console.WriteLine();
                MenuHelper.DisplayInfo($"This will create {numberOfRows} rows with random test data.");
                Console.Write("Continue? (y/n): ");
                
                var confirm = Console.ReadLine()?.ToLower();
                if (confirm != "y" && confirm != "yes")
                {
                    MenuHelper.DisplayInfo("Operation cancelled.");
                    return;
                }

                // Ask about parallel processing
                Console.WriteLine();
                Console.Write("Do you want to use parallel processing for faster uploads? (y/n): ");
                var useParallel = Console.ReadLine()?.ToLower();
                
                int parallelTasks = 1;
                if (useParallel == "y" || useParallel == "yes")
                {
                    parallelTasks = MenuHelper.GetNumberInput(
                        "How many parallel tasks do you want to run (1-10, recommended: 3-5)? ", 
                        1, 
                        10);
                }

                var testDataService = new TestDataGeneratorService(_graphClient!);
                var count = await testDataService.GenerateTestDataAsync(
                    _currentSite!.Id!, 
                    _currentList!.Id!, 
                    numberOfRows,
                    parallelTasks);

                Console.WriteLine();
                MenuHelper.DisplaySuccess($"Operation completed. {count} rows created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                MenuHelper.DisplayError($"Error generating test data: {ex.Message}");
            }
        }

        static async Task ExportListToJsonAsync()
        {
            try
            {
                MenuHelper.DisplayHeader("Export List to JSON File");

                MenuHelper.DisplayInfo($"Columns to export: {_settings!.Columns}");
                MenuHelper.DisplayInfo("You can change this in appsettings.json");
                Console.WriteLine();

                var exportService = new ListExportService(_graphClient!);
                
                var allItems = await exportService.DownloadAllListItemsAsync(
                    _currentSite!.Id!, 
                    _currentList!.Id!, 
                    _settings!.Columns);

                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}.json";
                await exportService.SaveToJsonFileAsync(allItems, fileName);

                Console.WriteLine();
                MenuHelper.DisplaySuccess($"Export completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                MenuHelper.DisplayError($"Error exporting list: {ex.Message}");
            }
        }
    }
}
