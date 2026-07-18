using GenericRepository.Repositories;
using SisApi.App.Centers.Interfaces;
using SisApi.App.Centers.Model;
using SisApi.Data;
namespace SisApi.App.Centers.Repository;

public class CentersRepository : GenericRepository<Center>, ICentersRepository
{
  public CentersRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}