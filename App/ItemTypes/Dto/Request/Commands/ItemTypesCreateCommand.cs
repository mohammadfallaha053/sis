using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.ItemTypes.Dto.Request.Commands;

public class ItemTypesCreateCommand
{
  public string Name{ get; set; } 
  public int PointsPerKg { get; set; }
  public int? FileId { get; set; }
}