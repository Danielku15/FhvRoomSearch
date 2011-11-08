using System;
using System.Globalization;
using System.Windows.Data;

namespace FhvRoomSearch.Converter
{
    class DoubleToIntPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = (double) value;
            return Math.Min(100, d*100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int d = (int)value;
            return d/100.0;
        }
    }
}
