using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FhvRoomSearch.Model;

namespace FhvRoomSearch.Converter
{
    public class RoomStateStyleConverter : IValueConverter
    {
        public Style OccupiedStyle { get; set; }
        public Style UnoccupiedStyle { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RoomState state = (RoomState) value;
            switch (state)
            {
                case RoomState.Occupied:
                    return OccupiedStyle;
                case RoomState.Unoccupied:
                    return UnoccupiedStyle;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
