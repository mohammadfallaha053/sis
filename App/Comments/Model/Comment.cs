using LapisApi.App.Users.Model;
using LapisApi.Data.Models;
namespace LapisApi.App.Comments.Model
{
  public class Comment
  {
    public int Id { get; set; }
    public required string Text { get; set; }

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsAccepted { get; set; } = false;

    public required string UserId { get; set; }
    public ApplicationUser User { get; set; }
  }
}