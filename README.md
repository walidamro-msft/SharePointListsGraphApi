# SharePoint Lists Graph API Management Tool

A comprehensive .NET 10 console application for managing SharePoint Online lists using Microsoft Graph API with application permissions.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-MIT-green)
![Microsoft Graph](https://img.shields.io/badge/Microsoft%20Graph-5.91.0-blue)

## ? Features

### 1. Add Columns from CSV File
- Import column definitions from a CSV file
- Supports multiple column types (text, number, date, choice, person, etc.)
- Progress indicator showing real-time status
- Detailed success/failure reporting

### 2. Generate Random Test Data
- Create test data with random values
- Specify the number of rows to generate (1-10,000)
- Automatically populates all columns with appropriate data types
- Progress bar showing creation status

### 3. Export List to JSON
- Downloads all items from a SharePoint list using pagination
- Supports filtering columns (specific columns or all with `*`)
- Saves results as JSON with timestamp filename (YYYYMMDDHHmmss.json)
- Handles Graph API pagination automatically

## Project Structure

```
SharePointListsGraphApi/
??? Program.cs                          # Main entry point with menu system
??? SharePointSettings.cs               # Configuration model
??? appsettings.json                    # Configuration file
??? sample-columns.csv                  # Example CSV template
??? Services/
?   ??? SharePointService.cs            # Site and list operations
?   ??? ColumnManagementService.cs      # Column creation from CSV
?   ??? TestDataGeneratorService.cs     # Random data generation
?   ??? ListExportService.cs            # List export to JSON
??? Helpers/
    ??? MenuHelper.cs                   # Menu display and user input
    ??? ValidationHelper.cs             # Settings validation
```

## Quick Links

- ??? **[Setup Guide](SETUP.md)** - Entra ID App Registration configuration (with screenshots)
- ?? **[Quick Start Guide](QUICKSTART.md)** - Get up and running in 5 minutes
- ?? **[CSV Format Guide](CSV-FORMAT-GUIDE.md)** - CSV file format reference
- ?? **[User Guide](USER-GUIDE.md)** - Detailed feature documentation
- ?? **[Troubleshooting](TROUBLESHOOTING.md)** - Common issues and solutions
- ??? **[Architecture](ARCHITECTURE.md)** - Code structure and design

## Prerequisites

1. **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download)
2. **Microsoft 365 tenant** with SharePoint Online
3. **Entra ID (Azure AD) App Registration** with:
   - Application permissions: `Sites.Read.All`, `Sites.ReadWrite.All`
   - Client Secret configured
   - Admin consent granted

?? **See [SETUP.md](SETUP.md) for detailed step-by-step App Registration setup**

## Quick Setup

### 1. Copy and configure settings file

```bash
cp appsettings.example.json appsettings.json
```

Edit `appsettings.json` with your values:

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

### 2. Configure Entra ID App Registration

**?? See [SETUP.md](SETUP.md) for complete Entra ID setup instructions with screenshots**

Quick summary of required permissions:
- `Sites.Read.All` (Application)
- `Sites.ReadWrite.All` (Application)
- `Sites.Selected` (Application)
- `User.Read` (Delegated)

### 3. Build and run

```bash
dotnet restore
dotnet build
dotnet run
```

## Usage

Run the application to see the main menu:

```bash
dotnet run
```

```
+============================================================+
|       SharePoint List Management Tool                     |
+============================================================+

  1. Add Columns from CSV File
  2. Generate Random Test Data
  3. Export List to JSON File
  4. Exit

Select an option (1-4):
```

**For detailed usage instructions, see [USER-GUIDE.md](USER-GUIDE.md)**

## Troubleshooting

### "Site not found" error
- Verify the SiteUrl is correct and accessible
- Ensure the app has permissions to the site

### "List not found" error
- Check the ListName matches exactly (case-sensitive)
- Verify the list exists on the specified site

### Authentication errors
- Verify TenantId, ClientId, and ClientSecret are correct
- Ensure admin consent has been granted for API permissions
- Check that the client secret hasn't expired

### Permission errors
- Ensure `Sites.Read.All` permission is granted
- Verify admin consent has been provided
- Check that the app has access to the specific SharePoint site

## Notes

- Graph API has pagination limits (typically 200-5000 items per page depending on the endpoint)
- The application automatically handles pagination using `@odata.nextLink`
- Large lists may take time to download
- JSON files can become large for lists with many items

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.

## ?? Support

- ?? [Setup Guide](SETUP.md) - Entra ID configuration
- ?? [Quick Start](QUICKSTART.md) - Get started quickly
- ?? [User Guide](USER-GUIDE.md) - Detailed documentation
- ?? [Troubleshooting](TROUBLESHOOTING.md) - Common issues
- ??? [Architecture](ARCHITECTURE.md) - Code structure

For issues and feature requests, please use the [GitHub Issues](https://github.com/walidamro-msft/SharePointListsGraphApi/issues) page.

## ?? Security Notice

**Important:** Never commit `appsettings.json` with real credentials to source control. Always use `appsettings.example.json` as a template and keep your actual credentials local.

The `.gitignore` file is configured to exclude `appsettings.json` automatically.

## ?? Acknowledgments

- Built with [Microsoft Graph SDK](https://github.com/microsoftgraph/msgraph-sdk-dotnet)
- Uses [Azure Identity](https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/identity/Azure.Identity) for authentication
