using SisApi.App.Centers.Model;
using SisApi.Shared.Infos;
using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Regions.Model
{
  public class Region
  {
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }
    public double Lat { get; set; }
    public double Long { get; set; }

    public List<VisitInfo> Visits { get; set; } = [];

    public bool IsActive { get; set; }

    public int CenterId { get; set; }
    public Center Center { get; set; }
  }
}