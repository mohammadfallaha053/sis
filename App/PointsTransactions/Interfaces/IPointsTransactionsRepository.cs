using GenericRepository.Interfaces;
using SisApi.App.PointsTransactions.Model;

namespace SisApi.App.PointsTransactions.Interfaces;

public interface IPointsTransactionsRepository :
  IGenericRepository<PointsTransaction>
{
}
