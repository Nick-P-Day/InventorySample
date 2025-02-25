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
    public class OrderItemsViewModel : ViewModelBase
    {
        public OrderItemsViewModel(IOrderItemService orderItemService, IOrderService orderService, ICommonServices commonServices) : base(commonServices)
        {
            OrderItemService = orderItemService;

            OrderItemList = new OrderItemListViewModel(OrderItemService, commonServices);
            OrderItemDetails = new OrderItemDetailsViewModel(OrderItemService, commonServices);
        }

        public OrderItemDetailsViewModel OrderItemDetails { get; set; }
        public OrderItemListViewModel OrderItemList { get; set; }
        public IOrderItemService OrderItemService { get; }

        public async Task LoadAsync(OrderItemListArgs args)
        {
            await OrderItemList.LoadAsync(args);
        }

        public void Subscribe()
        {
            MessageService.Subscribe<OrderItemListViewModel>(this, OnMessage);
            OrderItemList.Subscribe();
            OrderItemDetails.Subscribe();
        }

        public void Unload()
        {
            OrderItemDetails.CancelEdit();
            OrderItemList.Unload();
        }

        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            OrderItemList.Unsubscribe();
            OrderItemDetails.Unsubscribe();
        }

        private async void OnItemSelected()
        {
            if (OrderItemDetails.IsEditMode)
            {
                StatusReady();
                OrderItemDetails.CancelEdit();
            }
            OrderItemModel selected = OrderItemList.SelectedItem;
            if (!OrderItemList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            OrderItemDetails.Item = selected;
        }

        private async void OnMessage(OrderItemListViewModel viewModel, string message, object args)
        {
            if (viewModel == OrderItemList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async Task PopulateDetails(OrderItemModel selected)
        {
            try
            {
                OrderItemModel model = await OrderItemService.GetOrderItemAsync(selected.OrderID, selected.OrderLine);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("OrderItems", "Load Details", ex);
            }
        }
    }
}