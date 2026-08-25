namespace SisApi.App.Orders.Dto.Response;

public class OrderItemResponse
{
  public int Id { get; set; }

  public int ItemTypeId { get; set; }

  public string ItemTypeName { get; set; } = string.Empty;
  
  public decimal? WeightKg { get; set; }

  public int PointsPerKg { get; set; }

  public decimal Points { get; set; }
}