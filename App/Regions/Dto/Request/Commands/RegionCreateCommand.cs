using SisApi.Shared.Infos;
using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Regions.Dto.Request.Commands;

public class RegionCreateCommand
{
  [Required]
  public required string Name { get; set; }
  
  public List<VisitInfo> Visits { get; set; } = [];
  
  [Required]
  public required int CenterId { get; set; }
}