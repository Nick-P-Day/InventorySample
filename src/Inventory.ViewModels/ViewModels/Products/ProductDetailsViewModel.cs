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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Inventory.ViewModels
{
    #region ProductDetailsArgs

    public class ProductDetailsArgs
    {
        public bool IsNew => String.IsNullOrEmpty(ProductID);

        public string ProductID { get; set; }

        public static ProductDetailsArgs CreateDefault()
        {
            return new ProductDetailsArgs();
        }
    }

    #endregion

    public class ProductDetailsViewModel : GenericDetailsViewModel<ProductModel>
    {
        private object _newPictureSource = null;

        public ProductDetailsViewModel(IProductService productService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            ProductService = productService;
            FilePickerService = filePickerService;
        }

        public ICommand EditPictureCommand => new RelayCommand(OnEditPicture);
        public IFilePickerService FilePickerService { get; }
        public override bool ItemIsNew => Item?.IsNew ?? true;

        public object NewPictureSource
        {
            get => _newPictureSource;
            set => Set(ref _newPictureSource, value);
        }

        public IProductService ProductService { get; }
        public override string Title => (Item?.IsNew ?? true) ? "New Product" : TitleEdit;
        public string TitleEdit => Item == null ? "Product" : $"{Item.Name}";
        public ProductDetailsArgs ViewModelArgs { get; private set; }

        public override void BeginEdit()
        {
            NewPictureSource = null;
            base.BeginEdit();
        }

        public ProductDetailsArgs CreateArgs()
        {
            return new ProductDetailsArgs
            {
                ProductID = Item?.ProductID
            };
        }

        public async Task LoadAsync(ProductDetailsArgs args)
        {
            ViewModelArgs = args ?? ProductDetailsArgs.CreateDefault();

            if (ViewModelArgs.IsNew)
            {
                Item = new ProductModel();
                IsEditMode = true;
            }
            else
            {
                try
                {
                    ProductModel item = await ProductService.GetProductAsync(ViewModelArgs.ProductID);
                    Item = item ?? new ProductModel { ProductID = ViewModelArgs.ProductID, IsEmpty = true };
                }
                catch (Exception ex)
                {
                    LogException("Product", "Load", ex);
                }
            }
        }

        public void Subscribe()
        {
            MessageService.Subscribe<ProductDetailsViewModel, ProductModel>(this, OnDetailsMessage);
            MessageService.Subscribe<ProductListViewModel>(this, OnListMessage);
        }

        public void Unload()
        {
            ViewModelArgs.ProductID = Item?.ProductID;
        }

        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
        }

        protected override async Task<bool> ConfirmDeleteAsync()
        {
            return await DialogService.ShowAsync("Confirm Delete", "Are you sure you want to delete current product?", "Ok", "Cancel");
        }

        protected override async Task<bool> DeleteItemAsync(ProductModel model)
        {
            try
            {
                StartStatusMessage("Deleting product...");
                await Task.Delay(100);
                await ProductService.DeleteProductAsync(model);
                EndStatusMessage("Product deleted");
                LogWarning("Product", "Delete", "Product deleted", $"Product {model.ProductID} '{model.Name}' was deleted.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error deleting Product: {ex.Message}");
                LogException("Product", "Delete", ex);
                return false;
            }
        }

        protected override IEnumerable<IValidationConstraint<ProductModel>> GetValidationConstraints(ProductModel model)
        {
            yield return new RequiredConstraint<ProductModel>("Name", m => m.Name);
            yield return new RequiredGreaterThanZeroConstraint<ProductModel>("Category", m => m.CategoryID);
        }

        protected override async Task<bool> SaveItemAsync(ProductModel model)
        {
            try
            {
                StartStatusMessage("Saving product...");
                await Task.Delay(100);
                await ProductService.UpdateProductAsync(model);
                EndStatusMessage("Product saved");
                LogInformation("Product", "Save", "Product saved successfully", $"Product {model.ProductID} '{model.Name}' was saved successfully.");
                return true;
            }
            catch (Exception ex)
            {
                StatusError($"Error saving Product: {ex.Message}");
                LogException("Product", "Save", ex);
                return false;
            }
        }

        private async void OnDetailsMessage(ProductDetailsViewModel sender, string message, ProductModel changed)
        {
            ProductModel current = Item;
            if (current != null)
            {
                if (changed != null && changed.ProductID == current?.ProductID)
                {
                    switch (message)
                    {
                        case "ItemChanged":
                            await ContextService.RunAsync(async () =>
                            {
                                try
                                {
                                    ProductModel item = await ProductService.GetProductAsync(current.ProductID);
                                    item = item ?? new ProductModel { ProductID = current.ProductID, IsEmpty = true };
                                    current.Merge(item);
                                    current.NotifyChanges();
                                    NotifyPropertyChanged(nameof(Title));
                                    if (IsEditMode)
                                    {
                                        StatusMessage("WARNING: This product has been modified externally");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogException("Product", "Handle Changes", ex);
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

        private async void OnEditPicture()
        {
            NewPictureSource = null;
            ImagePickerResult result = await FilePickerService.OpenImagePickerAsync();
            if (result != null)
            {
                EditableItem.Picture = result.ImageBytes;
                EditableItem.PictureSource = result.ImageSource;
                EditableItem.Thumbnail = result.ImageBytes;
                EditableItem.ThumbnailSource = result.ImageSource;
                NewPictureSource = result.ImageSource;
            }
            else
            {
                NewPictureSource = null;
            }
        }

        /*
         *  Handle external messages
         ****************************************************************/

        private async Task OnItemDeletedExternally()
        {
            await ContextService.RunAsync(() =>
            {
                CancelEdit();
                IsEnabled = false;
                StatusMessage("WARNING: This product has been deleted externally");
            });
        }

        private async void OnListMessage(ProductListViewModel sender, string message, object args)
        {
            ProductModel current = Item;
            if (current != null)
            {
                switch (message)
                {
                    case "ItemsDeleted":
                        if (args is IList<ProductModel> deletedModels)
                        {
                            if (deletedModels.Any(r => r.ProductID == current.ProductID))
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        break;

                    case "ItemRangesDeleted":
                        try
                        {
                            ProductModel model = await ProductService.GetProductAsync(current.ProductID);
                            if (model == null)
                            {
                                await OnItemDeletedExternally();
                            }
                        }
                        catch (Exception ex)
                        {
                            LogException("Product", "Handle Ranges Deleted", ex);
                        }
                        break;
                }
            }
        }
    }
}