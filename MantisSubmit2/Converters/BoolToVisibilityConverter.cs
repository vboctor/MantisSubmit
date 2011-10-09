using System;
using System.Windows;
using System.Windows.Data;

namespace MantisSubmit2.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region Properties

        public Visibility TrueVisibility { get; set; }
        public Visibility FalseVisibility { get; set; }

        #endregion

        #region Constructors

        public BoolToVisibilityConverter()
        {
            TrueVisibility = Visibility.Visible;
            FalseVisibility = Visibility.Collapsed;
        }

        #endregion

        #region Public methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            if (visibility == TrueVisibility)
                return true;
            else
                return false;
        }

        #endregion
    }
}
