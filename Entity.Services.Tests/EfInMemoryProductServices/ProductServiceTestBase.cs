using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Entity.Services.Tests.EfInMemoryProductServices
{
    public abstract class ProductServiceTestBase 
    {
        protected readonly IEntityService<Product> Service;
        protected readonly DateTime UtcNow = DateTime.UtcNow;
        protected readonly PocDbContext Context;
        private readonly DbContextOptions<PocDbContext> _options;

        public ProductServiceTestBase()
        {
            _options = new DbContextOptionsBuilder<PocDbContext>()
                .EnableSensitiveDataLogging()
                .UseInMemoryDatabase(databaseName: $"TESTING_{this.GetType().Name}_{DateTime.Now.Ticks}")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;

            var services = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            Context = new PocDbContext(_options);

            Service = new ProductService(new PocDbContext(_options), services.GetService<ILogger<EntityService<Product>>>());
        }

        protected void InitializeData(Product[] entities)
        {
            using var context = new PocDbContext(_options);
            foreach (var entity in entities)
            {
                context.Add(entity);
            }
            context.SaveChanges();
        }
    }
}
