using AutoMapper;
using LapisApi.App.BackgroundJobs.Dto.Request.Commands;
using LapisApi.App.BackgroundJobs.Model;
using LapisApi.Data.Models;
using SisApi.App.BackgroundJobs.Dto.Response;
namespace LapisApi.App.BackgroundJobs.Dto.Mapping;

public class BackgroundJobProfile : Profile
{
  public BackgroundJobProfile()
  {
    CreateMap<BackgroundJobCreateRequest, BackgroundJob>();
    CreateMap<BackgroundJobUpdateRequest, BackgroundJob>();
    CreateMap<BackgroundJob, BackgroundJobResponse>()
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
  }
}