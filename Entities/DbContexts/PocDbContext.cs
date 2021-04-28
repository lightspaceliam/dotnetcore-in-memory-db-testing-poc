using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class PocDbContext : DbContext
    {
        public  PocDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions) { }

        public PocDbContext() { }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductJuncture> ProductJunctures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            #region Default Values

            modelBuilder.Entity<Product>()
                .Property(e => e.Id)
                .HasDefaultValueSql("newid()");

            modelBuilder.Entity<ProductJuncture>()
                .Property(e => e.Id)
                .HasDefaultValueSql("newid()");

            #endregion

            #region Indicies

            /*
             * {Entity}.Name is inherited across all entities. EF Core 5 allows you to decorate an entity with an index however,
             * due to the property being inherited and overridden with NotMapped, it causes unexpected side effects: "An index cannot use unmapped properties".
             * Workaround/solution, configure individually as required in the Fluent API.
             */
            modelBuilder.Entity<Product>()
                .HasIndex(e => e.Name);

            #endregion
        }
    }
}
