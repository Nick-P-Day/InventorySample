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
    public sealed partial class DetailToolbar : UserControl
    {
        public DetailToolbar()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            UpdateControl();
        }

        public event ToolbarButtonClickEventHandler ButtonClick;

        #region ToolbarMode
        public static readonly DependencyProperty ToolbarModeProperty = DependencyProperty.Register("ToolbarMode", typeof(DetailToolbarMode), typeof(DetailToolbar), new PropertyMetadata(DetailToolbarMode.Default, ToolbarModeChanged));

        public DetailToolbarMode ToolbarMode
        {
            get => (DetailToolbarMode)GetValue(ToolbarModeProperty);
            set => SetValue(ToolbarModeProperty, value);
        }

        private static void ToolbarModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DetailToolbar control = d as DetailToolbar;
            control.UpdateControl();
        }

        #endregion

        #region DefaultCommands*
        public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(DetailToolbar), new PropertyMetadata("edit,delete", DefaultCommandsChanged));

        public string DefaultCommands
        {
            get => (string)GetValue(DefaultCommandsProperty);
            set => SetValue(DefaultCommandsProperty, value);
        }

        private static void DefaultCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DetailToolbar control = d as DetailToolbar;
            control.UpdateControl();
        }

        #endregion

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is AppBarButton button)
            {
                switch (button.Name)
                {
                    case "buttonBack":
                        RaiseButtonClick(ToolbarButton.Back);
                        break;

                    case "buttonEdit":
                        RaiseButtonClick(ToolbarButton.Edit);
                        break;

                    case "buttonDelete":
                        RaiseButtonClick(ToolbarButton.Delete);
                        break;

                    case "buttonCancel":
                        RaiseButtonClick(ToolbarButton.Cancel);
                        break;

                    case "buttonSave":
                        RaiseButtonClick(ToolbarButton.Save);
                        break;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands).Click += OnButtonClick;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands).Click -= OnButtonClick;
        }

        private void RaiseButtonClick(ToolbarButton toolbarButton)
        {
            ButtonClick?.Invoke(this, new ToolbarButtonClickEventArgs(toolbarButton));
        }

        private void ShowCategory(params string[] categories)
        {
            ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands)
                .ForEach(v => v.Show(v.IsCategory(categories)));
        }

        private void UpdateControl()
        {
            switch (ToolbarMode)
            {
                default:
                case DetailToolbarMode.Default:
                    ShowCategory(DefaultCommands.Split(','));
                    break;

                case DetailToolbarMode.BackEditdDelete:
                    ShowCategory("back", "edit", "delete");
                    break;

                case DetailToolbarMode.CancelSave:
                    ShowCategory("cancel", "save");
                    break;
            }
        }
    }
}