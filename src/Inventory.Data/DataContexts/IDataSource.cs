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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Data.Services
{
    public interface IDataSource : IDisposable
    {
        DbSet<Category> Categories { get; }
        DbSet<CountryCode> CountryCodes { get; }
        DbSet<Customer> Customers { get; }
        DbSet<DbVersion> DbVersion { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderStatus> OrderStatus { get; }
        DbSet<PaymentType> PaymentTypes { get; }
        DbSet<Product> Products { get; }
        DbSet<Shipper> Shippers { get; }
        DbSet<TaxType> TaxTypes { get; }

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}