# Project Architecture

## Overview

This project follows a clean architecture pattern with separation of concerns, making it easy to maintain and extend.

## Solution Structure

```
SharePointListsGraphApi/
?
??? Program.cs                              # Main entry point and menu orchestration
??? SharePointSettings.cs                   # Configuration model
??? appsettings.json                        # User configuration file
??? sample-columns.csv                      # Example CSV template
?
??? Services/                               # Business logic layer
?   ??? SharePointService.cs                # Site and list operations
?   ??? ColumnManagementService.cs          # Column creation from CSV
?   ??? TestDataGeneratorService.cs         # Random data generation
?   ??? ListExportService.cs                # List export to JSON
?
??? Helpers/                                # Utility classes
?   ??? MenuHelper.cs                       # Menu display and user input
?   ??? ValidationHelper.cs                 # Settings validation
?
??? Documentation/
    ??? README.md                           # Project overview and setup
    ??? USER-GUIDE.md                       # Detailed usage instructions
    ??? TROUBLESHOOTING.md                  # Common issues and solutions
```

## Layer Responsibilities

### Program.cs
- Application entry point
- Configuration loading
- Menu loop management
- Service orchestration
- High-level error handling

### Services Layer
Each service handles a specific domain:

**SharePointService.cs**
- Authentication testing
- Site retrieval
- List retrieval
- Connection management

**ColumnManagementService.cs**
- CSV file parsing
- Column definition creation
- Column type mapping
- Batch column creation
- Progress reporting

**TestDataGeneratorService.cs**
- List schema retrieval
- Random data generation by type
- Bulk item creation
- Progress tracking

**ListExportService.cs**
- Paginated data retrieval
- Data transformation
- JSON serialization
- File I/O operations

### Helpers Layer
**MenuHelper.cs**
- Console UI rendering
- User input validation
- Display formatting
- Progress indicators

**ValidationHelper.cs**
- Configuration validation
- Input sanitization

## Data Flow

### 1. Add Columns Flow
```
User Input (CSV path)
    ?
MenuHelper.GetFilePathInput()
    ?
ColumnManagementService.AddColumnsFromCsvAsync()
    ?
ParseCsvFile() ? List<CsvColumn>
    ?
For each column:
    CreateColumnDefinition() ? ColumnDefinition
    ?
    GraphClient.Sites[].Lists[].Columns.PostAsync()
    ?
    Progress update
    ?
Return success count
```

### 2. Generate Test Data Flow
```
User Input (number of rows)
    ?
MenuHelper.GetNumberInput()
    ?
TestDataGeneratorService.GenerateTestDataAsync()
    ?
GetListColumnsAsync() ? List<ColumnDefinition>
    ?
For each row:
    GenerateRandomData() ? Dictionary<string, object>
    ?
    GraphClient.Sites[].Lists[].Items.PostAsync()
    ?
    Progress update
    ?
Return success count
```

### 3. Export List Flow
```
User selects export
    ?
ListExportService.DownloadAllListItemsAsync()
    ?
GraphClient.Sites[].Lists[].Items.GetAsync()
    ?
While has pages:
    Process items ? Add to List<Dictionary<string, object>>
    ?
    Get next page if exists
    ?
SaveToJsonFileAsync()
    ?
File.WriteAllTextAsync()
```

## Key Design Patterns

### 1. Dependency Injection
Services receive `GraphServiceClient` via constructor:
```csharp
public class SharePointService
{
    private readonly GraphServiceClient _graphClient;
    
    public SharePointService(GraphServiceClient graphClient)
    {
        _graphClient = graphClient;
    }
}
```

### 2. Single Responsibility Principle
Each service has one clear purpose:
- `SharePointService` ? Site/List operations only
- `ColumnManagementService` ? Column operations only
- etc.

### 3. Separation of Concerns
- UI logic ? `MenuHelper`
- Business logic ? `Services`
- Configuration ? `appsettings.json` + `SharePointSettings`
- Validation ? `ValidationHelper`

### 4. Error Handling Strategy
```
Try-Catch at operation level (Services)
    ?
Report to console with context
    ?
Continue processing or return to menu
    ?
Fatal errors caught in Main()
```

## Extension Points

### Adding New Features

1. **Create a new service** in `Services/` folder
   ```csharp
   internal class MyNewService
   {
       private readonly GraphServiceClient _graphClient;
       
       public MyNewService(GraphServiceClient graphClient)
       {
           _graphClient = graphClient;
       }
       
       public async Task<int> DoSomethingAsync()
       {
           // Implementation
       }
   }
   ```

2. **Add menu option** in `MenuHelper.DisplayMainMenu()`
   ```csharp
   Console.WriteLine("  5. My New Feature");
   ```

3. **Add handler** in `Program.Main()`
   ```csharp
   case 5:
       await MyNewFeatureAsync();
       break;
   ```

4. **Implement handler** in `Program.cs`
   ```csharp
   static async Task MyNewFeatureAsync()
   {
       try
       {
           MenuHelper.DisplayHeader("My New Feature");
           var service = new MyNewService(_graphClient!);
           await service.DoSomethingAsync();
       }
       catch (Exception ex)
       {
           MenuHelper.DisplayError($"Error: {ex.Message}");
       }
   }
   ```

### Adding New Column Types

Edit `ColumnManagementService.CreateColumnDefinition()`:
```csharp
case "my new type":
    column.MyProperty = new MyColumn { };
    break;
```

Add handling in `TestDataGeneratorService.GenerateRandomValueForColumn()`:
```csharp
if (column.MyProperty != null)
{
    return GenerateMyTypeData(rowNumber);
}
```

### Adding New Export Formats

1. Create new method in `ListExportService`:
   ```csharp
   public async Task SaveToCsvAsync(List<Dictionary<string, object>> items, string fileName)
   {
       // CSV serialization
   }
   ```

2. Add option to export menu for format selection

## Configuration Management

### appsettings.json Structure
```json
{
  "SharePointSettings": {
    "TenantId": "...",      // Azure AD tenant
    "ClientId": "...",       // App registration
    "ClientSecret": "...",   // App secret
    "SiteUrl": "...",        // SharePoint site URL
    "ListName": "...",       // Target list name
    "Columns": "*"           // Export columns (or specific names)
  }
}
```

### Adding New Settings

1. Update `SharePointSettings.cs`:
   ```csharp
   public string MyNewSetting { get; set; } = string.Empty;
   ```

2. Update `appsettings.json`:
   ```json
   "MyNewSetting": "value"
   ```

3. Update validation in `ValidationHelper.ValidateSettings()`

## Testing Strategy

### Manual Testing Checklist

**Authentication:**
- [ ] Valid credentials work
- [ ] Invalid credentials show clear error
- [ ] Expired secret shows helpful message

**Add Columns:**
- [ ] Valid CSV imports correctly
- [ ] Invalid CSV shows error
- [ ] Duplicate columns are handled
- [ ] All column types work

**Generate Data:**
- [ ] Small batches (10 rows) work
- [ ] Large batches (1000+ rows) work
- [ ] All data types generate correctly
- [ ] Progress updates properly

**Export:**
- [ ] Empty list exports
- [ ] Small list exports
- [ ] Large list with pagination exports
- [ ] Specific columns export correctly
- [ ] All columns (*) export correctly

### Unit Testing (Future Enhancement)

Recommended test structure:
```
SharePointListsGraphApi.Tests/
??? Services/
?   ??? ColumnManagementServiceTests.cs
?   ??? TestDataGeneratorServiceTests.cs
?   ??? ListExportServiceTests.cs
??? Helpers/
    ??? ValidationHelperTests.cs
```

## Performance Considerations

### Current Optimizations

1. **Pagination**: Handles large lists without memory issues
2. **Progress Indicators**: Provides user feedback during long operations
3. **Batch Processing**: Processes items in chunks from Graph API

### Potential Improvements

1. **Parallel Processing**: Could batch create items in parallel (within Graph API limits)
2. **Caching**: Could cache site/list metadata
3. **Retry Logic**: Could add exponential backoff for rate limiting
4. **Streaming**: Could stream JSON output for very large exports

### Graph API Limits

- **Throttling**: 2,000 requests per minute per tenant
- **Pagination**: Max 5,000 items per page (Graph returns typically 200)
- **Timeout**: 180 seconds per request

## Security Considerations

### Implemented
- ? Client secret stored in config file (not hardcoded)
- ? Application permissions (not delegated)
- ? Validation of all inputs
- ? Error messages don't expose sensitive data

### Recommended
- ?? Use Azure Key Vault for secrets
- ?? Enable certificate-based authentication
- ?? Implement audit logging
- ?? Add user confirmation for destructive operations

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.Graph | 5.91.0 | Graph API client |
| Azure.Identity | 1.13.1 | Authentication |
| Microsoft.Extensions.Configuration | 9.0.0 | Config loading |
| Microsoft.Extensions.Configuration.Json | 9.0.0 | JSON config |
| Microsoft.Extensions.Configuration.Binder | 9.0.0 | Config binding |

## Future Enhancements

### Planned Features
- [ ] Update existing list items
- [ ] Delete list items with filters
- [ ] Import data from JSON/CSV
- [ ] List comparison tool
- [ ] Scheduled exports
- [ ] Email notifications
- [ ] Logging to file
- [ ] Configuration profiles

### Architecture Improvements
- [ ] Add logging framework (Serilog)
- [ ] Implement repository pattern
- [ ] Add unit tests
- [ ] Add integration tests
- [ ] Implement async/await best practices throughout
- [ ] Add cancellation token support
- [ ] Implement proper dispose pattern

## Maintenance Guide

### Adding a NuGet Package
```bash
dotnet add package PackageName --version X.Y.Z
```

### Updating Dependencies
```bash
dotnet list package --outdated
dotnet add package Microsoft.Graph --version [latest]
```

### Building
```bash
dotnet build
dotnet build --configuration Release
```

### Publishing
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## Version History

**v1.0.0** - Initial release
- Add columns from CSV
- Generate random test data
- Export list to JSON
- Menu-driven interface

## License

[Specify your license here]

## Contributors

[List contributors here]
