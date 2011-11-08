using System;
using FhvRoomSearch.Properties;

namespace FhvRoomSearch.Model
{
    class DataService : IDataService
    {
        public string CalendarUrl
        {
            get
            {
                return Settings.Default.CalendarUrl;
            }
            set
            {
                Settings.Default.CalendarUrl = value;
                Settings.Default.Save();
            }
        }

        public long CalendarFileSize
        {
            get
            {
                return Settings.Default.CalendarFileSize;
            }
            set
            {
                Settings.Default.CalendarFileSize = value;
                Settings.Default.Save();
            }
        }

        public DateTime CalendarLastDownload
        {
            get
            {
                return Settings.Default.CalendarLastDownload;
            }
            set
            {
                Settings.Default.CalendarLastDownload = value;
                Settings.Default.Save();
            }
        }
    }
}
