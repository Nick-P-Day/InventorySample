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

using Inventory.Animations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Controls
{
    public sealed class Section : ContentControl
    {
        private IconLabelButton _button = null;

        private Border _container = null;

        private Grid _content = null;

        public Section()
        {
            DefaultStyleKey = typeof(Section);
        }

        public event RoutedEventHandler HeaderButtonClick;

        #region Header
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(Section), new PropertyMetadata(null, HeaderChanged));

        public object Header
        {
            get => (object)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        private static void HeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Section control = d as Section;
            control.UpdateControl();
        }

        #endregion

        #region HeaderTemplate
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Section), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        #endregion

        #region HeaderButtonGlyph
        public static readonly DependencyProperty HeaderButtonGlyphProperty = DependencyProperty.Register("HeaderButtonGlyph", typeof(string), typeof(Section), new PropertyMetadata(null, HeaderButtonGlyphChanged));

        public string HeaderButtonGlyph
        {
            get => (string)GetValue(HeaderButtonGlyphProperty);
            set => SetValue(HeaderButtonGlyphProperty, value);
        }

        private static void HeaderButtonGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Section control = d as Section;
            control.UpdateControl();
        }

        #endregion

        #region HeaderButtonLabel
        public static readonly DependencyProperty HeaderButtonLabelProperty = DependencyProperty.Register("HeaderButtonLabel", typeof(string), typeof(Section), new PropertyMetadata(null, HeaderButtonLabelChanged));

        public string HeaderButtonLabel
        {
            get => (string)GetValue(HeaderButtonLabelProperty);
            set => SetValue(HeaderButtonLabelProperty, value);
        }

        private static void HeaderButtonLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Section control = d as Section;
            control.UpdateControl();
        }

        #endregion

        #region IsButtonVisible
        public static readonly DependencyProperty IsButtonVisibleProperty = DependencyProperty.Register("IsButtonVisible", typeof(bool), typeof(Section), new PropertyMetadata(true, IsButtonVisibleChanged));

        public bool IsButtonVisible
        {
            get => (bool)GetValue(IsButtonVisibleProperty);
            set => SetValue(IsButtonVisibleProperty, value);
        }

        private static void IsButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Section control = d as Section;
            control.UpdateControl();
        }

        #endregion

        #region Footer
        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register("Footer", typeof(object), typeof(Section), new PropertyMetadata(null));

        public object Footer
        {
            get => (object)GetValue(FooterProperty);
            set => SetValue(FooterProperty, value);
        }

        #endregion

        #region FooterTemplate
        public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(Section), new PropertyMetadata(null));

        public DataTemplate FooterTemplate
        {
            get => (DataTemplate)GetValue(FooterTemplateProperty);
            set => SetValue(FooterTemplateProperty, value);
        }

        #endregion

        public void UpdateContainer()
        {
            if (IsEnabled)
            {
                _container.ClearEffects();
                _content.Opacity = 1.0;
            }
            else
            {
                _container.Grayscale();
                _content.Opacity = 0.5;
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _container = base.GetTemplateChild("container") as Border;
            _content = base.GetTemplateChild("content") as Grid;

            _button = base.GetTemplateChild("button") as IconLabelButton;
            if (_button != null)
            {
                _button.Click += OnClick;
            }
            IsEnabledChanged += OnIsEnabledChanged;

            UpdateControl();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            HeaderButtonClick?.Invoke(this, e);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateContainer();
        }

        private void UpdateControl()
        {
            if (_content != null)
            {
                _content.RowDefinitions[0].Height = Header == null ? GridLengths.Zero : GridLengths.Auto;
                _content.RowDefinitions[2].Height = Footer == null ? GridLengths.Zero : GridLengths.Auto;
                if (_button != null)
                {
                    _button.Visibility = IsButtonVisible && !String.IsNullOrEmpty($"{HeaderButtonGlyph}{HeaderButtonLabel}") ? Visibility.Visible : Visibility.Collapsed;
                }
                UpdateContainer();
            }
        }
    }
}