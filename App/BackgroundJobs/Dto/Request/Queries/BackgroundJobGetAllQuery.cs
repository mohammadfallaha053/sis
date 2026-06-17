using LapisApi.App.BackgroundJobs.Enums;
namespace LapisApi.App.BackgroundJobs.Dto;

public class BackgroundJobGetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Search { get; set; }
  public SortRequest<BackgroundJobSortFieldEnum>? Sort { get; set; }
}