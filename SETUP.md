# Entra ID App Registration Setup Guide

This guide walks you through creating and configuring an Entra ID (Azure AD) App Registration for the SharePoint List Management Tool.

## Prerequisites

- Azure subscription with access to Entra ID (Azure Active Directory)
- Global Administrator or Application Administrator role in Entra ID
- SharePoint Online tenant with a site collection

---

## Step 1: Create App Registration

1. Navigate to [Azure Portal](https://portal.azure.com)
2. Go to **Microsoft Entra ID** (formerly Azure Active Directory)
3. Select **App registrations** from the left menu
4. Click **+ New registration**

### Registration Details

- **Name**: `sharepoint-graph-api` (or your preferred name)
- **Supported account types**: 
  - Select **Accounts in this organizational directory only (Single tenant)**
- **Redirect URI**: 
  - Platform: **Mobile and desktop applications**
  - Add these redirect URIs:
    - `http://localhost:51189`
    - `https://login.microsoftonline.com/common/oauth2/nativeclient`
    - `msal[YOUR-CLIENT-ID]://auth`
    - `https://login.live.com/oauth20_desktop.srf`

5. Click **Register**

---

## Step 2: Configure Authentication

1. In your app registration, go to **Authentication** (left menu)
2. Under **Platform configurations**, verify the redirect URIs are added
3. Under **Advanced settings**:
   - **Allow public client flows**: Set to **Yes**
4. Click **Save**

### Required Redirect URIs

From the screenshots, you need these exact URIs:

```
http://localhost:51189
https://login.microsoftonline.com/common/oauth2/nativeclient
msal[YOUR-CLIENT-ID]://auth
https://login.live.com/oauth20_desktop.srf
```

---

## Step 3: Create Client Secret

1. Go to **Certificates & secrets** (left menu)
2. Select the **Client secrets** tab
3. Click **+ New client secret**

### Secret Details

- **Description**: `Console App` (or your preferred description)
- **Expires**: Choose expiration period (e.g., 24 months)
  - ?? **Important**: Note the expiration date - you'll need to renew before it expires

4. Click **Add**
5. **Copy the Value immediately** - you cannot retrieve it later!
   - This is your `ClientSecret` for `appsettings.json`

?? **Security Note**: Store this secret securely. Never commit it to source control.

---

## Step 4: Configure API Permissions

1. Go to **API permissions** (left menu)
2. Click **+ Add a permission**

### Add Microsoft Graph Permissions

1. Select **Microsoft Graph**
2. Select **Application permissions** (not Delegated)
3. Add these permissions:
   - `Sites.Read.All` - Read items in all site collections
   - `Sites.ReadWrite.All` - Read and write items in all site collections
   - `Sites.Selected` - Access selected site collections
   - `User.Read` - Sign in and read user profile (Delegated)

### Permission Summary

Your configured permissions should look like this:

| API / Permissions name | Type | Admin consent required | Status |
|------------------------|------|------------------------|--------|
| **Microsoft Graph (4)** | | | |
| Sites.Read.All | Application | Yes | Granted for MSFT ? |
| Sites.ReadWrite.All | Application | Yes | Granted for MSFT ? |
| Sites.Selected | Application | Yes | Granted for MSFT ? |
| User.Read | Delegated | No | Granted for MSFT ? |

4. Click **Grant admin consent for [Your Organization]**
5. Confirm by clicking **Yes**
6. Verify all permissions show "Granted for [Your Organization]" with green checkmarks

---

## Step 5: Collect Configuration Values

After completing the above steps, gather these values:

### From App Registration Overview Page

1. Go to **Overview** (left menu)
2. Copy these values:

| Setting | Location | Format |
|---------|----------|--------|
| **TenantId** | Directory (tenant) ID | GUID (e.g., `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`) |
| **ClientId** | Application (client) ID | GUID (e.g., `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`) |

### From Certificates & Secrets

| Setting | Location | Format |
|---------|----------|--------|
| **ClientSecret** | Value (from Step 3) | Alphanumeric string (e.g., `X~X8Q~...`) |

---

## Step 6: Configure appsettings.json

1. Copy `appsettings.example.json` to `appsettings.json`:
   ```bash
   cp appsettings.example.json appsettings.json
   ```

2. Edit `appsettings.json` with your values:

```json
{
  "SharePointSettings": {
    "TenantId": "YOUR-TENANT-ID-GUID",
    "ClientId": "YOUR-CLIENT-ID-GUID",
    "ClientSecret": "YOUR-CLIENT-SECRET-VALUE",
    "SiteUrl": "https://yourtenant.sharepoint.com/sites/yoursite",
    "ListName": "Your List Name",
    "Columns": "*"
  }
}
```

3. Update the SharePoint-specific values:
   - **SiteUrl**: Your SharePoint site URL (e.g., `https://contoso.sharepoint.com/sites/TeamSite`)
   - **ListName**: The exact name of your SharePoint list (case-sensitive)
   - **Columns**: Use `*` for all columns or specify column names: `"Title,Status,DueDate"`

---

## Step 7: Grant SharePoint Site Permissions (Optional)

For additional security and explicit site access, grant the app permissions directly to the SharePoint site:

1. Navigate to (replace with your values):
   ```
   https://yourtenant.sharepoint.com/sites/yoursite/_layouts/15/appinv.aspx
   ```

2. Enter the **App ID** (same as ClientId):
```
YOUR-CLIENT-ID-GUID
```

3. Click **Lookup**
   - The app details should appear

4. Enter the following in **Permission Request XML**:
   ```xml
   <AppPermissionRequests AllowAppOnlyPolicy="true">
     <AppPermissionRequest Scope="http://sharepoint/content/sitecollection" Right="Write"/>
   </AppPermissionRequests>
   ```

5. Click **Create**
6. Click **Trust It** to grant permissions

---

## Step 8: Test the Configuration

1. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```

2. The application should show:
   ```
   Testing authentication...
   + Authentication successful!

   Retrieving SharePoint site...
   + Site ID: ...
   + Site Name: ...

   Retrieving list 'Your List Name'...
   + List ID: ...
   ```

3. If successful, you're ready to use the application! ??

---

## Troubleshooting

### "Authentication failed" Error

**Cause**: Invalid TenantId, ClientId, or ClientSecret

**Solution**:
1. Double-check values in `appsettings.json`
2. Ensure ClientSecret hasn't expired
3. Verify TenantId and ClientId are correct GUIDs

### "Insufficient privileges" Error

**Cause**: Missing API permissions or admin consent not granted

**Solution**:
1. Verify all permissions in Step 4 are added
2. Ensure **admin consent** was granted (green checkmarks)
3. Wait a few minutes for permissions to propagate

### "Site not found" Error

**Cause**: Incorrect SiteUrl or app doesn't have access

**Solution**:
1. Verify the SiteUrl format: `https://tenant.sharepoint.com/sites/sitename`
2. Ensure the site exists and you have access
3. Try Step 7 to grant explicit site permissions

### "List not found" Error

**Cause**: ListName doesn't match exactly

**Solution**:
1. Verify the list name is spelled correctly (case-sensitive)
2. Use the exact display name from SharePoint
3. Ensure the list exists in the specified site

---

## Security Best Practices

### For Development
? Use `appsettings.json` (local file)  
? Add `appsettings.json` to `.gitignore`  
? Use short-lived client secrets (e.g., 6 months)  
? Rotate secrets regularly  

### For Production
?? Use **Azure Key Vault** to store secrets  
?? Use **Managed Identity** when hosting in Azure  
?? Use **Certificate-based authentication** instead of client secrets  
?? Enable **audit logging** for app activities  
?? Use **Conditional Access** policies  

---

## Required Permissions Reference

### Microsoft Graph API Permissions

| Permission | Type | Purpose | Required |
|------------|------|---------|----------|
| Sites.Read.All | Application | Read list items | For export only |
| Sites.ReadWrite.All | Application | Create columns and items | For all features |
| Sites.Selected | Application | Access specific sites | Optional |
| User.Read | Delegated | User profile access | For authentication |

### Permission Scopes

- **Sites.Read.All**: Allows the app to read documents and list items in all site collections
- **Sites.ReadWrite.All**: Allows the app to read and write documents and list items in all site collections
- **Sites.Selected**: Allows the app to access only the site collections explicitly granted

---

## Next Steps

After completing this setup:

1. ? Read **QUICKSTART.md** for a 5-minute usage guide
2. ? Read **USER-GUIDE.md** for detailed feature documentation
3. ? Try the sample CSV import with `sample-columns.csv`
4. ? Review **TROUBLESHOOTING.md** if you encounter issues

---

## Configuration Checklist

Use this checklist to verify your setup:

- [ ] App registration created in Entra ID
- [ ] Redirect URIs configured (4 URIs)
- [ ] Client secret created and copied
- [ ] API permissions added (Sites.Read.All, Sites.ReadWrite.All, Sites.Selected, User.Read)
- [ ] Admin consent granted (green checkmarks visible)
- [ ] TenantId copied from Overview
- [ ] ClientId copied from Overview
- [ ] appsettings.json created and configured
- [ ] SiteUrl points to valid SharePoint site
- [ ] ListName matches existing list
- [ ] Application runs and authenticates successfully

---

## Additional Resources

- [Microsoft Graph API Documentation](https://learn.microsoft.com/en-us/graph/)
- [SharePoint REST API Reference](https://learn.microsoft.com/en-us/sharepoint/dev/sp-add-ins/sharepoint-net-server-csom-jsom-and-rest-api-index)
- [Azure AD App Registration](https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app)
- [Graph API Permissions Reference](https://learn.microsoft.com/en-us/graph/permissions-reference)

---

**Questions or issues?** Check **TROUBLESHOOTING.md** or create an issue on GitHub.
