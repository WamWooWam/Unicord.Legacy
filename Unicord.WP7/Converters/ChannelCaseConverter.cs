using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Unicord.WP7.Converters
{
    public class ChannelCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return "";

            var str = (value as string ?? value.ToString()).ToLower();
            if (!str.Any(char.IsLetterOrDigit)) return str;

            return str.Replace('-', ' ').Replace('_', ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
