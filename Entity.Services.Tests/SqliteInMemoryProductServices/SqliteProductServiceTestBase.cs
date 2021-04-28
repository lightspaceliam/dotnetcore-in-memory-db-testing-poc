using Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Entity.Services.Tests.SqliteInMemoryProductServices
{
    public abstract class SqliteProductServiceTestBase : IDisposable
    {
        protected readonly IEntityService<Product> Service;
        protected readonly DateTime UtcNow = DateTime.UtcNow;
        protected readonly PocDbContext Context;
        private readonly DbContextOptions _options;
        private readonly SqliteConnection _connection;

        [DbFunction]
        private static Guid NewId() => Guid.NewGuid();

        public SqliteProductServiceTestBase()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            //  TODO: Work out how to resolve this. Identity/autoincrement does not translate from MSSQL to SQLite...
            //  For Primary keys that are Guid's this will do the job!
            _connection.CreateFunction("newid", NewId);

            _options = new DbContextOptionsBuilder<PocDbContext>()
                .EnableSensitiveDataLogging()
                .UseSqlite(_connection)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .Options;

            Context = new PocDbContext(_options);

            Context.Database.EnsureDeleted();
            Context.Database.Migrate();
            Context.Database.EnsureCreated();

            var services = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            Service = new ProductService(Context, services.GetRequiredService<ILogger<EntityService<Product>>>());
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Dispose();
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
