using System.Windows;
using System.Windows.Interactivity;
using GalaSoft.MvvmLight.Messaging;

namespace FhvRoomSearch.Behavior
{
    class MessageBoxBehavior : Behavior<FrameworkElement>
    {
        private readonly Messenger _messenger = Messenger.Default;

        protected override void OnAttached()
        {
            base.OnAttached();

            _messenger.Register<DialogMessage>(this, Identifier, ShowDialog);
        }

        public string Identifier
        {
            get;
            set;
        }
        public string Caption
        {
            get;
            set;
        }
        public string Text
        {
            get;
            set;
        }
        public MessageBoxButton Buttons
        {
            get;
            set;
        }

        private void ShowDialog(DialogMessage dm)
        {
            string caption = dm.Caption ?? Caption;
            string text = dm.Content ?? Text;
            var result = MessageBox.Show(text, caption, Buttons);
            dm.Callback(result);
        }

    }
}
