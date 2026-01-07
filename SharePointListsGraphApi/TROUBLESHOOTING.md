# Troubleshooting Access Denied Errors

## Common Causes and Solutions

### 1. Missing or Incorrect API Permissions

**Solution:**
1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure Active Directory** > **App registrations**
3. Select your app registration
4. Click **API permissions** on the left menu
5. Verify you have these **Application permissions** (NOT Delegated):
   - `Sites.Read.All` (Microsoft Graph)
   - OR `Sites.ReadWrite.All` (Microsoft Graph)
6. Click **Grant admin consent for [Your Tenant]**
7. Wait 5-10 minutes for permissions to propagate

### 2. App Not Granted Access to SharePoint Site

SharePoint Online has site-level permissions. Even with API permissions, you may need to grant the app explicit access to the site.

**Option A: Grant site collection app permissions**

Run this PowerShell script (requires SharePoint Online Management Shell):

```powershell
# Install module if needed
Install-Module -Name PnP.PowerShell -Scope CurrentUser

# Connect to SharePoint Admin
Connect-PnPOnline -Url "https://YOUR-TENANT-admin.sharepoint.com" -Interactive

# Grant app permission to specific site
Grant-PnPAzureADAppSitePermission -AppId "YOUR-CLIENT-ID" `
    -DisplayName "SharePoint List Management Tool" `
    -Site "https://YOUR-TENANT.sharepoint.com/sites/YOUR-SITE" `
    -Permissions Read
```

**Option B: Use App-Only Policy (Legacy)**

1. Navigate to: `https://YOUR-TENANT.sharepoint.com/sites/YOUR-SITE/_layouts/15/appinv.aspx`
2. In **App Id** field, enter your ClientId: `YOUR-CLIENT-ID`
3. Click **Lookup**
4. In **Permission Request XML**, paste:
```xml
<AppPermissionRequests AllowAppOnlyPolicy="true">
  <AppPermissionRequest Scope="http://sharepoint/content/sitecollection" Right="Read"/>
</AppPermissionRequests>
```
5. Click **Create**
6. Click **Trust It**

### 3. Client Secret Expired

Client secrets have an expiration date (typically 6 months, 1 year, or 2 years).

**Solution:**
1. Go to Azure Portal > App registrations > Your App
2. Click **Certificates & secrets**
3. Check if your secret is expired
4. If expired, create a new client secret
5. Update `appsettings.json` with the new secret value

### 4. Incorrect Credentials

**Verify your credentials:**

```powershell
# Test with Azure CLI
az login --service-principal `
    -u "YOUR-CLIENT-ID" `
    -p "YOUR-CLIENT-SECRET" `
    --tenant "YOUR-TENANT-ID"
```

If this fails, your credentials are incorrect.

### 5. Site URL Format Issues

Your site URL should be: `https://YOUR-TENANT.sharepoint.com/sites/YOUR-SITE`

**Verify the site is accessible:**
- Open the URL in a browser while logged in
- Make sure it's not a personal OneDrive site
- Ensure you're using the correct site path (case-sensitive)

### 6. Conditional Access Policies

Your tenant might have conditional access policies blocking app-only access.

**Solution:**
Contact your SharePoint/Azure admin to:
1. Check conditional access policies
2. Create an exclusion for service principals if needed
3. Allow app-only authentication for SharePoint

## Testing Steps

### Step 1: Verify App Registration Permissions

Run this PowerShell to check permissions:

```powershell
Install-Module Microsoft.Graph -Scope CurrentUser
Connect-MgGraph -Scopes "Application.Read.All"

$app = Get-MgApplication -Filter "appId eq 'YOUR-CLIENT-ID'"
$app.RequiredResourceAccess | Format-List
```

### Step 2: Test Graph API Access

Try accessing Graph API directly:

```powershell
$tenantId = "YOUR-TENANT-ID"
$clientId = "YOUR-CLIENT-ID"
$clientSecret = "YOUR-CLIENT-SECRET"

$body = @{
    client_id = $clientId
    scope = "https://graph.microsoft.com/.default"
    client_secret = $clientSecret
    grant_type = "client_credentials"
}

$token = Invoke-RestMethod -Method Post -Uri "https://login.microsoftonline.com/$tenantId/oauth2/v2.0/token" -Body $body
$headers = @{Authorization = "Bearer $($token.access_token)"}

# Test Sites access
Invoke-RestMethod -Headers $headers -Uri "https://graph.microsoft.com/v1.0/sites?search=YOUR-SITE"
```

### Step 3: Check Site Permissions

List all apps with access to your site:

```powershell
Connect-PnPOnline -Url "https://YOUR-TENANT.sharepoint.com/sites/YOUR-SITE" -Interactive
Get-PnPAzureADAppSitePermission
```

## Additional Diagnostic Information

The updated application now provides:
- Authentication testing before site access
- Better error messages
- Alternative site access methods
- Detailed debugging output

Run the application again and review the console output for specific error details.

## Still Having Issues?

If you've tried all the above and still get "Access denied":

1. **Check Azure AD Sign-in Logs:**
   - Azure Portal > Azure AD > Sign-in logs
   - Filter by your ClientId
   - Look for failed attempts and error codes

2. **Enable Graph API Logging:**
   Add this to your application for detailed logs:
   ```csharp
   var handler = new LoggingHandler();
   var graphClient = new GraphServiceClient(clientSecretCredential, handler);
   ```

3. **Contact your tenant administrator:**
   - They may need to adjust tenant-wide SharePoint settings
   - Check SharePoint Admin Center > Settings > Apps
   - Ensure "Apps that don't use modern authentication" is allowed

## Quick Checklist

- [ ] App registration has `Sites.Read.All` or `Sites.ReadWrite.All` application permission
- [ ] Admin consent granted for the permission
- [ ] Client secret is not expired
- [ ] Site URL is correct and accessible
- [ ] App has been granted access to the specific site collection
- [ ] Waited 5-10 minutes after granting permissions
- [ ] No conditional access policies blocking app-only access
