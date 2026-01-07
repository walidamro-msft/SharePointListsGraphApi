# User Guide - SharePoint List Management Tool

## Overview

This tool provides four main features for managing SharePoint Online lists:

1. **Add Columns from CSV** - Bulk import column definitions
2. **Generate Test Data** - Create random test data for testing
3. **Export to JSON** - Download list contents
4. **Grant Site Permissions** - Helper to set up site-level permissions

## Getting Started

### First Run

When you first run the application, it will:
1. Load settings from `appsettings.json`
2. Authenticate with Microsoft Graph API
3. Connect to your SharePoint site
4. Locate your list
5. Display the main menu

### Main Menu

```
??????????????????????????????????????????????????????????????
?       SharePoint List Management Tool                     ?
??????????????????????????????????????????????????????????????

  1. Add Columns from CSV File
  2. Generate Random Test Data
  3. Export List to JSON File
  4. Grant Site Permissions (Setup Helper)
  5. Exit

Select an option (1-5):
```

---

## Feature 1: Add Columns from CSV File

### Purpose
Quickly add multiple columns to your SharePoint list by importing definitions from a CSV file.

### How to Use

1. **Prepare Your CSV File**
   
   The application supports **both comma and tab delimited** CSV files!
   
   **Format:** Two columns separated by comma or tab
   - Column 1: Column Name
   - Column 2: Column Type

   **Example 1 - Comma-delimited** (`my-columns.csv`):
   ```
   Column Name,Column Type
   FirstName,Text
   LastName,Text
   BirthDate,Date
   Salary,Currency
   Department,Choice
   IsActive,Yes/No
   ```

   **Example 2 - Tab-delimited** (Excel default):
   ```
   Column Name	Type
   FirstName	Single line of text
   LastName	Single line of text
   BirthDate	Date and Time
   ```

   **Sample files included:**
   - `sample-columns.csv` - Tab-delimited with 22 columns
   - `sample-columns-comma.csv` - Comma-delimited with 10 columns
   
   ?? See **CSV-FORMAT-GUIDE.md** for detailed format information

2. **Run the Feature**
   - Select option **1** from the main menu
   - Enter the path to your CSV file when prompted
   - Press Enter
   - The app will auto-detect the delimiter (comma or tab)

3. **Watch Progress**
   ```
   Reading columns from my-columns.csv...
   Detected delimiter: ','
   Found 6 columns to add.
   
   [????????????????????] 100% - Adding column 'FirstName'...
   
   ? Successfully added 6 columns
   ```

### Supported Column Types

#### Full Type Names (Official SharePoint names)
| Type | Description | Example Use |
|------|-------------|-------------|
| Single line of text | Short text field | Names, IDs |
| Multiple lines of text | Long text field | Comments, descriptions |
| Number | Numeric values | Quantities, scores |
| Date and Time | Date/time values | Due dates, timestamps |
| Choice | Dropdown selection | Status, category |
| Person or Group | User picker | Assigned to, created by |
| Yes/No | Boolean checkbox | Is active, completed |
| Hyperlink | URL field | Website links |
| Currency | Money values | Price, salary |

#### Short Type Names (Also supported)
You can use these shortcuts for convenience:
- `Text` ? Single line of text
- `MultiLine` ? Multiple lines of text  
- `Number` ? Number
- `Date` ? Date and Time
- `Choice` ? Choice
- `Person` ? Person or Group
- `Boolean` ? Yes/No
- `URL` ? Hyperlink
- `Currency` ? Currency

**Example using short names:**
```csv
Column Name,Column Type
FirstName,Text
Notes,MultiLine
BirthDate,Date
IsActive,Boolean
```

### Tips
- Column names must be unique
- Column names cannot contain special characters: `% & * : < > ? / \ |`
- If a column already exists, it will be skipped with an error message
- Choice columns are created with default options (Option 1, 2, 3)
- You can edit choice options later in SharePoint

---

## Feature 2: Generate Random Test Data

### Purpose
Create realistic test data to populate your list for testing, demonstrations, or development.

### How to Use

1. **Select the Feature**
   - Choose option **2** from the main menu

2. **Specify Number of Rows**
   - Enter how many rows you want to create (1-10,000)
   - Example: `100`

3. **Confirm**
   ```
   This will create 100 rows with random test data.
   Continue? (y/n): y
   ```

4. **Watch Progress**
   ```
   [????????????????????] 100% - Creating row 100 of 100...
   
   ? Successfully created 100 rows
   ```

### Generated Data Types

The tool automatically generates appropriate random data for each column type:

| Column Type | Generated Data |
|-------------|----------------|
| Single line of text | "Test ColumnName 1", "Test ColumnName 2", etc. |
| Multiple lines of text | Multi-line text with 3 lines |
| Number | Random number between 1-1000 |
| Date and Time | Random date within ±1 year from today |
| Choice | Random selection from available choices |
| Yes/No | Random true/false |
| Currency | Random amount between $0-$10,000 |
| Hyperlink | https://example.com/link1, etc. |

### Tips
- Start with a small number (10-50) to test first
- Maximum 10,000 rows per run
- Generation speed depends on number of columns
- Person or Group fields are skipped (cannot auto-generate users)
- System fields (like ID, Created, Modified) are automatically managed by SharePoint

---

## Feature 3: Export List to JSON

### Purpose
Download all list items and save them as a JSON file for backup, analysis, or data migration.

### How to Use

1. **Select the Feature**
   - Choose option **3** from the main menu

2. **Automatic Export**
   - The tool automatically exports based on your `appsettings.json` settings
   - Handles pagination automatically

3. **Progress Display**
   ```
   Processing page 1 with 200 items... Total: 200
   Processing page 2 with 200 items... Total: 400
   
   ? Total items downloaded: 500
   ? Successfully saved 500 items to 20241215143022.json
   ```

### Output File

**File Name Format:** `YYYYMMDDHHmmss.json`
- Example: `20241215143022.json` (Dec 15, 2024 at 14:30:22)

**File Content:** JSON array of objects
```json
[
  {
    "Title": "Item 1",
    "FirstName": "John",
    "LastName": "Doe",
    "BirthDate": "1990-05-15T00:00:00Z",
    "Salary": 75000.00,
    "Department": "Engineering",
    "IsActive": true
  },
  {
    "Title": "Item 2",
    "FirstName": "Jane",
    "LastName": "Smith",
    ...
  }
]
```

### Column Selection

Configure in `appsettings.json`:

**Export all columns:**
```json
"Columns": "*"
```

**Export specific columns:**
```json
"Columns": "Title,FirstName,LastName,Department"
```

### Tips
- Files are saved in the same directory as the executable
- Use `*` to export all columns (including system fields)
- Specify columns to reduce file size
- JSON files can be opened in Excel, Power BI, or any text editor
- Large lists may take several minutes to export

---

## Feature 4: Grant Site Permissions (Setup Helper)

### Purpose
Interactive helper tool to guide you through granting your app the necessary permissions to modify SharePoint lists.

### When to Use
- **First time setup** - Before adding columns or generating test data
- **Access denied errors** - When you get "Access denied" while using features 1 or 2
- **New site** - When pointing the app to a different SharePoint site
- **Permission issues** - If features work inconsistently

### How to Use

1. **Select the Feature**
   - Choose option **4** from the main menu

2. **Choose Your Preferred Method**
   ```
   ??????????????????????????????????????????????????????????????
   ?           Grant Site Permissions to App                   ?
   ??????????????????????????????????????????????????????????????
   
   Choose an option:
   
     1. Open App Permission Page in Browser (Recommended)
     2. Show PowerShell Command
     3. Show Manual Instructions
     4. Back to Main Menu
   ```

### Method 1: Browser-Based (Recommended - Easiest)

**Best for:** Most users, especially those unfamiliar with PowerShell

**Steps:**
1. Select option **1**
2. Review the on-screen instructions
3. Press ENTER - the app will open your browser
4. In the browser page:
   - Paste your Client ID (shown on screen)
   - Click "Lookup"
   - Paste the Permission XML (shown on screen)
   - Click "Create"
   - Click "Trust It"
5. Wait 5-10 minutes for permissions to propagate

**What You'll See:**
```
Opening: https://your-site/_layouts/15/appinv.aspx

Follow these steps in the browser:

1. In the 'App Id' field, paste this:
   53885fbf-03ae-429b-b24f-8c1c30d1bb59

2. Click the 'Lookup' button

3. In the 'Permission Request XML' field, paste this:
   <AppPermissionRequests AllowAppOnlyPolicy="true">
     <AppPermissionRequest Scope="http://sharepoint/content/sitecollection" Right="Write"/>
   </AppPermissionRequests>

4. Click 'Create'

5. Review the permissions and click 'Trust It'

Press ENTER to open the browser...
? Browser opened. Follow the instructions above.
```

### Method 2: PowerShell (For IT Professionals)

**Best for:** IT admins, automated deployments, managing multiple sites

**Steps:**
1. Select option **2**
2. Copy the PowerShell commands shown on screen
3. Open PowerShell as Administrator
4. Run the commands
5. Authenticate when prompted

**What You'll See:**
```
????????????????????????????????????????????????????????????
  PowerShell Method
????????????????????????????????????????????????????????????

Copy and run these commands in PowerShell:

# Install PnP PowerShell module (if not already installed)
Install-Module -Name PnP.PowerShell -Scope CurrentUser

# Connect to SharePoint Admin Center
Connect-PnPOnline -Url "https://tenant-admin.sharepoint.com" -Interactive

# Grant Write permission to the site
Grant-PnPAzureADAppSitePermission `
    -AppId "YOUR-CLIENT-ID" `
    -DisplayName "SharePoint List Manager" `
    -Site "https://your-site-url" `
    -Permissions Write

# Verify the permission was granted
Connect-PnPOnline -Url "https://your-site-url" -Interactive
Get-PnPAzureADAppSitePermission
```

### Method 3: Manual Instructions (Step-by-Step)

**Best for:** Those who prefer detailed written instructions

**Steps:**
1. Select option **3**
2. Follow the detailed step-by-step instructions
3. All URLs and values are provided
4. Copy/paste as needed

**What You'll See:**
```
????????????????????????????????????????????????????????????
  Manual Instructions
????????????????????????????????????????????????????????????

Step 1: Navigate to the App Permission page
????????????????????????????????????????????
https://your-site/_layouts/15/appinv.aspx

Step 2: Enter App Id
????????????????????????????????????????????
In the 'App Id' field, enter:
53885fbf-03ae-429b-b24f-8c1c30d1bb59
Then click 'Lookup'

[... detailed instructions continue ...]
```

### Understanding the Permission

**What Right="Write" Allows:**
- ? Create new columns
- ? Add list items
- ? Update list items
- ? Read all list data
- ? Generate test data
- ? Does NOT allow deleting lists or sites

**Scope="sitecollection":**
- Permission applies to the specific site only
- Does NOT grant access to other sites in your tenant
- Safe for production environments

### Verification

After granting permissions, verify they were applied:

**Using the App:**
- Return to main menu
- Try option **1** (Add Columns) - should work
- Try option **2** (Generate Data) - should work

**Using PowerShell:**
```powershell
Connect-PnPOnline -Url "https://your-site" -Interactive
Get-PnPAzureADAppSitePermission | Where-Object {$_.AppId -eq "YOUR-CLIENT-ID"}
```

Should show:
- **App Id**: Your client ID
- **Permission**: Write
- **Scope**: Site Collection

### Troubleshooting Permission Grants

**Issue: "App not found" when clicking Lookup**
- **Solution**: Verify the Client ID is correct in `appsettings.json`
- Check that you're using the App ID, not the Object ID

**Issue: Permission page shows error**
- **Solution**: Ensure you're a Site Collection Administrator
- Try the PowerShell method instead

**Issue: Permission granted but still getting "Access denied"**
- **Solution**: Wait 10-15 minutes for propagation
- Clear browser cache
- Restart the application

**Issue: Can't access appinv.aspx**
- **Solution**: Check if custom scripts are disabled
- Contact tenant administrator
- Use PowerShell method instead

### Security Note

?? **Important Security Information:**
- This grants app-only access (no user context needed)
- Permission is site-specific (not tenant-wide)
- Can be revoked at any time
- All operations are logged in SharePoint audit logs
- Follows Microsoft's recommended security practices

### Revoking Permissions

If you need to remove the permissions later:

**PowerShell:**
```powershell
Connect-PnPOnline -Url "https://your-site" -Interactive
$permissions = Get-PnPAzureADAppSitePermission
$myApp = $permissions | Where-Object {$_.AppId -eq "YOUR-CLIENT-ID"}
Revoke-PnPAzureADAppSitePermission -PermissionId $myApp.Id
```

**SharePoint Admin Center:**
1. Go to SharePoint Admin Center
2. Active Sites ? Select your site
3. Permissions ? App Permissions
4. Find your app and revoke

### Tips
- Use Method 1 (Browser) for first-time setup - it's the easiest
- Use Method 2 (PowerShell) for automating multiple sites
- Always grant "Write" permission (not "Read") for full functionality
- Wait 5-10 minutes after granting before testing
- Bookmark the appinv.aspx URL for future use

---

## Troubleshooting

### Column Already Exists
**Error:** "Column 'XYZ' already exists"
**Solution:** Remove that column from your CSV file or delete it from SharePoint first

### Access Denied on Column Creation
**Error:** "Access denied" when adding columns
**Solution:** Ensure your app has `Sites.ReadWrite.All` permission (not just `Sites.Read.All`)

### Person or Group Columns
**Note:** Person or Group fields are created but won't be populated with test data
**Reason:** The tool cannot generate valid user references automatically

### Large Data Generation is Slow
**Normal:** Creating 1,000+ rows takes time (API rate limiting)
**Tip:** Run during off-hours or in smaller batches

### Export Missing Data
**Check:** `appsettings.json` "Columns" setting
**Solution:** Use `"*"` to include all columns

---

## Best Practices

### Before Adding Columns
1. Review your CSV file for typos
2. Test with a small subset first
3. Back up your list if it contains important data

### Test Data Generation
1. Start small (10-50 rows) to verify
2. Delete test data when done
3. Don't generate test data in production lists

### Exporting Data
1. Schedule exports during low-usage times
2. Keep exported files for backup
3. Consider version control for schema changes

---

## Sample Workflows

### Workflow 1: New List Setup
1. Create empty SharePoint list
2. Prepare CSV with column definitions
3. Run option **1** to add columns
4. Run option **2** to generate test data (50-100 rows)
5. Verify in SharePoint
6. Run option **3** to export and verify all data

### Workflow 2: List Migration
1. Export source list using option **3**
2. Create column CSV from exported JSON structure
3. Create new list in SharePoint
4. Run option **1** to add columns
5. Import data using SharePoint's import feature or custom script

### Workflow 3: Testing & Demo
1. Add columns to blank list (option **1**)
2. Generate large test dataset (option **2**, 500-1000 rows)
3. Test your application/workflow
4. Export results (option **3**)
5. Analyze exported data

---

## Command Reference

| Menu Option | Purpose | Required Permission |
|-------------|---------|-------------------|
| 1 | Add Columns | Sites.ReadWrite.All + Site Write |
| 2 | Generate Data | Sites.ReadWrite.All + Site Write |
| 3 | Export Data | Sites.Read.All |
| 4 | Grant Permissions | Site Collection Admin |
| 5 | Exit | None |

---

## File Locations

| File | Purpose | Location |
|------|---------|----------|
| appsettings.json | Configuration | Same as executable |
| sample-columns.csv | Template for column import | Same as executable |
| *.csv | Your column definitions | Anywhere (provide path) |
| YYYYMMDDHHmmss.json | Exported list data | Same as executable |

---

## Support

For issues related to:
- **Authentication:** See `TROUBLESHOOTING.md`
- **Permissions:** See `README.md` - "Required API Permissions"
- **Column Types:** See SharePoint column documentation
- **API Limits:** See Microsoft Graph API throttling docs
