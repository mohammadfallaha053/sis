using GenericRepository.Interfaces;
using GenericRepository.Repositories;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.BackgroundJobs.Repository;
using LapisApi.App.Centers.Interfaces;
using LapisApi.App.Centers.Repository;
using LapisApi.App.Regions.Interfaces;
using LapisApi.App.Comments.Interfaces;
using LapisApi.App.Comments.Repository;
using LapisApi.App.Settings.Interfaces;
using LapisApi.App.Settings.Repository;
using LapisApi.App.Users.Model;
using LapisApi.Data;
using LapisApi.Data.Interfaces;
using LapisApi.Interfaces.Cities;
using SisApi.App.Cities.Repository;
namespace LapisApi.Repository.Generic;

public class UnitOfWork : IUnitOfWork
{
  private readonly ApplicationDbContext _context;

  public UnitOfWork(ApplicationDbContext context)
  {
    _context = context;
    Regions = new RegionRepository(_context);
    Users = new GenericRepository<ApplicationUser>(_context);
    Cities = new CityRepository(_context);
    Centers = new CenterRepository(_context);
    Settings = new SettingRepository(_context);
    Comments = new CommentRepository(_context);
    BackgroundJobs = new BackgroundJobRepository(_context);
  }

  public ICenterRepository Centers { get; private set; }
  public IRegionRepository Regions { get; private set; }
  public ISettingRepository Settings { get; }
  public ICityRepository Cities { get; private set; }
  public IGenericRepository<ApplicationUser> Users { get; private set; }

  public ICommentRepository Comments { get; private set; }

  public IBackgroundJobRepository BackgroundJobs { get; }
  
  public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

  public void Dispose() => _context.Dispose();
}