using AutoMapper;
using SisApi.App.Products.Dto.Request.Commands;
using SisApi.App.Products.Dto.Response;
using SisApi.App.Products.Model;

namespace SisApi.App.Products.Dto.Mapping;

public class ProductsProfile : Profile
{
  public ProductsProfile()
  {
    CreateMap<ProductsCreateCommand, Product>();
    CreateMap<ProductsUpdateCommand, Product>();

    CreateMap<Product, ProductsResponse>()
      .ForMember(
        destination => destination.CategoryName,
        options => options.MapFrom(source => source.Category.Name)
      );
  }
}
