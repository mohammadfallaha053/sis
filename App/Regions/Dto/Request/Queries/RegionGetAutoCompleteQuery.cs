namespace LapisApi.App.Regions.Dto;

public class RegionGetAutoCompleteQuery
{
  public string? Search { get; set; }
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  
  public int? CityId { get; set; }
  
}