namespace SisApi.App.Users.Dto.Request.Commands
{
  public class ChangePasswordRequest
  {
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
  }
}