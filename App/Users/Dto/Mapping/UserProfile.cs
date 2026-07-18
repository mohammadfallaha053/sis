using AutoMapper;
using SisApi.App.Users.Dto.Response;
using SisApi.App.Users.Model;
namespace SisApi.App.Users.Dto.Mapping;

public class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<ApplicationUser, UserResponse>();
    CreateMap<ApplicationUser, UserBaseResponse>();
  }
}