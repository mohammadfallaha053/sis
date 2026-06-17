namespace LapisApi.Repositories.Helpers;

public class PagedResult<T>
{
  public IEnumerable<T> Data { get; set; }
  public int TotalRecords { get; set; }

  public PagedResult(IEnumerable<T> data, int totalRecords)
  {
    Data = data;
    TotalRecords = totalRecords;
  }

}