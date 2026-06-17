using System.ComponentModel.DataAnnotations;
using LapisApi.Data.Models;
namespace LapisApi.App.Cities.Model;

public class City
{
  [Key]
  public int Id { get; set; }

  public required string NameAr { get; set; }
  public required string NameEn { get; set; }

  public string? NotesAr { get; set; } = null;
  public string? NotesEn { get; set; } = null;
  public bool IsActive { get; set; }
  public bool IsAutomaticAcceptance { get; set; }
  public required decimal MaximumTransferLimit { get; set; }

  public decimal CommissionRate { get; set; }
  public ICollection<Region> Regions { get; set; } = new List<Region>();
}