using GenericRepository.Repositories;
using SisApi.App.PointsTransactions.Interfaces;
using SisApi.App.PointsTransactions.Model;
using SisApi.Data;

namespace SisApi.App.PointsTransactions.Repository;

public class PointsTransactionsRepository :
  GenericRepository<PointsTransaction>,
  IPointsTransactionsRepository
{
  public PointsTransactionsRepository(
    ApplicationDbContext context
  ) : base(context)
  {
  }
}
