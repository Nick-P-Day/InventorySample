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
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    #region SettingsArgs

    public class SettingsArgs
    {
        public static SettingsArgs CreateDefault()
        {
            return new SettingsArgs();
        }
    }

    #endregion

    public class SettingsViewModel : ViewModelBase
    {
        private bool _isBusy = false;

        private bool _isLocalProvider;

        private bool _isSqlProvider;

        private string _sqlConnectionString = null;

        public SettingsViewModel(ISettingsService settingsService, ICommonServices commonServices) : base(commonServices)
        {
            SettingsService = settingsService;
        }

        public ICommand CreateDatabaseCommand => new RelayCommand(OnCreateDatabase);

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public bool IsLocalProvider
        {
            get => _isLocalProvider;
            set
            {
                if (Set(ref _isLocalProvider, value))
                {
                    UpdateProvider();
                }
            }
        }

        public bool IsRandomErrorsEnabled
        {
            get => SettingsService.IsRandomErrorsEnabled;
            set => SettingsService.IsRandomErrorsEnabled = value;
        }

        public bool IsSqlProvider
        {
            get => _isSqlProvider;
            set => Set(ref _isSqlProvider, value);
        }

        public ICommand ResetLocalDataCommand => new RelayCommand(OnResetLocalData);
        public ICommand SaveChangesCommand => new RelayCommand(OnSaveChanges);
        public ISettingsService SettingsService { get; }

        public string SqlConnectionString
        {
            get => _sqlConnectionString;
            set => Set(ref _sqlConnectionString, value);
        }

        public ICommand ValidateSqlConnectionCommand => new RelayCommand(OnValidateSqlConnection);
        public string Version => $"v{SettingsService.Version}";
        public SettingsArgs ViewModelArgs { get; private set; }

        public Task LoadAsync(SettingsArgs args)
        {
            ViewModelArgs = args ?? SettingsArgs.CreateDefault();

            StatusReady();

            IsLocalProvider = SettingsService.DataProvider == DataProviderType.SQLite;

            SqlConnectionString = SettingsService.SQLServerConnectionString;
            IsSqlProvider = SettingsService.DataProvider == DataProviderType.SQLServer;

            return Task.CompletedTask;
        }

        private async void OnCreateDatabase()
        {
            StatusReady();
            DisableAllViews("Waiting for the database to be created...");
            Result result = await SettingsService.CreateDabaseAsync(SqlConnectionString);
            EnableOtherViews();
            EnableThisView("");
            await Task.Delay(100);
            if (result.IsOk)
            {
                StatusMessage(result.Message);
            }
            else
            {
                StatusError("Error creating database");
            }
        }

        private async void OnResetLocalData()
        {
            IsBusy = true;
            StatusMessage("Waiting database reset...");
            Result result = await SettingsService.ResetLocalDataProviderAsync();
            IsBusy = false;
            if (result.IsOk)
            {
                StatusReady();
            }
            else
            {
                StatusMessage(result.Message);
            }
        }

        private async void OnSaveChanges()
        {
            if (IsSqlProvider)
            {
                if (await ValidateSqlConnectionAsync())
                {
                    SettingsService.SQLServerConnectionString = SqlConnectionString;
                    SettingsService.DataProvider = DataProviderType.SQLServer;
                }
            }
            else
            {
                SettingsService.DataProvider = DataProviderType.SQLite;
            }
        }

        private async void OnValidateSqlConnection()
        {
            await ValidateSqlConnectionAsync();
        }

        private void UpdateProvider()
        {
            if (IsLocalProvider && !IsSqlProvider)
            {
                SettingsService.DataProvider = DataProviderType.SQLite;
            }
        }

        private async Task<bool> ValidateSqlConnectionAsync()
        {
            StatusReady();
            IsBusy = true;
            StatusMessage("Validating connection string...");
            Result result = await SettingsService.ValidateConnectionAsync(SqlConnectionString);
            IsBusy = false;
            if (result.IsOk)
            {
                StatusMessage(result.Message);
                return true;
            }
            else
            {
                StatusMessage(result.Message);
                return false;
            }
        }
    }
}