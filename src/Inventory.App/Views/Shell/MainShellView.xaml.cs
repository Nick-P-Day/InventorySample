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

using Inventory.Services;
using Inventory.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Views
{
    public sealed partial class MainShellView : Page
    {
        private INavigationService _navigationService = null;

        public MainShellView()
        {
            ViewModel = ServiceLocator.Current.GetService<MainShellViewModel>();
            InitializeContext();
            InitializeComponent();
            InitializeNavigation();
        }

        public MainShellViewModel ViewModel { get; }

        private SystemNavigationManager CurrentView => SystemNavigationManager.GetForCurrentView();

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Unload();
            ViewModel.Unsubscribe();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.LoadAsync(e.Parameter as ShellArgs);
            ViewModel.Subscribe();
        }

        private void InitializeContext()
        {
            IContextService context = ServiceLocator.Current.GetService<IContextService>();
            context.Initialize(Dispatcher, ApplicationView.GetForCurrentView().Id, CoreApplication.GetCurrentView().IsMain);
        }

        private void InitializeNavigation()
        {
            _navigationService = ServiceLocator.Current.GetService<INavigationService>();
            _navigationService.Initialize(frame);
            frame.Navigated += OnFrameNavigated;
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            var targetType = NavigationService.GetViewModel(e.SourcePageType);
            switch (targetType.Name)
            {
                case "SettingsViewModel":
                    ViewModel.SelectedItem = navigationView.SettingsItem;
                    break;

                default:
                    ViewModel.SelectedItem = ViewModel.Items.Where(r => r.ViewModel == targetType).FirstOrDefault();
                    break;
            }
            UpdateBackButton();
        }

        private async void OnLogoff(object sender, RoutedEventArgs e)
        {
            IDialogService dialogService = ServiceLocator.Current.GetService<IDialogService>();
            if (await dialogService.ShowAsync("Confirm logoff", "Are you sure you want to logoff?", "Ok", "Cancel"))
            {
                ILoginService loginService = ServiceLocator.Current.GetService<ILoginService>();
                loginService.Logoff();
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                }
            }
        }

        private void OnNavigationViewBackButton(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_navigationService.CanGoBack)
            {
                _navigationService.GoBack();
            }
        }

        private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationItem item)
            {
                ViewModel.NavigateTo(item.ViewModel);
            }
            else if (args.IsSettingsSelected)
            {
                ViewModel.NavigateTo(typeof(SettingsViewModel));
            }
            UpdateBackButton();
        }

        private void UpdateBackButton()
        {
            NavigationViewBackButton.IsEnabled = _navigationService.CanGoBack;
        }
    }
}