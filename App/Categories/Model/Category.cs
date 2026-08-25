using SisApi.App.Products.Model;

namespace SisApi.App.Categories.Model;

public class Category
{
  public int Id { get; set; }

  public required string Name { get; set; }

  public bool IsActive { get; set; } = true;

  public List<Product> Products { get; set; } = [];
}
