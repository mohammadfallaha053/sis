using LapisApi.App.Regions.Enums;
namespace SisApi.App.Regions.Dto.Request.Queries;

public class RegionGetAllQuery
{
  public string? Search { get; set; }

  public bool? IsActive { get; set; }

  public int? CenterId { get; set; }
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public SortRequest<RegionSortFieldEnum>? Sort { get; set; }
}