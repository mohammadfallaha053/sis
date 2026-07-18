using AutoMapper;
using SisApi.App.Centers.Dto.Request.Commands;
using SisApi.App.Centers.Dto.Response;
using SisApi.App.Centers.Model;

namespace SisApi.App.Centers.Dto.Mapping;

public class CentersProfile : Profile
{
  public CentersProfile()
  {
    CreateMap<CentersCreateCommand, Center>()
      .ForMember(
        destination => destination.ManagerId,
        options => options.Ignore()
      )
      .ForMember(
        destination => destination.Manager,
        options => options.Ignore()
      );

    CreateMap<CentersUpdateCommand, Center>()
      .ForMember(
        destination => destination.ManagerId,
        options => options.Ignore()
      )
      .ForMember(
        destination => destination.Manager,
        options => options.Ignore()
      );

    CreateMap<Center, CentersResponse>();
  }
}