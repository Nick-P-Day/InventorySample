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

using Inventory.Views;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Inventory.Services
{
    public class SettingsService : ISettingsService
    {
        public SettingsService(IDialogService dialogService)
        {
            DialogService = dialogService;
        }

        public DataProviderType DataProvider
        {
            get => AppSettings.Current.DataProvider;
            set => AppSettings.Current.DataProvider = value;
        }

        public string DbVersion => AppSettings.Current.DbVersion;
        public IDialogService DialogService { get; }

        public bool IsRandomErrorsEnabled
        {
            get => AppSettings.Current.IsRandomErrorsEnabled;
            set => AppSettings.Current.IsRandomErrorsEnabled = value;
        }

        public string PatternConnectionString => $"Data Source={AppSettings.DatabasePatternFileName}";

        public string SQLServerConnectionString
        {
            get => AppSettings.Current.SQLServerConnectionString;
            set => AppSettings.Current.SQLServerConnectionString = value;
        }

        public string UserName
        {
            get => AppSettings.Current.UserName;
            set => AppSettings.Current.UserName = value;
        }

        public string Version => AppSettings.Current.Version;

        public async Task<Result> CreateDabaseAsync(string connectionString)
        {
            CreateDatabaseView dialog = new CreateDatabaseView(connectionString);
            var res = await dialog.ShowAsync();
            switch (res)
            {
                case ContentDialogResult.Secondary:
                    return Result.Ok("Operation canceled by user");

                default:
                    break;
            }
            return dialog.Result;
        }

        public async Task<Result> ResetLocalDataProviderAsync()
        {
            Result result = null;
            string errorMessage = null;
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var databaseFolder = await localFolder.CreateFolderAsync(AppSettings.DatabasePath, CreationCollisionOption.OpenIfExists);
                var sourceFile = await databaseFolder.GetFileAsync(AppSettings.DatabasePattern);
                var targetFile = await databaseFolder.CreateFileAsync(AppSettings.DatabaseName, CreationCollisionOption.ReplaceExisting);
                await sourceFile.CopyAndReplaceAsync(targetFile);
                await DialogService.ShowAsync("Reset Local Data Provider", "Local Data Provider restore successfully.");
                result = Result.Ok();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                result = Result.Error(ex);
            }
            if (errorMessage != null)
            {
                await DialogService.ShowAsync("Reset Local Data Provider", errorMessage);
            }
            return result;
        }

        public async Task<Result> ValidateConnectionAsync(string connectionString)
        {
            ValidateConnectionView dialog = new ValidateConnectionView(connectionString);
            var res = await dialog.ShowAsync();
            switch (res)
            {
                case ContentDialogResult.Secondary:
                    return Result.Ok("Operation canceled by user");

                default:
                    break;
            }
            return dialog.Result;
        }
    }
}