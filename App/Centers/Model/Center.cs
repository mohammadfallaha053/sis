using System.ComponentModel.DataAnnotations;
using LapisApi.App.Centers.Enums;
using LapisApi.App.Users.Model;
using LapisApi.Data.Models;
namespace LapisApi.App.Centers.Model
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
    public required bool IsCanAccept { get; set; }

    public string? TemporaryPaymentNotes { get; set; }
    public decimal? LastTemporaryPaymentAmount { get; set; } = null;

    public TemporaryPaymentStatusEnum LastTemporaryPaymentStatus { get; set; } = TemporaryPaymentStatusEnum.Accepted;

    [Range(0, 1)]
    public decimal CommissionRate { get; set; } = 0;

    public required int AgentsCount { get; set; } = 0;
    public int RegionId { get; set; }
    public Region Region { get; set; }
    public ICollection<ApplicationUser> Agents { get; set; } = new List<ApplicationUser>();
  }
}