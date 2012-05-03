using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;
using FhvRoomSearch.Model;
using FhvRoomSearch.Properties;

namespace FhvRoomSearch.Import
{
    /// <summary>
    /// This class is used to parse an iCal file into the internal room searcher model. 
    /// </summary>
    class FhvICalParser
    {
        private readonly IDataService _dataService;
        private Dictionary<string, Room> _rooms;

        private State _state;
        private string _dtStart;
        private string _dtEnd;
        private string _summary;
        private string _location;

        public FhvICalParser(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IList<Wing> ParsedData
        {
            get;
            private set;
        }

        public void Prepare()
        {
            ParsedData = new List<Wing>();
            SetupDefaultStructure();
            _state = State.OutsideEvent;
        }

        private void SetupDefaultStructure()
        {
            try
            {
                _rooms = new Dictionary<string, Room>();

                XmlDocument document = new XmlDocument();
                document.LoadXml(Resources.DefaultWingData);

                WingSerializer serializer = new WingSerializer(document, _dataService);
                serializer.Deserialize();
                ParsedData = serializer.Wings;

                foreach (Wing wing in ParsedData)
                {
                    foreach (Level level in wing.Level)
                    {
                        foreach (Room room in level.Room)
                        {
                            _rooms.Add(room.RoomId, room);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw new ImportException("Could not load default data", e);
            }
        }

        public void ProcessLine(string line)
        {
            // transitions
            if (line == "BEGIN:VEVENT")
            {
                _state = State.InsideEvent;
            }
            else if (line == "END:VEVENT")
            {
                _state = State.OutsideEvent;
                FinishEvent();
            }
            else
            {
                // computations
                switch (_state)
                {
                    case State.InsideEvent:
                        ProcessEventLine(line);
                        break;
                }
            }
        }


        private void ProcessEventLine(string line)
        {
            if (line.StartsWith("DTSTART;TZID=Europe/Vienna"))
            {
                _dtStart = line.Substring(27);
            }
            else if (line.StartsWith("DTEND;TZID=Europe/Vienna:"))
            {
                _dtEnd = line.Substring(25);
            }
            else if (line.StartsWith("SUMMARY:"))
            {
                _summary = line.Substring(8);
            }
            else if (line.StartsWith("LOCATION:"))
            {
                _location = line.Substring(9);
            }
        }

        private void FinishEvent()
        {
            if (_dtStart == null || _dtEnd == null || _summary == null || _location == null)
            {
                // TODO: Do some error reporting here
                return;
            }

            Course course = new Course();

            course.StartTime = DecodeDateTime(_dtStart);
            course.EndTime = DecodeDateTime(_dtEnd);
            ParseSummary(course, _summary);

            AddToRooms(course, _location);
        }

        private void AddToRooms(Course course, string location)
        {
            // TODO: Better checking
            foreach (KeyValuePair<string, Room> c in _rooms)
            {
                if (location.Contains(c.Key))
                {
                    c.Value.Course.Add(course);
                }
            }
        }

        private DateTime DecodeDateTime(string dtStart)
        {
            Match match = Regex.Match(dtStart, @"^((\d{4})(\d{2})(\d{2}))T((\d{2})(\d{2})(\d{2})(Z)?)$", RegexOptions.IgnoreCase);
            if (!match.Success)
                return DateTime.Now; // TODO: Exception? Fallback?

            DateTime now = DateTime.Now;

            int year = now.Year;
            int month = now.Month;
            int date = now.Day;
            int hour = 0;
            int minute = 0;
            int second = 0;

            if (match.Groups[1].Success)
            {
                year = int.Parse(match.Groups[2].Value);
                month = int.Parse(match.Groups[3].Value);
                date = int.Parse(match.Groups[4].Value);
            }
            if (match.Groups[5].Success)
            {
                hour = int.Parse(match.Groups[6].Value);
                minute = int.Parse(match.Groups[7].Value);
                second = int.Parse(match.Groups[8].Value);
            }

            bool isUniversalTime = match.Groups[9].Success;

            DateTime dt = new DateTime(year, month, date, hour, minute, second, DateTimeKind.Utc);
            return isUniversalTime ? dt.ToLocalTime() : dt;
        }

        private void ParseSummary(Course course, string summary)
        {
            string[] lines = summary.Replace("\\,", ",").Split(new[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (line.StartsWith("Kategorie: "))
                {
                    course.Category = line.Substring(11);
                }
                else if (line.StartsWith("Modul: "))
                {
                    course.Module = line.Substring(7);
                }
                else if (line.StartsWith("DozentIn: "))
                {
                    course.Lecturer = line.Substring(10);
                }
                else if (line.StartsWith("Gruppe: "))
                {
                    course.Group = line.Substring(8);
                }
                else if (line.StartsWith("Hinweis: "))
                {
                    course.Notes = line.Substring(9);
                }
            }
        }

        public event ProgressChangedEventHandler ProgressChanged;
        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = ProgressChanged;
            if (handler != null)
                handler(this, e);
        }

        private enum State
        {
            OutsideEvent,
            InsideEvent
        }
    }
}
