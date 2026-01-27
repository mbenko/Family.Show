# Submenu Item Readability Fix

## Issue
Secondary navigation menus (recent files list under File?Open, skin picker under View?Skins) displayed white text on a silver/light background, making them difficult to read in the Black skin theme.

## Root Cause
The MenuItem template in `BlackResources.xaml` had two problems:

1. **Submenu Role Triggers**: The `SubmenuHeader` and `SubmenuItem` role triggers did not explicitly set foreground colors, so submenu items inherited colors that didn't work well with the dark popup background.

2. **IsHighlighted Trigger**: The highlighted state used `SystemColors.HighlightTextBrushKey` which returns system-dependent colors (often light colors on Windows). When combined with the `MenuIsHighlightedBrush` (dark gray gradient), this could create poor contrast.

## Solution
Modified `FamilyShow/Skins/Black/BlackResources.xaml`:

1. **Added explicit white foreground to submenu items** (lines 926-938):
   - `SubmenuHeader` role trigger now sets `Foreground="#FFFFFFFF"` 
   - `SubmenuItem` role trigger now sets `Foreground="#FFFFFFFF"`
   - This ensures submenu text is white on the dark gray `PopupMenuBackgroundBrush` (#FF2B2B2B)

2. **Fixed IsHighlighted trigger** (lines 949-954):
   - Replaced `SystemColors.HighlightTextBrushKey` with explicit `#FFFFFFFF` (white)
   - Ensures consistent white text on the dark `MenuIsHighlightedBrush` gradient
   - Avoids system color inconsistencies across Windows versions

## Technical Details

### Color Palette
- **PopupMenuBackgroundBrush**: `#FF2B2B2B` (dark gray, 43/255 RGB)
- **Submenu Text**: `#FFFFFFFF` (white)
- **MenuIsHighlightedBrush**: Gradient from `#FF4C4D4F` to `#FF221E1F` (dark grays)
- **Contrast Ratio**: ~12:1 (WCAG AAA compliant)

### Affected Menu Items
- File ? Open ? Recent files list
- View ? Skins ? Skin options (Silver, Blue, etc.)
- Any future submenus added to the application

## Testing
Build successful. To verify:
1. Launch FamilyShow application
2. Ensure Black skin is active
3. Click File ? Open to view recent files submenu
4. Click View ? Skins to view skin options submenu
5. Verify all text is readable in both normal and highlighted states

## Related Changes
This is the second menu readability fix:
- **First fix** (MENU_POPUP_FIX.md): Changed popup background from pure black (#FF000000) to dark gray (#FF2B2B2B)
- **This fix**: Set explicit white foreground for submenu items and highlighted state

## Date
January 2025
