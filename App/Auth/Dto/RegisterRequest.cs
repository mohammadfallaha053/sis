using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Auth.Dto
{
  public class RegisterRequest
  {
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? City { get; set; } = null;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
  }
}