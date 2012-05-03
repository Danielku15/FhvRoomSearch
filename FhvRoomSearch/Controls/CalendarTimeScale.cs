using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FhvRoomSearch.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FhvRoomSearch.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:FhvRoomSearch.Controls;assembly=FhvRoomSearch.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CalendarTimeScale/>
    ///
    /// </summary>
    public class CalendarTimeScale : Control
    {
        private static readonly CalendarTimeScaleBlock[] TwentyFourHourBlocks;
        private static readonly CalendarTimeScaleBlock[] AmPmBlocks;
        static CalendarTimeScale()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarTimeScale), new FrameworkPropertyMetadata(typeof(CalendarTimeScale)));

            TwentyFourHourBlocks = new[]
            {
                new CalendarTimeScaleBlock("00","00"),
                new CalendarTimeScaleBlock("01","00"),
                new CalendarTimeScaleBlock("02","00"),
                new CalendarTimeScaleBlock("03","00"),
                new CalendarTimeScaleBlock("04","00"),
                new CalendarTimeScaleBlock("05","00"),
                new CalendarTimeScaleBlock("06","00"),
                new CalendarTimeScaleBlock("07","00"),
                new CalendarTimeScaleBlock("08","00"),
                new CalendarTimeScaleBlock("09","00"),
                new CalendarTimeScaleBlock("10","00"),
                new CalendarTimeScaleBlock("11","00"),
                new CalendarTimeScaleBlock("12","00"),
                new CalendarTimeScaleBlock("13","00"),
                new CalendarTimeScaleBlock("14","00"),
                new CalendarTimeScaleBlock("15","00"),
                new CalendarTimeScaleBlock("16","00"),
                new CalendarTimeScaleBlock("17","00"),
                new CalendarTimeScaleBlock("18","00"),
                new CalendarTimeScaleBlock("19","00"),
                new CalendarTimeScaleBlock("20","00"),
                new CalendarTimeScaleBlock("21","00"),
                new CalendarTimeScaleBlock("22","00"),
                new CalendarTimeScaleBlock("23","00"),
            };
            AmPmBlocks = new[]
            {
                new CalendarTimeScaleBlock("12","AM"),
                new CalendarTimeScaleBlock("01","00"),
                new CalendarTimeScaleBlock("02","00"),
                new CalendarTimeScaleBlock("03","00"),
                new CalendarTimeScaleBlock("04","00"),
                new CalendarTimeScaleBlock("05","00"),
                new CalendarTimeScaleBlock("06","00"),
                new CalendarTimeScaleBlock("07","00"),
                new CalendarTimeScaleBlock("08","00"),
                new CalendarTimeScaleBlock("09","00"),
                new CalendarTimeScaleBlock("10","00"),
                new CalendarTimeScaleBlock("11","00"),
                new CalendarTimeScaleBlock("12","PM"),
                new CalendarTimeScaleBlock("01","00"),
                new CalendarTimeScaleBlock("02","00"),
                new CalendarTimeScaleBlock("03","00"),
                new CalendarTimeScaleBlock("04","00"),
                new CalendarTimeScaleBlock("05","00"),
                new CalendarTimeScaleBlock("06","00"),
                new CalendarTimeScaleBlock("07","00"),
                new CalendarTimeScaleBlock("08","00"),
                new CalendarTimeScaleBlock("09","00"),
                new CalendarTimeScaleBlock("10","00"),
                new CalendarTimeScaleBlock("11","00"),
            };
        }

        private static bool IsValidBlockHeight(object d)
        {
            return (double)d >= 10;
        }

        public double BlockHeight
        {
            get { return (double)GetValue(BlockHeightProperty); }
            set { SetValue(BlockHeightProperty, value); }
        }
        public static readonly DependencyProperty BlockHeightProperty =
            DependencyProperty.Register("BlockHeight", typeof(double), typeof(CalendarTimeScale), new UIPropertyMetadata(25d), IsValidBlockHeight);



        public CalendarTimeScaleMode Mode
        {
            get { return (CalendarTimeScaleMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(CalendarTimeScaleMode), typeof(CalendarTimeScale), new UIPropertyMetadata(CalendarTimeScaleMode.TwentyFourHour, OnModeChanged));

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarTimeScale calendar = (CalendarTimeScale)d;
            switch ((CalendarTimeScaleMode)e.NewValue)
            {
                case CalendarTimeScaleMode.TwentyFourHour:
                    calendar.Blocks = TwentyFourHourBlocks;
                    break;
                case CalendarTimeScaleMode.AmPm:
                     calendar.Blocks = AmPmBlocks;
                   break;
            }
        }

        public IEnumerable<CalendarTimeScaleBlock> Blocks
        {
            get { return (IEnumerable<CalendarTimeScaleBlock>)GetValue(BlocksProperty); }
            private set { SetValue(BlocksPropertyKey, value); }
        }

        public CalendarTimeScale()
        {
            Mode = CalendarTimeScaleMode.TwentyFourHour;
            Blocks = TwentyFourHourBlocks;
        }

        private static readonly DependencyPropertyKey BlocksPropertyKey =
            DependencyProperty.RegisterReadOnly("Blocks", typeof(IEnumerable<CalendarTimeScaleBlock>), typeof(CalendarTimeScale), new UIPropertyMetadata(TwentyFourHourBlocks));

        public static readonly DependencyProperty BlocksProperty = BlocksPropertyKey.DependencyProperty;

    }

    public class CalendarTimeScaleBlock
    {
        public string HugeLabel { get; set; }
        public string SmallLabel { get; set; }

        public CalendarTimeScaleBlock(string hugeLabel, string smallLabel)
        {
            HugeLabel = hugeLabel;
            SmallLabel = smallLabel;
        }
    }

    public enum CalendarTimeScaleMode
    {
        TwentyFourHour,
        AmPm
    }
}
