using Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Entity.Services
{
    public class ProductService : EntityService<Product>
    {
        public ProductService(
            PocDbContext context, 
            ILogger<EntityService<Product>> logger) : base(context, logger)
        { }

        public override async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await Context.Products
                .Include(p => p.ProductJunctures)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product;
        }
    }
}
