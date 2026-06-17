using LapisApi.App.Comments.Dto;
using LapisApi.App.Comments.Dto.Request.Commands;
using LapisApi.App.Comments.Dto.Request.Queries;
using LapisApi.App.Comments.Dto.Response;
namespace LapisApi.App.Comments.Interfaces;

public interface ICommentService
{
  Task<Result<CommentResponse>> AddAsync(CommentCreateCommand dto);
  Task<Result<IEnumerable<CommentResponse>>> GetAllAsync(CommentGetAllQuery query);

  Task<Result<IEnumerable<CommentSliderResponse>>> GetSlider();
  Task<Result<CommentResponse>> GetByIdAsync(int id);
  Task<Result<CommentResponse>> UpdateAsync(int id, CommentUpdateCommand command);
  Task<Result<object>> DeleteAsync(int id);
}