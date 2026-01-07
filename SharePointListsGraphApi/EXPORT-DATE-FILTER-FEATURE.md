# Export Date Filter Feature

## Overview

The Export List to JSON feature now includes an **interactive date filter** based on the SharePoint item's **Modified** metadata field. This allows users to export only items that were modified within a specific date range.

## Key Features

### 1. Interactive Prompting
- When exporting, users are asked: "Do you want to filter by Modified date?"
- If **No**: All items are exported (no filtering)
- If **Yes**: Date range prompts appear with smart defaults

### 2. Smart Default Values
- **From Date**: Today at 00:00:00 (start of day)
- **To Date**: Today at 23:59:59 (end of day)
- Users can **press Enter** to accept defaults or **type custom dates**

### 3. Flexible Date Input
- Format: `yyyy-MM-dd HH:mm:ss`
- Examples:
  - `2024-01-01 00:00:00` - Specific date at midnight
  - `2024-12-15 14:30:00` - Specific date and time
  - Just press **Enter** - Use default (today)

### 4. OData Filter Application
- Filters are applied via Microsoft Graph API using OData syntax
- Query format: `fields/Modified ge 'fromDate' and fields/Modified le 'toDate'`
- Dates are converted to UTC before sending to API

## User Experience

### Example Session 1: Daily Export (Accept Defaults)
```
??????????????????????????????????????????????????????????????
?           Export List to JSON File                         ?
??????????????????????????????????????????????????????????????

? Columns to export: *
? You can change this in appsettings.json

Do you want to filter by Modified date? (y/n): y

Default date range (press Enter to accept, or type new values):
  From: 2024-12-15 00:00:00
  To:   2024-12-15 23:59:59

From date and time [default: 2024-12-15 00:00:00]: [ENTER]
To date and time [default: 2024-12-15 23:59:59]: [ENTER]

? Applying filter: 2024-12-15 00:00:00 to 2024-12-15 23:59:59

Downloading list items...

Applying date filter on Modified field:
  From: 2024-12-15 00:00:00
  To: 2024-12-15 23:59:59

| Downloading... Page #1 (50 items) | Total: 50 items

? Total items downloaded: 50
Saving to 20241215143022.json...
? Successfully saved 50 items to 20241215143022.json

? Export completed successfully!
```

### Example Session 2: Custom Date Range
```
Do you want to filter by Modified date? (y/n): y

Default date range (press Enter to accept, or type new values):
  From: 2024-12-15 00:00:00
  To:   2024-12-15 23:59:59

From date and time [default: 2024-12-15 00:00:00]: 2024-12-01 00:00:00
To date and time [default: 2024-12-15 23:59:59]: 2024-12-31 23:59:59

? Applying filter: 2024-12-01 00:00:00 to 2024-12-31 23:59:59

Downloading list items...

Applying date filter on Modified field:
  From: 2024-12-01 00:00:00
  To: 2024-12-31 23:59:59

| Downloading... Page #1 (200 items) | Total: 200 items
\ Downloading... Page #2 (200 items) | Total: 400 items

? Total items downloaded: 400
```

### Example Session 3: No Filtering
```
Do you want to filter by Modified date? (y/n): n

? No date filter applied. Downloading all items.

Downloading list items...

| Downloading... Page #1 (200 items) | Total: 200 items
/ Downloading... Page #2 (200 items) | Total: 400 items
- Downloading... Page #3 (200 items) | Total: 600 items

? Total items downloaded: 600
```

## Use Cases

### 1. Daily Exports
**Scenario**: Export items modified today for daily reporting

**Steps**:
1. Select Export option
2. Answer "y" to filter
3. Press Enter twice (accept today's defaults)
4. Result: Only today's modifications are exported

**Benefit**: Smaller, focused exports for daily processing

### 2. Date Range Analysis
**Scenario**: Analyze all changes made last week

**Steps**:
1. Select Export option
2. Answer "y" to filter
3. Enter: `2024-12-08 00:00:00`
4. Enter: `2024-12-14 23:59:59`
5. Result: One week of modifications

**Benefit**: Historical analysis without downloading entire list

### 3. Full Backup
**Scenario**: Complete backup of all list items

**Steps**:
1. Select Export option
2. Answer "n" to filter
3. Result: All items exported

**Benefit**: Traditional full export when needed

### 4. Recent Changes
**Scenario**: Check recent activity in the last hour

**Steps**:
1. Select Export option
2. Answer "y" to filter
3. Enter: `2024-12-15 13:00:00`
4. Enter: `2024-12-15 14:00:00`
5. Result: Only items modified between 1-2 PM

**Benefit**: Audit recent changes or troubleshoot issues

## Technical Implementation

### Code Changes

#### 1. SharePointSettings.cs
- **Removed**: DateFilter class and property
- **Reason**: Filtering is now interactive, not configuration-based

#### 2. ListExportService.cs
```csharp
public async Task<List<Dictionary<string, object>>> DownloadAllListItemsAsync(
    string siteId, 
    string listId, 
    string columns,
    DateTime? fromDate = null,      // NEW: Optional from date
    DateTime? toDate = null)        // NEW: Optional to date
{
    // Build OData filter if dates provided
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
    // ... rest of implementation
}
```

#### 3. Program.cs
```csharp
static async Task ExportListToJsonAsync()
{
    DateTime? fromDate = null;
    DateTime? toDate = null;

    // Prompt user for filtering
    Console.Write("Do you want to filter by Modified date? (y/n): ");
    var applyFilter = Console.ReadLine()?.ToLower();
    
    if (applyFilter == "y" || applyFilter == "yes")
    {
        // Set defaults to today
        var defaultFromDate = DateTime.Today;
        var defaultToDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        
        // Display defaults and get user input
        fromDate = GetDateTimeInputWithDefault(
            $"From date and time [default: {defaultFromDate:yyyy-MM-dd HH:mm:ss}]: ",
            defaultFromDate);
        
        toDate = GetDateTimeInputWithDefault(
            $"To date and time [default: {defaultToDate:yyyy-MM-dd HH:mm:ss}]: ",
            defaultToDate);
    }

    // Call export service with date parameters
    var allItems = await exportService.DownloadAllListItemsAsync(
        _currentSite!.Id!, 
        _currentList!.Id!, 
        _settings!.Columns,
        fromDate,
        toDate);
}

static DateTime GetDateTimeInputWithDefault(string prompt, DateTime defaultValue)
{
    while (true)
    {
        Console.Write(prompt);
        var input = Console.ReadLine()?.Trim();
        
        // Empty input = use default
        if (string.IsNullOrWhiteSpace(input))
            return defaultValue;
        
        // Try to parse custom input
        if (DateTime.TryParse(input, out DateTime result))
            return result;
        
        MenuHelper.DisplayError("Invalid date format. Please use yyyy-MM-dd HH:mm:ss format.");
    }
}
```

### OData Filter Syntax

The filter is constructed as:
```
fields/Modified ge '2024-12-15T00:00:00Z' and fields/Modified le '2024-12-15T23:59:59Z'
```

**Breakdown**:
- `fields/Modified` - SharePoint's Modified metadata field
- `ge` - Greater than or equal to (>=)
- `le` - Less than or equal to (<=)
- `and` - Logical AND operator
- Dates are in ISO 8601 format with UTC timezone

## Configuration Changes

### appsettings.json (Before)
```json
{
  "SharePointSettings": {
    "TenantId": "...",
    "ClientId": "...",
    "ClientSecret": "...",
    "SiteUrl": "...",
    "ListName": "...",
    "Columns": "*",
    "DateFilter": {
      "FromDate": "2024-01-01T00:00:00",
      "ToDate": "2024-12-31T23:59:59"
    }
  }
}
```

### appsettings.json (After)
```json
{
  "SharePointSettings": {
    "TenantId": "...",
    "ClientId": "...",
    "ClientSecret": "...",
    "SiteUrl": "...",
    "ListName": "...",
    "Columns": "*"
  }
}
```

**Key Change**: DateFilter section removed - filtering is now done interactively at runtime

## Important Notes for Developers

### 1. Copy to Output Directory Setting
?? **CRITICAL**: `appsettings.json` must be set to **Copy to Output Directory**

**Why**: The application looks for appsettings.json at runtime in the output directory. Without this setting, the file won't be found and the app will crash.

**How to Set**:
- **Visual Studio**: Right-click `appsettings.json` ? Properties ? **Copy to Output Directory**: "Copy if newer"
- **Or in .csproj file**:
  ```xml
  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
  ```

### 2. Modified Field
- **Field Name**: `Modified` (system field, always exists)
- **Type**: DateTime
- **Behavior**: Automatically updated by SharePoint when item changes
- **Timezone**: Stored in UTC, automatically converted by Graph API

### 3. Date Parsing
- Uses `DateTime.TryParse()` which supports multiple formats
- Recommended format: `yyyy-MM-dd HH:mm:ss`
- User's local timezone is used, then converted to UTC for API

### 4. Performance
- Filtering reduces data transferred from SharePoint
- Smaller result sets = faster downloads
- Recommended for large lists (>1000 items)

## Documentation Updates

All documentation has been updated to reflect this feature:

? **SETUP.md** - Added Copy to Output Directory warning  
? **USER-GUIDE.md** - Complete date filtering section  
? **README.md** - Feature summary updated  
? **QUICKSTART.md** - Quick test instructions  
? **ARCHITECTURE.md** - Data flow diagram updated  

## Testing Recommendations

### Test Scenarios

1. **Accept Defaults**
   - Answer "y"
   - Press Enter twice
   - Verify only today's items exported

2. **Custom Range**
   - Answer "y"
   - Enter custom dates
   - Verify correct range exported

3. **No Filter**
   - Answer "n"
   - Verify all items exported

4. **Invalid Date**
   - Enter invalid format
   - Verify error message shows
   - Verify re-prompt occurs

5. **Empty Date**
   - Press Enter on prompts
   - Verify defaults used

6. **Edge Cases**
   - From date > To date (should still work, returns 0 items)
   - Future dates (returns 0 items)
   - Very old dates (returns all or matching items)

## Future Enhancements

Potential improvements for future versions:

1. **Additional Fields**
   - Filter by Created date
   - Filter by custom date columns

2. **Preset Ranges**
   - "Today", "Yesterday", "This Week", "Last 7 Days"
   - Menu selection instead of manual entry

3. **Save Filters**
   - Save commonly used date ranges
   - Quick selection from saved filters

4. **Multiple Criteria**
   - Combine date filter with other field filters
   - Advanced query builder

5. **Schedule**
   - Automated exports with saved filters
   - Email results

## Troubleshooting

### Issue: "No items found" but items exist
**Cause**: Date range doesn't match any Modified dates  
**Solution**: Try expanding date range or remove filter

### Issue: Invalid date format error
**Cause**: Date entered in wrong format  
**Solution**: Use `yyyy-MM-dd HH:mm:ss` format exactly

### Issue: Timezone confusion
**Cause**: User timezone vs SharePoint timezone  
**Solution**: Application handles conversion automatically. Enter dates in your local time.

### Issue: Some items missing
**Cause**: Items not modified in the date range  
**Solution**: Remember this filters by Modified, not Created. Use no filter to get all items.

## Summary

The interactive date filter feature provides:
- ? **Flexibility** - Filter or don't filter per export
- ? **Convenience** - Smart defaults for common use case (today)
- ? **Power** - Custom date ranges for specific needs
- ? **Performance** - Reduced data transfer for large lists
- ? **Simplicity** - No configuration file changes needed

This feature makes the export function more versatile while maintaining ease of use for both daily operations and specialized reporting needs.
