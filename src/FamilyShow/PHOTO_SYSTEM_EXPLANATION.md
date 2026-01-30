# Photo System in FamilyShow

## How Photos Are Stored and Referenced

### File Storage Architecture
1. **`.familyx` File Structure**
   - The `.familyx` file is a **ZIP archive** (OPC - Open Package Convention)
   - Contains:
     - `content.xml` - Serialized family tree data
     - `Images/` folder - All photo files (JPG, PNG, GIF)
     - `Stories/` folder - RTF story files

2. **Runtime Storage Location**
   - When opened, ZIP is extracted to: `%LocalAppData%\Family.Show\CurrentFamily\`
   - Photos are extracted to: `CurrentFamily\Images\`
   - Example: `C:\Users\<username>\AppData\Local\Family.Show\CurrentFamily\Images\photo.jpg`

### Photo-Person Relationship

#### Person Class
```csharp
public class Person
{
    // Collection of photos attached to this person
    public PhotoCollection Photos { get; }
}
```

#### Photo Class Properties
```csharp
public class Photo
{
    // Stored in XML - relative path within the archive
    // Example: "Images/john_smith_wedding.jpg"
    public string RelativePath { get; set; }
    
    // Computed at runtime - full path on disk
    // Example: "C:\Users\mike\AppData\Local\Family.Show\CurrentFamily\Images\john_smith_wedding.jpg"
    public string FullyQualifiedPath { get; }
    
    // Marks which photo is the person's primary/avatar photo
    public bool IsAvatar { get; set; }
}
```

#### PhotoCollection
```csharp
// Simple collection wrapper
public class PhotoCollection : ObservableCollection<Photo>
{
}
```

### XML Serialization Example
In `content.xml`, a person's photos are serialized like:
```xml
<Person>
  <FirstName>John</FirstName>
  <LastName>Smith</LastName>
  <Photos>
    <Photo>
      <RelativePath>Images/john_smith_portrait.jpg</RelativePath>
      <IsAvatar>true</IsAvatar>
    </Photo>
    <Photo>
      <RelativePath>Images/john_smith_wedding.jpg</RelativePath>
      <IsAvatar>false</IsAvatar>
    </Photo>
  </Photos>
</Person>
```

## Gallery Window Implementation

### Thumbnail Display
The `PhotoGalleryWindow` now shows:
- Thumbnail image (150x150 button)
- **Filename label below** (10pt, truncated with ellipsis)
- Tooltip with full filename
- Missing/error states with appropriate placeholders

### States Handled
1. **Normal Photo**
   - Loads image from disk
   - Shows filename below: `john_smith.jpg`
   - Gray text using theme `FontColor`

2. **Missing Photo**
   - File referenced in XML but not on disk
   - Shows "Image Not Found" placeholder
   - Filename in **gray** text
   - Tooltip shows full path for debugging

3. **Load Error**
   - File exists but can't be loaded (corrupt, wrong format)
   - Shows "Load Error" placeholder
   - Filename in **red** text
   - Tooltip shows error message

### Code Structure
```csharp
for each photo in person.Photos:
    - Extract filename from photo.RelativePath
    - Create StackPanel container (thumbnail + label)
    - Check if file exists
    - If exists: Load image, show thumbnail
    - If missing: Show placeholder
    - Add filename TextBlock below (always shown)
    - Add container to WrapPanel
```

## Benefits of This Approach
1. **Portability** - Single `.familyx` file contains everything
2. **Simplicity** - Relative paths work across different computers
3. **Efficiency** - Photos extracted once when file opened
4. **Debugging** - Filename labels help identify missing/corrupt photos
5. **User Friendly** - Clear visual feedback for all photo states

## Related Files
- `FamilyShowLib/Photo.cs` - Photo model and collection
- `FamilyShowLib/Person.cs` - Person model with Photos property
- `FamilyShowLib/OPCUtility.cs` - ZIP archive handling
- `FamilyShow/PhotoGalleryWindow.xaml.cs` - Gallery UI implementation
