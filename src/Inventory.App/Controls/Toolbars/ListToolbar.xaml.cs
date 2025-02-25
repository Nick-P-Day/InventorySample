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

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Controls
{
    public sealed partial class ListToolbar : UserControl
    {
        public ListToolbar()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            UpdateControl();
        }

        public event ToolbarButtonClickEventHandler ButtonClick;

        public event TypedEventHandler<AutoSuggestBox, AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

        #region NewLabel
        public static readonly DependencyProperty NewLabelProperty = DependencyProperty.Register(nameof(NewLabel), typeof(string), typeof(ListToolbar), new PropertyMetadata("New"));

        public string NewLabel
        {
            get => (string)GetValue(NewLabelProperty);
            set => SetValue(NewLabelProperty, value);
        }

        #endregion

        #region ToolbarMode
        public static readonly DependencyProperty ToolbarModeProperty = DependencyProperty.Register("ToolbarMode", typeof(ListToolbarMode), typeof(ListToolbar), new PropertyMetadata(ListToolbarMode.Default, ToolbarModeChanged));

        public ListToolbarMode ToolbarMode
        {
            get => (ListToolbarMode)GetValue(ToolbarModeProperty);
            set => SetValue(ToolbarModeProperty, value);
        }

        private static void ToolbarModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListToolbar control = d as ListToolbar;
            control.UpdateControl();
        }

        #endregion

        #region DefaultCommands*
        public static readonly DependencyProperty DefaultCommandsProperty = DependencyProperty.Register(nameof(DefaultCommands), typeof(string), typeof(ListToolbar), new PropertyMetadata("new,select,refresh,search", DefaultCommandsChanged));

        public string DefaultCommands
        {
            get => (string)GetValue(DefaultCommandsProperty);
            set => SetValue(DefaultCommandsProperty, value);
        }

        private static void DefaultCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListToolbar control = d as ListToolbar;
            control.UpdateControl();
        }

        #endregion

        #region Query
        public static readonly DependencyProperty QueryProperty = DependencyProperty.Register("Query", typeof(string), typeof(ListToolbar), new PropertyMetadata(null));

        public string Query
        {
            get => (string)GetValue(QueryProperty);
            set => SetValue(QueryProperty, value);
        }

        #endregion

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is AppBarButton button)
            {
                switch (button.Name)
                {
                    case "buttonNew":
                        RaiseButtonClick(ToolbarButton.New);
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

                    case "buttonSelect":
                        RaiseButtonClick(ToolbarButton.Select);
                        break;

                    case "buttonRefresh":
                        RaiseButtonClick(ToolbarButton.Refresh);
                        break;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands).Click += OnButtonClick;
            ElementSet.Children<AppBarButton>(commandBar.Content).Click += OnButtonClick;
        }

        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            QuerySubmitted?.Invoke(sender, args);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands).Click -= OnButtonClick;
            ElementSet.Children<AppBarButton>(commandBar.Content).Click -= OnButtonClick;
        }

        private void RaiseButtonClick(ToolbarButton toolbarButton)
        {
            ButtonClick?.Invoke(this, new ToolbarButtonClickEventArgs(toolbarButton));
        }

        private void ShowCategory(params string[] categories)
        {
            ElementSet.Children<AppBarButton>(commandBar.PrimaryCommands)
                .ForEach(v => v.Show(v.IsCategory(categories)));
            ElementSet.Children<AppBarButton>(commandBar.Content)
                .ForEach(v => v.Show(v.IsCategory(categories)));
            search.Show(search.IsCategory(categories));
        }

        private void UpdateControl()
        {
            switch (ToolbarMode)
            {
                default:
                case ListToolbarMode.Default:
                    ShowCategory(DefaultCommands.Split(','));
                    break;

                case ListToolbarMode.Cancel:
                    ShowCategory("cancel");
                    break;

                case ListToolbarMode.CancelDelete:
                    ShowCategory("cancel", "delete");
                    break;
            }
        }
    }
}