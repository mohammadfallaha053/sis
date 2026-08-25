using SisApi.App.Categories.Enums;

namespace SisApi.App.Categories.Dto.Request.Queries;

public class CategoriesGetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Search { get; set; }
  public bool? IsActive { get; set; }
  public SortRequest<CategoriesSortFieldEnum>? Sort { get; set; }
}
