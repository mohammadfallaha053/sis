using LapisApi.App.ItemTypes.Enums;
namespace SisApi.App.ItemTypes.Dto.Request.Queries;

public class ItemTypesGetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Search { get; set; }
  public bool? IsActive { get; set; }
  public SortRequest<ItemTypesSortFieldEnum>? Sort { get; set; }
}