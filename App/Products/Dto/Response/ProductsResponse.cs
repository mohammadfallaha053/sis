using LapisApi.App.MediaFiles.Dto;

namespace SisApi.App.Products.Dto.Response;

public class ProductsResponse
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public int CategoryId { get; set; }
  public string CategoryName { get; set; } = string.Empty;
  public decimal PointsPrice { get; set; }
  public int StockQuantity { get; set; }
  public bool IsActive { get; set; }
  public DateTime CreatedAt { get; set; }
  public FileResponse? Image { get; set; }
}
