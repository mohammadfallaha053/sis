using GenericRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LapisApi.Data;
using LapisApi.Repositories.Helpers;

namespace GenericRepository.Repositories
{
  public class GenericRepository<T> : IGenericRepository<T> where T : class
  {
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
      _context = context;
      _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
      await _dbSet.AddAsync(entity);
    }

    public async Task<T?> GetByIdAsync(object id)
    {
      return await _dbSet.FindAsync(id);
    }

    public async Task<T?> GetFirstOrDefaultAsync(
      Expression<Func<T, bool>> predicate,
      Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null
    )
    {
      IQueryable<T> query = _dbSet.AsQueryable();

      if (predicate != null)
        query = query.Where(predicate);

      if (queryBuilder != null)
        query = queryBuilder(query);

      return await query.FirstOrDefaultAsync();
    }


    public async Task<IEnumerable<T>> GetAllAsync()
    {
      return await _dbSet.ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(
      Expression<Func<T, bool>>? predicate = null,
      Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null
    )
    {
      IQueryable<T> query = _dbSet.AsQueryable();

      if (predicate != null)
        query = query.Where(predicate);

      if (queryBuilder != null)
        query = queryBuilder(query);

      return await query.ToListAsync();
    }

    public async Task<PagedResult<T>> GetPagedAsync(
      Expression<Func<T, bool>>? predicate = null,
      int pageNumber = 1,
      int pageSize = 10,
      Func<IQueryable<T>, IOrderedQueryable<T>>? sort = null,
      Func<IQueryable<T>, IQueryable<T>>? queryBuilder = null
    )
    {
      IQueryable<T> query = _dbSet.AsQueryable();

      if (predicate != null)
        query = query.Where(predicate);

      if (queryBuilder != null)
        query = queryBuilder(query);

      if (sort != null)
        query = sort(query);

      var totalRecords = await query.CountAsync();

      var pagedData = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

      return new PagedResult<T>(pagedData, totalRecords);
    }

    public Task UpdateAsync(T entity)
    {
      _context.Entry(entity).State = EntityState.Modified;
      return Task.CompletedTask;
    }

    public Task RemoveAsync(T entity)
    {
      _dbSet.Remove(entity);
      return Task.CompletedTask;
    }

    public Task RemoveRange(IEnumerable<T> entities)
    {
      _dbSet.RemoveRange(entities);
      return Task.CompletedTask;
    }
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
      if (predicate == null)
      {
        return await _dbSet.CountAsync();
      }

      return await _dbSet.CountAsync(predicate);
    }
  }
}


public class SortRequest<TField> where TField : Enum
{
  public TField? Field { get; set; }
  public bool Descending { get; set; } = false;
}