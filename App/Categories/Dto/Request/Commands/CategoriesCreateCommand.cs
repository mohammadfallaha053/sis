using System.ComponentModel.DataAnnotations;

namespace SisApi.App.Categories.Dto.Request.Commands;

public class CategoriesCreateCommand
{
  [Required]
  [MaxLength(200)]
  public required string Name { get; set; }

  public int? FileId { get; set; }
}
