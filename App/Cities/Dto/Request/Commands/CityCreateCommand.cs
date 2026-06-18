using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Cities.Dto.Request.Commands;

public class CityCreateCommand
{
  [Required]
  public string NameAr { get; set; }
  [Required]
  public string NameEn { get; set; }
  [Required]
  public bool IsActive { get; set; }
}