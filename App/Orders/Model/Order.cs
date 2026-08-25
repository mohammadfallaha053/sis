using SisApi.App.Centers.Model;
using SisApi.App.Orders.Enums;
using SisApi.App.Regions.Model;
using SisApi.App.Users.Model;

namespace SisApi.App.Orders.Model;

public class Order
{
  public int Id { get; set; }

  public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
  
  public OrderCancellationTypeEnum? CancellationType { get; set; }

  public string? CancellationReason { get; set; }

  public required string ClientId { get; set; }
  public ApplicationUser Client { get; set; } = null!;

  public int RegionId { get; set; }
  public Region Region { get; set; } = null!;

  public int CenterId { get; set; }
  public Center Center { get; set; } = null!;
  public string? EmployeeId { get; set; }
  public ApplicationUser? Employee { get; set; }

  public bool IsActive { get; set; } = true;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? CompletedAt { get; set; }
  public DateTime? CanceledAt { get; set; }
  public List<OrderItem> OrderItems { get; set; } = [];
}