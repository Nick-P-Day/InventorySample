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

using Inventory.Models;
using Inventory.Services;
using System;
using System.Threading.Tasks;

namespace Inventory.ViewModels
{
    public class AppLogsViewModel : ViewModelBase
    {
        public AppLogsViewModel(IOrderService orderService, ICommonServices commonServices) : base(commonServices)
        {
            AppLogList = new AppLogListViewModel(commonServices);
            AppLogDetails = new AppLogDetailsViewModel(commonServices);
        }

        public AppLogDetailsViewModel AppLogDetails { get; }
        public AppLogListViewModel AppLogList { get; }

        public async Task LoadAsync(AppLogListArgs args)
        {
            await AppLogList.LoadAsync(args);
        }

        public void Subscribe()
        {
            MessageService.Subscribe<AppLogListViewModel>(this, OnMessage);
            AppLogList.Subscribe();
            AppLogDetails.Subscribe();
        }

        public void Unload()
        {
            AppLogList.Unload();
        }

        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            AppLogList.Unsubscribe();
            AppLogDetails.Unsubscribe();
        }

        private async void OnItemSelected()
        {
            if (AppLogDetails.IsEditMode)
            {
                StatusReady();
            }
            AppLogModel selected = AppLogList.SelectedItem;
            if (!AppLogList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            AppLogDetails.Item = selected;
        }

        private async void OnMessage(AppLogListViewModel viewModel, string message, object args)
        {
            if (viewModel == AppLogList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async Task PopulateDetails(AppLogModel selected)
        {
            try
            {
                AppLogModel model = await LogService.GetLogAsync(selected.Id);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("AppLogs", "Load Details", ex);
            }
        }
    }
}