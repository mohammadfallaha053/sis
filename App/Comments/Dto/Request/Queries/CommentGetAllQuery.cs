using LapisApi.App.Comments.Enums;
namespace LapisApi.App.Comments.Dto.Request.Queries;

public class CommentGetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Search { get; set; }
  public bool? IsAccepted { get; set; }
  public SortRequest<CommentSortFieldEnum>? Sort { get; set; }
}