﻿#region copyright
// ****************************************************************** Copyright
// (c) Microsoft. All rights reserved. This code is licensed under the MIT
// License (MIT). THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO
// EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
// OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE CODE OR THE USE OR OTHER
// DEALINGS IN THE CODE. ******************************************************************
#endregion

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Controls
{
    public sealed class IconLabel : Control
    {
        private FontIcon _icon = null;
        private TextBlock _text = null;

        public IconLabel()
        {
            DefaultStyleKey = typeof(IconLabel);
        }

        #region Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(IconLabel), new PropertyMetadata(Orientation.Horizontal, OrientationChanged));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        private static void OrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IconLabel control = d as IconLabel;
            control.UpdateControl();
        }

        #endregion

        #region Glyph
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(IconLabel), new PropertyMetadata(null));

        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }

        #endregion

        #region GlyphSize
        public static readonly DependencyProperty GlyphSizeProperty = DependencyProperty.Register("GlyphSize", typeof(double), typeof(IconLabel), new PropertyMetadata(0.0));

        public double GlyphSize
        {
            get => (double)GetValue(GlyphSizeProperty);
            set => SetValue(GlyphSizeProperty, value);
        }

        #endregion

        #region Label
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(IconLabel), new PropertyMetadata(null, LabelChanged));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        private static void LabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IconLabel control = d as IconLabel;
            control.UpdateControl();
        }

        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _icon = base.GetTemplateChild("icon") as FontIcon;
            _text = base.GetTemplateChild("text") as TextBlock;

            UpdateControl();
        }

        private void UpdateControl()
        {
            if (_text != null)
            {
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        _text.Margin = String.IsNullOrEmpty(Label) ? new Thickness(0) : new Thickness(12, 0, 0, 0);
                        break;

                    case Orientation.Vertical:
                        _text.Margin = new Thickness(0);
                        break;
                }
            }
        }
    }
}