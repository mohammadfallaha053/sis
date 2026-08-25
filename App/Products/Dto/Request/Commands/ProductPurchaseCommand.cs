using System.ComponentModel.DataAnnotations;

namespace SisApi.App.Products.Dto.Request.Commands;

public class ProductPurchaseCommand
{
  [Range(1, int.MaxValue)]
  public int Quantity { get; set; } = 1;
}
