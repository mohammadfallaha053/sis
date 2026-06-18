using GenericRepository.Interfaces;
using SisApi.App.Regions.Model;
namespace LapisApi.App.Regions.Interfaces
{
  public interface IRegionRepository : IGenericRepository<Region>
  {
    Task<IEnumerable<Region>> GetRegionsWithNameContainingAsync(string letter);
  }
}