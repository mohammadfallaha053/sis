namespace SisApi.App.Cities.Dto.Request.Queries;

public class CityGetAutoCompleteQuery
{
  public string? Search { get; set; }
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  
}