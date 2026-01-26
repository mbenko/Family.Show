# Copilot Instructions for FamilyShow

## Project Overview
FamilyShow is a genealogy application with a hybrid architecture:
- **WPF Desktop App** (`FamilyShow/`) - Original Windows application using .NET Framework 4.8
- **Web App** (`FamilyShowWeb/`) - Modern web interface using .NET 9.0 
- **Shared Core Library** (`FamilyShowLib/`) - Business logic and domain models
- **Shared Web Library** (`FamilyShowLib.Shared/`) - Adapted models for web scenarios

## Architecture Patterns

### Domain Model Structure
The genealogy domain is built around core entities in `FamilyShowLib/`:
- `Person` - Central entity with demographics, relationships, photos, and stories
- `People` - Root aggregate containing `PeopleCollection` and metadata (XML serializable)
- `Relationship` - Abstract base with concrete types: `ParentRelationship`, `ChildRelationship`, `SpouseRelationship`, `SiblingRelationship`
- `Photo`, `Story`, `Contact` - Rich content attached to persons

```csharp
// Key pattern: Global static collections shared across application
public static People FamilyCollection = new People();
public static PeopleCollection Family = FamilyCollection.PeopleCollection;
```

### Data Persistence & File Format
- **Native Format**: `.familyx` files are **OPC (Open Package Convention) ZIP ARCHIVES** containing:
  - `content.xml` - Serialized `People` object with family tree data
  - `Photos/` folder - Image files (JPG, PNG, GIF)
  - `Stories/` folder - Rich text files (RTF) with person narratives
- **GEDCOM Support**: Import/export genealogy standard format via `GedcomImport`/`GedcomExport`
- **File Structure**: User documents stored in `%USERPROFILE%\Documents\Family.Show\`

#### Critical File Format Issue
**FamilyShowWeb currently broken**: The web app tries to deserialize `.familyx` files as direct XML, but they're ZIP archives. Use `OPCUtility.ExtractPackage()` to extract, then deserialize the `content.xml` file.

```csharp
// Correct pattern for .familyx files:
OPCUtility.ExtractPackage(familyFilePath, tempFolder);
// Then deserialize: tempFolder + "content.xml"
```

### WPF Application Patterns
- **MVVM**: Data binding with `INotifyPropertyChanged` on domain models
- **Value Converters**: Extensive use for UI formatting (`FirstNamePossessiveFormConverter`, `PrimaryAvatarConverter`, etc.)
- **Routed Commands**: Custom commands like `ImportGedcomCommand`, `ExportXpsCommand`
- **Control Composition**: Custom UserControls in `Controls/` folder (Details, PersonInfo, Welcome)

### Web Application Architecture
- **ASP.NET Core Razor Pages**: Modern web interface in `FamilyShowWeb/`
- **Shared Models**: Uses `FamilyShowLib.Shared/` for web-compatible domain models
- **File Upload**: Large file support (300MB limit) for family archive uploads
- **Known Issue**: Web app cannot currently read `.family` OPC archives - needs `OPCUtility` integration

## Development Workflows

### Building & Testing
```powershell
# Build entire solution
msbuild FamilyShow.sln /p:Configuration=Release

# Run unit tests (MSTest framework)
dotnet test UnitTestFamilyShow/

# Web project (modern .NET)
cd FamilyShowWeb
dotnet run
```

### Key File Locations
- **Domain Models**: `FamilyShowLib/*.cs` (Person, People, Relationship hierarchy)
- **Web Models**: `FamilyShowLib.Shared/*.cs` (adapted for web scenarios)
- **UI Logic**: `FamilyShow/MainWindow.xaml.cs` (1000+ lines, central orchestration)
- **Web Pages**: `FamilyShowWeb/Pages/*.cshtml.cs` (Razor page models)
- **Import/Export**: `FamilyShowLib/Gedcom*.cs` files
- **OPC Utilities**: `FamilyShowLib/OPCUtility.cs` (ZIP archive handling)
- **Sample Data**: `FamilyShow/Sample Files/` with Windsor family example

## Project-Specific Conventions

### Relationship Management
Relationships are bidirectional but stored unidirectionally. Use `RelationshipHelper` for complex family tree operations:
```csharp
// Pattern: Check for existing relationship before adding
if (!person1.Relationships.Contains(person2))
    person1.Relationships.Add(new SpouseRelationship(person2));
```

### ID Management
- Person IDs are string GUIDs, stored separately from object references for XML serialization
- Pattern: Store `personId` and `personFullname` in relationships to avoid circular references

### Date Handling
Support flexible date formats for genealogy data:
- Full dates: `DateTime?` for birth/death dates
- Partial dates: String storage for incomplete historical data
- Format variations: "MM/dd/yyyy", "MM-dd-yyyy" patterns in tests

### File Extensions & Associations
- `.familyx` - Native FamilyShow format (OPC archive)
- `.ged` - GEDCOM genealogy standard
- Application folder: `Family.Show` in user documents

## Integration Points

### GEDCOM Interoperability
Critical for genealogy software ecosystem. Import process:
1. Convert GEDCOM → temp XML via `GedcomConverter`
2. Parse individuals first, then families
3. Resolve person references after all entities loaded

### Photo Management
Photos stored in application folder with database references:
- `Photo.FullyQualifiedPath` for file system integration
- `PhotoCollection` manages multiple photos per person
- Support for avatar/primary photo designation

### Skin/Theme System
UI themes in `Skins/` folder - WPF resource dictionary pattern for visual customization.

---
*Last updated: July 2025 - Focus on core patterns that enable rapid feature development in this genealogy domain*
