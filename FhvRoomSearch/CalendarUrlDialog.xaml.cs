using System.Diagnostics;

namespace FhvRoomSearch
{
    /// <summary>
    /// Interaction logic for CalendarUrlDialog.xaml
    /// </summary>
    public partial class CalendarUrlDialog
    {
        public CalendarUrlDialog()
        {
            InitializeComponent();
        }

        public string CalendarUrl
        {
            get { return calendarUrlTextbox.Text; }
            set { calendarUrlTextbox.Text = value; }
        }

        private void OnHyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void OnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
