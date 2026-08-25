using GenericRepository.Interfaces;
using GenericRepository.Repositories;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.Regions.Interfaces;
using LapisApi.App.Settings.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using SisApi.App.BackgroundJobs.Repository;
using SisApi.App.Categories.Interfaces;
using SisApi.App.Categories.Repository;
using SisApi.App.Centers.Interfaces;
using SisApi.App.Centers.Repository;
using SisApi.App.ItemTypes.Interfaces;
using SisApi.App.ItemTypes.Repository;
using SisApi.App.Orders.Interfaces;
using SisApi.App.Orders.Repository;
using SisApi.App.PointsTransactions.Interfaces;
using SisApi.App.PointsTransactions.Repository;
using SisApi.App.Products.Interfaces;
using SisApi.App.Products.Repository;
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
    BackgroundJobs = new BackgroundJobRepository(_context);
    ItemTypes = new ItemTypesRepository(_context);
    Orders = new OrdersRepository(_context);
    Categories = new CategoriesRepository(_context);
    Products = new ProductsRepository(_context);
    PointsTransactions = new PointsTransactionsRepository(_context);
  }

  public ICentersRepository Centers { get; private set; }
  public IRegionRepository Regions { get; private set; }
  public ISettingRepository Settings { get; }
  public IGenericRepository<ApplicationUser> Users { get; private set; }
  
  public IItemTypesRepository ItemTypes { get; }

  public IBackgroundJobRepository BackgroundJobs { get; }
  
  public IOrdersRepository Orders { get; }
  
  public ICategoriesRepository Categories { get; }
  public  IProductsRepository Products { get; }
  public IPointsTransactionsRepository PointsTransactions { get; }


  
  public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

  public void Dispose() => _context.Dispose();
  
  public async Task<IDbContextTransaction> BeginTransactionAsync()
  {
    return await _context.Database.BeginTransactionAsync();
  }
}