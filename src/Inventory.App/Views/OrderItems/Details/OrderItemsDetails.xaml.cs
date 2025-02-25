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

using Inventory.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Views
{
    public sealed partial class OrderItemsDetails : UserControl
    {
        public OrderItemsDetails()
        {
            InitializeComponent();
        }

        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(OrderItemDetailsViewModel), typeof(OrderItemsDetails), new PropertyMetadata(null));

        public OrderItemDetailsViewModel ViewModel
        {
            get => (OrderItemDetailsViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        #endregion
    }
}