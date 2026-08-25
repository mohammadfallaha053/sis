using GenericRepository.Repositories;
using SisApi.App.Categories.Interfaces;
using SisApi.App.Categories.Model;
using SisApi.Data;

namespace SisApi.App.Categories.Repository;

public class CategoriesRepository : GenericRepository<Category>, ICategoriesRepository
{
  public CategoriesRepository(ApplicationDbContext context) : base(context)
  {
  }
}
