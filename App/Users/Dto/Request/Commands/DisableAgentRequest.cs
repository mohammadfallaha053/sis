using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Users.Dto.Request.Commands;

public class DisableAgentRequest
{
  [Required]
  public string UserId { get; set; } = string.Empty;
}