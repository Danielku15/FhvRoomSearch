using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace FhvRoomSearch.Controls
{
    class PlaceHolderTextBox : TextBox
    {
        private readonly TextAdorner _textAdorner;
        private AdornerLayer _adornerLayer;

        public static readonly DependencyProperty PlaceHolderTextProperty =
            DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(PlaceHolderTextBox), new PropertyMetadata(default(string), OnPlaceHolderTextChanged));

        private bool _adornerAdded;

        public string PlaceHolderText
        {
            get
            {
                return (string)GetValue(PlaceHolderTextProperty);
            }
            set
            {
                SetValue(PlaceHolderTextProperty, value);
            }
        }

        public PlaceHolderTextBox()
        {
            _textAdorner = new TextAdorner(this);
            Loaded += OnLoaded;
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            AddAdorner();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            RemoveAdorner();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            AddAdorner();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (!string.IsNullOrEmpty(Text))
            {
                RemoveAdorner();
            }
            else if (!IsMouseOver)
            {
                AddAdorner();
            }
        }

        private static void OnPlaceHolderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlaceHolderTextBox) d).OnPlaceHolderTextChanged(e);
        }

        private void OnPlaceHolderTextChanged(DependencyPropertyChangedEventArgs e)
        {
            _textAdorner.Text = PlaceHolderText;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _textAdorner.Text = PlaceHolderText;
        }

        private void AddAdorner()
        {
            if (_adornerLayer == null || _adornerAdded)
                return;
            if (string.IsNullOrWhiteSpace(Text))
            {
                _adornerLayer.Add(_textAdorner);
                _adornerAdded = true;
            }
        }



        private void RemoveAdorner()
        {
            if (_adornerLayer == null)
                return;
            Adorner[] adorners = _adornerLayer.GetAdorners(this);

            if (adorners != null && adorners.Length > 0)
            {
                _adornerLayer.Remove(adorners[0]);
            }
            _adornerAdded = false;
        }



    }
}
