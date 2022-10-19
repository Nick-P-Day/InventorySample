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
using Inventory.Services;
using Inventory.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Inventory.Views
{
    public sealed partial class LoginView : Page
    {
        private EffectMode _currentEffectMode = EffectMode.None;

        public LoginView()
        {
            ViewModel = ServiceLocator.Current.GetService<LoginViewModel>();
            InitializeContext();
            InitializeComponent();
        }

        public enum EffectMode
        {
            None,
            Background,
            Foreground,
            Disabled
        }

        public LoginViewModel ViewModel { get; }

        protected override async void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                DoEffectOut();
                await Task.Delay(100);
                ViewModel.Login();
            }
            base.OnKeyDown(e);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            _currentEffectMode = EffectMode.None;
            await ViewModel.LoadAsync(e.Parameter as ShellArgs);
            InitializeNavigation();
        }

        private void DoEffectIn(double milliseconds = 1000)
        {
            if (_currentEffectMode == EffectMode.Foreground || _currentEffectMode == EffectMode.None)
            {
                _currentEffectMode = EffectMode.Background;
                background.Scale(milliseconds, 1.0, 1.1);
                background.Blur(milliseconds, 6.0, 0.0);
                foreground.Scale(500, 1.0, 0.95);
                foreground.Fade(milliseconds, 1.0, 0.75);
            }
        }

        private void DoEffectOut(double milliseconds = 1000)
        {
            if (_currentEffectMode == EffectMode.Background || _currentEffectMode == EffectMode.None)
            {
                _currentEffectMode = EffectMode.Foreground;
                background.Scale(milliseconds, 1.1, 1.0);
                background.Blur(milliseconds, 0.0, 6.0);
                foreground.Scale(500, 0.95, 1.0);
                foreground.Fade(milliseconds, 0.75, 1.0);
            }
        }

        private void InitializeContext()
        {
            IContextService context = ServiceLocator.Current.GetService<IContextService>();
            context.Initialize(Dispatcher, ApplicationView.GetForCurrentView().Id, CoreApplication.GetCurrentView().IsMain);
        }

        private void InitializeNavigation()
        {
            INavigationService navigationService = ServiceLocator.Current.GetService<INavigationService>();
            navigationService.Initialize(Frame);
        }

        private void OnBackgroundFocus(object sender, RoutedEventArgs e)
        {
            DoEffectIn();
        }

        private void OnForegroundFocus(object sender, RoutedEventArgs e)
        {
            DoEffectOut();
        }

        private async void OnShowLoginWithPassword(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
            passwordView.Focus();
        }
    }
}