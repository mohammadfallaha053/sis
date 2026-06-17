using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Regions.Dto;

public class RegionCreateCommand
{
  [Required]
  public required string NameAr { get; set; }

  [Required]
  public required string NameEn { get; set; }

  [Required]
  public bool IsActive { get; set; }

  [Required]
  public required int CityId { get; set; }
}