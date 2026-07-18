namespace SisApi.App.Users.Dto.Response;

public class UserBaseResponse
{
  public string Id { get; set; } = default!;
  public string Email { get; set; } = default!;
  public string FirstName { get; set; } = default!;
  public string LastName { get; set; } = default!;
  public string? PhoneNumber { get; set; }
  public string Role { get; set; } = default!;
  public bool IsActive { get; set; }
}