using GenericRepository.Repositories;
using SisApi.App.Orders.Interfaces;
using SisApi.App.Orders.Model;
using SisApi.Data;
namespace SisApi.App.Orders.Repository;

public class OrdersRepository : GenericRepository<Order>, IOrdersRepository
{
  public OrdersRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}