using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Users.Dto.Request.Commands;

public class CreateUserRequest
{
  [Required]
  public string Email { get; set; } = string.Empty;
}