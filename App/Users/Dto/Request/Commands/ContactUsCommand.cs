using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Users.Dto.Request.Commands;

public class ContactUsCommand
{
  [Required]
  public string Email { get; set; }

  [Required]
  public string FullName { get; set; }

  [Required]
  public string PhoneNumber { get; set; }

  public string? JobType { get; set; } = null;

  [Required]
  public string Message { get; set; }
  
  [Required]
  public bool IsAgent { get; set; }
}