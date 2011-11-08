using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;

namespace FhvRoomSearch.Converter
{
    class ProgressStateIndeterminateBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskbarItemProgressState progressState = (TaskbarItemProgressState) value;
            return progressState == TaskbarItemProgressState.Indeterminate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
