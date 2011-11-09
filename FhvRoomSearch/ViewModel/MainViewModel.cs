using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using FhvRoomSearch.Behavior;
using FhvRoomSearch.IO;
using FhvRoomSearch.Import;
using FhvRoomSearch.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System.Diagnostics;

namespace FhvRoomSearch.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        #region Commands

        public ICommand ReloadCoursesCommand
        {
            get;
            private set;
        }

        public ICommand UpdateUrlCommand
        {
            get;
            private set;
        }

        #endregion

        #region Progress

        private bool _isReloading;
        private bool _showCalendarViewerAfterReload;

        private double _progressValue;
        public double ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                if (_progressValue == value)
                    return;
                _progressValue = value;
                RaisePropertyChanged("ProgressValue");
            }
        }

        private TaskbarItemProgressState _progressState;
        public TaskbarItemProgressState ProgressState
        {
            get
            {
                return _progressState;
            }
            set
            {
                if (_progressState == value)
                    return;
                _progressState = value;
                RaisePropertyChanged("ProgressState");
            }
        }

        private string _progressStatus;
        public string ProgressStatus
        {
            get
            {
                return _progressStatus;
            }
            set
            {
                if (_progressStatus == value)
                    return;
                _progressStatus = value;
                RaisePropertyChanged("ProgressStatus");
            }
        }

        #endregion

        #region Debug Props

        private string _debugOutput;
        public string DebugOutput
        {
            get
            {
                return _debugOutput;
            }
            set
            {
                if (_debugOutput == value)
                    return;
                _debugOutput = value;
                RaisePropertyChanged("DebugOutput");
            }
        }

        #endregion

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            ProgressState = TaskbarItemProgressState.None;
            ProgressValue = 0;
            ProgressStatus = "Ready";
            ReloadCoursesCommand = new RelayCommand(ReloadCourses);
            UpdateUrlCommand = new RelayCommand(RequestNewCalendarUrl);
        }

        #region Course Reloading

        public void ReloadCourses()
        {
            ReloadCourses(_dataService.CalendarUrl);
        }

        public void ReloadCourses(string url)
        {
            lock (this)
            {
                if (_isReloading)
                    return;
                _isReloading = true;
            }

            Task.Factory.StartNew(() => DoReloadCourses(url)).ContinueWith(
                t =>
                {
                    if (_showCalendarViewerAfterReload)
                    {
                        RequestNewCalendarUrl(true);
                    }
                }, TaskContinuationOptions.ExecuteSynchronously);


            _isReloading = false;
        }

        private void UpdateProgress(TaskbarItemProgressState state, double value, string status)
        {
            if (!DispatcherHelper.UIDispatcher.CheckAccess())
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action<TaskbarItemProgressState, double, string>(UpdateProgress), state, value, status);
                return;
            }

            ProgressState = state;
            ProgressValue = value;
            ProgressStatus = status;
        }

        private void UpdateProgress(double value)
        {
            if (!DispatcherHelper.UIDispatcher.CheckAccess())
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action<double>(UpdateProgress), value);
                return;
            }

            ProgressValue = value;
        }

        private void DoReloadCourses(string url)
        {
            try
            {
                _showCalendarViewerAfterReload = false;
                UpdateProgress(TaskbarItemProgressState.Indeterminate, 0, "Connecting to Server (Checking for Changes)");

                //
                // Check for Changes
                //

                // make HTTP request
                WebRequest request = WebRequest.Create(url);
                request.Proxy = null;

                // use HEAD to determine chnages
                request.Method = "HEAD";

                // get response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                long contentLength = response.ContentLength;
                DateTime lastChange = response.LastModified;

                // check for changes
                // (it seems the FHV timetable service generates the timetables
                //  on-the-fly and always returns the current date as "LastModified"
                //  this makes caching impossible, but we are prepared!)
                if (_dataService.CalendarLastDownload >= lastChange)
                {
                    return;
                }

                //
                // Download the calendar
                //

                // We need to load the file
                UpdateProgress(TaskbarItemProgressState.Indeterminate, 0, "Connecting to Server (Download)");
                request = WebRequest.Create(url);
                request.Proxy = null;

                // get response
                response = (HttpWebResponse)request.GetResponse();

                // get stream for reading
                ByteCountingStream remoteStream = new ByteCountingStream(response.GetResponseStream());

                Encoding encoding = Encoding.UTF8;
                StreamReader reader = new StreamReader(remoteStream, encoding);

                // create parser
                UpdateProgress(TaskbarItemProgressState.Indeterminate, 0, "Setup default data");
                FhvICalParser parser = new FhvICalParser(_dataService);
                parser.Prepare();

                // load data
                UpdateProgress(
                    contentLength == -1 ? TaskbarItemProgressState.Indeterminate : TaskbarItemProgressState.Normal, 0,
                    "Reload courses from event");
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (contentLength != -1)
                    {
                        double percentage = remoteStream.ReadByteCount / (double)contentLength;
                        if (percentage > 1)
                        {
                            UpdateProgress(TaskbarItemProgressState.Indeterminate, 0, "Reload courses from event");
                        }
                        else
                        {
                            UpdateProgress(percentage);
                        }
                    }
                    parser.ProcessLine(line);
                }

                // store everything in the new database
                UpdateProgress(TaskbarItemProgressState.Indeterminate, 0, "Saving data to local storage");


                if(_dataService.ResetDatabase(parser.ParsedData))
                {
                    // update settings for next download
                    _dataService.CalendarFileSize = remoteStream.ReadByteCount;
                    _dataService.CalendarLastDownload = lastChange;
                }
                else
                {
                    DownloadError("Could not save loaded data in the local storage");
                }

                //#region Debug Code

                //StringBuilder builder = new StringBuilder();

                //foreach (var wing in parser.ParsedData)
                //{
                //    builder.AppendLine(wing.Name);
                //    foreach (var level in wing.Level)
                //    {
                //        builder.AppendLine("    " + level.Name);

                //        foreach (var room in level.Room)
                //        {
                //            builder.AppendLine("        " + room.RoomId);

                //            foreach (var course in room.Course)
                //            {
                //                builder.AppendLine(string.Format("            {0}({1} - {2})", course.Module, course.StartTime, course.EndTime));
                //            }
                //        }
                //    }
                //}

                //DispatcherHelper.UIDispatcher.BeginInvoke(new Action(
                //                                              () =>
                //                                              {
                //                                                  DebugOutput = builder.ToString();
                //                                              }));

                //#endregion
            }
            catch (Exception e)
            {
                DownloadError(e.Message);
                Debug.Fail(e.Message, e.ToString());
            }
            finally
            {
                UpdateProgress(TaskbarItemProgressState.None, 0, "Ready");
                // We have a lot of unneeded objects in our memory. let the GC get rid of them
                GC.Collect();
            }
        }

        private void DownloadError(string message)
        {
            if (!DispatcherHelper.UIDispatcher.CheckAccess())
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action<String>(DownloadError), message);
                return;
            }

            Messenger.Default.Send(new DialogMessage(
                                      "Could not load calender from current URL: '" + message + "'" +
                                      Environment.NewLine +
                                      "Do you want to specify a new calendar URL?",
                                      res =>
                                      {
                                          if (res == MessageBoxResult.Yes)
                                          {
                                              _showCalendarViewerAfterReload = true;
                                              RequestNewCalendarUrl(true);
                                          }
                                      }

                                      ), "error");
        }

        #endregion

        #region Calendar URL

        private void RequestNewCalendarUrl()
        {
            RequestNewCalendarUrl(true);
        }

        private void RequestNewCalendarUrl(bool reloadCourses)
        {
            Messenger.Default.Send(new CalendarUrlMessage(
                                       _dataService.CalendarUrl,
                                       newUrl =>
                                       {
                                           _dataService.CalendarUrl = newUrl;
                                           if (reloadCourses)
                                           {
                                               ReloadCourses(newUrl);
                                           }
                                       }
                                       ), "calendar");
        }
        #endregion

    }
}
