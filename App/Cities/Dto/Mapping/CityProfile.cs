using AutoMapper;
using LapisApi.App.Cities.Dto;
using SisApi.App.Cities.Dto.Request.Commands;
using SisApi.App.Cities.Model;
namespace SisApi.App.Cities.Dto.Mapping;

public class CityProfile : Profile
{
  public CityProfile()
  {
    CreateMap<City, CityBaseResponse>();

    CreateMap<CityCreateCommand, City>();
    CreateMap<City, CityResponse>();

      
    CreateMap<City, CityAutoCompleteResponse>();
    
    CreateMap<UpdateCityCommand, City>();
    
  }
}