using System;
using System.Collections.Generic;
using System.Text;
using DDay.iCal;
using FhvRoomSearch.Model;

namespace FhvRoomSearch.Import
{
    /// <summary>
    /// This class is used to parse an iCal file into the internal room searcher model. 
    /// </summary>
    class FhvICalParser
    {
        private IICalendar _calendar;

        public IList<Wing> ParsedData { get; private set; }

        public FhvICalParser(IICalendar calendar)
        {
            if(calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            _calendar = calendar;
        }

        public void Parse()
        {
            ParsedData = new List<Wing>();
        }


        public static FhvICalParser FromFile(string file, Encoding encoding)
        {
            try
            {
                IICalendarCollection collection = iCalendar.LoadFromFile(file, encoding);
                if (collection.Count == 0)
                {
                    throw new ImportException("Could not find calendar data in the specified file.");
                }

                return new FhvICalParser(collection[0]);
            }
            catch (ImportException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ImportException(string.Format("Error loading the calendar file: {0}", e.Message), e);
            }
        }
    }
}
