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

using Inventory.Models;
using Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    #region OrderItemDetailsArgs

    public class OrderItemDetailsArgs
    {
        public bool IsNew => OrderLine <= 0;

        public long OrderID { get; set; }

        public int OrderLine { get; set; }

        public static OrderItemDetailsArgs CreateDefault()
        {
            return new OrderItemDetailsArgs();
        }
    }

    #endregion

    public class OrderItemDetailsViewModel : GenericDetailsViewModel<OrderItemModel>
    {
        public OrderItemDetailsViewModel(IOrderItemService orderItemService, ICommonServices commonServices) : base(commonServices)
        {
            OrderItemService = orderItemService;
        }

        public override bool ItemIsNew => Item?.IsNew ?? true;
        public long OrderID { get; set; }
        public IOrderItemService OrderItemService { get; }

        public ICommand ProductSelectedCommand => new RelayCommand<ProductModel>(ProductSelected);
        public override string Title => (Item?.IsNew ?? true) ? TitleNew : TitleEdit;
        public string TitleEdit => $"Order Line {Item?.OrderLine}, #{Item?.OrderID}" ?? String.Empty;
        public string TitleNew => $"New Order Item, Order #{OrderID}";
        public OrderItemDetailsArgs ViewModelArgs { get; private set; }

        public OrderItemDetailsArgs CreateArgs()
        {
            return new OrderItemDetailsArgs
            {
                OrderID = Item?.OrderID ?? 0,
                OrderLine = Item?.OrderLine ?? 0
            };
        }

        public async Task LoadAsync(OrderItemDetailsArgs args)
        {
            ViewModelArgs = args ?? OrderItemDetailsArgs.CreateDefault();
            OrderID = ViewModelArgs.OrderID;

            if (ViewModelArgs.IsNew)
            {
                Item = new OrderItemModel { OrderID = OrderID };
                IsEditMode = true;
            }
            else
            {
                try
                {
                    OrderItemModel item = await OrderItemService.GetOrderItemAsync(OrderID, ViewModelArgs.OrderLine);
                    Item = item ?? new OrderItemModel { OrderID = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("OrderItem", "Load", ex);
                }
            }
        }

        public void Subscribe()
        {
            MessageService.Subscribe<OrderItemDetailsViewModel, OrderItemModel>(this, OnDetailsMessage);
            MessageService.Subscribe<OrderItemListViewModel>(this, OnListMessage);
        }

        public void Unload()
        {
            ViewModelArgs.OrderID = Item?.OrderID ?? 0;
        }

        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current order item?", "Ok", "Cancel");
        }

        protected override async Task<bool> DeleteItemAsync(OrderItemModel model)
        {
            try
            {
                StartStatusMessage("Deleting order item...");
                await Task.Delay(100);
                await OrderItemService.DeleteOrderItemAsync(model);
                EndStatusMessage("Order item deleted");
                LogWarning("OrderItem", "Delete", "Order item deleted", $"Order item #{model.OrderID}, {model.OrderLine} was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Order item: {ex.Message}");
                LogException("OrderItem", "Delete", ex);
                return false;
            }
        }

        protected override IEnumerable<IValidationConstraint<OrderItemModel>> GetValidationConstraints(OrderItemModel model)
        {
            yield return new RequiredConstraint<OrderItemModel>("Product", m => m.ProductID);
            yield return new NonZeroConstraint<OrderItemModel>("Quantity", m => m.Quantity);
            yield return new PositiveConstraint<OrderItemModel>("Quantity", m => m.Quantity);
            yield return new LessThanConstraint<OrderItemModel>("Quantity", m => m.Quantity, 100);
            yield return new PositiveConstraint<OrderItemModel>("Discount", m => m.Discount);
            yield return new NonGreaterThanConstraint<OrderItemModel>("Discount", m => m.Discount, (double)model.Subtotal, "'Subtotal'");
        }

        protected override async Task<bool> SaveItemAsync(OrderItemModel model)
        {
            try
            {
                StartStatusMessage("Saving order item...");
                await Task.Delay(100);
                await OrderItemService.UpdateOrderItemAsync(model);
                EndStatusMessage("Order item saved");
                LogInformation("OrderItem", "Save", "Order item saved successfully", $"Order item #{model.OrderID}, {model.OrderLine} was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Order item: {ex.Message}");
                LogException("OrderItem", "Save", ex);
                return false;
            }
        }

        private async void OnDetailsMessage(OrderItemDetailsViewModel sender, string message, OrderItemModel changed)
        {
            OrderItemModel current = Item;
            if (current != null)
            {
                if (changed != null && changed.OrderID == current?.OrderID && changed.OrderLine == current?.OrderLine)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    OrderItemModel item = await OrderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
                                    item = item ?? new OrderItemModel { OrderID = OrderID, OrderLine = ViewModelArgs.OrderLine, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This orderItem has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("OrderItem", "Handle Changes", ex);
                                }
                            });
                            break;

                        case "ItemDeleted":
                            await OnItemDeletedExternally();
                            break;
                    }
                }
            }
        }

        private async Task OnItemDeletedExternally()
        {
            await ContextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This orderItem has been deleted externally");
            });
        }

        private async void OnListMessage(OrderItemListViewModel sender, string message, object args)
        {
            OrderItemModel current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<OrderItemModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.OrderID == current.OrderID && r.OrderLine == current.OrderLine))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;

                    case "ItemRangesDeleted":
                        try
                        {
                            OrderItemModel model = await OrderItemService.GetOrderItemAsync(current.OrderID, current.OrderLine);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("OrderItem", "Handle Ranges Deleted", ex);
                        }
                        break;
                }
            }
        }

        private void ProductSelected(ProductModel product)
        {
            EditableItem.ProductID = product.ProductID;
            EditableItem.UnitPrice = product.ListPrice;
            EditableItem.Product = product;

            EditableItem.NotifyChanges();
        }

        /*
         *  Handle external messages
         ****************************************************************/
    }
}