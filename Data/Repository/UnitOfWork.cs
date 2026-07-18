using GenericRepository.Interfaces;
using GenericRepository.Repositories;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.Comments.Interfaces;
using LapisApi.App.Regions.Interfaces;
using LapisApi.App.Settings.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using SisApi.App.BackgroundJobs.Repository;
using SisApi.App.Centers.Interfaces;
using SisApi.App.Centers.Repository;
using SisApi.App.Comments.Repository;
using SisApi.App.Regions.Repository;
using SisApi.App.Settings.Repository;
using SisApi.App.Users.Model;
using SisApi.Data.Interfaces;
namespace SisApi.Data.Repository;

public class UnitOfWork : IUnitOfWork
{
  private readonly ApplicationDbContext _context;

  public UnitOfWork(ApplicationDbContext context)
  {
    _context = context;
    Regions = new RegionRepository(_context);
    Users = new GenericRepository<ApplicationUser>(_context);
    Centers = new CentersRepository(_context);
    Settings = new SettingRepository(_context);
    Comments = new CommentRepository(_context);
    BackgroundJobs = new BackgroundJobRepository(_context);
  }

  public ICentersRepository Centers { get; private set; }
  public IRegionRepository Regions { get; private set; }
  public ISettingRepository Settings { get; }
  public IGenericRepository<ApplicationUser> Users { get; private set; }

  public ICommentRepository Comments { get; private set; }

  public IBackgroundJobRepository BackgroundJobs { get; }
  
  public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

  public void Dispose() => _context.Dispose();
  
  public async Task<IDbContextTransaction> BeginTransactionAsync()
  {
    return await _context.Database.BeginTransactionAsync();
  }
}