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
    }
}
