using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace FhvRoomSearch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private const string DatabaseFile = "RoomCourseModel.sdf";
        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            DispatcherHelper.Initialize();
            // create database file if not available
            try
            {
                if (!File.Exists(DatabaseFile))
                {
                    File.WriteAllBytes(DatabaseFile, FhvRoomSearch.Properties.Resources.RoomCourseModel);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please ensure the application has write permissions to it's parent directory!"
                                + Environment.NewLine + "Application will exit.", "Could not create database file",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
