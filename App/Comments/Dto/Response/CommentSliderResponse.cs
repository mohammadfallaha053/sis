using LapisApi.App.Users.Dto.Response;
namespace LapisApi.App.Comments.Dto.Response;

public class CommentSliderResponse
{
  public required string Text { get; set; }
  public required DateTime CreatedAt { get; set; }
  //public string UserId { get; set; }
  public UserBaseResponse User { get; set; }
}