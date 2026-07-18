using LapisApi.App.Centers.Enums;
namespace SisApi.App.Centers.Dto.Request.Queries;

public class CentersGetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Search { get; set; }
  public bool? IsActive { get; set; }
  public SortRequest<CentersSortFieldEnum>? Sort { get; set; }
}