using SisApi.App.PointsTransactions.Dto.Request.Queries;
using SisApi.App.PointsTransactions.Dto.Response;

namespace SisApi.App.PointsTransactions.Interfaces;

public interface IPointsTransactionsService
{
  Task<Result<IEnumerable<PointsTransactionResponse>>> GetAllAsync(
    PointsTransactionsGetAllQuery query
  );
}
