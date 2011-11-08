using System.Windows;
using System.Windows.Interactivity;
using GalaSoft.MvvmLight.Messaging;

namespace FhvRoomSearch.Behavior
{
    class CalendarUrlBehavior : Behavior<FrameworkElement>
    {
        private readonly Messenger _messenger = Messenger.Default;

        protected override void OnAttached()
        {
            base.OnAttached();
            _messenger.Register<CalendarUrlMessage>(this, Identifier, ShowDialog);
        }

        private void ShowDialog(CalendarUrlMessage obj)
        {
            var window = AssociatedObject as Window;

            CalendarUrlDialog dialog = new CalendarUrlDialog();
            dialog.CalendarUrl = obj.Content;
            if (window != null)
                dialog.Owner = window;
            if(dialog.ShowDialog().GetValueOrDefault())
            {
                obj.Callback(dialog.CalendarUrl);
            }
        }

        public string Identifier
        {
            get;
            set;
        }
    }
}
