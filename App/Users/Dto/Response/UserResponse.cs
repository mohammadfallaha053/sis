using LapisApi.App.MediaFiles.Dto;
namespace LapisApi.App.Users.Dto.Response;

public class UserResponse
{
  public string Id { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string FirstName { get; set; } = default!;
  public string LastName { get; set; } = default!;
  public string PhoneNumber { get; set; } = default!;
  public string Role { get; set; } = default!;
  
  public bool IsActive { get; set; }
  public DateTime CreatedAt { get; set; }

  public FileResponse? Image { get; set; }
  
  
}