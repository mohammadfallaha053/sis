using AutoMapper;
using LapisApi.App.Cities.Dto.Request.Commands;
using LapisApi.App.Cities.Model;
using LapisApi.Data.Models;
namespace LapisApi.App.Cities.Dto.Mapping;

public class CityProfile : Profile
{
  public CityProfile()
  {
    CreateMap<City, CityBaseResponse>();

    CreateMap<CityCreateCommand, City>();
    CreateMap<City, CityResponse>()
      .ForMember(dest => dest.CommissionRate, opt => opt.MapFrom(src => src.CommissionRate * 100));
      
    CreateMap<City, CityAutoCompleteResponse>();
    
    CreateMap<UpdateCityCommand, City>();
    
  }
}