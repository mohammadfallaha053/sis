using System.ComponentModel.DataAnnotations;

namespace SisApi.App.Orders.Dto.Request.Commands;

public class OrdersCancelCommand
{
  [Required]
  [MaxLength(500)]
  public required string Reason { get; set; }
}