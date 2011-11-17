using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FhvRoomSearch.Model;

namespace FhvRoomSearch.Converter
{
    class RoomStateColorConverter : IValueConverter
    {
        public Brush OccupiedBrush
        {
            get;
            set;
        }
        public Brush UnoccupiedBrush
        {
            get;
            set;
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RoomState state = (RoomState)value;
            switch (state)
            {
                case RoomState.Occupied:
                    return OccupiedBrush;
                case RoomState.Unoccupied:
                    return UnoccupiedBrush;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
