using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Auth.Dto;

public class OtpRequest
{
  [EmailAddress]
  public required string Email { get; set; }
  [MaxLength (6)] [MinLength(6)] 
  public required string Code { get; set; }
}