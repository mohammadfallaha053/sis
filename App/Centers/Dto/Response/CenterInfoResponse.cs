using LapisApi.App.Centers.Enums;
using LapisApi.App.Regions.Dto;
using LapisApi.App.Cities.Dto;
using LapisApi.App.MediaFiles.Dto;
using SisApi.App.Regions.Dto;
namespace LapisApi.App.Centers.Dto.Response;

public class CenterInfoResponse
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
  public int AgentsCount { get; set; }
  public bool IsActive { get; set; }
  public bool IsCanAccept { get; set; }
  public RegionBaseResponse? Region { get; set; }
  public decimal CommissionRate { get; set; }
  public CityBaseResponse? City { get; set; }

  public decimal? LastTemporaryPaymentAmount { get; set; }
  public TemporaryPaymentStatusEnum LastTemporaryPaymentStatus { get; set; }
  public string? TemporaryPaymentNotes { get; set; }
  public FileResponse? Image { get; set; }
}