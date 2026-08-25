using SisApi.App.Categories.Model;

namespace SisApi.App.Products.Model;

public class Product
{
  public int Id { get; set; }

  public required string Name { get; set; }

  public string? Description { get; set; }

  public int CategoryId { get; set; }
  public Category Category { get; set; } = null!;

  public decimal PointsPrice { get; set; }

  public int StockQuantity { get; set; }

  public bool IsActive { get; set; } = true;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
