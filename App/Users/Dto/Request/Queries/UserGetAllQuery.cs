using LapisApi.App.Users.Enums;
using SisApi.App.Auth.Enums;
namespace SisApi.App.Users.Dto.Request.Queries;

public class UserGetAllQuery
{
  public string? Search { get; set; }
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public SortRequest<UserSortField>? Sort { get; set; }
  public bool? IsActive { get; set; } = null;
  
  public RoleEnum? Role { get; set; }
}

