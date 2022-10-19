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
    public sealed partial class Details : UserControl, INotifyExpressionChanged
    {
        private static readonly DependencyExpressions DependencyExpressions = new DependencyExpressions();

        private static readonly DependencyExpression ToolbarModeExpression = DependencyExpressions.Register(nameof(ToolbarMode), nameof(IsEditMode), nameof(CanGoBack));

        public Details()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            DependencyExpressions.Initialize(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DetailToolbarMode ToolbarMode => IsEditMode ? DetailToolbarMode.CancelSave : CanGoBack ? DetailToolbarMode.BackEditdDelete : DetailToolbarMode.Default;

        public void SetFocus()
        {
            GetFormControls().FirstOrDefault()?.Focus(FocusState.Programmatic);
        }

        private IEnumerable<IFormControl> GetFormControls()
        {
            return ElementSet.Children<Control>(container)
                .Where(r =>
                {
                    return r is IFormControl ctrl;
                })
                .Cast<IFormControl>();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var ctrl in GetFormControls())
            {
                ctrl.VisualStateChanged += OnVisualStateChanged;
            }
            UpdateEditMode();
        }

        #region CanGoBack*
        public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(Details), new PropertyMetadata(false, CanGoBackChanged));

        public bool CanGoBack
        {
            get => (bool)GetValue(CanGoBackProperty);
            set => SetValue(CanGoBackProperty, value);
        }

        private static void CanGoBackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Details control = d as Details;
            DependencyExpressions.UpdateDependencies(control, nameof(CanGoBack));
        }

        #endregion

        #region IsEditMode*
        public static readonly DependencyProperty IsEditModeProperty = DependencyProperty.Register(nameof(IsEditMode), typeof(bool), typeof(Details), new PropertyMetadata(false, IsEditModeChanged));

        public bool IsEditMode
        {
            get => (bool)GetValue(IsEditModeProperty);
            set => SetValue(IsEditModeProperty, value);
        }

        private static void IsEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Details control = d as Details;
            DependencyExpressions.UpdateDependencies(control, nameof(IsEditMode));
            control.UpdateEditMode();
        }

        #endregion

        #region DetailsContent
        public static readonly DependencyProperty DetailsContentProperty = DependencyProperty.Register(nameof(DetailsContent), typeof(object), typeof(Details), new PropertyMetadata(null));

        public object DetailsContent
        {
            get => (object)GetValue(DetailsContentProperty);
            set => SetValue(DetailsContentProperty, value);
        }

        #endregion

        #region DetailsTemplate
        public static readonly DependencyProperty DetailsTemplateProperty = DependencyProperty.Register(nameof(DetailsTemplate), typeof(DataTemplate), typeof(Details), new PropertyMetadata(null));

        public DataTemplate DetailsTemplate
        {
            get => (DataTemplate)GetValue(DetailsTemplateProperty);
            set => SetValue(DetailsTemplateProperty, value);
        }

        #endregion

        #region DefaultCommands
        public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(Details), new PropertyMetadata("edit,delete"));

        public string DefaultCommands
        {
            get => (string)GetValue(DefaultCommandsProperty);
            set => SetValue(DefaultCommandsProperty, value);
        }

        #endregion

        #region BackCommand
        public static readonly DependencyProperty BackCommandProperty = DependencyProperty.Register(nameof(BackCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));

        public ICommand BackCommand
        {
            get => (ICommand)GetValue(BackCommandProperty);
            set => SetValue(BackCommandProperty, value);
        }

        #endregion

        #region EditCommand
        public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));

        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        #endregion

        #region DeleteCommand
        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        #endregion

        #region SaveCommand
        public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register(nameof(SaveCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));

        public ICommand SaveCommand
        {
            get => (ICommand)GetValue(SaveCommandProperty);
            set => SetValue(SaveCommandProperty, value);
        }

        #endregion

        #region CancelCommand
        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(nameof(CancelCommand), typeof(ICommand), typeof(Details), new PropertyMetadata(null));

        public ICommand CancelCommand
        {
            get => (ICommand)GetValue(CancelCommandProperty);
            set => SetValue(CancelCommandProperty, value);
        }

        #endregion

        private void OnToolbarClick(object sender, ToolbarButtonClickEventArgs e)
        {
            switch (e.ClickedButton)
            {
                case ToolbarButton.Back:
                    BackCommand?.TryExecute();
                    break;

                case ToolbarButton.Edit:
                    EditCommand?.TryExecute();
                    break;

                case ToolbarButton.Delete:
                    DeleteCommand?.TryExecute();
                    break;

                case ToolbarButton.Save:
                    SaveCommand?.TryExecute();
                    break;

                case ToolbarButton.Cancel:
                    CancelCommand?.TryExecute();
                    break;
            }
        }

        private void OnVisualStateChanged(object sender, FormVisualState e)
        {
            if (e == FormVisualState.Focused)
            {
                if (!IsEditMode)
                {
                    EditCommand?.TryExecute();
                }
            }
        }

        private void UpdateEditMode()
        {
            if (IsEditMode)
            {
                foreach (var ctrl in GetFormControls().Where(r => r.VisualState != FormVisualState.Focused))
                {
                    ctrl.SetVisualState(FormVisualState.Ready);
                }
            }
            else
            {
                Focus(FocusState.Programmatic);
                foreach (var ctrl in GetFormControls())
                {
                    ctrl.SetVisualState(FormVisualState.Idle);
                }
            }
        }

        #region NotifyPropertyChanged

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}