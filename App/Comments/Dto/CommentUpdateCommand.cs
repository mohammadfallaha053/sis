using System.ComponentModel.DataAnnotations;

namespace LapisApi.App.Comments.Dto;

public class CommentUpdateCommand
{
  public bool IsAccepted { get; set; } = true;
}