using LapisApi.App.MediaFiles.Dto;
using SisApi.App.Users.Dto.Response;
namespace SisApi.App.Centers.Dto.Response;

public class CentersResponse
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Phone { get; set; }
  public string Location { get; set; }
  public UserBaseResponse? Manager { get; set; }
  
  public DateTime CreatedAt { get; set; }
  public bool IsActive { get; set; } = true;
}