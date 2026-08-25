namespace SisApi.App.Products.Dto.Response;

public class ProductPurchaseResponse
{
  public int ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public int Quantity { get; set; }
  public decimal PointsSpent { get; set; }
  public decimal RemainingPoints { get; set; }
  public int RemainingStock { get; set; }
}
