namespace LapisApi.App.Cities.Dto;

public class CityBaseResponse
{
  public int Id { get; set; }
  public required string NameAr { get; set; }
  public required string NameEn { get; set; }
  public bool IsActive { get; set; }
}