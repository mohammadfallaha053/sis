using TransfersApi.App.Countries.Dto;
namespace TransfersApi.App.__Feature__s.Dto.Response;

public class __Feature__Response
{
  public int Id { get; set; }

  public required string Code { get; set; } 
      
  public decimal DiscountRate { get; set; } // 0.2 = 20% خصم

  public int? MaxUsageCount { get; set; } // عدد مرات الاستخدام المسموح به (null = غير محدود)

  public int UsedCount { get; set; } = 0; // عدد مرات الاستخدام حتى الآن
    
    
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }

  public bool IsActive { get; set; } = true;
  public string? Note { get; set; }
  public CountryBaseDto? Country { get; set; }
}