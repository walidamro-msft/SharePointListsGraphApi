using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace SharePointListsGraphApi.Services
{
    internal class ColumnManagementService
    {
        private readonly GraphServiceClient _graphClient;

        public ColumnManagementService(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<int> AddColumnsFromCsvAsync(string siteId, string listId, string csvFilePath)
        {
            if (!File.Exists(csvFilePath))
            {
                throw new FileNotFoundException($"CSV file not found: {csvFilePath}");
            }

            Console.WriteLine($"\nReading columns from {csvFilePath}...");
            
            var columnsToAdd = ParseCsvFile(csvFilePath);
            
            Console.WriteLine($"Found {columnsToAdd.Count} columns to add.\n");

            int successCount = 0;
            int failureCount = 0;

            for (int i = 0; i < columnsToAdd.Count; i++)
            {
                var column = columnsToAdd[i];
                var progress = (i + 1) * 100 / columnsToAdd.Count;
                
                Console.Write($"\r[{new string('?', progress / 5)}{new string('?', 20 - progress / 5)}] {progress}% - Adding column '{column.Name}'...");
                
                try
                {
                    var columnDefinition = CreateColumnDefinition(column.Name, column.Type);
                    await _graphClient.Sites[siteId].Lists[listId].Columns.PostAsync(columnDefinition);
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n? Failed to add column '{column.Name}': {ex.Message}");
                    failureCount++;
                }
            }

            Console.WriteLine($"\n\n? Successfully added {successCount} columns");
            if (failureCount > 0)
            {
                Console.WriteLine($"? Failed to add {failureCount} columns");
            }

            return successCount;
        }

        private List<CsvColumn> ParseCsvFile(string csvFilePath)
        {
            var columns = new List<CsvColumn>();
            var lines = File.ReadAllLines(csvFilePath);

            if (lines.Length == 0)
            {
                throw new Exception("CSV file is empty");
            }

            // Detect delimiter from first line (header)
            var firstLine = lines[0];
            char delimiter = DetectDelimiter(firstLine);
            
            Console.WriteLine($"Detected delimiter: '{(delimiter == '\t' ? "TAB" : delimiter.ToString())}'");

            // Skip header row
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(delimiter);
                if (parts.Length >= 2)
                {
                    var name = parts[0].Trim();
                    var type = parts[1].Trim();
                    
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        columns.Add(new CsvColumn { Name = name, Type = type });
                    }
                }
                else if (parts.Length == 1 && !string.IsNullOrWhiteSpace(parts[0]))
                {
                    Console.WriteLine($"\n? Warning: Line {i + 1} has only one column. Expected format: 'Column Name{delimiter}Column Type'");
                }
            }

            if (columns.Count == 0)
            {
                throw new Exception($"No valid columns found in CSV file. Make sure the format is:\n" +
                    $"Column Name{delimiter}Column Type\n" +
                    $"Example:\n" +
                    $"FirstName{delimiter}Single line of text\n" +
                    $"BirthDate{delimiter}Date and Time");
            }

            return columns;
        }

        private char DetectDelimiter(string headerLine)
        {
            // Check for common delimiters
            if (headerLine.Contains('\t'))
                return '\t';
            if (headerLine.Contains(','))
                return ',';
            if (headerLine.Contains(';'))
                return ';';
            if (headerLine.Contains('|'))
                return '|';
            
            // Default to comma
            return ',';
        }

        private ColumnDefinition CreateColumnDefinition(string name, string type)
        {
            var column = new ColumnDefinition
            {
                Name = name,
                DisplayName = name
            };

            // Normalize the type string
            var normalizedType = type.ToLower().Trim();

            switch (normalizedType)
            {
                case "single line of text":
                case "text":
                case "string":
                case "singleline":
                    column.Text = new TextColumn { };
                    break;

                case "multiple lines of text":
                case "multiline":
                case "note":
                case "multilinetext":
                    column.Text = new TextColumn
                    {
                        AllowMultipleLines = true
                    };
                    break;

                case "number":
                case "integer":
                case "int":
                case "decimal":
                    column.Number = new NumberColumn { };
                    break;

                case "date and time":
                case "date":
                case "datetime":
                case "timestamp":
                    column.DateTime = new DateTimeColumn
                    {
                        Format = "dateTime"
                    };
                    break;

                case "choice":
                case "dropdown":
                case "select":
                    column.Choice = new ChoiceColumn
                    {
                        AllowTextEntry = false,
                        Choices = new List<string> { "Option 1", "Option 2", "Option 3" }
                    };
                    break;

                case "person or group":
                case "person":
                case "user":
                case "people":
                    column.PersonOrGroup = new PersonOrGroupColumn
                    {
                        AllowMultipleSelection = false
                    };
                    break;

                case "yes/no":
                case "boolean":
                case "bool":
                case "checkbox":
                    column.Boolean = new BooleanColumn { };
                    break;

                case "hyperlink":
                case "hyperlink or picture":
                case "url":
                case "link":
                    column.HyperlinkOrPicture = new HyperlinkOrPictureColumn { };
                    break;

                case "currency":
                case "money":
                case "price":
                    column.Currency = new CurrencyColumn { };
                    break;

                default:
                    // Default to text column and show warning
                    Console.WriteLine($"\n? Warning: Unknown column type '{type}' for column '{name}'. Using 'Text' as default.");
                    column.Text = new TextColumn { };
                    break;
            }

            return column;
        }

        private class CsvColumn
        {
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
        }
    }
}

