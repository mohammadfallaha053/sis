using LapisApi.App.Centers.Enums;
using LapisApi.App.Regions.Dto;
using LapisApi.App.Cities.Dto;
using LapisApi.App.MediaFiles.Dto;
using SisApi.App.Regions.Dto;
namespace LapisApi.App.Centers.Dto.Response;

public class CenterResponse
{
  public string Id { get; set; }
  public string NameAr { get; set; }
  public string NameEn { get; set; }
  public string Phone { get; set; }
  
  public string Email { get; set; }
  
  public string LocationAr { get; set; }
  public string LocationEn { get; set; }
  public double Lat { get; set; }
  public double Long { get; set; }
  
  public bool IsActive { get; set; }
  
  public RegionBaseResponse? Region { get; set; }
  public CityBaseResponse? City { get; set; }
  
  public FileResponse? Image { get; set; }
}