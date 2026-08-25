using GenericRepository.Repositories;
using SisApi.App.Products.Interfaces;
using SisApi.App.Products.Model;
using SisApi.Data;

namespace SisApi.App.Products.Repository;

public class ProductsRepository : GenericRepository<Product>, IProductsRepository
{
  public ProductsRepository(ApplicationDbContext context) : base(context)
  {
  }
}
