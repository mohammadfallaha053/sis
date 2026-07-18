using GenericRepository.Repositories;
using LapisApi.App.Regions.Interfaces;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Regions.Model;
using SisApi.Data;
namespace SisApi.App.Regions.Repository;

public class RegionRepository : GenericRepository<Region>, IRegionRepository
{
  public RegionRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}