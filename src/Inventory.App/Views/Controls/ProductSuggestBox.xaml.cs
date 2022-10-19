﻿#region copyright
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
using Inventory.Services;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Controls
{
    public sealed partial class ProductSuggestBox : UserControl
    {
        public ProductSuggestBox()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                ProductService = ServiceLocator.Current.GetService<IProductService>();
            }
            InitializeComponent();
        }

        private IProductService ProductService { get; }

        #region Items
        public IList<ProductModel> Items
        {
            get => (IList<ProductModel>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(nameof(Items), typeof(IList<ProductModel>), typeof(ProductSuggestBox), new PropertyMetadata(null));
        #endregion

        #region DisplayText
        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(nameof(DisplayText), typeof(string), typeof(ProductSuggestBox), new PropertyMetadata(null));
        #endregion

        #region IsReadOnly*
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ProductSuggestBox control = d as ProductSuggestBox;
            control.suggestBox.Mode = ((bool)e.NewValue == true) ? FormEditMode.ReadOnly : FormEditMode.Auto;
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(ProductSuggestBox), new PropertyMetadata(false, IsReadOnlyChanged));
        #endregion

        #region ProductSelectedCommand
        public ICommand ProductSelectedCommand
        {
            get => (ICommand)GetValue(ProductSelectedCommandProperty);
            set => SetValue(ProductSelectedCommandProperty, value);
        }

        public static readonly DependencyProperty ProductSelectedCommandProperty = DependencyProperty.Register(nameof(ProductSelectedCommand), typeof(ICommand), typeof(ProductSuggestBox), new PropertyMetadata(null));
        #endregion

        private async void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (args.CheckCurrent())
                {
                    Items = String.IsNullOrEmpty(sender.Text) ? null : await GetItems(sender.Text);
                }
            }
        }

        private async Task<IList<ProductModel>> GetItems(string query)
        {
            DataRequest<Product> request = new DataRequest<Product>()
            {
                Query = query,
                OrderBy = r => r.Name
            };
            return await ProductService.GetProductsAsync(0, 20, request);
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ProductSelectedCommand?.TryExecute(args.SelectedItem);
        }
    }
}
