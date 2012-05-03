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
    ///     <MyNamespace:CalendarDay/>
    ///
    /// </summary>
    public class CalendarDay : Control
    {


        public IEnumerable<CalendarTimeScaleBlock> Blocks
        {
            get { return (IEnumerable<CalendarTimeScaleBlock>)GetValue(BlocksProperty); }
            set { SetValue(BlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Blocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlocksProperty =
            DependencyProperty.Register("Blocks", typeof(IEnumerable<CalendarTimeScaleBlock>), typeof(CalendarDay), new UIPropertyMetadata(null));

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
            DependencyProperty.Register("BlockHeight", typeof(double), typeof(CalendarDay), new UIPropertyMetadata(25d), IsValidBlockHeight);


        static CalendarDay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarDay), new FrameworkPropertyMetadata(typeof(CalendarDay)));
        }
    }
}
