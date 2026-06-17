using AutoMapper;
using TransfersApi.App.__Feature__s.Dto.Request.Commands;
using TransfersApi.App.__Feature__s.Dto.Response;
using TransfersApi.App.__Feature__s.Model;
using TransfersApi.Data.Models;
using TransfersApi.Dto.City;
namespace TransfersApi.App.__Feature__s.Dto.Mapping;

public class __Feature__Profile : Profile
{
  public __Feature__Profile()
  {
    CreateMap<__Feature__CreateCommand, __Feature__>();
    CreateMap<__Feature__UpdateCommand, __Feature__>();

    CreateMap<__Feature__, __Feature__Response>()
      .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
      .ForMember(dest => dest.DiscountRate, opt => opt.MapFrom(src => src.DiscountRate * 100));


  }
}