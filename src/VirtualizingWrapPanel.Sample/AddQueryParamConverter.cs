using System;
using System.Globalization;
using System.Windows.Data;

namespace VirtualizingWrapPanel.Sample
{
    [ValueConversion(typeof(string), typeof(string))]
    public class AddQueryParamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string url && !string.IsNullOrWhiteSpace(url) && parameter is string para)
                return value + "?" + para;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s) && s.IndexOf("?", StringComparison.Ordinal) > 0)
            {
                return s.Substring(0, s.IndexOf("?", StringComparison.Ordinal));
            }

            return value;
        }
    }
}
