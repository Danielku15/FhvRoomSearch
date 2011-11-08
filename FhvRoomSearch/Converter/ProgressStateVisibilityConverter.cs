using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shell;

namespace FhvRoomSearch.Converter
{
    class ProgressStateVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskbarItemProgressState progressState = (TaskbarItemProgressState) value;
            switch (progressState)
            {
                case TaskbarItemProgressState.None:
                    return Visibility.Hidden;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
