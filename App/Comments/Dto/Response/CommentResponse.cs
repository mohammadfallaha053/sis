namespace LapisApi.App.Comments.Dto.Response;

public class CommentResponse
{
  public int Id { get; set; }
  public required string Text { get; set; }
  public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public bool IsAccepted { get; set; } = true;
  public required string UserId { get; set; }
  public string ClientName { get; set; }
}