using AutoMapper;
using LapisApi.App.Users.Dto.Response;
using LapisApi.App.Users.Model;
namespace LapisApi.App.Users.Dto.Mapping;

public class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<ApplicationUser, UserBaseResponse>();
  }
}