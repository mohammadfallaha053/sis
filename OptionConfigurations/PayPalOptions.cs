namespace LapisApi.OptionConfigurations;

public class PayPalOptions
{
  public string ClientId { get; set; }
  public string ClientSecret { get; set; }
  public string Environment { get; set; }
  public string ReturnUrl { get; set; }
  public string CancelUrl { get; set; }
}