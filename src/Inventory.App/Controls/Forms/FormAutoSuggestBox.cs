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

using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Inventory.Controls
{
    public class FormAutoSuggestBox : Control, IFormControl
    {
        private readonly Brush OpaqueBrush = new SolidColorBrush(Colors.White);

        private readonly Brush TransparentBrush = new SolidColorBrush(Colors.Transparent);

        private AutoSuggestBox _autoSuggestBox = null;

        private Border _borderElement;

        private Border _displayContent = null;

        private bool _isInitialized = false;

        public FormAutoSuggestBox()
        {
            DefaultStyleKey = typeof(FormAutoSuggestBox);
        }

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxTextChangedEventArgs> TextChanged;

        public event EventHandler<FormVisualState> VisualStateChanged;

        public FormVisualState VisualState { get; private set; }

        #region Header
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region HeaderTemplate
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion

        #region TextMemberPath
        public static readonly DependencyProperty TextMemberPathProperty = DependencyProperty.Register(nameof(TextMemberPath), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public string TextMemberPath
        {
            get => (string)GetValue(TextMemberPathProperty);
            set => SetValue(TextMemberPathProperty, value);
        }

        #endregion

        #region DisplayText
        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public object ItemsSource
        {
            get => (object)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        #endregion

        #region ItemTemplate
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        #endregion

        #region ItemContainerStyle
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public Style ItemContainerStyle
        {
            get => (Style)GetValue(ItemContainerStyleProperty);
            set => SetValue(ItemContainerStyleProperty, value);
        }

        #endregion

        #region PlaceholderText
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        #endregion

        #region AutoMaximizeSuggestionArea
        public static readonly DependencyProperty AutoMaximizeSuggestionAreaProperty = DependencyProperty.Register(nameof(AutoMaximizeSuggestionArea), typeof(bool), typeof(FormAutoSuggestBox), new PropertyMetadata(null));

        public bool AutoMaximizeSuggestionArea
        {
            get => (bool)GetValue(AutoMaximizeSuggestionAreaProperty);
            set => SetValue(AutoMaximizeSuggestionAreaProperty, value);
        }

        #endregion

        #region Mode*
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(FormEditMode), typeof(FormAutoSuggestBox), new PropertyMetadata(FormEditMode.Auto, ModeChanged));

        public FormEditMode Mode
        {
            get => (FormEditMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        private static void ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FormAutoSuggestBox control = d as FormAutoSuggestBox;
            control.UpdateMode();
            control.UpdateVisualState();
        }

        #endregion

        public void SetVisualState(FormVisualState visualState)
        {
            if (Mode == FormEditMode.ReadOnly)
            {
                visualState = FormVisualState.Disabled;
            }

            if (visualState != VisualState)
            {
                VisualState = visualState;
                UpdateVisualState();
                VisualStateChanged?.Invoke(this, visualState);
            }
        }

        protected override void OnApplyTemplate()
        {
            _borderElement = base.GetTemplateChild("BorderElement") as Border;
            _autoSuggestBox = base.GetTemplateChild("AutoSuggestBox") as AutoSuggestBox;
            _displayContent = base.GetTemplateChild("DisplayContent") as Border;

            _autoSuggestBox.TextChanged += (s, a) => TextChanged?.Invoke(s, a);
            _autoSuggestBox.SuggestionChosen += (s, a) => SuggestionChosen?.Invoke(s, a);
            _autoSuggestBox.QuerySubmitted += (s, a) => QuerySubmitted?.Invoke(s, a);

            _isInitialized = true;

            UpdateMode();
            UpdateVisualState();

            base.OnApplyTemplate();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            _autoSuggestBox.Text = DisplayText;

            if (Mode == FormEditMode.Auto)
            {
                SetVisualState(FormVisualState.Focused);
            }

            VisualStateManager.GoToState(this, "Focused", false);
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (VisualState == FormVisualState.Focused)
            {
                SetVisualState(FormVisualState.Ready);
            }

            VisualStateManager.GoToState(this, "Normal", false);
            base.OnLostFocus(e);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", false);
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);
            base.OnPointerExited(e);
        }

        private void UpdateMode()
        {
            switch (Mode)
            {
                case FormEditMode.Auto:
                    VisualState = FormVisualState.Idle;
                    break;

                case FormEditMode.ReadWrite:
                    VisualState = FormVisualState.Ready;
                    break;

                case FormEditMode.ReadOnly:
                    VisualState = FormVisualState.Disabled;
                    break;
            }
        }

        private void UpdateVisualState()
        {
            if (_isInitialized)
            {
                switch (VisualState)
                {
                    case FormVisualState.Idle:
                        _borderElement.Opacity = 0.40;
                        _autoSuggestBox.Opacity = 0.0;
                        _displayContent.Background = TransparentBrush;
                        _displayContent.Visibility = Visibility.Visible;
                        break;

                    case FormVisualState.Ready:
                        _borderElement.Opacity = 1.00;
                        _autoSuggestBox.Opacity = 0.0;
                        _displayContent.Background = OpaqueBrush;
                        _displayContent.Visibility = Visibility.Visible;
                        break;

                    case FormVisualState.Focused:
                        _autoSuggestBox.Opacity = 1.0;
                        _displayContent.Visibility = Visibility.Collapsed;
                        break;

                    case FormVisualState.Disabled:
                        _borderElement.Opacity = 0.40;
                        _autoSuggestBox.Opacity = 0.0;
                        _displayContent.Visibility = Visibility.Visible;
                        IsEnabled = false;
                        Opacity = 0.75;
                        break;
                }
            }
        }
    }
}