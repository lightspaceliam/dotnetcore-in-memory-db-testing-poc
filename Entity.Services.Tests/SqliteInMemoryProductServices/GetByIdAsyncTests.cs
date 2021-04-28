using Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Entity.Services.Tests.SqliteInMemoryProductServices
{
    public class GetByIdAsyncTests : SqliteProductServiceTestBase
    {
        [Fact(DisplayName = "Can find existing product")]
        public async Task CanFindExistingEntity_Success()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Colour = "Green", Created = UtcNow, LastModified = UtcNow };
            InitializeData(new Product[] { product });

            var response = await Service.GetByIdAsync(productId);

            Assert.Equal(product.Name, response.Name);
            Assert.Equal(product.Code, response.Code);
            Assert.Null(response.Price);
            Assert.Equal(product.Created, response.Created);
            Assert.Equal(product.LastModified, response.LastModified);
        }

        [Fact(DisplayName = "Can include most recent juncture product price")]
        public async Task CanIncludeMostRecentJuncture_Success()
        {
            var productId = Guid.NewGuid();
            const decimal mostRecentPrice = 10.00m;

            var product = new Product
            {
                Id = productId,
                Name = "MacBook Pro",
                Code = "XYZ",
                Colour = "Red",
                Created = UtcNow,
                LastModified = UtcNow,
                ProductJunctures = new List<ProductJuncture> {
                    new ProductJuncture { Price = 11.95m,           Juncture = UtcNow.AddDays(-1) },
                    new ProductJuncture { Price = mostRecentPrice,  Juncture = UtcNow },
                    new ProductJuncture { Price = 10.95m,           Juncture = UtcNow.AddDays(-4) },
                }
            };
            InitializeData(new Product[] { product });

            var response = await Service.GetByIdAsync(productId);

            Assert.Equal(product.Name, response.Name);
            Assert.Equal(product.Code, response.Code);
            Assert.Equal(product.Created, response.Created);
            Assert.Equal(product.LastModified, response.LastModified);

            Assert.Equal(mostRecentPrice, response.Price);
        }

        [Fact(DisplayName = "Null if product does not exists")]
        public async Task NullIfEntityNotExists_Success()
        {
            var response = await Service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(response);
        }
    }
}
