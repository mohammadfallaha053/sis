namespace SisApi.App.Cities.Dto.Request.Commands;

public class UpdateCityCommand
{
  public string? NotesAr { get; set; } = null;
  public string? NotesEn { get; set; } = null;
  public bool IsActive { get; set; }
}