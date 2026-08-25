using LapisApi.App.MediaFiles.Dto;

namespace SisApi.App.Categories.Dto.Response;

public class CategoriesResponse
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public bool IsActive { get; set; }
  public FileResponse? Image { get; set; }
}
