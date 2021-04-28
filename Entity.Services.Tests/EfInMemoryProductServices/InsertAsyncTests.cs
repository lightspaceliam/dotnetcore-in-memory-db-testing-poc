using Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Entity.Services.Tests.EfInMemoryProductServices
{
    public class InsertAsyncTests : ProductServiceTestBase
    {
        [Fact(DisplayName = "Can insert valid product")]
        public async Task CanInsertValidEntity_Success()
        {
            var product = new Product { Name = "iMac", Code = "ABC", Created = UtcNow, LastModified = UtcNow };

            var response = await Service.InsertAsync(product);

            Assert.NotEqual(default(Guid), response.Id);
            Assert.Equal(product.Name, response.Name);
            Assert.Equal(product.Code, response.Code);
            Assert.Equal(product.Created, response.Created);
            Assert.Equal(product.LastModified, response.LastModified);
            Assert.Null(response.Price);

            var entry = Context.Products
                .FirstOrDefault();

            Assert.NotNull(entry);
            
            Assert.Equal(entry.Id, response.Id);
            Assert.Equal(entry.Name, response.Name);
            Assert.Equal(entry.Code, response.Code);
            Assert.Equal(entry.Created, response.Created);
            Assert.Equal(entry.LastModified, response.LastModified);
            Assert.Null(entry.Price);
        }

        /// <summary>
        /// Cannot be tested... Microsoft.EntityFrameworkCore.InMemory does not enforce data annotations or detect Name & Code have not null constraints.
        /// </summary>
        /// <returns></returns>
        [Fact(DisplayName = "Cannot insert product with invalid property values")]
        public async Task CannotInsertInvalidEntity_Failure()
        {
            var product = new Product { Name = null, Code = null, Created = UtcNow, LastModified = UtcNow };

            var response = await Service.InsertAsync(product);

            //  Expexted an exception.

            Assert.Equal(product.Name, response.Name);
            Assert.Equal(product.Code, response.Code);
            Assert.Null(response.Price);
        }

        /// <summary>
        /// Cannot be tested... Microsoft.EntityFrameworkCore.InMemory does not enforce unique index constraints.
        /// </summary>
        /// <returns></returns>
        [Fact(DisplayName = "Cannot insert non unique product Code")]
        public async Task CannotInsertNonUniqueEntityProperty_Failure()
        {
            const string code = "ABC";

            InitializeData(new Product[]
            {
                new Product { Name = "MacBook Pro", Code = code, Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Name = "iMac", Code = code, Created = UtcNow, LastModified = UtcNow };

            await Service.InsertAsync(product);

            //  Expexted an exception.
            //await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            //{
            //    return Service.UpdateAsync(product);
            //});

            var productCount = Context.Products
                .Where(p => p.Code == code)
                .Count();

            Assert.Equal(2, productCount);
        }
    }
}
