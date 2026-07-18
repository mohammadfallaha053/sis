using SisApi.App.Centers.Dto.Response;
using SisApi.Shared.Infos;
namespace SisApi.App.Regions.Dto.Response;

public class RegionResponse
{
  public int Id { get; set; }
  public required string Name { get; set; }
  
  public double Lat { get; set; }
  public double Long { get; set; }

  public List<VisitInfo> Visits { get; set; }
  public required int CenterId { get; set; }
  
  public bool IsActive { get; set; }

  public CentersResponse Center { get; set; }
}