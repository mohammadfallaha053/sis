namespace LapisApi.App.Settings.Dto.Commands;

public class SettingUpdateCommand
{
  public string? PhoneNumber { get; set; } 
  public string? Email { get; set; } 
  public string? Address { get; set; }
  
  public decimal PayPalCommissionAmount { get; set; }
  public decimal CreditCommissionAmount { get; set; }
  public decimal DebitCommissionAmount { get; set; }
    
  public string? AboutUs_Ar { get; set; } 
  public string? AboutUs_En { get; set; } 
    
  public string? ContactUs_Ar { get; set; } 
  public string? ContactUs_En { get; set; } 
    
  public string? FacebookUrl { get; set; }
  public string? InstagramUrl { get; set; }
  public string? TwitterUrl { get; set; }
  public string? YoutubeUrl { get; set; }
  public string? TiktokUrl { get; set; }
}