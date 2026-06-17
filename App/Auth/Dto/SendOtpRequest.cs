using System.ComponentModel.DataAnnotations;
using LapisApi.App.Auth.Enums;
namespace LapisApi.App.Auth.Dto;

public class SendOtpRequest
{
  [Required]
  [EmailAddress]
  public string Email { get; set; }

  public OtpPurposeEnum Purpose { get; set; }
}