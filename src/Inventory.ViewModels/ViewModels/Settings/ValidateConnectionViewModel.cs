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

using Inventory.Data.Services;
using Inventory.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.ViewModels
{
    public class ValidateConnectionViewModel : ViewModelBase
    {
        private string _message = null;

        private string _primaryButtonText;

        private string _progressStatus = null;

        private string _secondaryButtonText = "Cancel";

        public ValidateConnectionViewModel(ISettingsService settingsService, ICommonServices commonServices) : base(commonServices)
        {
            SettingsService = settingsService;
            Result = Result.Error("Operation cancelled");
        }

        public bool HasMessage => _message != null;

        public string Message
        {
            get => _message;
            set
            {
                if (Set(ref _message, value))
                {
                    NotifyPropertyChanged(nameof(HasMessage));
                }
            }
        }

        public string PrimaryButtonText
        {
            get => _primaryButtonText;
            set => Set(ref _primaryButtonText, value);
        }

        public string ProgressStatus
        {
            get => _progressStatus;
            set => Set(ref _progressStatus, value);
        }

        public Result Result { get; private set; }

        public string SecondaryButtonText
        {
            get => _secondaryButtonText;
            set => Set(ref _secondaryButtonText, value);
        }

        public ISettingsService SettingsService { get; }

        public async Task ExecuteAsync(string connectionString)
        {
            try
            {
                using (SQLServerDb db = new SQLServerDb(connectionString))
                {
                    RelationalDatabaseCreator dbCreator = db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                    if (await dbCreator.ExistsAsync())
                    {
                        Data.DbVersion version = db.DbVersion.FirstOrDefault();
                        if (version != null)
                        {
                            if (version.Version == SettingsService.DbVersion)
                            {
                                Message = $"Database connection succeeded and version is up to date.";
                                Result = Result.Ok("Database connection succeeded");
                            }
                            else
                            {
                                Message = $"Database version mismatch. Current version is {version.Version}, expected version is {SettingsService.DbVersion}. Please, recreate the database.";
                                Result = Result.Error("Database version mismatch");
                            }
                        }
                        else
                        {
                            Message = $"Database schema mismatch.";
                            Result = Result.Error("Database schema mismatch");
                        }
                    }
                    else
                    {
                        Message = $"Database does not exists. Please, create the database.";
                        Result = Result.Error("Database does not exist");
                    }
                }
            }
            catch (Exception ex)
            {
                Result = Result.Error("Error creating database. See details in Activity Log");
                Message = $"Error validating connection: {ex.Message}";
                LogException("Settings", "Validate Connection", ex);
            }
            PrimaryButtonText = "Ok";
            SecondaryButtonText = null;
        }
    }
}