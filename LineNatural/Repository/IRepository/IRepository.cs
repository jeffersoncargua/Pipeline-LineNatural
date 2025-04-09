using LineNatural.Entities;
using System.Linq.Expressions;

namespace LineNatural.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);

        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool traked = true, string? includeProperties = null);

        Task CreateAsync(T entity);

        Task RemoveAsync(T entity);

        Task SaveAsync();
    }
}
