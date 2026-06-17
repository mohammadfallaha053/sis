using LapisApi.App.MediaFiles.Dto;
namespace LapisApi.App.Settings.Dto.Response;

public class AdsResponse
{
  public string? Text { get; set; }
  public bool isExternal { get; set; }
  public string? Url { get; set; }
  public FileResponse? Video { get; set; }
  public bool IsActive { get; set; }
}