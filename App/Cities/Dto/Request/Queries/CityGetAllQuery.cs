using LapisApi.MyEnum.RegionSort;
namespace SisApi.App.Cities.Dto.Request.Queries;

public class CityGetAllQuery
{
  public string? Search { get; set; }
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public SortRequest<CitySortField>? Sort { get; set;}
  public bool? IsActive  { get; set; }
}