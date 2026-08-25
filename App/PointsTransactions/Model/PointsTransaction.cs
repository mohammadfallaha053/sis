using SisApi.App.Orders.Model;
using SisApi.App.PointsTransactions.Enums;
using SisApi.App.Products.Model;
using SisApi.App.Users.Model;

namespace SisApi.App.PointsTransactions.Model;

public class PointsTransaction
{
  public int Id { get; set; }

  public required string ClientId { get; set; }
  public ApplicationUser Client { get; set; } = null!;

  public PointsTransactionTypeEnum Type { get; set; }

  // Always stored as a positive magnitude.
  // Type determines whether it was added or deducted.
  public decimal Points { get; set; }

  public decimal BalanceBefore { get; set; }

  public decimal BalanceAfter { get; set; }

  public int? OrderId { get; set; }
  public Order? Order { get; set; }

  public int? ProductId { get; set; }
  public Product? Product { get; set; }

  public int? Quantity { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
