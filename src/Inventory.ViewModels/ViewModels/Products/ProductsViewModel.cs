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
using System.Threading.Tasks;

namespace Inventory.ViewModels
{
    public class ProductsViewModel : ViewModelBase
    {
        public ProductsViewModel(IProductService productService, IOrderService orderService, IFilePickerService filePickerService, ICommonServices commonServices) : base(commonServices)
        {
            ProductService = productService;

            ProductList = new ProductListViewModel(ProductService, commonServices);
            ProductDetails = new ProductDetailsViewModel(ProductService, filePickerService, commonServices);
        }

        public ProductDetailsViewModel ProductDetails { get; set; }
        public ProductListViewModel ProductList { get; set; }
        public IProductService ProductService { get; }

        public async Task LoadAsync(ProductListArgs args)
        {
            await ProductList.LoadAsync(args);
        }

        public void Subscribe()
        {
            MessageService.Subscribe<ProductListViewModel>(this, OnMessage);
            ProductList.Subscribe();
            ProductDetails.Subscribe();
        }

        public void Unload()
        {
            ProductDetails.CancelEdit();
            ProductList.Unload();
        }

        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            ProductList.Unsubscribe();
            ProductDetails.Unsubscribe();
        }

        private async void OnItemSelected()
        {
            if (ProductDetails.IsEditMode)
            {
                StatusReady();
                ProductDetails.CancelEdit();
            }
            ProductModel selected = ProductList.SelectedItem;
            if (!ProductList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            ProductDetails.Item = selected;
        }

        private async void OnMessage(ProductListViewModel viewModel, string message, object args)
        {
            if (viewModel == ProductList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async Task PopulateDetails(ProductModel selected)
        {
            try
            {
                ProductModel model = await ProductService.GetProductAsync(selected.ProductID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Products", "Load Details", ex);
            }
        }
    }
}