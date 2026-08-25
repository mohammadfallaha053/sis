using SisApi.App.Orders.Enums;

namespace SisApi.App.Orders.Dto.Response;

public class OrdersResponse
{
  public int Id { get; set; }

  public OrderStatusEnum Status { get; set; }
  public OrderCancellationTypeEnum? CancellationType { get; set; }
  
  public string? CancellationReason { get; set; }

  public string ClientId { get; set; } = string.Empty;

  public int RegionId { get; set; }

  public int CenterId { get; set; }
  
  public string? EmployeeId { get; set; }

  public bool IsActive { get; set; }

  public DateTime CreatedAt { get; set; }
  public DateTime? CompletedAt { get; set; }
  public DateTime? CanceledAt { get; set; }

  public List<OrderItemResponse> OrderItems { get; set; } = [];
}