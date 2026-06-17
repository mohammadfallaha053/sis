using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Centers.Dto;

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

  [Required]
  public bool IsCanAccept { get; set; }

  [Range(0, 1, ErrorMessage = "Commission must be between 0 and 1")]
  [Required]
  public decimal CommissionRate { get; set; }
}