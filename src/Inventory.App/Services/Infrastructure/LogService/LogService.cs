#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using Inventory.Data;
using Inventory.Models;

namespace Inventory.Services
{
    public class LogService : ILogService
    {
        public LogService(IMessageService messageService)
        {
            MessageService = messageService;
        }

        public IMessageService MessageService { get; }
        public async Task WriteAsync(LogType type, string source, string action, Exception ex)
        {
            await WriteAsync(LogType.Error, source, action, ex.Message, ex.ToString());
            Exception deepException = ex.InnerException;
            while (deepException != null)
            {
                await WriteAsync(LogType.Error, source, action, deepException.Message, deepException.ToString());
                deepException = deepException.InnerException;
            }
        }
        public async Task WriteAsync(LogType type, string source, string action, string message, string description)
        {
            AppLog appLog = new AppLog
            {
                User = AppSettings.Current.UserName ?? "App",
                Type = type,
                Source = source,
                Action = action,
                Message = message,
                Description = description,
                IsRead = type != LogType.Error
            };

            await CreateLogAsync(appLog);
            MessageService.Send(this, "LogAdded", appLog);
        }

        private AppLogDb CreateDataSource()
        {
            return new AppLogDb(AppSettings.Current.AppLogConnectionString);
        }

        public async Task<AppLogModel> GetLogAsync(long id)
        {
            using (AppLogDb ds = CreateDataSource())
            {
                var item = await ds.GetLogAsync(id);
                return item != null ? CreateAppLogModel(item) : null;
            }
        }

        public async Task<IList<AppLogModel>> GetLogsAsync(DataRequest<AppLog> request)
        {
            LogCollection collection = new LogCollection(this);
            await collection.LoadAsync(request);
            return collection;
        }

        public async Task<IList<AppLogModel>> GetLogsAsync(int skip, int take, DataRequest<AppLog> request)
        {
            var models = new List<AppLogModel>();
            using (AppLogDb ds = CreateDataSource())
            {
                var items = await ds.GetLogsAsync(skip, take, request);
                foreach (var item in items)
                {
                    models.Add(CreateAppLogModel(item));
                }
                return models;
            }
        }

        public async Task<int> GetLogsCountAsync(DataRequest<AppLog> request)
        {
            using (AppLogDb ds = CreateDataSource())
            {
                return await ds.GetLogsCountAsync(request);
            }
        }

        public async Task<int> CreateLogAsync(AppLog appLog)
        {
            using (AppLogDb ds = CreateDataSource())
            {
                return await ds.CreateLogAsync(appLog);
            }
        }

        public async Task<int> DeleteLogAsync(AppLogModel model)
        {
            AppLog appLog = new AppLog { Id = model.Id };
            using (AppLogDb ds = CreateDataSource())
            {
                return await ds.DeleteLogsAsync(appLog);
            }
        }

        public async Task<int> DeleteLogRangeAsync(int index, int length, DataRequest<AppLog> request)
        {
            using (AppLogDb ds = CreateDataSource())
            {
                var items = await ds.GetLogKeysAsync(index, length, request);
                return await ds.DeleteLogsAsync(items.ToArray());
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            using (AppLogDb ds = CreateDataSource())
            {
                await ds.MarkAllAsReadAsync();
            }
        }

        private AppLogModel CreateAppLogModel(AppLog source)
        {
            return new AppLogModel()
            {
                Id = source.Id,
                IsRead = source.IsRead,
                DateTime = source.DateTime,
                User = source.User,
                Type = source.Type,
                Source = source.Source,
                Action = source.Action,
                Message = source.Message,
                Description = source.Description,
            };
        }
    }
}
