using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Cities.Dto;

public class CityCreateCommand
{
  [Required]
  public string NameAr { get; set; }

  [Required]
  public string NameEn { get; set; }

  public string? NotesAr { get; set; } = null;
  public string? NotesEn { get; set; } = null;

  [Required]
  public bool IsActive { get; set; }

  [Required]
  public bool IsAutomaticAcceptance { get; set; }

  [Required]
  public required decimal MaximumTransferLimit { get; set; }
  
  [Range(0, 1, ErrorMessage = "Commission must be between 0 and 1")]
  public decimal CommissionRate { get; set; }
}