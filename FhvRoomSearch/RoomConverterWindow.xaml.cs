using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FhvRoomSearch.ViewModel;

namespace FhvRoomSearch
{
    /// <summary>
    /// Interaction logic for RoomConverterWindow.xaml
    /// </summary>
    public partial class RoomConverterWindow : Window
    {
        private readonly RoomConverterViewModel _viewModel;
        public RoomConverterWindow()
        {
            InitializeComponent();
            _viewModel = new RoomConverterViewModel();
            DataContext = _viewModel;
        }
    }
}
