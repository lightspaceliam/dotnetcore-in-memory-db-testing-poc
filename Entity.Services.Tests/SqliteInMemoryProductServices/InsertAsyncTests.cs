using Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Entity.Services.Tests.SqliteInMemoryProductServices
{
    public class InsertAsyncTests : SqliteProductServiceTestBase
    {
        [Fact(DisplayName = "Can insert valid product")]
        public async Task CanInsertValidEntity_Success()
        {
            var product = new Product { Name = "iMac", Code = "ABC", Colour = "Green", Created = UtcNow, LastModified = UtcNow };

            var response = await Service.InsertAsync(product);

            Assert.Equal(product.Name, response.Name);
            Assert.Equal(product.Code, response.Code);
            Assert.Null(response.Price);
            Assert.Equal(product.Created, response.Created);
            Assert.Equal(product.LastModified, response.LastModified);

            var entry = Context.Products
                .FirstOrDefault();

            Assert.NotNull(entry);

            Assert.Equal(entry.Id, response.Id);
            Assert.Equal(entry.Name, response.Name);
            Assert.Equal(entry.Code, response.Code);
            Assert.Null(entry.Price);
            Assert.Equal(entry.Created, response.Created);
            Assert.Equal(entry.LastModified, response.LastModified);
        }

        [Fact(DisplayName = "Cannot insert product with invalid property values")]
        public async Task CannotInsertInvalidEntity_Failure()
        {
            var product = new Product { Name = null, Code = null, Created = UtcNow, LastModified = UtcNow };

            await Assert.ThrowsAsync<DbUpdateException>(() =>
            {
                return Service.InsertAsync(product);
            });
        }

        [Fact(DisplayName = "Cannot insert non unique product Code")]
        public async Task CannotInsertNonUniqueEntityProperty_Failure()
        {
            const string code = "ABC";

            InitializeData(new Product[]
            {
                new Product { Name = "MacBook Pro", Code = code, Colour = "Green", Created = UtcNow, LastModified = UtcNow }
            });

            var product = new Product { Name = "iMac", Code = code, Created = UtcNow, LastModified = UtcNow };

            await Assert.ThrowsAsync<DbUpdateException>(() =>
            {
                return Service.InsertAsync(product);
            });
        }
    }
}
