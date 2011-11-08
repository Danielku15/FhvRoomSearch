using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Input;
using FhvRoomSearch.Behavior;
using FhvRoomSearch.Import;
using FhvRoomSearch.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FhvRoomSearch.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        private string _lastError;
        public string LastError
        {
            get
            {
                return _lastError;
            }
            set
            {
                if (_lastError == value)
                    return;
                _lastError = value;
                RaisePropertyChanged("LastError");
            }
        }



        public ICommand ReloadCoursesCommand
        {
            get;
            private set;
        }

        #region Debug Output

        private string _debugData;
        public string DebugData
        {
            get
            {
                return _debugData;
            }
            set
            {
                if (_debugData == value)
                    return;
                _debugData = value;
                RaisePropertyChanged("DebugData");
            }
        }

        #endregion

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            ReloadCoursesCommand = new RelayCommand(ReloadCourses);
        }

        public void ReloadCourses()
        {
            ReloadCourses(_dataService.CalendarUrl);
        }

        public void ReloadCourses(string url)
        {
            try
            {
                WebClient client = new WebClient();
                Stream calendarStream = client.OpenRead(url);

                if (calendarStream == null)
                {
                    throw new IOException("Could not open connection to server");
                }
                FhvICalParser parser;
                using (StreamReader reader = new StreamReader(calendarStream, Encoding.UTF8))
                {
                    parser = new FhvICalParser(reader);
                    parser.Parse();
                }

                StringBuilder builder = new StringBuilder();

                foreach (var wing in parser.ParsedData)
                {
                    builder.AppendLine(wing.Name);
                    foreach (var level in wing.Level)
                    {
                        builder.AppendLine("    " + level.Name);

                        foreach (var room in level.Room)
                        {
                            builder.AppendLine("        " + room.RoomId);

                            foreach (var course in room.Course)
                            {
                                builder.AppendLine(string.Format("            {0}({1} - {2})", course.Module, course.StartTime, course.EndTime));
                            }
                        }
                    }
                }


                DebugData = builder.ToString();

            }
            catch (Exception e)
            {
                Messenger.Default.Send(new DialogMessage(
                                           "Could not load calender from current URL: '" + e.Message + "'" +
                                           Environment.NewLine +
                                           "Do you want to specify a new calendar URL?",
                                           res =>
                                           {
                                               if (res == MessageBoxResult.Yes)
                                               {
                                                   RequestNewCalendarUrl(true);
                                               }
                                           }

                                           ), "error");
            }
        }

        private void RequestNewCalendarUrl(bool reloadCourses)
        {
            Messenger.Default.Send(new CalendarUrlMessage(
                                       _dataService.CalendarUrl,
                                       newUrl =>
                                           {
                                               _dataService.CalendarUrl = newUrl;
                                               if(reloadCourses)
                                               {
                                                   ReloadCourses(newUrl);                                                   
                                               }
                                           }
                                       ),"calendar");
        }
    }
}
