# Quick Start Guide

## 5-Minute Setup

### Prerequisites

Before starting, make sure you have:
- ? .NET 10 SDK installed
- ? Microsoft 365 tenant with SharePoint Online
- ? Access to create Entra ID App Registrations

### Step 1: Clone and Setup (1 minute)

```bash
# Clone the repository
git clone https://github.com/walidamro-msft/SharePointListsGraphApi.git
cd SharePointListsGraphApi/SharePointListsGraphApi

# Copy the example config file
cp appsettings.example.json appsettings.json
```

### Step 2: Configure Entra ID App Registration (2 minutes)

**?? For detailed setup with screenshots, see [SETUP.md](SETUP.md)**

Quick steps:
1. Go to [Azure Portal](https://portal.azure.com) ? Entra ID ? App registrations
2. Create new registration: `sharepoint-graph-api`
3. Add API permissions: `Sites.Read.All`, `Sites.ReadWrite.All`, `Sites.Selected`, `User.Read`
4. Grant admin consent
5. Create client secret
6. Copy TenantId, ClientId, and ClientSecret

### Step 3: Configure Settings (1 minute)

Edit `appsettings.json`:
```json
{
  "SharePointSettings": {
    "TenantId": "YOUR-TENANT-ID",
    "ClientId": "YOUR-CLIENT-ID",
    "ClientSecret": "YOUR-CLIENT-SECRET",
    "SiteUrl": "https://yourtenant.sharepoint.com/sites/yoursite",
    "ListName": "Your List Name",
    "Columns": "*"
  }
}
```

**Where to find these values:**
- See [SETUP.md](SETUP.md) for complete instructions with screenshots
- Azure Portal ? Entra ID ? App Registrations ? Your App
- **TenantId**: Overview ? Directory (tenant) ID
- **ClientId**: Overview ? Application (client) ID
- **ClientSecret**: Certificates & secrets ? Client secrets

### Step 4: Run (1 minute)

```bash
dotnet run
```

You should see:
```
Testing authentication...
+ Authentication successful!

Retrieving SharePoint site...
+ Site ID: ...
+ Site Name: ...

Retrieving list 'Your List Name'...
+ List ID: ...

+============================================================+
|       SharePoint List Management Tool                     |
+============================================================+

  1. Add Columns from CSV File
  2. Generate Random Test Data
  3. Export List to JSON File
  4. Exit

Select an option (1-4):
```

## Quick Feature Test

### Test 1: Add Columns
1. Select option **1**
2. Enter: `sample-columns.csv`
3. Wait for completion
4. Check your SharePoint list for new columns

### Test 2: Generate Data
1. Select option **2**
2. Enter: `10`
3. Type: `y`
4. Wait for completion
5. Check your SharePoint list for 10 new items

### Test 3: Export Data
1. Select option **3**
2. Wait for completion
3. Check for `YYYYMMDDHHmmss.json` file
4. Open in text editor or Excel

## Troubleshooting

### "Access denied" Error
? See Step 3 above - Grant site access

### "Authentication failed"
? Check TenantId, ClientId, ClientSecret in appsettings.json

### "List not found"
? Check ListName spelling (case-sensitive)

### "Site not found"
? Check SiteUrl format: `https://tenant.sharepoint.com/sites/sitename`

## Next Steps

?? Read **USER-GUIDE.md** for detailed feature documentation  
?? Read **TROUBLESHOOTING.md** if you encounter issues  
??? Read **ARCHITECTURE.md** to understand the code structure

## Common Commands

```bash
# Build the project
dotnet build

# Run the project
dotnet run

# Publish for distribution
dotnet publish -c Release

# Run published version
cd bin/Release/net10.0/
dotnet SharePointListsGraphApi.dll
```

## File Locations

After running, you'll find:
- **Exported JSON files**: Same folder as executable
- **Config file**: `appsettings.json` (same folder)
- **Sample CSV**: `sample-columns.csv` (same folder)

## Quick Reference

| Action | Menu Option |
|--------|-------------|
| Add columns | 1 |
| Create test data | 2 |
| Export to JSON | 3 |
| Exit | 4 |

## Support

- **Setup issues**: Check TROUBLESHOOTING.md
- **Usage questions**: Check USER-GUIDE.md
- **Code questions**: Check ARCHITECTURE.md

---

?? **You're ready to go!** Select option 3 to export your first list!
