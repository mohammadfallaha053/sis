using LapisApi.App.MediaFiles.Dto;
namespace SisApi.App.Auth.Dto
{
  public class AuthResponse
  {
    public string Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresOn { get; set; }
    public string PhoneNumber { get; set; }
    public FileResponse? Image { get; set; }
    
    public int? RegionId { get; set; }
  }
}