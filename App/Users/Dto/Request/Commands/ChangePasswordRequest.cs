namespace LapisApi.App.Users.Dto
{
  public class ChangePasswordRequest
  {
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
  }
}