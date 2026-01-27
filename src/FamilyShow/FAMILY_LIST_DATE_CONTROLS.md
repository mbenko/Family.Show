# Family List Living/Deceased Checkbox and Smart Date Display

## Overview
Enhanced the Family Data list view on the right side of the application to include:
1. **Living checkbox column** - Indicates if a person is living or deceased
2. **Smart date display and editing** - Shows year-only (e.g., "1950") when only year is known, or full date (e.g., "3/15/1950") when complete date is available

## Changes Made

### 1. FamilyShow/ValueConverters.cs

#### New YearOrDateConverter Class
Added intelligent date converter that:
- **Displays year only** when date is January 1st (indicating only year was entered)
- **Displays full date** when month/day information is available
- **Editing support**:
  - Type just "1950" ? stored as Jan 1, 1950, displays as "1950"
  - Type "3/15/1950" ? stored with full date, displays as "3/15/1950"

```csharp
public class YearOrDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;

        DateTime date = (DateTime)value;

        // If date is Jan 1, show just the year
        if (date.Month == 1 && date.Day == 1)
            return date.Year.ToString();

        // Otherwise show full date
        return date.ToShortDateString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string dateString = ((string)value).Trim();

        // 4-digit year ? Jan 1 of that year
        if (dateString.Length == 4 && int.TryParse(dateString, out int year))
            return new DateTime(year, 1, 1);

        // Otherwise parse as full date
        if (DateTime.TryParse(dateString, out DateTime date))
            return date;

        return Binding.DoNothing;
    }
}
```

### 2. FamilyShow/Controls/FamilyData/FamilyData.xaml

#### Resources Section (lines 9-13)
```xml
<local:DateFormattingConverter x:Key="DateFormattingConverter" />
<local:YearOrDateConverter x:Key="YearOrDateConverter" />
```

#### IsLiving Checkbox Template (lines 67-73)
```xml
<DataTemplate x:Key="IsLivingColumnTemplate">
  <CheckBox
    HorizontalAlignment="Center"
    VerticalAlignment="Center"
    IsChecked="{Binding Path=IsLiving, Mode=TwoWay}"
    Foreground="{DynamicResource FamilyDataFontColor}" />
</DataTemplate>
```

#### BirthDate TextBox Template (lines 75-78)
**REPLACED DatePicker with styled TextBox**
```xml
<DataTemplate x:Key="BirthDateColumnTemplate">
  <TextBox Style="{StaticResource TextBoxStyle}" 
           Text="{Binding Path=BirthDate, Converter={StaticResource YearOrDateConverter}}" />
</DataTemplate>
```

#### DeathDate TextBox Template (lines 91-94)
**REPLACED DatePicker with styled TextBox**
```xml
<DataTemplate x:Key="DeathDateColumnTemplate">
  <TextBox Style="{StaticResource TextBoxStyle}" 
           Text="{Binding Path=DeathDate, Converter={StaticResource YearOrDateConverter}}" />
</DataTemplate>
```

#### GridView Column (lines 423-429)
Added **Living** column between Age and Birth Date

## Features

### Living/Deceased Checkbox
- **Location**: Column between "Age" and "Birth Date"
- **Behavior**: 
  - Checked = Person is living
  - Unchecked = Person is deceased
- **Data Binding**: Two-way binding to `Person.IsLiving` property
- **Sorting**: Column is sortable by clicking header

### Smart Date Display and Editing

#### Display Behavior
- **Year only** (e.g., "1950"): Shown when only year is known
  - Internally stored as January 1, 1950
  - Display shows just "1950" for clean appearance

- **Full date** (e.g., "3/15/1950"): Shown when complete date is available
  - Display shows in short date format (MM/DD/YYYY)

#### Editing Behavior
Click on any date field to edit:

1. **Enter year only**: Type "1950" and press Enter
   - Stored as 1/1/1950
   - Displays as "1950"

2. **Enter full date**: Type "3/15/1950" and press Enter
   - Stored as 3/15/1950
   - Displays as "3/15/1950"

3. **Multiple date formats supported**:
   - "1950" ? Jan 1, 1950
   - "3/15/1950" ? March 15, 1950
   - "March 15, 1950" ? March 15, 1950
   - "3-15-1950" ? March 15, 1950

#### Visual Style
- **Normal state**: Transparent background, matches theme
- **Focused state**: White background with black border
- **No white boxes**: Clean integration with application theme
- **No calendar icons**: Text-only display for cleaner appearance

## Technical Details

### Date Storage Convention
- **Year-only dates** stored as January 1 of that year
- This allows the converter to distinguish between:
  - Dates where only year is known (show year only)
  - Dates with full information (show complete date)

### Data Binding
All controls use **Mode=TwoWay** binding:
- UI changes immediately update Person object
- Person object changes update UI
- `INotifyPropertyChanged` ensures synchronization

### Styling
Uses existing `TextBoxStyle`:
- Transparent background in normal state
- White background when focused
- Theme-aware foreground color
- No visible borders unless focused

## Column Order
The updated family list columns are:
1. Empty (spacing)
2. **First Name**
3. **Last Name**
4. **Age** (read-only, calculated)
5. **Living** ? NEW - Checkbox
6. **Birth Date** ? ENHANCED - Smart year/date display
7. **Birth Place**
8. **Death Date** ? ENHANCED - Smart year/date display
9. **Death Place**
10. Empty (spacing)

## User Experience

### To Mark Someone as Deceased:
1. Uncheck the "Living" checkbox in that person's row
2. Click the death date field
3. Type the year (e.g., "2010") or full date (e.g., "12/25/2010")
4. Press Enter or Tab to save

### To Enter Birth/Death Dates:

**If you only know the year:**
1. Click the date field
2. Type just the year (e.g., "1950")
3. Press Enter ? displays as "1950"

**If you know the full date:**
1. Click the date field
2. Type the date (e.g., "3/15/1950")
3. Press Enter ? displays as "3/15/1950"

### Visual Appearance
- **No white boxes** - dates blend seamlessly with the theme
- **No calendar icons** - clean text-only appearance
- **Compact display** - years take less space than full dates
- **Focus highlighting** - white background when editing

## Advantages Over DatePicker

1. **Theme Integration**: No white boxes, matches application colors
2. **Compact Display**: Year-only dates take minimal space
3. **Flexible Input**: Type dates in various formats
4. **Cleaner Look**: No calendar icon clutter
5. **Fast Entry**: Quick keyboard-only editing

## Testing
Build successful. To verify:
1. Launch FamilyShow application
2. Open Family Data view (right panel)
3. Verify new "Living" checkbox column appears
4. Look at birth/death dates - should show years or full dates
5. Click a date field - no white box, just text becomes editable
6. Test entering:
   - Just a year: "1950"
   - Full date: "3/15/1950"
7. Verify year-only entries display as just "1950"
8. Verify full dates display as "3/15/1950"
9. Test checking/unchecking Living checkbox
10. Verify changes persist when saving family file

## Date
January 2025
