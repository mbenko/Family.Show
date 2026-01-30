# Family List Photo and Story Icons

## Overview
Added small icons to the First Name column in the Family Data list to indicate when a person has:
- ?? Photos attached (image icon)
- ?? Story attached (document icon)

Clicking either icon opens the person details view.

## Changes Made

### 1. src/FamilyShow/ValueConverters.cs

**Added three new value converters:**

#### HasPhotosConverter (lines 130-153)
```csharp
public class HasPhotosConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is FamilyShowLib.Person person)
    {
      bool hasPhotos = person.Photos != null && person.Photos.Count > 0;
      return hasPhotos ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }
    return System.Windows.Visibility.Collapsed;
  }
}
```
- Checks if person has any photos
- Returns Visible/Collapsed for icon visibility

#### HasStoryConverter (lines 155-178)
```csharp
public class HasStoryConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is FamilyShowLib.Person person)
    {
      bool hasStory = person.Story != null;
      return hasStory ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }
    return System.Windows.Visibility.Collapsed;
  }
}
```
- Checks if person has a story
- Returns Visible/Collapsed for icon visibility

#### BooleanToVisibilityConverter (lines 180-203)
```csharp
public class BooleanToVisibilityConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is bool boolValue)
    {
      return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
    }
    return System.Windows.Visibility.Collapsed;
  }
}
```
- General-purpose boolean to visibility converter
- May be useful for other UI features

### 2. src/FamilyShow/Controls/FamilyData/FamilyData.xaml

#### Added Converter Resources (lines 14-16)
```xml
<local:HasPhotosConverter x:Key="HasPhotosConverter" />
<local:HasStoryConverter x:Key="HasStoryConverter" />
<local:BooleanToVisibilityConverter x:Key="BooleanToVisibilityConverter" />
```

#### Updated FirstNameColumnTemplate (lines 55-97)
**Before:** Simple TextBox
```xml
<DataTemplate x:Key="FirstNameColumnTemplate">
  <TextBox Style="{StaticResource TextBoxStyle}" Text="{Binding Path=FirstName}" />
</DataTemplate>
```

**After:** DockPanel with icons and TextBox
```xml
<DataTemplate x:Key="FirstNameColumnTemplate">
  <DockPanel>
    <!--  Photo icon  -->
    <Path
      Width="12"
      Height="12"
      Margin="2,0,2,0"
      Data="M3,3 L11,3 L11,11 L3,11 Z M5,6 A1.5,1.5 0 1,1 5,9 A1.5,1.5 0 1,1 5,6 M10,9 L8,7 L6,9 L4,7 L3,10 L11,10 Z"
      DockPanel.Dock="Left"
      Fill="{DynamicResource FontColor}"
      Stretch="Uniform"
      ToolTip="Has Photos - Click to open details"
      Cursor="Hand"
      MouseLeftButtonDown="PhotoIcon_Click"
      Tag="{Binding}"
      Visibility="{Binding Path=., Converter={StaticResource HasPhotosConverter}}" />
    <!--  Story icon  -->
    <Path
      Width="12"
      Height="12"
      Margin="2,0,2,0"
      Data="M4,2 L10,2 L10,12 L4,12 Z M5,4 L9,4 M5,6 L9,6 M5,8 L9,8 M5,10 L8,10"
      DockPanel.Dock="Left"
      Fill="{DynamicResource FontColor}"
      Stretch="Uniform"
      Stroke="{DynamicResource FontColor}"
      StrokeThickness="0.5"
      ToolTip="Has Story - Click to open details"
      Cursor="Hand"
      MouseLeftButtonDown="StoryIcon_Click"
      Tag="{Binding}"
      Visibility="{Binding Path=., Converter={StaticResource HasStoryConverter}}" />
    <!--  First Name TextBox  -->
    <TextBox
      Style="{StaticResource TextBoxStyle}"
      Text="{Binding Path=FirstName}"
      DockPanel.Dock="Left" />
  </DockPanel>
</DataTemplate>
```

### 3. src/FamilyShow/Controls/FamilyData/FamilyData.xaml.cs

#### Added Click Event Handlers (lines 203-229)

**PhotoIcon_Click Handler:**
```csharp
private void PhotoIcon_Click(object sender, RoutedEventArgs e)
{
  if (sender is System.Windows.Shapes.Path path && path.Tag is Person person)
  {
    // Set the selected person and raise the close event to return to details view
    App.Family.Current = person;
    RaiseEvent(new RoutedEventArgs(CloseButtonClickEvent));
  }
}
```

**StoryIcon_Click Handler:**
```csharp
private void StoryIcon_Click(object sender, RoutedEventArgs e)
{
  if (sender is System.Windows.Shapes.Path path && path.Tag is Person person)
  {
    // Set the selected person and raise the close event to return to details view
    App.Family.Current = person;
    RaiseEvent(new RoutedEventArgs(CloseButtonClickEvent));
  }
}
```

## Icon Design

### Photo Icon (??)
```
M3,3 L11,3 L11,11 L3,11 Z 
M5,6 A1.5,1.5 0 1,1 5,9 A1.5,1.5 0 1,1 5,6 
M10,9 L8,7 L6,9 L4,7 L3,10 L11,10 Z
```
- Represents a camera/photo frame
- Circle in center (lens)
- Mountain/landscape shape (typical photo)
- 12x12 pixels

### Document Icon (??)
```
M4,2 L10,2 L10,12 L4,12 Z 
M5,4 L9,4 
M5,6 L9,6 
M5,8 L9,8 
M5,10 L8,10
```
- Represents a document page
- Horizontal lines (text lines)
- 12x12 pixels

## Features

### Icon Visibility
- **Photo icon appears** when `person.Photos != null && person.Photos.Count > 0`
- **Story icon appears** when `person.Story != null`
- **No icon** shown if person has neither photos nor story
- Icons only visible when person has the corresponding content

### Icon Appearance
- **Size**: 12x12 pixels (small, unobtrusive)
- **Color**: Uses `{DynamicResource FontColor}` to match theme
- **Position**: Before first name text in the column
- **Spacing**: 2px margins between icons and text
- **Cursor**: Changes to hand cursor on hover
- **Tooltip**: 
  - "Has Photos - Click to open details"
  - "Has Story - Click to open details"

### Click Behavior
1. User clicks photo or story icon
2. Person is set as current: `App.Family.Current = person`
3. CloseButtonClick event is raised
4. Family Data view closes
5. Main view shows person details panel
6. User can view/edit photos or story

### Theme Integration
- Uses `FontColor` brush from active theme
- Icons match text color in Black theme (light gray)
- Will adapt to other themes (Silver, Blue, etc.)
- Consistent with application's visual style

## User Experience

### Scenario: Finding People with Photos
1. Open Family Data view
2. Scan down the First Name column
3. Look for photo icons (??)
4. Click icon to quickly jump to that person's photos

### Scenario: Finding People with Stories
1. Open Family Data view
2. Scan down the First Name column
3. Look for document icons (??)
4. Click icon to quickly jump to that person's story

### Scenario: Quick Navigation
1. User wants to edit a specific person's story
2. Instead of:
   - Finding person in list
   - Selecting the row
   - Clicking Back button
   - Navigating to story tab
3. Simply:
   - Click the document icon next to their name
   - Immediately in details view ready to edit

## Technical Details

### Data Binding
- Converters bind to entire Person object: `{Binding Path=.}`
- Tag property stores Person reference: `Tag="{Binding}"`
- Event handler retrieves Person from Tag property
- No additional properties needed on Person class

### Event Flow
1. MouseLeftButtonDown event fires on Path element
2. Event handler checks if sender is Path with Person Tag
3. Sets App.Family.Current to the person
4. Raises CloseButtonClick routed event
5. Main window handles event and shows details

### Performance
- Icons only rendered when visible (Visibility=Collapsed when not needed)
- No impact on scrolling or list performance
- Converter called only when data changes
- Lightweight Path rendering

## Build Status
? **Build successful** - Ready for testing

## Testing

**Test Icon Visibility:**
1. Launch FamilyShow application
2. Open Family Data view
3. Verify photo icon appears for people with photos
4. Verify document icon appears for people with stories
5. Verify no icons for people without photos/stories

**Test Icon Click:**
1. Click a photo icon
2. Verify person details opens
3. Verify correct person is selected
4. Go back to Family Data
5. Click a story icon
6. Verify person details opens
7. Verify correct person is selected

**Test Theme Integration:**
1. View icons in Black theme ? light gray icons
2. Switch to Silver theme (if available) ? verify colors adapt
3. Icons should always be readable against background

**Test Multiple Icons:**
1. Find person with both photos AND story
2. Verify both icons appear
3. Photo icon first, document icon second
4. Both clickable and functional

## Future Enhancements (Optional)
- Add icon for contacts/addresses
- Add icon for baptism events
- Different icon for primary/avatar photo
- Icon badge showing photo count (e.g., "3 photos")
- Tooltip showing photo count or story excerpt

## Date
January 2025
