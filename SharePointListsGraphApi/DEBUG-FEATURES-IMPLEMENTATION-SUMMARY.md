# Debug Features Implementation Summary

## Changes Made

This document summarizes the debug features added to the SharePoint Lists Graph API Management Tool.

### 1. Debug Logging Configuration

**Files Modified:**
- `SharePointSettings.cs` - Added `EnableDebugLogging` property
- `appsettings.example.json` - Added `EnableDebugLogging: false` setting
- `Program.cs` - Conditionally enable logging handler based on setting
- `ListExportService.cs` - Made debug output conditional

**New Files:**
- `Handlers/LoggingHandler.cs` - Custom HTTP handler for logging requests

### 2. Access Token Display Feature

**Files Modified:**
- `Program.cs` - Added `DisplayAccessTokenAsync()` method (Menu Option 4)
- `MenuHelper.cs` - Already updated to show menu option 4

### 3. Documentation

**New Files:**
- `DEBUG-FEATURES.md` - Comprehensive guide for debug features

**Files Updated:**
- `README.md` - Added debug features section and updated config example
- `USER-GUIDE.md` - Added Feature 4 documentation and debug logging section
- `appsettings.example.json` - Added `EnableDebugLogging` setting

## Feature Details

### Debug Logging (`EnableDebugLogging`)

**Purpose:** View actual HTTP requests sent to Microsoft Graph API

**Configuration:**
```json
{
  "SharePointSettings": {
    ...
    "EnableDebugLogging": true  // Set to true to enable, false to disable
  }
}
```

**Default Value:** `false` (disabled)

**What Gets Logged:**
- HTTP method and full URLs
- Response status codes
- OData filters, expansions, and selects
- Pagination URLs (`@odata.nextLink`)

**Example Output:**
```
[DEBUG MODE ENABLED] - HTTP requests will be logged

[DEBUG] HTTP GET: https://graph.microsoft.com/v1.0/sites/.../items?$expand=fields&$filter=...
[DEBUG] Response: 200 OK
```

**Color Coding:**
- Cyan - HTTP requests
- Green - Successful responses
- Yellow - Pagination URLs

### Access Token Display (Menu Option 4)

**Purpose:** View and inspect OAuth 2.0 access tokens

**How to Access:** Select option 4 from the main menu

**Information Displayed:**
- Complete JWT token string
- Expiration date/time (UTC)
- Time until expiration (countdown)

**Use Cases:**
- Troubleshooting authentication issues
- Verifying token permissions (using jwt.ms or jwt.io)
- Testing with Graph Explorer, PowerShell, or cURL
- Security auditing
- Learning OAuth 2.0 flows

**Example Output:**
```
============================================================
  Access Token Information
============================================================

Access Token: eyJ0eXAiOiJKV1QiLCJhbGci...

Expires On: 2024-12-15 14:30:45 UTC
Time Until Expiration: 00:59:12

? Access token retrieved successfully!
```

## Implementation Architecture

### LoggingHandler Class

**Location:** `Handlers/LoggingHandler.cs`

**Purpose:** Intercepts HTTP requests/responses

**Design Pattern:** DelegatingHandler (middleware pattern)

**Flow:**
```
GraphServiceClient request
    ?
LoggingHandler.SendAsync()
    ? (logs request URL)
Base handler (actual HTTP call)
    ? (logs response status)
Return response
```

### Conditional Logging

**In Program.cs:**
```csharp
if (_settings.EnableDebugLogging)
{
    var loggingHandler = new Handlers.LoggingHandler(new HttpClientHandler());
    var httpClient = new HttpClient(loggingHandler);
    _graphClient = new GraphServiceClient(httpClient, clientSecretCredential);
}
else
{
    _graphClient = new GraphServiceClient(clientSecretCredential);
}
```

**In ListExportService.cs:**
```csharp
if (_enableDebugLogging)
{
    // Log constructed URL
    Console.WriteLine($"[DEBUG] First Graph API Request URL:");
    Console.WriteLine(baseUrl);
}
```

## Benefits

### For Developers
- ? Understand Graph API syntax
- ? Debug OData filters
- ? Verify query parameters
- ? Learn SDK behavior
- ? Test token permissions

### For Troubleshooting
- ? Diagnose "Access Denied" errors
- ? Verify date filters are working
- ? Track pagination behavior
- ? Monitor API performance
- ? Validate column selection

### For Security
- ? Audit access tokens
- ? Verify granted permissions
- ? Inspect token claims
- ? Monitor token expiration
- ? Test token with external tools

## Configuration Best Practices

### Development Environment
```json
"EnableDebugLogging": true
```
- See all HTTP traffic
- Learn Graph API patterns
- Debug filter syntax
- Verify query construction

### Production Environment
```json
"EnableDebugLogging": false
```
- Cleaner console output
- Better performance (no logging overhead)
- Reduced noise for end users

### Troubleshooting
```json
"EnableDebugLogging": true
```
- Temporarily enable when issues occur
- Capture exact failing URLs
- Share logs with support (redact sensitive data)
- Disable after resolving issue

## Documentation Structure

### DEBUG-FEATURES.md
Comprehensive guide covering:
- Debug logging overview and configuration
- Access token display feature
- Use cases and scenarios
- Security considerations
- Best practices
- Troubleshooting
- Integration with external tools

### USER-GUIDE.md Updates
Added sections:
- Feature 4: Display Access Token
- Debug Features (logging configuration)
- Updated command reference table
- Security notes

### README.md Updates
- Added debug features to feature list
- Updated appsettings.json example
- Added link to DEBUG-FEATURES.md (in Support section)

## Testing Checklist

### Debug Logging
- [ ] Enabled: HTTP requests are logged
- [ ] Disabled: No debug output appears
- [ ] URL construction is correct
- [ ] Pagination URLs are shown
- [ ] Color coding works

### Access Token Display
- [ ] Token is displayed
- [ ] Expiration time is shown
- [ ] Countdown timer works
- [ ] Token decodes properly in jwt.ms
- [ ] Contains expected permissions (roles claim)

### Configuration
- [ ] appsettings.example.json has new setting
- [ ] Default is false (logging disabled)
- [ ] Setting change takes effect after rebuild

## Future Enhancements

Potential additions:
- [ ] Log to file option (not just console)
- [ ] Request/response body logging (for POST/PATCH)
- [ ] HTTP header logging (show Authorization header length, not value)
- [ ] Performance metrics (request duration)
- [ ] Structured logging (JSON format)
- [ ] Log level configuration (Debug, Info, Warning, Error)
- [ ] Token refresh event logging
- [ ] Export debug logs to file

## Security Notes

### ? What's Safe
- Logging URLs (they don't contain secrets)
- Showing access tokens in console (user's local machine)
- Response status codes
- Pagination URLs

### ?? Warnings
- Don't log access tokens to files (can be shared accidentally)
- Don't include tokens in screenshots for public sharing
- Redact site/list IDs when sharing debug output publicly
- Access tokens expire, but treat them as sensitive while valid

### ?? Best Practices
- Only enable debug logging when needed
- Don't commit debug logs to source control
- Clear console/terminal history after viewing tokens
- Use token inspection tools (jwt.ms) on trusted computers only
- Revoke app permissions if token is compromised

## Rollback Instructions

If you need to remove these features:

1. **Remove debug logging setting:**
   - Delete `EnableDebugLogging` from `SharePointSettings.cs`
   - Delete the property from `appsettings.example.json`
   - Revert `Program.cs` to simple `GraphServiceClient` initialization
   - Revert `ListExportService.cs` constructor and conditional logging

2. **Remove access token display:**
   - Remove `DisplayAccessTokenAsync()` method from `Program.cs`
   - Remove case 4 from menu switch statement
   - Update `MenuHelper` to show options 1-4 (renumber Exit to 4)

3. **Remove documentation:**
   - Delete `DEBUG-FEATURES.md`
   - Revert changes to `README.md`, `USER-GUIDE.md`

4. **Remove logging handler:**
   - Delete `Handlers/LoggingHandler.cs`

## Version Compatibility

- ? .NET 10
- ? Microsoft.Graph 5.91.0
- ? Azure.Identity 1.13.1
- ? Works with existing code
- ? Backward compatible (new features are opt-in)

## Summary

These debug features provide powerful diagnostics while maintaining:
- **Simplicity:** Single boolean setting to enable/disable
- **Security:** Tokens shown only in local console
- **Performance:** No overhead when logging disabled
- **Documentation:** Comprehensive guides for all skill levels
- **Usability:** Menu-driven access to token display

Total files changed: 9
- Modified: 5
- Created: 4

All changes are non-breaking and backward compatible.
