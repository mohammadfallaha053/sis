namespace SisApi.App.ItemTypes.Dto.Request.Commands;

public class ItemTypesUpdateCommand
{
  public string Name{ get; set; } 
  public int PointsPerKg { get; set; }
  public int? FileId { get; set; }
  public bool IsActive { get; set; }
}