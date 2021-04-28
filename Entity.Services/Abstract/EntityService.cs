using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Entity.Services
{
    public abstract class EntityService<T> : IEntityService<T>
       where T : class, IEntity
    {
        protected readonly PocDbContext Context;
        protected readonly ILogger<EntityService<T>> Logger;

        protected EntityService(
            PocDbContext context,
            ILogger<EntityService<T>> logger)
        {
            Context = context;
            Logger = logger;
        }

        public virtual async Task<T> InsertAsync(T entity)
        {
            Context.Add(entity);
            await Context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await Context.Set<T>()
                .FirstOrDefaultAsync(e => e.Id == id);

            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            try
            {
                Context.Update(entity);
                await Context.SaveChangesAsync();

                return entity;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new DbUpdateConcurrencyException("Update failed because the record has been modified or deleted by another user.", ex);
            }
        }
    }
}
