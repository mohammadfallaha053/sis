using LapisApi.App.MediaFiles.Dto;
namespace SisApi.App.Centers.Dto.Response;

public class CenterBaseResponse
{
  public string Id { get; set; }
  public string NameAr { get; set; }
  public string NameEn { get; set; }
  public FileResponse? Image { get; set; }
}