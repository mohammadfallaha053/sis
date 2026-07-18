using GenericRepository.Repositories;
using LapisApi.App.Comments.Interfaces;
using LapisApi.App.Comments.Model;
using SisApi.Data;
namespace SisApi.App.Comments.Repository;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
  public CommentRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}