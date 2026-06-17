using Microsoft.Build.Framework;
namespace SisApi.App.Regions.Dto;

public class RegionUpdateCommand
{
  [Required]
  public string NameAr { get; set; }
  [Required]
  public string NameEn { get; set; }
  [Required]
  public bool IsActive { get; set; }
}