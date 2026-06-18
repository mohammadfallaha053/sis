using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Centers.Dto.Request.Commands;

public class CenterUpdateCommand
{
  [Required]
  public string NameAr { get; set; }

  [Required]
  public string NameEn { get; set; }

  [Required]
  public int RegionId { get; set; }

  [Required]
  public string LocationAr { get; set; }

  public string LocationEn { get; set; }

  [Required]
  public double Lat { get; set; }

  [Required]
  public double Long { get; set; }

  [Required]
  public bool IsActive { get; set; }
  
  [Required]
  public string ManagerId { get; set; }
  
}