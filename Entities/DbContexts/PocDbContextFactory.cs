using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Entities
{
    public class PocDbContextFactory : IDesignTimeDbContextFactory<PocDbContext>
    {
        public PocDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PocDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=PocDbContext;Integrated Security=True;MultipleActiveResultSets=True");

            return new PocDbContext(optionsBuilder.Options);
        }
    }
}
