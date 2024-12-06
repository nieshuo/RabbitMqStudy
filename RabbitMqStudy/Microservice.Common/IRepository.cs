using System.Linq.Expressions;

namespace Microservice.Common
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(long id);
        Task CancelAsync(long id);
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<T> GetAsync(long id);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
    }
}
