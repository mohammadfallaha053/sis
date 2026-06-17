namespace LapisApi.OptionConfigurations;

public class EmailOptionSettings
{
  public string Server { get; set; }
  public int Port { get; set; }
  public string ClientName { get; set; }
  public string ClientEmail { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public bool EnableSsl { get; set; }

  public string SupportEmail { get; set; } = string.Empty;
}