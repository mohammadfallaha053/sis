using GenericRepository.Interfaces;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.Centers.Interfaces;
using LapisApi.App.Regions.Interfaces;
using LapisApi.App.Comments.Interfaces;
using LapisApi.App.Settings.Interfaces;
using LapisApi.App.Users.Model;
using LapisApi.Interfaces.Cities;
namespace LapisApi.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
  IRegionRepository Regions { get; }
  ICenterRepository Centers { get; }
  ISettingRepository Settings { get; }
  ICityRepository Cities { get; }
  
  IGenericRepository<ApplicationUser> Users { get; }
  
  ICommentRepository Comments { get; }
  
  IBackgroundJobRepository BackgroundJobs { get; }
  
  Task<int> SaveChangesAsync();
}