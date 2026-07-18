using LapisApi.App.MediaFiles.Dto;
using SisApi.App.Centers.Dto.Response;
using SisApi.App.Regions.Dto.Response;
namespace SisApi.App.Users.Dto.Response;

public class UserResponse
{
  public string Id { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string FirstName { get; set; } = default!;
  public string LastName { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Role { get; set; } = default!;
  public CentersResponse? Center { get; set; }
  
  
  public int? RegionId { get; set; }
  public RegionResponse?  Region { get; set; } = default!;
  public bool IsActive { get; set; }
  public DateTime CreatedAt { get; set; }

  public FileResponse? Image { get; set; }
  
}