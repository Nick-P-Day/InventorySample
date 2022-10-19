#region copyright
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
    public sealed class IconLabelButton : Control
    {
        private Button _button = null;

        public IconLabelButton()
        {
            DefaultStyleKey = typeof(IconLabelButton);
        }

        public event RoutedEventHandler Click;

        #region Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(IconLabelButton), new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        #endregion

        #region Glyph
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(IconLabelButton), new PropertyMetadata(null));

        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }

        #endregion

        #region GlyphSize
        public static readonly DependencyProperty GlyphSizeProperty = DependencyProperty.Register("GlyphSize", typeof(double), typeof(IconLabelButton), new PropertyMetadata(0.0));

        public double GlyphSize
        {
            get => (double)GetValue(GlyphSizeProperty);
            set => SetValue(GlyphSizeProperty, value);
        }

        #endregion

        #region Label
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(IconLabelButton), new PropertyMetadata(null));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _button = base.GetTemplateChild("button") as Button;
            _button.Click += OnClick;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}