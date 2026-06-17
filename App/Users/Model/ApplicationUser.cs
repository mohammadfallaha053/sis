using Microsoft.AspNetCore.Identity;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Centers.Model;
namespace LapisApi.App.Users.Model
{
  public class ApplicationUser : IdentityUser
  {
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime CreatedAt { get; set; }

    public string? City { get; set; } = null;
    public string? CenterId { get; set; } = null;
    public Center Center { get; set; }
    public required bool IsActive { get; set; } = true;
    public string Language { get; set; } = "en";
    public required RoleEnum Role { get; set; }
  }
}