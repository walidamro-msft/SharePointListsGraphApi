# CSV File Format Guide

## Supported Delimiters

The application automatically detects and supports:
- **Comma** (`,`) - Most common
- **Tab** (`\t`) - Excel default
- **Semicolon** (`;`) - European Excel
- **Pipe** (`|`) - Alternative

## CSV File Structure

### Header Row (Required)
```
Column Name,Column Type
```

### Data Rows
```
FirstName,Text
LastName,Text
BirthDate,Date
```

## Example Files Included

### 1. `sample-columns.csv` (Tab-delimited)
Tab-delimited file with 22 real-world columns:
```
Column Name	Type
AgencyAddress	Single line of text
AgencyDueDate	Date and Time
AmountOfCalls	Number
```

### 2. `sample-columns-comma.csv` (Comma-delimited)
Comma-delimited file with common columns:
```
Column Name,Column Type
FirstName,Text
LastName,Text
BirthDate,Date
```

## Supported Column Types

### Full Names (Recommended)
```
Single line of text
Multiple lines of text
Number
Date and Time
Choice
Person or Group
Yes/No
Hyperlink
Currency
```

### Short Names (Also Supported)
```
Text
MultiLine
Number
Date
Choice
Person
Boolean
URL
Money
```

### Aliases (Flexible)
```
Text          ? string, singleline
MultiLine     ? note, multilinetext
Number        ? integer, int, decimal
Date          ? datetime, timestamp
Choice        ? dropdown, select
Person        ? user, people
Boolean       ? bool, checkbox, yes/no
Hyperlink     ? link, url
Currency      ? money, price
```

## Creating Your CSV File

### Option 1: Excel
1. Open Excel
2. Create two columns: "Column Name" and "Column Type"
3. Fill in your data
4. Save As ? CSV (Comma delimited) (*.csv)

### Option 2: Notepad
1. Open Notepad
2. Type your columns using commas:
   ```
   Column Name,Column Type
   FirstName,Text
   LastName,Text
   ```
3. Save As ? All Files ? filename.csv

### Option 3: Copy from Sample
1. Copy `sample-columns.csv` or `sample-columns-comma.csv`
2. Edit to add/remove columns
3. Save

## Common Issues & Solutions

### Issue: "No valid columns found"
**Cause:** Missing delimiter or wrong format

**Solution:** 
- Check that you have two columns separated by comma or tab
- Make sure first line is the header
- Ensure file has at least one data row

Example of correct format:
```
Column Name,Column Type
FirstName,Text
```

### Issue: "Warning: Line X has only one column"
**Cause:** Missing column type

**Solution:** 
Add the column type for that row
```
Before: FirstName
After:  FirstName,Text
```

### Issue: "Unknown column type 'XYZ'"
**Cause:** Unrecognized column type

**Solution:** 
Use one of the supported types (see list above). The app will default to "Text" but shows a warning.

Supported: Text, Number, Date, Choice, Person, Boolean, Hyperlink, Currency

### Issue: Delimiter not detected correctly
**Cause:** Unusual delimiter or mixed delimiters

**Solution:**
- Use consistent delimiters throughout the file
- Stick to comma (,) or tab for best compatibility
- Check for extra spaces

## Testing Your CSV

1. Open the file in Notepad to verify format
2. Check that all lines have the same number of columns
3. Look for the delimiter character between columns
4. Run the app - it will show detected delimiter:
   ```
   Detected delimiter: ','
   ```

## Best Practices

? **Do:**
- Use simple column names (no special characters)
- Use comma or tab delimiters
- Include header row
- Save as .csv extension
- Test with a few columns first

? **Don't:**
- Mix delimiters in the same file
- Use special characters in column names (% & * : < > ? / \ |)
- Forget the column type
- Skip the header row

## Quick Examples

### Minimal Example
```csv
Column Name,Column Type
Title,Text
```

### Employee List
```csv
Column Name,Column Type
EmployeeID,Text
FirstName,Text
LastName,Text
Department,Choice
HireDate,Date
Salary,Currency
IsActive,Yes/No
Notes,Multiple lines of text
```

### Contact List
```csv
Column Name,Column Type
FullName,Text
Email,Text
Phone,Text
Company,Text
ContactDate,Date
FollowUp,Yes/No
Manager,Person or Group
```

### Project Tracker
```csv
Column Name,Column Type
ProjectName,Text
Description,Multiple lines of text
StartDate,Date
EndDate,Date
Budget,Currency
Status,Choice
ProjectManager,Person or Group
Website,Hyperlink
```

## Troubleshooting Checklist

Before importing, verify:
- [ ] File is saved as .csv
- [ ] First line is header (Column Name, Column Type)
- [ ] All rows have both column name and type
- [ ] Using supported column types
- [ ] No special characters in column names
- [ ] Consistent delimiter throughout file
- [ ] File has at least one data row (besides header)

## App Output Examples

### Successful Import
```
Reading columns from my-columns.csv...
Detected delimiter: ','
Found 5 columns to add.

[????????????????????] 100% - Adding column 'FirstName'...

? Successfully added 5 columns
```

### With Warnings
```
Reading columns from my-columns.csv...
Detected delimiter: ','

? Warning: Line 3 has only one column. Expected format: 'Column Name,Column Type'
? Warning: Unknown column type 'CustomType' for column 'Field1'. Using 'Text' as default.

Found 4 columns to add.
```

## Need Help?

- Check **USER-GUIDE.md** for feature details
- Check **TROUBLESHOOTING.md** for common issues
- Use included sample files as templates
