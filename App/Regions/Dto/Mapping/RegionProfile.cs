using AutoMapper;
using LapisApi.App.Regions.Dto;
using SisApi.App.Regions.Dto.Request.Commands;
using SisApi.App.Regions.Dto.Response;
using SisApi.App.Regions.Model;
namespace SisApi.App.Regions.Dto.Mapping;

public class RegionProfile : Profile
{
  public RegionProfile()
  {
    CreateMap<RegionCreateCommand, Region>();

    CreateMap<RegionUpdateCommand, Region>();

    CreateMap<Region, RegionResponse>();
  }
}