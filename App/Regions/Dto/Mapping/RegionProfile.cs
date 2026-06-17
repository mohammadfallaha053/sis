using AutoMapper;
using LapisApi.App.Regions.Dto;
using LapisApi.Data.Models;
namespace SisApi.App.Regions.Dto.Mapping;

public class RegionProfile : Profile
{
  public RegionProfile()
  {
    CreateMap<RegionCreateCommand, Region>();

    CreateMap<RegionUpdateCommand, Region>();

    CreateMap<Region, RegionBaseResponse>();

    CreateMap<Region, RegionResponse>();

    CreateMap<Region, RegionAutoCompleteResponse>();
  }
}