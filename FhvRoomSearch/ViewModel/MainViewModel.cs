using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
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

        #region Search Form Data

        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                if (_selectedDate == value)
                    return;
                _selectedDate = value;
                RaisePropertyChanged("SelectedDate");
            }
        }

        private DateTime _selectedStartTime;

        public DateTime SelectedStartTime
        {
            get
            {
                return _selectedStartTime;
            }
            set
            {
                if (_selectedStartTime == value)
                    return;
                _selectedStartTime = value;
                RaisePropertyChanged("SelectedStartTime");
            }
        }

        private DateTime _selectedEndTime;

        public DateTime SelectedEndTime
        {
            get
            {
                return _selectedEndTime;
            }
            set
            {
                if (_selectedEndTime == value)
                    return;
                _selectedEndTime = value;
                RaisePropertyChanged("SelectedEndTime");
            }
        }


        private IEnumerable<Wing> _displayedWings;

        public IEnumerable<Wing> DisplayedWings
        {
            get
            {
                return _displayedWings;
            }
            private set
            {
                if (_displayedWings == value)
                    return;
                _displayedWings = value;
                RaisePropertyChanged("DisplayedWings");
            }
        }

        private IEnumerable<Level> _displayedLevels;

        public IEnumerable<Level> DisplayedLevels
        {
            get
            {
                return _displayedLevels;
            }
            private set
            {
                if (_displayedLevels == value)
                    return;
                _displayedLevels = value;
                RaisePropertyChanged("DisplayedLevels");
            }
        }

        private IEnumerable<Room> _displayedRooms;

        public IEnumerable<Room> DisplayedRooms
        {
            get
            {
                return _displayedRooms;
            }
            private set
            {
                if (_displayedRooms == value)
                    return;
                _displayedRooms = value;
                RaisePropertyChanged("DisplayedRooms");
            }
        }


        private ObservableCollection<Wing> _selectedWings;
        public ObservableCollection<Wing> SelectedWings
        {
            get
            {
                return _selectedWings;
            }
            private set
            {
                if (_selectedWings == value)
                    return;
                _selectedWings = value;
                RaisePropertyChanged("SelectedWings");
            }
        }

        private ObservableCollection<Level> _selectedLevels;
        public ObservableCollection<Level> SelectedLevels
        {
            get
            {
                return _selectedLevels;
            }
            private set
            {
                if (_selectedLevels == value)
                    return;
                _selectedLevels = value;
                RaisePropertyChanged("SelectedLevels");
            }
        }

        private ObservableCollection<Room> _selectedRooms;
        public ObservableCollection<Room> SelectedRooms
        {
            get
            {
                return _selectedRooms;
            }
            private set
            {
                if (_selectedRooms == value)
                    return;
                _selectedRooms = value;
                RaisePropertyChanged("SelectedRooms");
            }
        }

        private RoomExtras _selectedExtras;
        public RoomExtras SelectedExtras
        {
            get
            {
                return _selectedExtras;
            }
            set
            {
                if (_selectedExtras == value)
                    return;
                _selectedExtras = value;
                RaisePropertyChanged("SelectedExtras");
            }
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

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            ProgressState = TaskbarItemProgressState.None;
            ProgressValue = 0;
            ProgressStatus = "Ready";
            ReloadCoursesCommand = new RelayCommand(ReloadCourses);
            UpdateUrlCommand = new RelayCommand(RequestNewCalendarUrl);

            ResetData();

            SelectedDate = DateTime.Today;
            SelectedStartTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            SelectedEndTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);

            SelectedExtras = RoomExtras.None;

        }

        private void ResetData()
        {
            DisplayedWings = _dataService.Wings.OrderBy(w => w.Name);

            SelectedWings = new ObservableCollection<Wing>(DisplayedWings);
            SelectedWings.CollectionChanged += OnSelectedWingsChanged;
            SelectedLevels = new ObservableCollection<Level>();
            SelectedLevels.CollectionChanged += OnSelectedLevelsChanged;
            SelectedRooms = new ObservableCollection<Room>();
        }

        private void OnSelectedLevelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DisplayedRooms = _dataService.Rooms.Where(r => SelectedLevels.Any( l => l.Id == r.Level.Id)).OrderBy(r => r.RoomId);
        }

        private void OnSelectedWingsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DisplayedLevels = _dataService.Levels.Where(
            l => SelectedWings.Any(w => w.Id == l.Wing.Id)).OrderBy(l => l.Name);
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


                if (_dataService.ResetDatabase(parser.ParsedData))
                {
                    // update settings for next download
                    _dataService.CalendarFileSize = remoteStream.ReadByteCount;
                    _dataService.CalendarLastDownload = lastChange;

                    ResetData();
                }
                else
                {
                    DownloadError("Could not save loaded data in the local storage");
                }
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
