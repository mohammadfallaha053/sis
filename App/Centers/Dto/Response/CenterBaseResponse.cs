using LapisApi.App.Centers.Enums;
using LapisApi.App.Regions.Dto;
using LapisApi.App.Cities.Dto;
using LapisApi.App.MediaFiles.Dto;
namespace LapisApi.App.Centers.Dto.Response;

public class CenterBaseResponse
{
  public string Id { get; set; }
  public string NameAr { get; set; }
  public string NameEn { get; set; }
  public FileResponse? Image { get; set; }
}