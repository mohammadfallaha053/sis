using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Cities.Dto.Request.Commands;

public class UpdateCityCommand
{
  public string? NotesAr { get; set; } = null;
  public string? NotesEn { get; set; } = null;

  public bool IsActive { get; set; }
  public bool IsAutomaticAcceptance { get; set; }
    
  public required decimal MaximumTransferLimit { get; set; }
  
  [Range(0, 1, ErrorMessage = "Commission must be between 0 and 1")]
  public decimal CommissionRate { get; set; }
}