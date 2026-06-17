using LapisApi.App.MediaFiles.Dto;
namespace LapisApi.App.Auth.Dto
{
  public class AuthResponse
  {
    public string Id { get; set; }
    public bool EmailConfirmed { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresOn { get; set; }
    public string PhoneNumber { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public FileResponse? Image { get; set; }
  }
}