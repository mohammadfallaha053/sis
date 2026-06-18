using AutoMapper;
using LapisApi.App.Centers.Dto;
using LapisApi.App.Centers.Dto.Request.Commands;
using LapisApi.App.Centers.Dto.Response;
using SisApi.App.Centers.Dto.Request.Commands;
using SisApi.App.Centers.Dto.Response;
using SisApi.App.Centers.Model;
namespace SisApi.App.Centers.Dto.Mapping;

public class CenterProfile : Profile
{
  public CenterProfile()
  {
    CreateMap<CenterCreateCommand, Center>();
    CreateMap<CenterUpdateCommand, Center>();
    CreateMap<CenterUpdateInfoCommand, Center>();

    CreateMap<Center, CenterBaseResponse>();

    CreateMap<Center, CenterResponse>();
    CreateMap<Center, CenterGetForClientResponse>()
      .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Region.City));

    CreateMap<Center, CenterInfoResponse>();
  }
}