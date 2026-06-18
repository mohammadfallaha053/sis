using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Centers.Dto.Request.Commands;

public class CenterUpdateInfoCommand
{
  [Required]
  public required string Phone { get; set; }

  [Required]
  [EmailAddress]
  public required string Email { get; set; }

  public int? FileId { get; set; }
}