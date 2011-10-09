using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using MantisSubmit2.ShellIcons;

namespace MantisSubmit2.Converters
{
    public class FileNameToIconConverter : IValueConverter
    {
        #region IValueConverter members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fileName = (string)value;
            IconContainer iconContainer = ShellIcon.GetIconForFile(fileName, true, true);
            return iconContainer.Icon.ToImageSource();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
