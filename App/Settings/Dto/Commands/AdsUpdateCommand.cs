namespace LapisApi.App.Settings.Dto.Commands;

public class AdsUpdateCommand
{
  public string? Text { get; set; }
  public bool isExternal { get; set; }
  public string? Url { get; set; }
  public int? FileId { get; set; }

  public bool IsActive { get; set; }
}