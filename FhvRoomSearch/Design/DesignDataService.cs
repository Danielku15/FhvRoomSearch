using System;
using System.Collections.Generic;
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

        public DesignDataService()
        {
            Wings = new Wing[0];
            Levels = new Level[0];
            Rooms = new Room[0];
        }
        
        public IEnumerable<Wing> Wings { get; private set; }
        public IEnumerable<Level> Levels { get; private set; }
        public IEnumerable<Room> Rooms { get; private set; }


        public bool ResetDatabase(IList<Wing> parsedData)
        {
            return true;
        }

        public void Cleanup()
        {
            
        }

        public IEnumerable<SearchResult> PerformSearch(DateTime start, DateTime end, IList<Room> rooms)
        {
            return new SearchResult[0];
        }
    }
}
