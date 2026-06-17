using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Users.Dto;

public class CreateAgentRequest
{
  [Required]
  public string Email { get; set; } = string.Empty;
  
  [Required]
  public required string CenterId { get; set; }
}