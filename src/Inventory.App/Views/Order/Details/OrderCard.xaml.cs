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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Views
{
    public sealed partial class OrderCard : UserControl
    {
        public OrderCard()
        {
            InitializeComponent();
        }

        #region Item
        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register(nameof(Item), typeof(OrderModel), typeof(OrderCard), new PropertyMetadata(null));

        public OrderModel Item
        {
            get => (OrderModel)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        #endregion
    }
}