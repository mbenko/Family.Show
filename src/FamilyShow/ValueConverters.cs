using System;
using System.Globalization;
using System.Windows.Data;

namespace FamilyShow
{
  /// <summary>
  /// This converter is used to show DateTime in short date format
  /// </summary>
  public class DateFormattingConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value != null)
      {
        return ((DateTime)value).ToShortDateString();
      }

      return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      // Ignore empty strings. this will cause the binding to bypass validation.
      if (string.IsNullOrEmpty((string)value))
      {
        return Binding.DoNothing;
      }

      string dateString = (string)value;

      // Append first month and day if just the year was entered
      if (dateString.Length == 4)
      {
        dateString = "1/1/" + dateString;
      }

      _ = DateTime.TryParse(dateString, out DateTime date);
      return date;
    }

    #endregion
  }

  /// <summary>
  /// This converter shows year only if date is Jan 1, otherwise shows full date.
  /// This allows compact display in the family list.
  /// </summary>
  public class YearOrDateConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
      {
        return string.Empty;
      }

      DateTime date = (DateTime)value;

      // If the date is January 1st, we assume only the year was entered
      // and display just the year
      if (date.Month == 1 && date.Day == 1)
      {
        return date.Year.ToString();
      }

      // Otherwise show the full date
      return date.ToShortDateString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      // Ignore empty strings
      if (string.IsNullOrEmpty((string)value))
      {
        return Binding.DoNothing;
      }

      string dateString = ((string)value).Trim();

      // If just a year (4 digits), use January 1st of that year
      if (dateString.Length == 4 && int.TryParse(dateString, out int year))
      {
        return new DateTime(year, 1, 1);
      }

      // Otherwise parse as a full date
      if (DateTime.TryParse(dateString, out DateTime date))
      {
        return date;
      }

      // If parsing fails, return the original value unchanged
      return Binding.DoNothing;
    }

        #endregion
      }

      /// <summary>
      /// This converter shows "-" for null values, used for Age display
      /// </summary>
      public class NullToDashConverter : IValueConverter
      {
        #region IValueConverter Members

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
                  // Age is read-only, no conversion back needed
                  return Binding.DoNothing;
                }

                #endregion
              }

              /// <summary>
              /// Converter to check if a person has photos attached
              /// </summary>
              public class HasPhotosConverter : IValueConverter
              {
                #region IValueConverter Members

                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                  if (value is FamilyShowLib.Person person)
                  {
                    bool hasPhotos = person.Photos != null && person.Photos.Count > 0;
                    return hasPhotos ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                  }
                  return System.Windows.Visibility.Collapsed;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                  return Binding.DoNothing;
                }

                #endregion
              }

              /// <summary>
              /// Converter to check if a person has a story attached
              /// </summary>
              public class HasStoryConverter : IValueConverter
              {
                #region IValueConverter Members

                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                  if (value is FamilyShowLib.Person person)
                  {
                    bool hasStory = person.Story != null;
                    return hasStory ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                  }
                  return System.Windows.Visibility.Collapsed;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                  return Binding.DoNothing;
                }

                #endregion
              }

              /// <summary>
              /// Converter for boolean to Visibility (true = Visible, false = Collapsed)
              /// </summary>
              public class BooleanToVisibilityConverter : IValueConverter
              {
                #region IValueConverter Members

                public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                {
                  if (value is bool boolValue)
                  {
                    return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                  }
                  return System.Windows.Visibility.Collapsed;
                }

                public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                {
                  return Binding.DoNothing;
                }

                #endregion
              }
            }