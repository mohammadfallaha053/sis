using LapisApi.App.MediaFiles.Dto;
namespace SisApi.App.ItemTypes.Dto.Response;

public class ItemTypesResponse
{
  public int Id { get; set; }
  public string Name{ get; set; } 
  public int PointsPerKg { get; set; }
  public FileResponse? Image { get; set; }
  public bool IsActive { get; set; } = true;
}