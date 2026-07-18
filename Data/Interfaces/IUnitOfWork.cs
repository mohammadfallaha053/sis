using GenericRepository.Interfaces;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.Comments.Interfaces;
using LapisApi.App.Regions.Interfaces;
using LapisApi.App.Settings.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using SisApi.App.Centers.Interfaces;
using SisApi.App.Users.Model;
namespace SisApi.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
  IRegionRepository Regions { get; }
  ICentersRepository Centers { get; }
  ISettingRepository Settings { get; }
  IGenericRepository<ApplicationUser> Users { get; }
  
  ICommentRepository Comments { get; }
  
  IBackgroundJobRepository BackgroundJobs { get; }
  
  Task<int> SaveChangesAsync();
  
  Task<IDbContextTransaction> BeginTransactionAsync();
}