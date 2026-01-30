# Photo Icon with Drag & Drop Functionality

## Overview
Implemented photo icon column in family list with drag-drop support and photo removal capability.

## Changes Made

### 1. Family List - Photo Icon Column (Details.xaml)
**Location:** Before Name column in GridView

**Features:**
- Separate 25px wide column for photo icon
- Icon only shows if person has photos (`HasPhotosConverter`)
- Click icon → Opens photo gallery
- Drag image onto icon → Adds photo to person
- Tooltip: "Click to view photos or drag image to add"

**Icon Design:**
- 16x16 camera icon
- Themed using `FontColor` resource
- Hand cursor on hover
- AllowDrop enabled

### 2. Drag & Drop Implementation (Details.xaml.cs)

#### Event Handlers:

**`PhotoIcon_DragEnter` / `PhotoIcon_DragOver`:**
- Validates dropped files are images (.jpg, .jpeg, .png, .gif, .bmp)
- Shows copy cursor for valid image files
- Rejects non-image files

**`PhotoIcon_Drop`:**
- Accepts dropped image file
- Creates new `Photo` object from file path
- Adds photo to `person.Photos` collection
- Copies file to `/Images` folder (handled by Photo constructor)
- Refreshes UI to show icon
- Shows success message

**Example Usage:**
```csharp
// Drag an image file from Windows Explorer onto the photo icon
// The Photo constructor handles copying to Images folder:
Photo photo = new Photo(files[0]); // Copies to %LocalAppData%\Family.Show\CurrentFamily\Images\
person.Photos.Add(photo);
```

### 3. Photo Gallery - Remove Photo Feature (PhotoGalleryWindow.xaml.cs)

#### Context Menu Updated:
- **"Open in Explorer"** - Opens folder with file selected
- **"Remove Photo"** - NEW: Removes photo with confirmation

#### `RemovePhoto_Click` Handler:

**Process:**
1. Shows confirmation dialog: "Are you sure you want to remove this photo?"
2. If confirmed:
   - Removes photo from `person.Photos` collection
   - Deletes physical file if it exists
   - Handles missing files gracefully (only removes reference)
   - Refreshes gallery thumbnails
   - Updates header count
   - Returns to thumbnail view if viewing that photo
3. Shows success message

**Safety Features:**
- Confirmation dialog before deletion
- No error if file already deleted
- Graceful handling of edge cases

## User Workflows

### Adding Photos (3 Methods)

**Method 1: Drag & Drop onto Icon**
1. Find person in family list
2. Drag image file from Windows Explorer
3. Drop onto photo icon (camera icon appears in left column)
4. Photo added automatically

**Method 2: From Photo Gallery**
(Existing functionality - Edit Person → Photos & Stories)

**Method 3: From Details Panel**
(Existing functionality - drag onto avatar area)

### Removing Photos

1. Click photo icon in family list → Opens gallery
2. Right-click on thumbnail
3. Select "Remove Photo"
4. Confirm deletion
5. Photo deleted from disk and removed from person

### Viewing Photos

1. Click photo icon (📷) in family list
2. Gallery opens with all photos
3. Click thumbnail to view full size
4. Use arrows or keyboard to navigate

## Technical Details

### Photo Storage
- Files copied to: `%LocalAppData%\Family.Show\CurrentFamily\Images\`
- Stored in ZIP archive when saved: `.familyx` file → `Images/` folder
- References stored as relative paths: `Images/filename.jpg`

### Supported Image Formats
- JPG/JPEG
- PNG
- GIF
- BMP

### Icon Visibility Logic
```csharp
// HasPhotosConverter
person.Photos != null && person.Photos.Count > 0
  ? Visibility.Visible
  : Visibility.Collapsed
```

### Data Binding
- Icon column binds to `Person` object via Tag
- Click/Drop handlers retrieve person from Tag
- UI refreshes automatically via `ICollectionView.Refresh()`

## UI Layout

```
Family List Columns:
┌────┬──────────────┬──────┬──────┬─────┐
│ 📷 │ Name         │ Born │ Died │ Age │
├────┼──────────────┼──────┼──────┼─────┤
│ 📷 │ John Smith   │ 1950 │  -   │  74 │
│    │ Jane Doe     │ 1955 │  -   │  69 │  ← No icon (no photos)
│ 📷 │ Bob Johnson  │ 1948 │ 2020 │  72 │
└────┴──────────────┴──────┴──────┴─────┘
```

## Benefits

1. **Quick Access:** One-click to view photos from family list
2. **Easy Addition:** Drag-drop from Explorer (no dialogs)
3. **Visual Indicator:** Instantly see who has photos
4. **Photo Management:** Remove unwanted photos easily
5. **Safe Deletion:** Confirmation prevents accidents
6. **Clean UI:** Icon in dedicated column (not cluttering name)

## Future Enhancements (Potential)

- Drag multiple images at once
- Show photo count badge on icon
- Drag photo between people
- Set primary/avatar photo from icon
- Thumbnail preview on hover
- Story icon in adjacent column

---
*Last Updated: January 2025 - Photo icon with drag-drop and removal functionality*
