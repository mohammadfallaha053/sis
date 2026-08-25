namespace SisApi.App.Users.Dto.Request.Commands;

public class UpdateUserRequest
{
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public string PhoneNumber { get; set; } = null!;
  public int? FileId { get; set; }  
  public double? Lat { get; set; } 
  public double? Long { get; set; } 
  public int? RegionId { get; set; } //RegionId
}