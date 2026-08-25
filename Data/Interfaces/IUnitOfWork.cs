using GenericRepository.Interfaces;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.Regions.Interfaces;
using LapisApi.App.Settings.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using SisApi.App.Categories.Interfaces;
using SisApi.App.Centers.Interfaces;
using SisApi.App.ItemTypes.Interfaces;
using SisApi.App.Orders.Interfaces;
using SisApi.App.PointsTransactions.Interfaces;
using SisApi.App.Products.Interfaces;
using SisApi.App.Users.Model;
namespace SisApi.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
  IRegionRepository Regions { get; }
  ICentersRepository Centers { get; }
  ISettingRepository Settings { get; }
  IGenericRepository<ApplicationUser> Users { get; }
  
  IItemTypesRepository ItemTypes { get; }
  
  IBackgroundJobRepository BackgroundJobs { get; }
  
  IOrdersRepository Orders { get; }
  
  ICategoriesRepository Categories { get; }
  IProductsRepository Products { get; }
  IPointsTransactionsRepository PointsTransactions { get; }
  
  Task<int> SaveChangesAsync();
  
  Task<IDbContextTransaction> BeginTransactionAsync();
}