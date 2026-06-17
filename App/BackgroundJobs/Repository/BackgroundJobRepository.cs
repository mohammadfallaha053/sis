using GenericRepository.Repositories;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.BackgroundJobs.Model;
using LapisApi.Data;
namespace LapisApi.App.BackgroundJobs.Repository;

public class BackgroundJobRepository : GenericRepository<BackgroundJob>, IBackgroundJobRepository
{
  public BackgroundJobRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}