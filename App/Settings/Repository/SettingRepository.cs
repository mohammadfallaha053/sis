using GenericRepository.Repositories;
using LapisApi.App.Settings.Interfaces;
using LapisApi.App.Settings.Model;
using LapisApi.Data;
namespace LapisApi.App.Settings.Repository;

public class SettingRepository : GenericRepository<Setting>, ISettingRepository
{
  public SettingRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}