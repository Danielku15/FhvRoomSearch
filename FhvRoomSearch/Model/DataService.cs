using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Objects;
using System.Diagnostics;
using FhvRoomSearch.Properties;

namespace FhvRoomSearch.Model
{
    class DataService : IDataService
    {
        private RoomCourseModelContainer _database;

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

        private bool InsertNewData(IList<Wing> newData)
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

        private void Reconnect()
        {
            _database.Dispose();
            _database = new RoomCourseModelContainer();
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

    }


}
