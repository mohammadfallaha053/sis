using GenericRepository.Repositories;
using SisApi.App.ItemTypes.Interfaces;
using SisApi.App.ItemTypes.Model;
using SisApi.Data;
namespace SisApi.App.ItemTypes.Repository;

public class ItemTypesRepository : GenericRepository<ItemType>, IItemTypesRepository
{
  public ItemTypesRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}