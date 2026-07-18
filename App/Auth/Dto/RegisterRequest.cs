using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Auth.Dto
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
    [Required]
    public int RegionId { get; set; } 

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
  }
}