using GenericRepository.Repositories;
using LapisApi.App.Centers.Interfaces;
using LapisApi.App.Centers.Model;
using LapisApi.Data;
namespace LapisApi.App.Centers.Repository;

public class CenterRepository : GenericRepository<Center>, ICenterRepository
{
  public CenterRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}