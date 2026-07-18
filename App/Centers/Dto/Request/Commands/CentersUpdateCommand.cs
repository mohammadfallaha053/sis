namespace SisApi.App.Centers.Dto.Request.Commands;

public class CentersUpdateCommand
{
  public string Name { get; set; }
  public string Phone { get; set; }
  public string Location { get; set; }
  public string? ManagerId { get; set; }
  public bool IsActive { get; set; }
}