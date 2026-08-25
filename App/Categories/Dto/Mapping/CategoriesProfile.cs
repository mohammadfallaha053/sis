using AutoMapper;
using SisApi.App.Categories.Dto.Request.Commands;
using SisApi.App.Categories.Dto.Response;
using SisApi.App.Categories.Model;

namespace SisApi.App.Categories.Dto.Mapping;

public class CategoriesProfile : Profile
{
  public CategoriesProfile()
  {
    CreateMap<CategoriesCreateCommand, Category>();
    CreateMap<CategoriesUpdateCommand, Category>();
    CreateMap<Category, CategoriesResponse>();
  }
}
