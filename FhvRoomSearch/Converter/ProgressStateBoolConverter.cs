using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;

namespace FhvRoomSearch.Converter
{
    class ProgressStateBoolConverter : IValueConverter
    {
        public TaskbarItemProgressState TrueValue
        {
            get;
            set;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskbarItemProgressState progressState = (TaskbarItemProgressState) value;
            return progressState == TrueValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
