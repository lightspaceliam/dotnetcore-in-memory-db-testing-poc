using Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Entity.Services.Tests.EfInMemoryProductServices
{
    public class GetByIdAsyncTests : ProductServiceTestBase
    {
        [Fact(DisplayName = "Can find existing product")]
        public async Task CanFindExistingEntity_Success()
        {
            var productId = Guid.NewGuid();

            // Product.Colour is required however EF InMemoryDatabase allows null.
            var product = new Product { Id = productId, Name = "MacBook Pro", Code = "XYZ", Created = UtcNow, LastModified = UtcNow };
            InitializeData(new Product[] { product });

            var response = await Service.GetByIdAsync(productId);

            Assert.Equal(product.Name, response.Name);
            Assert.Equal(product.Code, response.Code);
            Assert.Equal(product.Created, response.Created);
            Assert.Equal(product.LastModified, response.LastModified);

            Assert.Null(response.Price);
        }

        [Fact(DisplayName = "Can include most recent juncture product price")]
        public async Task CanIncludeMostRecentJuncture_Success()
        {
            var productId = Guid.NewGuid();
            const decimal mostRecentPrice = 10.00m;

            var product = new Product { 
                Id = productId, 
                Name = "MacBook Pro", 
                Code = "XYZ", 
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
