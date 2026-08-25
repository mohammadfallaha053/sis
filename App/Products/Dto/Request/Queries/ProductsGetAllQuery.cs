using SisApi.App.Products.Enums;

namespace SisApi.App.Products.Dto.Request.Queries;

public class ProductsGetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Search { get; set; }
  public bool? IsActive { get; set; }
  public int? CategoryId { get; set; }
  public SortRequest<ProductsSortFieldEnum>? Sort { get; set; }
}
