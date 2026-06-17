using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Users.Dto;

public class DisableAgentRequest
{
  [Required]
  public string Email { get; set; } = string.Empty;
}