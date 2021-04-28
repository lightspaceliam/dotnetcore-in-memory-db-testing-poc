using Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Entity.Services.Tests.SqliteInMemoryProductServices
{
    public class UpdateAsyncTests : SqliteProductServiceTestBase
    {
        [Fact(DisplayName = "Can update valid product")]
        public async Task CanUpdateValidEntity_Success()
        {
            var productId = Guid.NewGuid();

            InitializeData(new Product[]
            {
                new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Colour = "Green", Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Id = productId, Name = "iMac", Code = "ABC", Colour = "Red", Created = UtcNow, LastModified = UtcNow };

            var response = await Service.UpdateAsync(product);

            Assert.Equal(product.Name, response.Name);
            Assert.Equal(product.Code, response.Code);
            Assert.Null(product.Price);
        }

        [Fact(DisplayName = "Invalid concurrency cannot update product")]
        public async Task InvalidConcurrencyCannotUpdate_Failure()
        {
            var productId = Guid.NewGuid();

            InitializeData(new Product[]
            {
                new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Colour = "Green", Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Id = productId, Name = "iMac", Code = "ABC", Colour = "Red", Created = UtcNow, LastModified = UtcNow.AddDays(1) };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            {
                return Service.UpdateAsync(product);
            });
        }

        [Fact(DisplayName = "Cannot update with invalid product properties")]
        public async Task InvalidEntityPropertyValues_Failure()
        {
            var productId = Guid.NewGuid();

            InitializeData(new Product[]
            {
                new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Colour = "Green", Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Id = productId, Name = null, Code = null, Created = UtcNow, LastModified = UtcNow };

            await Assert.ThrowsAsync<DbUpdateException>(() =>
            {
                return Service.UpdateAsync(product);
            });
        }

        [Fact(DisplayName = "Product does not exist, cannot update product")]
        public async Task EntityRecordDoesNotExistCannotUpdate_Failure()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "iMac", Code = "ABC", Colour = "Green", Created = UtcNow, LastModified = UtcNow };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            {
                return Service.UpdateAsync(product);
            });
        }

        [Fact(DisplayName = "Cannot duplicate unique product Code")]
        public async Task CannotDuplicateUniqueEntityProperty_Failure()
        {
            var productId = Guid.NewGuid();
            const string code = "ABC";

            InitializeData(new Product[]
            {
                new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Colour = "Green", Created = UtcNow, LastModified = UtcNow },
                new Product { Id = Guid.NewGuid(), Name = "iMac", Code = code, Colour = "Red", Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Id = productId, Name = "iMac", Code = code, Colour = "Yellow", Created = UtcNow, LastModified = UtcNow };

            //  Both MS &Sqlite in memory testing throw a DbUpdateException how ever to get the error number you have to case to an SqliteException.This limits the granularity you can test to.
            var dbUpdateException = await Assert.ThrowsAsync<DbUpdateException>(() =>
            {
                return Service.UpdateAsync(product);
            });

            var uniqueConstraintErrorNumber = (dbUpdateException.InnerException as SqliteException)?.SqliteErrorCode;

            //  MS SQL's equivelent is 2601 - reference in the readme file.
            Assert.Equal(19, uniqueConstraintErrorNumber);
        }
    }
}
