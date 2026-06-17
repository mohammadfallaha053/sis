namespace LapisApi.App.Settings.Model
{
  public class Setting
  {
    public int Id { get; set; }
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

    public string? Text { get; set; } = null;
    public bool isExternal { get; set; } = false;
    public string? Url { get; set; } = null;
    public bool IsActive { get; set; } = false;
  }
}