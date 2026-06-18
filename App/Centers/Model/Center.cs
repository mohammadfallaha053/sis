using LapisApi.App.Users.Model;
using SisApi.App.Regions.Model;
using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Centers.Model
{
  public class Center
  {
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string NameAr { get; set; }
    public required string NameEn { get; set; }
    public required string Phone { get; set; } = string.Empty;

    public required string Email { get; set; } = string.Empty;

    public required string LocationAr { get; set; }
    public required string LocationEn { get; set; }
    public required double Lat { get; set; }
    public required double Long { get; set; }
    public required bool IsActive { get; set; }
    public int RegionId { get; set; }
    public Region Region { get; set; }
    
    public  string ManagerId { get; set; }
    public  ApplicationUser Manager { get; set; }
  }
}