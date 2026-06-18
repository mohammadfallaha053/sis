using GenericRepository.Interfaces;
using LapisApi.App.BackgroundJobs.Model;
namespace LapisApi.App.BackgroundJobs.Interfaces
{
  public interface IBackgroundJobRepository : IGenericRepository<BackgroundJob>
  {
  }
}