using SisApi.App.PointsTransactions.Enums;

namespace SisApi.App.PointsTransactions.Dto.Request.Queries;

public class PointsTransactionsGetAllQuery
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;

  // Admin only. ClientId is ignored for Client role.
  public string? ClientId { get; set; }

  public PointsTransactionTypeEnum? Type { get; set; }

  public DateTime? FromDate { get; set; }

  public DateTime? ToDate { get; set; }
}
