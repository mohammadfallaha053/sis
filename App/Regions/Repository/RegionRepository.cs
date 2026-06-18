using GenericRepository.Interfaces;
using GenericRepository.Repositories;
using Microsoft.EntityFrameworkCore;
using LapisApi.App.Regions.Interfaces;
using LapisApi.Data;
using SisApi.App.Regions.Model;
namespace LapisApi.Repository;

public class RegionRepository : GenericRepository<Region>, IRegionRepository
{
  public RegionRepository(ApplicationDbContext context) : base(context)
  {
  }

  public async Task<IEnumerable<Region>> GetRegionsWithNameContainingAsync(string letter)
  {
    return await _context.Set<Region>()
      .Where(c => c.NameEn.ToLower().Contains(letter.ToLower()))
      .ToListAsync();
  }
}