namespace LapisApi.Helpers;
using System.Linq.Expressions;
public static class SortHelper
{
  public static Func<IQueryable<T>, IOrderedQueryable<T>>? BuildSort<T, TSortField>(
    SortRequest<TSortField>? sortRequest
  ) where TSortField : struct, Enum
  {
    if (sortRequest == null)
      return null;

    string propertyName = sortRequest.Field.ToString();
    var property = typeof(T).GetProperty(propertyName);

    if (property == null)
    {
      // Log warning if desired
      return null;
    }

    var parameter = Expression.Parameter(typeof(T), "x");
    var propertyAccess = Expression.Property(parameter, property);
    var converted = Expression.Convert(propertyAccess, typeof(object));
    var expression = Expression.Lambda<Func<T, object>>(converted, parameter);

    return query => sortRequest.Descending
      ? query.OrderByDescending(expression)
      : query.OrderBy(expression);
  }
}

