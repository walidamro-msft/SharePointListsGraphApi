# Debug Features

This document describes the debugging and diagnostic features available in the SharePoint Lists Graph API Management Tool.

## Table of Contents

- [Debug Logging](#debug-logging)
- [Access Token Display](#access-token-display)
- [Use Cases](#use-cases)
- [Configuration](#configuration)

---

## Debug Logging

### Overview

The debug logging feature allows you to see the actual HTTP requests being sent to Microsoft Graph API. This is invaluable for:

- **Troubleshooting API calls** - See exactly what URLs are being requested
- **Understanding query parameters** - View filters, expansions, and selections
- **Debugging pagination** - Track the `@odata.nextLink` URLs
- **API learning** - Understand how the Graph SDK translates operations into HTTP requests

### Features

When debug logging is enabled, you'll see:

1. **HTTP Method and URL** - Every Graph API request (GET, POST, etc.)
2. **Response Status** - HTTP status codes (200 OK, 201 Created, etc.)
3. **Query Parameters** - See $filter, $expand, $select in action
4. **Pagination URLs** - Track how the API handles large datasets
5. **Color-coded output**:
   - ?? **Cyan** - HTTP requests
   - ?? **Green** - Successful responses
   - ?? **Yellow** - Pagination/next page URLs

### Enabling Debug Logging

#### Method 1: Configuration File (Recommended)

Edit `appsettings.json`:

```json
{
  "SharePointSettings": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-secret",
    "SiteUrl": "https://yourtenant.sharepoint.com/sites/yoursite",
    "ListName": "Your List Name",
    "Columns": "*",
    "EnableDebugLogging": true  // ? Set to true to enable
  }
}
```

**Default Value:** `false` (logging disabled)

#### What Gets Logged

**Example output when exporting list items:**

```
[DEBUG MODE ENABLED] - HTTP requests will be logged

[DEBUG] HTTP GET: https://graph.microsoft.com/v1.0/sites/contoso.sharepoint.com,abc123.../lists/def456.../items?$expand=fields&$filter=fields%2FModified%20ge%20'2024-01-01T00:00:00Z'
[DEBUG] Response: 200 OK

| Downloading... Page #1 (200 items) | Total: 200 items

[DEBUG] Next page URL: https://graph.microsoft.com/v1.0/sites/contoso.sharepoint.com,abc123.../lists/def456.../items?$expand=fields&$skiptoken=Paged%3DTRUE...
[DEBUG] Response: 200 OK
```

### Use Cases for Debug Logging

#### 1. Verify Date Filters

When using date filtering in exports, see the exact OData filter syntax:

```
[DEBUG] First Graph API Request URL:
https://graph.microsoft.com/v1.0/sites/.../items?$expand=fields&$filter=fields%2FModified%20ge%20'2024-01-15T00:00:00Z'%20and%20fields%2FModified%20le%20'2024-01-15T23:59:59Z'
```

This shows you:
- The filter is applied to `fields/Modified`
- Dates are in ISO 8601 format (UTC)
- URL encoding is handled automatically

#### 2. Troubleshoot Permission Issues

If you get "Access Denied" errors, the debug log shows the exact endpoint:

```
[DEBUG] HTTP GET: https://graph.microsoft.com/v1.0/sites/contoso.sharepoint.com.../lists/abc123.../items
[DEBUG] Response: 403 Forbidden
```

You can test this URL directly in Graph Explorer to diagnose permissions.

#### 3. Understand Column Selection

See how `$select` works with specific columns:

```
[DEBUG] HTTP GET: https://graph.microsoft.com/v1.0/sites/.../items?$expand=fields&$select=Title,Status,DueDate
```

#### 4. Monitor API Performance

Track how many requests are made for large lists:

```
Page #1 - 200 items
Page #2 - 200 items
Page #3 - 200 items
...
Total items downloaded: 5,432
```

---

## Access Token Display

### Overview

The **Access Token Display** feature (Menu Option 4) allows you to view and inspect the OAuth 2.0 access token being used to authenticate with Microsoft Graph API.

### Purpose

This feature is useful for:

- **Token Validation** - Verify the token is being generated correctly
- **Expiration Monitoring** - See when the token will expire
- **Debugging Authentication Issues** - Test the token in external tools
- **Security Auditing** - Understand what permissions the token contains
- **Learning OAuth 2.0** - See how Azure AD issues tokens

### Using the Feature

1. Run the application
2. Select **Option 4: Display Access Token** from the main menu
3. View the token information

**Example Output:**

```
============================================================
  Access Token Information
============================================================

Access Token: eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ij...

Expires On: 2024-01-15 14:30:45 UTC
Time Until Expiration: 00:59:12

? Access token retrieved successfully!
```

### Token Information Displayed

| Field | Description |
|-------|-------------|
| **Access Token** | The full JWT (JSON Web Token) string |
| **Expires On** | When the token will expire (UTC timezone) |
| **Time Until Expiration** | Countdown in HH:MM:SS format |

### Inspecting the Token

You can decode and inspect the token using online tools:

1. **Copy the token** from the console output
2. Visit [jwt.ms](https://jwt.ms) or [jwt.io](https://jwt.io)
3. Paste the token to see:
   - **Claims** - User ID, tenant, application ID
   - **Permissions** (scp/roles) - Sites.Read.All, Sites.ReadWrite.All, etc.
   - **Issuer** - Microsoft identity platform
   - **Audience** - https://graph.microsoft.com
   - **Issued/Expiry Times**

### Security Considerations

?? **Important Security Notes:**

1. **Never share access tokens** - They provide full access to your SharePoint data
2. **Tokens expire** - Default lifetime is 60-90 minutes
3. **Token rotation** - The SDK automatically refreshes tokens
4. **Scope validation** - Ensure the token contains the expected permissions

### Using the Token with Other Tools

#### Graph Explorer

1. Go to [Graph Explorer](https://developer.microsoft.com/graph/graph-explorer)
2. Click "Modify permissions" ? "Access token"
3. Paste your token
4. Run queries to test permissions

#### PowerShell

```powershell
$token = "eyJ0eXAiOiJKV1QiLCJhbGci..."
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Invoke-RestMethod -Uri "https://graph.microsoft.com/v1.0/sites" -Headers $headers
```

#### cURL

```bash
curl -X GET "https://graph.microsoft.com/v1.0/sites" \
  -H "Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGci..."
```

---

## Configuration

### appsettings.json Schema

```json
{
  "SharePointSettings": {
    "TenantId": "string",           // Required: Your Microsoft 365 tenant ID
    "ClientId": "string",           // Required: App registration client ID
    "ClientSecret": "string",       // Required: App registration secret
    "SiteUrl": "string",            // Required: SharePoint site URL
    "ListName": "string",           // Required: Target list name
    "Columns": "string",            // Optional: "*" or "Col1,Col2,Col3"
    "EnableDebugLogging": boolean   // Optional: Default = false
  }
}
```

### Debug Logging Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `EnableDebugLogging` | boolean | `false` | Enable/disable HTTP request logging |

**Recommended Settings:**

- **Development/Testing:** `true` - See all API interactions
- **Production:** `false` - Cleaner output, better performance
- **Troubleshooting:** `true` - Diagnose issues

---

## Use Cases

### Scenario 1: Debugging "No Items Found" Issue

**Problem:** Export returns 0 items when you know items exist.

**Solution:**
1. Enable `EnableDebugLogging: true`
2. Run the export
3. Check the debug output:

```
[DEBUG] HTTP GET: https://graph.microsoft.com/v1.0/sites/.../items?$filter=fields%2FModified%20ge%20'2025-01-15T00:00:00Z'
```

4. Notice the date filter - perhaps it's excluding all items
5. Adjust the date range or disable filtering

### Scenario 2: Testing Graph API Permissions

**Problem:** Unsure if your app has the right permissions.

**Solution:**
1. Run **Option 4: Display Access Token**
2. Copy the token
3. Paste into [jwt.ms](https://jwt.ms)
4. Look for `roles` claim:

```json
{
  "roles": [
    "Sites.Read.All",
    "Sites.ReadWrite.All"
  ]
}
```

5. Verify your required permissions are present

### Scenario 3: Learning Graph API Syntax

**Problem:** Want to learn how to construct Graph API queries.

**Solution:**
1. Enable `EnableDebugLogging: true`
2. Perform various operations (export, filter by date, select columns)
3. Study the generated URLs:

```
Without filters:
https://graph.microsoft.com/v1.0/sites/.../items?$expand=fields

With date filter:
https://graph.microsoft.com/v1.0/sites/.../items?$expand=fields&$filter=fields%2FModified%20ge%20'2024-01-01T00:00:00Z'

With column selection:
https://graph.microsoft.com/v1.0/sites/.../items?$expand=fields&$select=Title,Status
```

4. Use this knowledge to build your own Graph API queries

### Scenario 4: Performance Optimization

**Problem:** Exports are slow for large lists.

**Solution:**
1. Enable debug logging
2. Count the number of pagination requests
3. Monitor the page sizes:

```
Page #1 (200 items)
Page #2 (200 items)
Page #3 (200 items)
...
Page #25 (200 items)
Total: 5,000 items
```

4. Consider filtering by date to reduce the dataset
5. Graph API has a maximum page size (typically 200-5000 depending on endpoint)

---

## Troubleshooting Debug Features

### Debug Logging Not Working

**Symptom:** Set `EnableDebugLogging: true` but no debug output appears.

**Solution:**
1. Verify `appsettings.json` is being copied to output directory
2. Check the JSON syntax is valid (use a JSON validator)
3. Rebuild the solution: `dotnet clean && dotnet build`
4. Ensure you're editing the correct `appsettings.json` (not the example file)

### Access Token Expiration

**Symptom:** Token expires during long-running operations.

**Solution:**
- The Graph SDK automatically handles token refresh
- No action needed - tokens are refreshed transparently
- If issues persist, verify the client secret hasn't expired in Entra ID

### Token Shows Limited Permissions

**Symptom:** Token inspection shows missing permissions.

**Solution:**
1. Go to Entra ID ? App Registrations ? Your App
2. Verify **API Permissions** includes:
   - `Sites.Read.All` (Application)
   - `Sites.ReadWrite.All` (Application)
3. Click **"Grant admin consent for [tenant]"**
4. Wait 5-10 minutes for changes to propagate
5. Run Option 4 again and verify permissions

---

## Best Practices

### Debug Logging

? **DO:**
- Enable during development and troubleshooting
- Use to learn Graph API patterns
- Review logs when encountering errors
- Disable in production for better performance

? **DON'T:**
- Leave enabled in production (performance impact)
- Share logs containing sensitive site/list IDs publicly
- Assume all URLs work in browsers (some require POST bodies)

### Access Token Display

? **DO:**
- Use to verify authentication is working
- Decode tokens to understand permissions
- Check expiration times for long-running jobs
- Use tokens for testing in Graph Explorer

? **DON'T:**
- Share tokens with others (security risk)
- Store tokens in files or source control
- Assume tokens last forever (they expire)
- Use expired tokens for API calls

---

## Additional Resources

- [Microsoft Graph REST API Reference](https://docs.microsoft.com/en-us/graph/api/overview)
- [OData Query Parameters](https://docs.microsoft.com/en-us/graph/query-parameters)
- [Graph Explorer](https://developer.microsoft.com/graph/graph-explorer)
- [JWT.ms Token Decoder](https://jwt.ms)
- [Troubleshooting Guide](TROUBLESHOOTING.md)
- [User Guide](USER-GUIDE.md)

---

## Summary

| Feature | Purpose | How to Enable |
|---------|---------|---------------|
| **Debug Logging** | See all HTTP requests/responses | Set `EnableDebugLogging: true` in appsettings.json |
| **Access Token Display** | View and inspect OAuth tokens | Select Menu Option 4 |

Both features are designed to help developers understand, debug, and optimize their Graph API interactions.
