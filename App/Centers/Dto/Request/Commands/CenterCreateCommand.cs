using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Centers.Dto.Request.Commands;

public class CenterCreateCommand
{
  [Required]
  public string NameAr { get; set; }

  [Required]
  public string NameEn { get; set; }
  
  [Required]
  public int RegionId { get; set; }

  [Required]
  public string LocationAr { get; set; }

  [Required]
  public string LocationEn { get; set; }

  [Required]
  public double Lat { get; set; }

  [Required]
  public double Long { get; set; }

  [Required]
  public bool IsActive { get; set; }
  
  public string? ManagerId { get; set; }
}