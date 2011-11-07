using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using FhvRoomSearch.Model;
using FhvRoomSearch.Properties;

namespace FhvRoomSearch.Import
{
    /// <summary>
    /// This class is used to parse an iCal file into the internal room searcher model. 
    /// </summary>
    class FhvICalParser
    {
        private readonly StreamReader _calendar;
        private Dictionary<string, Room> _rooms;

        public IList<Wing> ParsedData
        {
            get;
            private set;
        }

        public FhvICalParser(StreamReader calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            _calendar = calendar;
        }

        public void Parse()
        {
            ParsedData = new List<Wing>();
            SetupDefaultStructure();
            ReadCourses();
        }

        private void SetupDefaultStructure()
        {
            try
            {
                _rooms = new Dictionary<string, Room>();

                // TODO: Create own serializer, this one does not work
                XmlSerializer serializer = new XmlSerializer(typeof(List<Wing>));
                StringReader reader = new StringReader(Resources.DefaultWingData);
                ParsedData = (IList<Wing>)serializer.Deserialize(reader);

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

        private void ReadCourses()
        {
            string currentLine;

            while ((currentLine = _calendar.ReadLine()) != null)
            {
                if (currentLine.Trim() == "BEGIN:VEVENT")
                {
                    ReadCourse();
                }
            }
        }

        private void ReadCourse()
        {
            string dtStart = null;
            string dtEnd = null;
            string summary = null;
            string location = null;

            string currentLine;
            while ((currentLine = _calendar.ReadLine()) != null)
            {
                if(currentLine == "END:VEVENT")
                {
                    break;
                }

                if(currentLine.StartsWith("DTSTART;TZID=Europe/Vienna"))
                {
                    dtStart = currentLine.Substring(28); 
                }
                else if(currentLine.StartsWith("DTEND;TZID=Europe/Vienna:"))
                {
                    dtEnd = currentLine.Substring(26);
                }
                else if(currentLine.StartsWith("SUMMARY:"))
                {
                    summary = currentLine.Substring(8);
                }
                else if(currentLine.StartsWith("LOCATION:"))
                {
                    location = currentLine.Substring(9);
                }
            }

            if(dtStart == null || dtEnd == null || summary == null || location == null)
            {
                // TODO: Do some error reporting here
                return;
            }

            Course course = new Course {StartTime = DecodeDateTime(dtStart), EndTime = DecodeDateTime(dtEnd)};
            ParseSummary(course, summary);

            AddToRooms(course, location);
        }

        private void AddToRooms(Course course, string location)
        {
            // TODO: Better checking
            foreach (KeyValuePair<string, Room> c in _rooms)
            {
                if(location.Contains(c.Key))
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
            string[] lines = summary.Replace("\\,", ",").Split(new[] {"\\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if(line.StartsWith("Kategorie: "))
                {
                    course.Category = line.Substring(11);
                }
                else if(line.StartsWith("Modul: "))
                {
                    course.Module = line.Substring(7);
                }
                else if (line.StartsWith("DozentIn: "))
                {
                    course.Lecturer = line.Substring(10);
                }               
                else if (line.StartsWith("Gruppe: "))
                {
                    course.Lecturer = line.Substring(8);
                }
                else if (line.StartsWith("Hinweis: "))
                {
                    course.Notes = line.Substring(9);
                }
            }
        }
    }
}
