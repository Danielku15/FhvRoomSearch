using System;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace FhvRoomSearch.Controls
{
    class MinuteDateTimeUpDown : DateTimeUpDown
    {
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(int), typeof(DateTimeUpDown), new UIPropertyMetadata(1));

        public int Step
        {
            get
            {
                return (int)GetValue(StepProperty);
            }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public MinuteDateTimeUpDown()
        {
            Format = DateTimeFormat.Custom;
            FormatString = "HH:mm";
        }

        protected override void OnIncrement()
        {
            if (Value.HasValue)
                UpdateDateTime(1);
            else
                Value = DefaultValue;
        }

        protected override void OnDecrement()
        {
            if (Value.HasValue)
                UpdateDateTime(-1);
            else
                Value = DefaultValue;
        }

        private void UpdateDateTime(int p)
        {
            Value = ((DateTime)Value).AddMinutes(p * Step);
        }
    }
}
