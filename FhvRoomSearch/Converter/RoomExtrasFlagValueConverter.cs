using System;
using System.Globalization;
using System.Windows.Data;
using FhvRoomSearch.Model;

namespace FhvRoomSearch.Converter
{
    class RoomExtrasFlagValueConverter : IValueConverter
    {
        private RoomExtras _target;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RoomExtras mask = (RoomExtras)parameter;
            _target = (RoomExtras)value;
            return ((mask & _target) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _target ^= (RoomExtras)parameter;
            return _target;
        }
    }
}
