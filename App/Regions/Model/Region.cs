using SisApi.App.Centers.Model;
using SisApi.App.Cities.Model;
using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Regions.Model
{
  public class Region
  {
    [Key]
    public int Id { get; set; }
    public required string NameAr { get; set; }
    public required string NameEn { get; set; }
    
    public double Lat { get; set; }
    public double Long { get; set; }

    public bool IsActive { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
    public ICollection<Center> Centers { get; set; } = new List<Center>(); 
  }
}