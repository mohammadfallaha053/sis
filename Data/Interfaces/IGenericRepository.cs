using System.Linq.Expressions;
using LapisApi.Repositories.Helpers;

namespace GenericRepository.Interfaces
{
  public interface IGenericRepository<T> where T : class
  {
    Task<T?> GetByIdAsync(object id);

    Task<T?> GetFirstOrDefaultAsync(
      Expression<Func<T, bool>> predicate,
      Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null
    );
    Task<IEnumerable<T>> GetAllAsync();

    Task<List<T>> GetAllAsync(
      Expression<Func<T, bool>>? predicate = null,
      Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null
    );

    Task<PagedResult<T>> GetPagedAsync(
      Expression<Func<T, bool>>? predicate = null,
      int pageNumber = 1,
      int pageSize = 10,
      Func<IQueryable<T>, IOrderedQueryable<T>>? sort = null,
      Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null
    );

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task RemoveAsync(T entity);
    Task RemoveRange(IEnumerable<T> entities);

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
  }
}