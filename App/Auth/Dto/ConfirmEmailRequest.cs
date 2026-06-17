namespace LapisApi.App.Auth.Dto;

public class ConfirmEmailRequest
{
  public string Email { get; set; }
  public string Code { get; set; }
}