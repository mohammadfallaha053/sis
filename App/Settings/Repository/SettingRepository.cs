using GenericRepository.Repositories;
using LapisApi.App.Settings.Interfaces;
using SisApi.App.Settings.Model;
using SisApi.Data;
namespace SisApi.App.Settings.Repository;

public class SettingRepository : GenericRepository<Setting>, ISettingRepository
{
  public SettingRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}