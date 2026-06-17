using LapisApi.App.Cities.Dto;
namespace LapisApi.App.Regions.Dto;

public class RegionResponse
{
  public int Id { get; set; }
  public string NameAr { get; set; }
  public string NameEn { get; set; }

  public int CityId { get; set; }
  
  public bool IsActive { get; set; }

  public CityBaseResponse City { get; set; }
}