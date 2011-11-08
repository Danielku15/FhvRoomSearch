using System;

namespace FhvRoomSearch.Model
{
    public interface IDataService
    {
        string CalendarUrl
        {
            get; set;
        }
        long CalendarFileSize
        {
            get; set;
        }
        DateTime CalendarLastDownload
        {
            get; set;
        }
    }
}
