using System;
using System.Threading.Tasks;

namespace Entity.Services
{
    public interface IEntityService<T>
    {
        Task<T> InsertAsync(T entity);
        Task<T> GetByIdAsync(Guid id);
        Task<T> UpdateAsync(T entity);
    }
}
