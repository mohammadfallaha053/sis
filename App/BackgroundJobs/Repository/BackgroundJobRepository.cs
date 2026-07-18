using GenericRepository.Repositories;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.BackgroundJobs.Model;
using SisApi.Data;
namespace SisApi.App.BackgroundJobs.Repository;

public class BackgroundJobRepository : GenericRepository<BackgroundJob>, IBackgroundJobRepository
{
  public BackgroundJobRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}