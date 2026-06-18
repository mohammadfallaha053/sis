using GenericRepository.Repositories;
using LapisApi.Data;
using LapisApi.Interfaces.Cities;
using SisApi.App.Cities.Model;
namespace SisApi.App.Cities.Repository;

public class CityRepository : GenericRepository<City>, ICityRepository
{
  public CityRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}