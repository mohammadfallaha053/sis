using AutoMapper;
using LapisApi.App.Centers.Dto.Request.Commands;
using LapisApi.App.Centers.Dto.Response;
using LapisApi.App.Centers.Model;
using LapisApi.Data.Models;
namespace LapisApi.App.Centers.Dto.Mapping;

public class CenterProfile : Profile
{
  public CenterProfile()
  {
    CreateMap<CenterCreateCommand, Center>();
    CreateMap<CenterUpdateCommand, Center>();
    CreateMap<CenterUpdateInfoCommand, Center>();

    CreateMap<Center, CenterBaseResponse>();

    CreateMap<Center, CenterResponse>()
      .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Region.City))
      .ForMember(dest => dest.CommissionRate, opt => opt.MapFrom(src => src.CommissionRate * 100));

    CreateMap<Center, CenterGetForClientResponse>()
      .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Region.City));

    CreateMap<Center, CenterInfoResponse>()
      .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Region.City))
      .ForMember(dest => dest.CommissionRate, opt => opt.MapFrom(src => src.CommissionRate * 100));
  }
}