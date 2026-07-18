using Microsoft.AspNetCore.Identity;
using SisApi.App.Auth.Enums;
using SisApi.App.Centers.Model;
using SisApi.App.Regions.Model;
namespace SisApi.App.Users.Model
{
  public class ApplicationUser : IdentityUser
  {
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? CenterId { get; set; }

    // المركز الذي يعمل فيه المستخدم
    public Center? Center { get; set; }

    // المركز الذي يديره المستخدم
    public Center? ManagedCenter { get; set; }
    public required bool IsActive { get; set; } = true;
    public required RoleEnum Role { get; set; }
    public int? RegionId { get; set; }  
    public Region Region { get; set; }
  }
}