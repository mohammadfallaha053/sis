using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Auth.Dto;

public class ResetPasswordRequest
{
  [Required]
  [EmailAddress]
  public string Email { get; set; }

  [Required]
  public string Code { get; set; }

  [Required]
  [StringLength(20, MinimumLength = 6)]
  public string NewPassword { get; set; }
}