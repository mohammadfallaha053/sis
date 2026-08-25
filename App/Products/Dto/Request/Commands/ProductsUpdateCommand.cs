using System.ComponentModel.DataAnnotations;

namespace SisApi.App.Products.Dto.Request.Commands;

public class ProductsUpdateCommand
{
  [Required]
  [MaxLength(200)]
  public required string Name { get; set; }

  [MaxLength(1000)]
  public string? Description { get; set; }

  [Range(1, int.MaxValue)]
  public int CategoryId { get; set; }

  [Range(typeof(decimal), "0.01", "9999999999999999")]
  public decimal PointsPrice { get; set; }

  [Range(0, int.MaxValue)]
  public int StockQuantity { get; set; }

  public int? FileId { get; set; }

  public bool IsActive { get; set; }
}
