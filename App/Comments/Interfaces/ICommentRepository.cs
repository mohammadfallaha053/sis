using GenericRepository.Interfaces;
using LapisApi.App.Comments.Model;
namespace LapisApi.App.Comments.Interfaces
{
  public interface ICommentRepository : IGenericRepository<Comment>
  {
  }
}