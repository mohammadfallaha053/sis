using LapisApi.App.Centers.Enums;
namespace LapisApi.App.Centers.Dto;

public class CenterGetAllQuery
{
  public string? Search { get; set; }

  public bool? IsActive { get; set; }

  public bool? IsCanAccept { get; set; }

  public int? RegionId { get; set; }
  
  public int? CityId { get; set; }
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public SortRequest<CenterSortFieldEnum>? Sort { get; set; }
}