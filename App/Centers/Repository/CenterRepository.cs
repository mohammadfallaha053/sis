using GenericRepository.Repositories;
using LapisApi.App.Centers.Interfaces;
using LapisApi.Data;
using SisApi.App.Centers.Model;
namespace LapisApi.App.Centers.Repository;

public class CenterRepository : GenericRepository<Center>, ICenterRepository
{
  public CenterRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}