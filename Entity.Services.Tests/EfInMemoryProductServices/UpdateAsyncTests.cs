using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Entity.Services.Tests.EfInMemoryProductServices
{
    public class UpdateAsyncTests : ProductServiceTestBase
    {
        [Fact(DisplayName = "Can update valid product")]
        public async Task CanUpdateValidEntity_Success()
        {
            var productId = Guid.NewGuid();

            InitializeData(new Product[]
            {
                new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Id = productId, Name = "iMac", Code = "ABC", Created = UtcNow, LastModified = UtcNow };

            var response = await Service.UpdateAsync(product);

            Assert.Equal(product.Name, response.Name);
            Assert.Equal(product.Code, response.Code);
            Assert.Null(response.Price);
        }

        [Fact(DisplayName = "Invalid concurrency cannot update product")]
        public async Task InvalidConcurrencyCannotUpdate_Failure()
        {
            var productId = Guid.NewGuid();

            InitializeData(new Product[]
            {
                new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Id = productId, Name = "iMac", Code = "ABC", Created = UtcNow, LastModified = UtcNow.AddDays(1) };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            {
                return Service.UpdateAsync(product);
            });
        }

        /// <summary>
        /// Cannot be tested... Microsoft.EntityFrameworkCore.InMemory does not enforce data annotations or detect Name & Code have not null constraints.
        /// </summary>
        /// <returns></returns>
        [Fact(DisplayName = "Cannot update with invalid product properties")]
        public async Task InvalidEntityPropertyValues_Failure()
        {
            var productId = Guid.NewGuid();

            InitializeData(new Product[]
            {
                new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Id = productId, Name = null, Code = null, Created = UtcNow, LastModified = UtcNow };

            var response = await Service.UpdateAsync(product);

            //  Expected.
            //await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            //{
            //    return Service.UpdateAsync(product);
            //});

            Assert.Null(response.Name);
            Assert.Null(response.Code);
        }

        [Fact(DisplayName = "Product does not exist, cannot update product")]
        public async Task EntityRecordDoesNotExistCannotUpdate_Failure()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "iMac", Code = "ABC", Created = UtcNow, LastModified = UtcNow };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            {
                return Service.UpdateAsync(product);
            });
        }


        /// <summary>
        /// Cannot be tested... Microsoft.EntityFrameworkCore.InMemory does not enforce unique index constraints.
        /// </summary>
        /// <returns></returns>
        [Fact(DisplayName = "Cannot duplicate unique product Code")]
        public async Task CannotDuplicateUniqueEntityProperty_Failure()
        {
            var productId = Guid.NewGuid();
            const string code = "ABC";

            InitializeData(new Product[]
            {
                new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Created = UtcNow, LastModified = UtcNow },
                new Product { Id = Guid.NewGuid(), Name = "iMac", Code = code, Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Id = productId, Name = "iMac", Code = code, Created = UtcNow, LastModified = UtcNow };

            await Service.UpdateAsync(product);

            //  Expected.
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
