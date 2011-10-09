using System;
using System.Globalization;
using System.Windows.Data;

namespace MantisSubmit2.Converters
{
    public class BooleanComplementConverter : IValueConverter
    {
        #region Public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean)
                return !((bool)value);
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean)
                return !((bool)value);
            else
                return false;
        }

        #endregion
    }
}
