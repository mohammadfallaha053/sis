using Microsoft.Build.Framework;
using SisApi.Shared.Infos;
namespace SisApi.App.Regions.Dto.Request.Commands;

public class RegionUpdateCommand
{
  [Required]
  public string Name { get; set; }

  public List<VisitInfo> Visits { get; set; }
  [Required]
  public bool IsActive { get; set; }
}