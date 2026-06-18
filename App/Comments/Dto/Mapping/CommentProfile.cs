using AutoMapper;
using LapisApi.App.Comments.Dto.Request.Commands;
using LapisApi.App.Comments.Dto.Response;
using LapisApi.App.Comments.Model;
namespace LapisApi.App.Comments.Dto.Mapping;

public class CommentProfile : Profile
{
  public CommentProfile()
  {
    CreateMap<CommentCreateCommand, Comment>();
    CreateMap<CommentUpdateCommand, Comment>();

    CreateMap<Comment, CommentResponse>()
      .ForMember(
        dest => dest.ClientName, opt =>
          opt.MapFrom(src => src.User.FirstName+ " " + src.User.LastName)
      );
  }
}