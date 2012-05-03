using System;
using System.Windows.Shell;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

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
    public class ProgressViewModel : ViewModelBase
    {

        #region Progress


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


        /// <summary>
        /// Initializes a new instance of the ProgressViewModel class.
        /// </summary>
        public ProgressViewModel()
        {
            ProgressState = TaskbarItemProgressState.None;
            ProgressValue = 0;
            ProgressStatus = "Ready";
        }

        public void UpdateProgress(TaskbarItemProgressState state, double value, string status)
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

        public void UpdateProgress(double value)
        {
            if (!DispatcherHelper.UIDispatcher.CheckAccess())
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action<double>(UpdateProgress), value);
                return;
            }

            ProgressValue = value;
        }

        public event EventHandler DataReloaded;
        public void InvokeDataReloaded(EventArgs e)
        {
            EventHandler handler = DataReloaded;
            if (handler != null) handler(this, e);
        }
    }
}