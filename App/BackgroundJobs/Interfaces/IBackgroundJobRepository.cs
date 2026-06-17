using GenericRepository.Interfaces;
using LapisApi.App.BackgroundJobs.Model;
using LapisApi.Data.Models;
namespace LapisApi.App.BackgroundJobs.Interfaces
{
  public interface IBackgroundJobRepository : IGenericRepository<BackgroundJob>
  {
  }
}