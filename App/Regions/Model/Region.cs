using System.ComponentModel.DataAnnotations;
using LapisApi.App.Centers.Model;
using LapisApi.App.Cities.Model;
namespace LapisApi.Data.Models
{
  public class Region
  {
    [Key]
    public int Id { get; set; }

    public required string NameAr { get; set; }
    public required string NameEn { get; set; }

    public bool IsActive { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
    public ICollection<Center> Centers { get; set; } = new List<Center>(); 
  }
}