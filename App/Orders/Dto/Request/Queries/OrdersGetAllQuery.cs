using SisApi.App.Orders.Enums;
namespace SisApi.App.Orders.Dto.Request.Queries;

public class OrdersGetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Search { get; set; }
  public bool? IsActive { get; set; }
  // public SortRequest<OrdersSortFieldEnum>? Sort { get; set; }
  
  public OrderStatusEnum? Status { get; set; }

  public int? CenterId { get; set; }
}