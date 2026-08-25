using SisApi.App.PointsTransactions.Enums;

namespace SisApi.App.PointsTransactions.Dto.Response;

public class PointsTransactionResponse
{
  public int Id { get; set; }
  public string ClientId { get; set; } = string.Empty;
  public PointsTransactionTypeEnum Type { get; set; }
  public decimal Points { get; set; }
  public decimal BalanceBefore { get; set; }
  public decimal BalanceAfter { get; set; }
  public int? OrderId { get; set; }
  public int? ProductId { get; set; }
  public string? ProductName { get; set; }
  public int? Quantity { get; set; }
  public DateTime CreatedAt { get; set; }
}
