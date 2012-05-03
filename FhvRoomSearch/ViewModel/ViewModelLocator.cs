using FhvRoomSearch.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace FhvRoomSearch.ViewModel
{
    class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<RoomSearchViewModel>();
            SimpleIoc.Default.Register<RoomConverterViewModel>();
            SimpleIoc.Default.Register<ProgressViewModel>();
            SimpleIoc.Default.Register<RoomCourseViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public RoomConverterViewModel Converter
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RoomConverterViewModel>();
            }
        }

        public RoomSearchViewModel Searcher
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RoomSearchViewModel>();
            }
        }

        public ProgressViewModel Progress
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ProgressViewModel>();
            }
        }

        public RoomCourseViewModel RoomCourse
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RoomCourseViewModel>();
            }
        }

        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<IDataService>().Cleanup();
        }
    }
}
