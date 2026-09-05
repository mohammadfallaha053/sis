namespace SisApi.App.Statistics.Dto.Request.Queries;

public class StatisticsGetQuery
{
  // Applied to Order.CreatedAt.
  // For Admin-only points statistics it is also applied to PointsTransaction.CreatedAt.
  // Dates are treated as full UTC calendar days.
  public DateTime? FromDate { get; set; }

  public DateTime? ToDate { get; set; }

  // Admin only.
  // Manager never controls this value; backend forces Manager.CenterId.
  public int? CenterId { get; set; }

  // Used for TopProducts / TopCancellationReasons.
  // Service clamps it to 1..20.
  public int Top { get; set; } = 5;
}
