# Family List Age Display and Styling Improvements

## Overview
Enhanced the Family Data list to:
1. Display "-" for age when person is deceased without a death date
2. Confirm all columns are editable (except Age which is calculated)
3. Update styling to match the application theme

## Changes Made

### 1. FamilyShowLib/Person.cs - Age Property

**Updated Age calculation** (lines 241-267):
```csharp
public int? Age
{
  get
  {
    if (BirthDate == null)
    {
      return null;
    }

    // If person is deceased but we don't have a death date, age is unknown
    if (!IsLiving && DeathDate == null)
    {
      return null;
    }

    // Calculate age based on birth and death/current date
    DateTime startDate = BirthDate.Value;
    DateTime endDate = (IsLiving || DeathDate == null) ? DateTime.Now : DeathDate.Value;
    int age = endDate.Year - startDate.Year;

    // Compensate for month and day
    if (endDate.Month < startDate.Month || 
        (endDate.Month == startDate.Month && endDate.Day < startDate.Day))
    {
      age--;
    }

    return Math.Max(0, age);
  }
}
```

**Key Logic Change:**
- Returns `null` when person is deceased (`IsLiving = false`) but has no death date
- This handles the case where we only know birth year but person is marked deceased

### 2. FamilyShow/ValueConverters.cs - New Converter

**Added NullToDashConverter** (lines 105-125):
```csharp
public class NullToDashConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null)
    {
      return "-";
    }
    return value.ToString();
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return Binding.DoNothing; // Age is read-only
  }
}
```

### 3. FamilyShow/Controls/FamilyData/FamilyData.xaml

#### Added NullToDashConverter Resource (line 13)
```xml
<local:NullToDashConverter x:Key="NullToDashConverter" />
```

#### Updated Age Column Template (lines 62-65)
```xml
<DataTemplate x:Key="AgeColumnTemplate">
  <TextBlock Style="{StaticResource AgeTextBlockStyle}" 
             Text="{Binding Path=Age, Converter={StaticResource NullToDashConverter}}" />
</DataTemplate>
```

#### Updated Filter Area Styling (lines 357-370)
**Before:** Hardcoded colors
```xml
Background="#90ffffff"
Foreground="#FF1F3066"
```

**After:** Dynamic theme brushes
```xml
Background="{DynamicResource InputBackgroundBrush}"
BorderBrush="{DynamicResource BorderBrush}"
BorderThickness="1"
Foreground="{DynamicResource FontColor}"
```

#### Updated Count Label Styling (lines 378-383)
**Before:** Hardcoded color
```xml
Foreground="#FFCCCCCC"
```

**After:** Dynamic theme brush
```xml
Foreground="{DynamicResource AlternateFontColor}"
```

## Features

### Age Display Logic

**When Age Shows a Number:**
- Person is living with birth date ? shows current age
- Person is deceased with both birth and death dates ? shows age at death

**When Age Shows "-":**
- Person has no birth date ? age unknown
- Person is deceased but has no death date ? age at death unknown
  - Example: Birth year = 1950, Living = unchecked, Death date = empty ? Age = "-"

### Editable Columns

All columns are editable via TextBox controls:
1. **First Name** - Editable
2. **Last Name** - Editable
3. **Age** - Read-only (calculated field)
4. **Living** - Editable via checkbox
5. **Birth Date** - Editable with YearOrDateConverter
6. **Birth Place** - Editable
7. **Death Date** - Editable with YearOrDateConverter
8. **Death Place** - Editable

**Click any editable field** to modify the value directly in the list.

### Theme Integration

**Filter Area:**
- Uses `InputBackgroundBrush` (dark gradient in Black theme)
- Uses `BorderBrush` for consistent borders
- Uses `FontColor` for label text
- No more bright white background that clashed with dark theme

**Count Display:**
- Uses `AlternateFontColor` for subtle, theme-appropriate text
- Consistent with other secondary text in the application

## Technical Details

### Age Calculation Flow

1. **Check if birth date exists** ? if not, return null
2. **Check if deceased without death date** ? if so, return null
3. **Determine end date**:
   - Living or no death date ? use current date
   - Deceased with death date ? use death date
4. **Calculate years** between birth and end date
5. **Adjust for month/day** if birthday hasn't occurred yet
6. **Return age** (minimum 0)

### Null Handling

The `NullToDashConverter` ensures:
- Null ages display as "-" in the UI
- Empty string would look odd (blank cell)
- "-" clearly indicates "unknown" or "not applicable"

### Theme Brushes Used

From BlackResources.xaml:
- **InputBackgroundBrush**: Dark blue-black gradient for input fields
- **BorderBrush**: Gray (#FF747474) for borders
- **FontColor**: Light gray (#FFE6E6E6) for primary text
- **AlternateFontColor**: Medium gray (#FF888888) for secondary text

## User Experience

### Scenario: Deceased Person with Only Birth Year

**Setup:**
1. Person: John Smith
2. Birth Date: 1950 (stored as 1/1/1950)
3. Living: Unchecked
4. Death Date: Empty

**Result:**
- Age column displays: **-**
- Indicates: "We know he's deceased but don't know when, so age is unknown"

### Editing Data

**To edit any field:**
1. Click on the cell in the list
2. Type the new value
3. Press Enter or Tab to save
4. Press Escape to cancel

**Focused cell styling:**
- Background changes to semi-transparent white
- Border becomes visible
- Text becomes black for contrast

### Visual Consistency

The Family Data page now matches the rest of the application:
- Dark theme colors throughout
- No jarring white backgrounds
- Consistent font colors
- Proper use of theme brushes

## Testing Scenarios

**Test Age Display:**
1. Living person with birth date ? shows current age
2. Deceased with both dates ? shows age at death
3. Deceased with only birth year ? shows "-"
4. Person with no birth date ? shows "-"

**Test Editability:**
1. Click First Name ? can type
2. Click Last Name ? can type
3. Click Age ? cannot edit (read-only)
4. Click Living ? checkbox toggles
5. Click Birth Date ? can type year or full date
6. Click Birth Place ? can type
7. Click Death Date ? can type year or full date
8. Click Death Place ? can type

**Test Theme:**
1. Launch with Black theme ? filter area is dark
2. Switch to Silver theme (if available) ? colors adjust
3. All text is readable against backgrounds

## Build Status
? Build successful

## Date
January 2025
