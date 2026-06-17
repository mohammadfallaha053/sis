using System.ComponentModel.DataAnnotations;
namespace LapisApi.App.Comments.Dto.Request.Commands;

public class CommentCreateCommand
{
  [Required]
  public required string Text { get; set; }
}