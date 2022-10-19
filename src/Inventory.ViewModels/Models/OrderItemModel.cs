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

using Inventory.Services;

namespace Inventory.Models
{
    public class OrderItemModel : ObservableObject
    {
        private decimal _discount;
        private int _quantity;
        private int _taxType;

        public decimal Discount
        {
            get => _discount;
            set
            {
                if (Set(ref _discount, value))
                {
                    UpdateTotals();
                }
            }
        }

        public bool IsNew => OrderLine <= 0;
        public long OrderID { get; set; }
        public int OrderLine { get; set; }

        public ProductModel Product { get; set; }
        public string ProductID { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (Set(ref _quantity, value))
                {
                    UpdateTotals();
                }
            }
        }

        public decimal Subtotal => Quantity * UnitPrice;

        public int TaxType
        {
            get => _taxType;
            set
            {
                if (Set(ref _taxType, value))
                {
                    UpdateTotals();
                }
            }
        }

        public decimal Total => (Subtotal - Discount) * (1 + (LookupTablesProxy.Instance.GetTaxRate(TaxType) / 100m));
        public decimal UnitPrice { get; set; }

        public override void Merge(ObservableObject source)
        {
            if (source is OrderItemModel model)
            {
                Merge(model);
            }
        }

        public void Merge(OrderItemModel source)
        {
            if (source != null)
            {
                OrderID = source.OrderID;
                OrderLine = source.OrderLine;
                ProductID = source.ProductID;
                Quantity = source.Quantity;
                UnitPrice = source.UnitPrice;
                Discount = source.Discount;
                TaxType = source.TaxType;
                Product = source.Product;
            }
        }

        private void UpdateTotals()
        {
            NotifyPropertyChanged(nameof(Subtotal));
            NotifyPropertyChanged(nameof(Total));
        }
    }
}