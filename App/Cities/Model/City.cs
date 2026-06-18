using SisApi.App.Regions.Model;
using System.ComponentModel.DataAnnotations;
namespace SisApi.App.Cities.Model;

public class City
{
  [Key]
  public int Id { get; set; }

  public required string NameAr { get; set; }
  public required string NameEn { get; set; }
  
  public bool IsActive { get; set; }
  public bool IsAutomaticAcceptance { get; set; }
  public required decimal MaximumTransferLimit { get; set; }

  public decimal CommissionRate { get; set; }
  public ICollection<Region> Regions { get; set; } = new List<Region>();
}