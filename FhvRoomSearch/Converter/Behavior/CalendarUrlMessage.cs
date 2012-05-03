using System;
using GalaSoft.MvvmLight.Messaging;

namespace FhvRoomSearch.Behavior
{
    class CalendarUrlMessage : GenericMessage<string>
    {
        public CalendarUrlMessage(
            string content,
            Action<string> callback)
            : base(content)
        {
            Callback = callback;
        }

        public CalendarUrlMessage(
            object sender,
            string content,
            Action<string> callback)
            : base(sender, content)
        {
            Callback = callback;
        }

        public CalendarUrlMessage(
            object sender,
            object target,
            string content,
            Action<string> callback)
            : base(sender, target, content)
        {
            Callback = callback;
        }

        public Action<string> Callback
        {
            get;
            private set;
        }
    }
}
