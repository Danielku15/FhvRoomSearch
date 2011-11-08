using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace FhvRoomSearch.Controls
{
    class TextAdorner : Adorner
    {
        private static readonly SolidColorBrush PlaceHolderBrush = new SolidColorBrush(new Color
        {
            A = 150,
            R = 128,
            G = 128,
            B = 128
        });

        private Point _anchor = new Point(7, 1);
        private FormattedText _formattedText;
        private readonly PlaceHolderTextBox _textBox;

        public string Text
        {
            get
            {
                return _formattedText.Text;
            }
            set
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    value = "";
                }
                _formattedText = new FormattedText(value, CultureInfo.CurrentUICulture,
                FlowDirection, new Typeface(_textBox.FontFamily, _textBox.FontStyle, _textBox.FontWeight, _textBox.FontStretch), _textBox.FontSize, PlaceHolderBrush);
                _anchor = new Point(7, (_textBox.ActualHeight - _formattedText.Height)/2);
            }
        }

        public TextAdorner(PlaceHolderTextBox element)
            : base(element)
        {
            _textBox = element;
            Text = element.PlaceHolderText;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawText(_formattedText, _anchor);
        }
    }
}
