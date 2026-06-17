using AutoMapper;
using LapisApi.App.Settings.Dto.Commands;
using LapisApi.App.Settings.Dto.Response;
using LapisApi.App.Settings.Model;
namespace LapisApi.App.Settings.Dto.Mapping;

public class SettingProfile : Profile
{
  public SettingProfile()
  {
    CreateMap<SettingUpdateCommand, Setting>();

    CreateMap<Setting, SettingResponse>();

    CreateMap<AdsUpdateCommand, Setting>();

    CreateMap<Setting, AdsResponse>();

    CreateMap<Setting, AdsClientResponse>();
  }
}