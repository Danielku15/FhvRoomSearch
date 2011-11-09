using System;
using System.Collections.Generic;

namespace FhvRoomSearch.Model
{
    public interface IDataService
    {
        string CalendarUrl
        {
            get;
            set;
        }
        long CalendarFileSize
        {
            get;
            set;
        }
        DateTime CalendarLastDownload
        {
            get;
            set;
        }

        bool ResetDatabase(IList<Wing> parsedData);

        void Cleanup();
    }
}
