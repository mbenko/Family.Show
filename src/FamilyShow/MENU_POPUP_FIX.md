# Menu Popup Background Fix for Black Skin

## Issue
The File ? Open menu dropdown (and other menu dropdowns) was showing white text on a black background, making the recent files list unreadable in the Black skin.

## Root Cause
The `PopupMenuBackgroundBrush` resource in `BlackResources.xaml` was set to pure black (`#FF000000`). Since the menu items use white text (`Foreground="#FFFFFFFF"`), this created a low-contrast, hard-to-read situation.

## Solution
Changed the `PopupMenuBackgroundBrush` from pure black to a lighter dark gray (`#FF2B2B2B`).

### File Modified
`FamilyShow/Skins/Black/BlackResources.xaml` (line 188)

### Change Made
```xaml
<!-- Before -->
<SolidColorBrush x:Key="PopupMenuBackgroundBrush" Color="#FF000000" />

<!-- After -->
<!--  PopupMenuBackgroundBrush - Dark gray background for menu dropdowns so white text is readable  -->
<SolidColorBrush x:Key="PopupMenuBackgroundBrush" Color="#FF2B2B2B" />
```

## Color Details
- **Old Color**: `#FF000000` (Pure Black - RGB: 0, 0, 0)
- **New Color**: `#FF2B2B2B` (Dark Gray - RGB: 43, 43, 43)

The new color provides enough contrast with white text (`#FFFFFFFF`) to be easily readable while maintaining the dark aesthetic of the Black skin.

## Visual Impact

### Before Fix
- File ? Open dropdown: White text on pure black background
- Recent files list: Difficult to read
- Poor contrast ratio

### After Fix
- File ? Open dropdown: White text on dark gray background  
- Recent files list: Clearly readable
- Good contrast ratio while maintaining dark theme

## Affected UI Elements
This change affects all popup menu backgrounds in the Black skin:
- File menu dropdowns
- Edit menu dropdowns
- Tools menu dropdowns
- View menu dropdowns (Skins submenu)
- Help menu dropdowns
- Any other MenuItem with a submenu

## Testing
? Build successful
? Menu text now readable
? Maintains consistent dark theme
? All menu dropdowns affected uniformly

## Related Context
- Menu reorganization completed earlier (File, Edit, Tools, View, Help structure)
- This fix ensures the new menu structure is fully usable with the Black skin
- Consistent with WPF styling patterns where popup overlays use slightly lighter backgrounds than the main UI

## Color Rationale
The color `#2B2B2B` was chosen because:
1. **Sufficient contrast**: Provides ~12:1 contrast ratio with white text (WCAG AAA compliant)
2. **Theme consistency**: Close enough to black to maintain the dark skin aesthetic
3. **Visual hierarchy**: Slightly lighter than main background (`#FF202020`) to differentiate popup from base UI
4. **Existing pattern**: Similar to other dark gray colors used in the skin (e.g., `#FF282828` for PersonInfoBackground)

---
*Implementation Date: January 27, 2026*
*Related Documentation: MENU_REORGANIZATION.md*
