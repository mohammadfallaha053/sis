namespace LapisApi.Dto.Client;

public class ClientRegisterResponse
{
  public string Id { get; set; }
  public bool EmailConfirmed { get; set; }
  public string Email { get; set; }
  public string Role { get; set; }
  public DateTime CreatedAt { get; set; }
  public string PhoneNumber { get; set; }

}