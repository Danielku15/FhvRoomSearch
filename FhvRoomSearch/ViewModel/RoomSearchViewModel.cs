using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shell;
using FhvRoomSearch.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FhvRoomSearch.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class RoomSearchViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;
        private readonly ProgressViewModel _progressViewModel;

        #region Commands

        public ICommand SearchCommand
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
                if (_selectedStartTime > _selectedEndTime)
                {
                    SelectedEndTime = _selectedStartTime.AddMinutes(15);
                }
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
                if (_selectedEndTime < _selectedStartTime)
                {
                    SelectedStartTime = _selectedEndTime.AddMinutes(-15);
                }
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


        private IEnumerable<SearchResult> _searchResults;

        public IEnumerable<SearchResult> SearchResults
        {
            get
            {
                return _searchResults;
            }
            private set
            {
                if (_searchResults == value)
                    return;
                _searchResults = value;
                RaisePropertyChanged("SearchResults");
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
                OnSelectedLevelsChanged(this, null);
                RaisePropertyChanged("SelectedExtras");
            }
        }

        #endregion


        public ProgressViewModel Progress
        {
            get { return _progressViewModel; }
        }

        public RoomSearchViewModel(IDataService dataService, ProgressViewModel progressViewModel)
        {
            _dataService = dataService;
            _progressViewModel = progressViewModel;

            SearchCommand = new RelayCommand(PerformSearch,
                               () => SelectedRooms != null && SelectedRooms.Count > 0);

            ResetData();

            SelectedDate = DateTime.Today;
            SelectedStartTime = DateTime.Now;

            var today = Today;
            if (DateTime.Now > today.AddHours(16))
            {
                SelectedEndTime = DateTime.Now.AddHours(1);
            }
            else if (DateTime.Now >= today.AddHours(12))
            {
                SelectedEndTime = today.AddHours(16);
            }
            else
            {
                SelectedEndTime = today.AddHours(12);
            }
            SelectedExtras = RoomExtras.None;

            _progressViewModel.DataReloaded += OnDataReloaded;

        }

        private void OnDataReloaded(object sender, EventArgs e)
        {
            ResetData();
        }

        private static DateTime Today
        {
            get
            {
                return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
            }
        }

        private void ResetData()
        {
            DisplayedWings = _dataService.Wings.OrderBy(w => w.Name);

            SelectedWings = new ObservableCollection<Wing>(DisplayedWings);
            SelectedWings.CollectionChanged += OnSelectedWingsChanged;
            SelectedLevels = new ObservableCollection<Level>();
            SelectedLevels.CollectionChanged += OnSelectedLevelsChanged;
            SelectedRooms = new ObservableCollection<Room>();
            SelectedRooms.CollectionChanged += OnSelectedRoomsChanged;

            SearchResults = new ObservableCollection<SearchResult>();
        }

        private void OnSelectedRoomsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ((RelayCommand)SearchCommand).RaiseCanExecuteChanged();
        }

        private void OnSelectedLevelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DisplayedRooms = from r in _dataService.Rooms
                             where (r.Extras & SelectedExtras) == SelectedExtras && SelectedLevels.Any(l => l.Id == r.Level.Id)
                             orderby r.RoomId
                             select r;
            //_dataService.Rooms.Where(r => SelectedLevels.Any(l => l.Id == r.Level.Id)).OrderBy(r => r.RoomId);
        }

        private void OnSelectedWingsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DisplayedLevels = from l in _dataService.Levels
                              where SelectedWings.Any(w => w.Id == l.Wing.Id)
                              orderby l.Name
                              select l;
            //DisplayedLevels = _dataService.Levels.Where(
            //l => SelectedWings.Any(w => w.Id == l.Wing.Id)).OrderBy(l => l.Name);
        }



        public void PerformSearch()
        {
            if (SelectedRooms.Count == 0)
            {
                SearchResults = new SearchResult[0];
                return;
            }

            _progressViewModel.ProgressState = TaskbarItemProgressState.Indeterminate;
            _progressViewModel.ProgressStatus = "Searching Rooms";

            Task.Factory.StartNew(DoSearchAsync).ContinueWith(
                t =>
                {
                    _progressViewModel.ProgressState = TaskbarItemProgressState.None;
                    _progressViewModel.ProgressStatus = "Ready";
                    RaisePropertyChanged("SearchResults");
                }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private void DoSearchAsync()
        {
            DateTime start = SelectedDate.Date + SelectedStartTime.TimeOfDay;
            DateTime end = SelectedDate.Date + SelectedEndTime.TimeOfDay;

            _searchResults = _dataService.PerformSearch(start, end, SelectedRooms);
        }
    }
}