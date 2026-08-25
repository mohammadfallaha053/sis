using GenericRepository.Interfaces;
using SisApi.App.Orders.Model;
namespace SisApi.App.Orders.Interfaces
{
  public interface IOrdersRepository : IGenericRepository<Order>
  {
  }
}