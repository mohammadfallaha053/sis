using LapisApi.App.Cities.Dto;
using SisApi.App.Regions.Dto;
namespace SisApi.App.Centers.Dto.Response;

public class CenterGetForClientResponse
{
 
  public string NameAr { get; set; }
  public string NameEn { get; set; }

  public string Phone { get; set; }
  public string Email { get; set; }
  public string LocationAr { get; set; }
  public string LocationEn { get; set; }

  public double Lat { get; set; }
  public double Long { get; set; }

  public RegionBaseResponse? Region { get; set; }
  public CityBaseResponse? City { get; set; }

  public string? ImageUrl { get; set; }
}