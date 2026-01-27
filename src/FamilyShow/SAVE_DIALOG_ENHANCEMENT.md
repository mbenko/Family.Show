# Save Dialog Enhancement - Implementation Summary

## Overview
Enhanced the FamilyShow WPF application's save prompt dialog to include a Cancel option and set the default filename to the currently open file when saving.

## Changes Implemented

### 1. CommonDialog.cs
**Location:** `FamilyShow/CommonDialog.cs`

**Change:** Added a setter to the `FileName` property to support pre-populating the save dialog.

```csharp
public string FileName
{
  get { return openFileName.file; }
  set
  {
    // Set the initial filename - need to ensure it fits in the buffer
    if (!string.IsNullOrEmpty(value))
    {
      openFileName.file = value.PadRight(260, '\0');
    }
  }
}
```

### 2. MainWindow.xaml.cs - PromptToSave Method
**Location:** `FamilyShow/MainWindow.xaml.cs` (lines 951-1010)

**Changes:**
- Changed return type from `void` to `bool`
- Changed `MessageBoxButton.YesNo` to `MessageBoxButton.YesNoCancel`
- Added handling for `MessageBoxResult.Cancel` to return `true` (operation should be canceled)
- Added logic to set default filename if a file is currently open
- Added handling for when user cancels the save dialog itself

**Key Logic:**
```csharp
// Set default filename to current file if it exists and is not a new tree
if (!string.IsNullOrEmpty(familyCollection.FullyQualifiedFilename) && 
    File.Exists(familyCollection.FullyQualifiedFilename))
{
  dialog.FileName = familyCollection.FullyQualifiedFilename;
}
```

### 3. MainWindow.xaml.cs - OnClosing Method
**Location:** `FamilyShow/MainWindow.xaml.cs` (lines 939-949)

**Changes:**
- Now checks the return value from `PromptToSave()`
- Sets `e.Cancel = true` if user clicked Cancel
- Prevents window from closing when user cancels

### 4. MainWindow.xaml.cs - OpenFamily Method
**Location:** `FamilyShow/MainWindow.xaml.cs` (lines 260-266)

**Changes:**
- Checks return value from `PromptToSave()`
- Returns early if user canceled, preventing new file from being opened

### 5. MainWindow.xaml.cs - OpenRecentFile_Click Method
**Location:** `FamilyShow/MainWindow.xaml.cs` (lines 319-332)

**Changes:**
- Checks return value from `PromptToSave()`
- Returns early if user canceled, preventing recent file from being opened

### 6. MainWindow.xaml.cs - ImportGedcom Method
**Location:** `FamilyShow/MainWindow.xaml.cs` (lines 455-467)

**Changes:**
- Checks return value from `PromptToSave()`
- Returns early if user canceled, preventing GEDCOM import

## User Experience

### Before Changes
When closing the app or opening a new file with unsaved changes:
- Prompt showed: **Yes** | **No**
- No way to cancel the operation
- Save dialog had empty filename even if a file was already open

### After Changes
When closing the app or opening a new file with unsaved changes:
- Prompt shows: **Yes** | **No** | **Cancel**
- **Yes**: Opens save dialog with current filename pre-filled (if file was previously saved), saves, then continues operation
- **No**: Discards changes and continues operation
- **Cancel**: Cancels the entire operation (close/open/import) and returns to the app
- If **Yes** is clicked but user cancels the save dialog, the operation is also canceled

## Testing Scenarios

1. **Close app with new unsaved tree**
   - Click Cancel ? App stays open ?
   - Click No ? App closes without saving ?
   - Click Yes ? Save dialog opens with empty filename ? Save or Cancel

2. **Close app with existing file modified**
   - Click Cancel ? App stays open ?
   - Click No ? App closes without saving ?
   - Click Yes ? Save dialog opens with current filename pre-filled ? Save or Cancel

3. **Open new file with unsaved changes**
   - Same Cancel/Yes/No behavior as close
   - Cancel prevents opening new file ?

4. **Import GEDCOM with unsaved changes**
   - Same Cancel/Yes/No behavior
   - Cancel prevents import ?

## Build Status
? Build successful - All changes compile without errors

## Notes
- Changes are scoped to WPF application only (`FamilyShow` project)
- Web projects (Blazor/Razor Pages) are not affected
- Maintains existing code style and patterns
- No breaking changes to existing functionality

---
*Implementation Date: January 26, 2026*
