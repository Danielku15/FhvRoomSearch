using System;
using FhvRoomSearch.Model;

namespace FhvRoomSearch.Design
{
    class DesignDataService : IDataService
    {
        public string CalendarUrl
        {
            get;
            set;
        }

        public long CalendarFileSize
        {
            get;
            set;
        }

        public DateTime CalendarLastDownload
        {
            get;
            set;
        }
    }
}
