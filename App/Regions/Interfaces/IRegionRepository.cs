using GenericRepository.Interfaces;
using LapisApi.Data.Models;
namespace LapisApi.App.Regions.Interfaces
{
  public interface IRegionRepository : IGenericRepository<Region>
  {
    Task<IEnumerable<Region>> GetRegionsWithNameContainingAsync(string letter);
  }
}