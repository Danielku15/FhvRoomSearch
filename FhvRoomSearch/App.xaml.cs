using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace FhvRoomSearch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.Initialize();
        }
    }
}
