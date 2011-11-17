using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq;

namespace FhvRoomSearch.Model
{
    class DataService : IDataService
    {
        private const string CalendarUrlKey = "CalendarUrl";
        private const string CalendarFileSizeKey = "CalendarFileSize";
        private const string CalendarLastDownloadKey = "CalendarLastDownload";
        private readonly RoomCourseModelContainer _database;

        public DataService()
        {
            _database = new RoomCourseModelContainer();
            _database.WingSet.MergeOption = MergeOption.NoTracking;
            _database.LevelSet.MergeOption = MergeOption.NoTracking;
            _database.RoomSet.MergeOption = MergeOption.NoTracking;
            _database.CourseSet.MergeOption = MergeOption.NoTracking;
        }

        public string CalendarUrl
        {
            get
            {
                Config c = GetConfig(CalendarUrlKey);
                return c == null ? string.Empty : c.Value;
            }
            set
            {
                Config c = GetConfigOrCreate(CalendarUrlKey);
                c.Value = value;
                _database.SaveChanges();
            }
        }

        private Config GetConfigOrCreate(string key)
        {
            Config entry = (from c in _database.ConfigSet
                        where c.Key == key
                        select c).FirstOrDefault();
            if(entry == null)
            {
                entry = new Config {Key = key, Value = ""};
                _database.ConfigSet.AddObject(entry);
            }
            return entry;
        }

        private Config GetConfig(string key)
        {
            return (from c in _database.ConfigSet
                    where c.Key == key
                    select c).FirstOrDefault();
        }

        public long CalendarFileSize
        {
            get
            {
                Config c = GetConfig(CalendarFileSizeKey);
                return c == null ? 0 : long.Parse(c.Value);
            }
            set
            {
                Config c = GetConfigOrCreate(CalendarFileSizeKey);
                c.Value = value.ToString();
                _database.SaveChanges();
            }
        }

        public DateTime CalendarLastDownload
        {
            get
            {
                Config c = GetConfig(CalendarLastDownloadKey);
                return c == null ? DateTime.Now.AddDays(-1) : DateTime.Parse(c.Value);
            }
            set
            {
                Config c = GetConfigOrCreate(CalendarLastDownloadKey);
                c.Value = value.ToString();
                _database.SaveChanges();
            }

        }

        public IEnumerable<Wing> Wings
        {
            get
            {
                return _database.WingSet;
            }
        }

        public IEnumerable<Level> Levels
        {
            get
            {
                return _database.LevelSet;
            }
        }

        public IEnumerable<Room> Rooms
        {
            get
            {
                return _database.RoomSet;
            }
        }

        public bool ResetDatabase(IList<Wing> newData)
        {
            bool success = DeleteOldData();
            if (!success)
                return false;
            return InsertNewData(newData);
        }

        private bool InsertNewData(IEnumerable<Wing> newData)
        {
            try
            {
                foreach (Wing wing in newData)
                {
                    _database.WingSet.AddObject(wing);
                }
                _database.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message, e.ToString());
                return false;
            }
        }

        private bool DeleteOldData()
        {
            bool success;
            using (var transaction = BeginTransaction(_database))
            {
                try
                {
                    string[] tables = { "RoomCourse", "WingSet", "LevelSet", "RoomSet", "CourseSet" };
                    bool[] alter = { false, true, true, true, true };
                    for (int i = 0; i < tables.Length; i++)
                    {
                        string table = tables[i];
                        _database.ExecuteStoreCommand(string.Format("DELETE FROM [{0}]", table));
                        if (alter[i])
                            _database.ExecuteStoreCommand(
                                string.Format("ALTER TABLE [{0}] ALTER COLUMN Id IDENTITY (1,1)", table));
                    }
                    // Delete old data
                    transaction.Commit();
                    _database.AcceptAllChanges();
                    success = true;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Debug.Fail(e.Message, e.ToString());
                    success = false;
                }
            }

            return success;
        }

        public static DbTransaction BeginTransaction(ObjectContext context)
        {
            if (context.Connection.State != ConnectionState.Open)
            {
                context.Connection.Open();
            }
            return context.Connection.BeginTransaction();
        }

        public void Cleanup()
        {
            _database.Dispose();
        }

        // {0} StartTime
        // {1} EndTime
        // {2} Now
        // {3} Rooms
        private const string UnoccupiedRoomsSql =
            "SELECT a.Id, COUNT(c.Id) AS ConflictingCourses FROM RoomSet AS a  " +
            "LEFT OUTER JOIN RoomCourse AS b ON b.Rooms_Id = a.Id " +
            //"-- Conflicting Courses " +
            "LEFT OUTER JOIN CourseSet AS c ON c.Id = b.Course_Id AND {0} <= c.EndTime AND {1} >= c.StartTime " +
            //"-- Current Course  " +
            "LEFT OUTER JOIN CourseSet AS d ON d.Id = b.Course_Id AND {2} <= d.EndTime AND {2} >= d.StartTime  " +
            //"-- All Upcoming Courses " +
            "LEFT OUTER JOIN CourseSet AS e ON e.Id = b.Course_Id AND e.StartTime >= {2} " +
            "WHERE (a.Id IN ({3})) GROUP BY a.Id";

        public IEnumerable<SearchResult> PerformSearch(DateTime start, DateTime end, IList<Room> rooms)
        {
            // 
            // Load Query Results
            // 
            int[] roomIds = (from r in rooms
                             select r.Id).ToArray();
            string inClause = string.Join(", ", roomIds);

            string startStr = "'" + start.ToString("yyyy-MM-dd HH:mm:00") + "'";
            string endStr = "'" + end.ToString("yyyy-MM-dd HH:mm:00") + "'";
            string nowStr = "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:00") + "'";

            string query = string.Format(UnoccupiedRoomsSql, startStr, endStr, nowStr, inClause);
            Debug.WriteLine(query);
            var queryResults = _database.ExecuteStoreQuery<QueryResult>(query);



            // 
            // Load Courses
            //
            List<SearchResult> searchResults = new List<SearchResult>();
            foreach (QueryResult queryResult in queryResults)
            {
                Room room = (from r in rooms
                             where queryResult.Id == r.Id
                             select r).First();
                Course currentCourse = null;

                if (queryResult.CurrentCourseId.HasValue)
                {
                    int currentCourseId = queryResult.CurrentCourseId.GetValueOrDefault();
                    currentCourse = (from c in _database.CourseSet
                                     where currentCourseId == c.Id
                                     select c).FirstOrDefault();
                }

                RoomState state = queryResult.ConflictingCourses == 0 ? RoomState.Unoccupied : RoomState.Occupied;

                SearchResult searchResult = new SearchResult(room, state, currentCourse, queryResult.NextCourseStart);
                searchResults.Add(searchResult);
            }

            return searchResults;
        }

        internal class QueryResult
        {
            public int Id
            {
                get;
                set;
            }
            public int? CurrentCourseId
            {
                get;
                set;
            }
            public int ConflictingCourses
            {
                get;
                set;
            }
            public DateTime? NextCourseStart
            {
                get;
                set;
            }
        }
    }


}
