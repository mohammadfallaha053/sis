using LapisApi.App.MediaFiles.Dto;
namespace SisApi.App.Users.Dto.Response;

public class UserBaseResponse
{
  public string Id { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string FirstName { get; set; } = default!;
  public string LastName { get; set; } = default!;
  public FileResponse? Image { get; set; }

  public string? City { get; set; }
}