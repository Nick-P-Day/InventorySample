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
using Inventory.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Views
{
    public sealed partial class ProductCard : UserControl
    {
        public ProductCard()
        {
            InitializeComponent();
        }

        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ProductDetailsViewModel), typeof(ProductCard), new PropertyMetadata(null));

        public ProductDetailsViewModel ViewModel
        {
            get => (ProductDetailsViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        #endregion

        #region Item
        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(ProductModel), typeof(ProductCard), new PropertyMetadata(null));

        public ProductModel Item
        {
            get => (ProductModel)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        #endregion
    }
}