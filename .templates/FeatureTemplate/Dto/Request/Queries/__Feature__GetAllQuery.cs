using TransfersApi.App.__Feature__s.Enums;
namespace TransfersApi.App.__Feature__s.Dto.Request.Queries;

public class __Feature__GetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string? Search { get; set; }
  public bool? IsActive { get; set; }
  public int? CountryId { get; set; }
  public SortRequest<__Feature__SortFieldEnum>? Sort { get; set; }
}