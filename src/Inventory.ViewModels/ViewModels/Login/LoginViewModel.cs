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

using Inventory.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private bool _isBusy = false;

        private bool _isLoginWithPassword = false;

        private bool _isLoginWithWindowsHello = false;

        private string _password = "UserPassword";

        private string _userName = null;

        public LoginViewModel(ILoginService loginService, ISettingsService settingsService, ICommonServices commonServices) : base(commonServices)
        {
            LoginService = loginService;
            SettingsService = settingsService;
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public bool IsLoginWithPassword
        {
            get => _isLoginWithPassword;
            set => Set(ref _isLoginWithPassword, value);
        }

        public bool IsLoginWithWindowsHello
        {
            get => _isLoginWithWindowsHello;
            set => Set(ref _isLoginWithWindowsHello, value);
        }

        public ILoginService LoginService { get; }
        public ICommand LoginWithPasswordCommand => new RelayCommand(LoginWithPassword);
        public ICommand LoginWithWindowHelloCommand => new RelayCommand(LoginWithWindowHello);

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public ISettingsService SettingsService { get; }

        public ICommand ShowLoginWithPasswordCommand => new RelayCommand(ShowLoginWithPassword);

        public string UserName
        {
            get => _userName;
            set => Set(ref _userName, value);
        }

        private ShellArgs ViewModelArgs { get; set; }

        public Task LoadAsync(ShellArgs args)
        {
            ViewModelArgs = args;

            UserName = SettingsService.UserName ?? args.UserInfo.AccountName;
            IsLoginWithWindowsHello = LoginService.IsWindowsHelloEnabled(UserName);
            IsLoginWithPassword = !IsLoginWithWindowsHello;
            IsBusy = false;

            return Task.CompletedTask;
        }

        public void Login()
        {
            if (IsLoginWithPassword)
            {
                LoginWithPassword();
            }
            else
            {
                LoginWithWindowHello();
            }
        }

        public async void LoginWithPassword()
        {
            IsBusy = true;
            Result result = ValidateInput();
            if (result.IsOk)
            {
                if (await LoginService.SignInWithPasswordAsync(UserName, Password))
                {
                    if (!LoginService.IsWindowsHelloEnabled(UserName))
                    {
                        await LoginService.TrySetupWindowsHelloAsync(UserName);
                    }
                    SettingsService.UserName = UserName;
                    EnterApplication();
                    return;
                }
            }
            await DialogService.ShowAsync(result.Message, result.Description);
            IsBusy = false;
        }

        public async void LoginWithWindowHello()
        {
            IsBusy = true;
            Result result = await LoginService.SignInWithWindowsHelloAsync();
            if (result.IsOk)
            {
                EnterApplication();
                return;
            }
            await DialogService.ShowAsync(result.Message, result.Description);
            IsBusy = false;
        }

        private void EnterApplication()
        {
            if (ViewModelArgs.UserInfo.AccountName != UserName)
            {
                ViewModelArgs.UserInfo = new UserInfo
                {
                    AccountName = UserName,
                    FirstName = UserName,
                    PictureSource = null
                };
            }
            NavigationService.Navigate<MainShellViewModel>(ViewModelArgs);
        }

        private void ShowLoginWithPassword()
        {
            IsLoginWithWindowsHello = false;
            IsLoginWithPassword = true;
        }

        private Result ValidateInput()
        {
            return String.IsNullOrWhiteSpace(UserName)
                ? Result.Error("Login error", "Please, enter a valid user name.")
                : String.IsNullOrWhiteSpace(Password) ? Result.Error("Login error", "Please, enter a valid password.") : Result.Ok();
        }
    }
}