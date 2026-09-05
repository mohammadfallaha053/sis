using System.Text.Json.Serialization;

namespace SisApi.App.Statistics.Dto.Response;

public class StatisticsResponse
{
  public DateTime GeneratedAt { get; set; }
  public DateTime? FromDate { get; set; }
  public DateTime? ToDate { get; set; }

  // "Admin" or "Manager". Frontend can use it to choose the dashboard layout.
  public string ViewerRole { get; set; } = string.Empty;

  // Null for Admin when no center filter is selected.
  public int? CenterId { get; set; }

  // Data useful for both Admin and Manager dashboards.
  public CommonStatisticsResponse Common { get; set; } = new();

  // Returned only to Admin.
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public AdminStatisticsResponse? Admin { get; set; }

  // Returned only to Manager.
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public ManagerStatisticsResponse? Manager { get; set; }
}

public class CommonStatisticsResponse
{
  public OrderStatisticsResponse Orders { get; set; } = new();
  public CollectionStatisticsResponse Collection { get; set; } = new();

  public List<OrderStatusStatisticResponse> OrdersByStatus { get; set; } = [];
  public List<MonthlyOrderStatisticResponse> OrdersByMonth { get; set; } = [];
  public List<ItemTypeStatisticResponse> ItemTypes { get; set; } = [];
  public List<CancellationTypeStatisticResponse> CancellationsByType { get; set; } = [];
  public List<CancellationReasonStatisticResponse> TopCancellationReasons { get; set; } = [];
}

// ============================================================
// Admin-only section
// ============================================================

public class AdminStatisticsResponse
{
  public AdminGeneralStatisticsResponse General { get; set; } = new();
  public PointsStatisticsResponse Points { get; set; } = new();
  public ProductStatisticsResponse Products { get; set; } = new();

  public List<CenterStatisticResponse> Centers { get; set; } = [];
  public List<TopProductStatisticResponse> TopProducts { get; set; } = [];
}

public class AdminGeneralStatisticsResponse
{
  public int TotalClients { get; set; }
  public int TotalEmployees { get; set; }
  public int TotalCenters { get; set; }
  public int TotalRegions { get; set; }
  public int TotalCategories { get; set; }
  public int TotalProducts { get; set; }
}

// ============================================================
// Manager-only section
// No products/catalog/points spending statistics here.
// ============================================================

public class ManagerStatisticsResponse
{
  public CenterSummaryResponse Center { get; set; } = new();
  public ManagerGeneralStatisticsResponse General { get; set; } = new();

  public List<EmployeePerformanceStatisticResponse> Employees { get; set; } = [];
  public List<RegionPerformanceStatisticResponse> Regions { get; set; } = [];
}

public class CenterSummaryResponse
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
}

public class ManagerGeneralStatisticsResponse
{
  public int TotalClients { get; set; }
  public int TotalEmployees { get; set; }
  public int ActiveEmployees { get; set; }
  public int TotalRegions { get; set; }
}

// ============================================================
// Common order / collection statistics
// ============================================================

public class OrderStatisticsResponse
{
  public int Total { get; set; }
  public int Pending { get; set; }
  public int Assigned { get; set; }
  public int InProgress { get; set; }
  public int Completed { get; set; }
  public int Cancelled { get; set; }

  public decimal CompletionRate { get; set; }
  public decimal CancellationRate { get; set; }

  // 0 when there are no completed orders with CompletedAt.
  public decimal AverageCompletionHours { get; set; }
}

public class CollectionStatisticsResponse
{
  public decimal TotalWeightKg { get; set; }
  public decimal AverageWeightPerCompletedOrderKg { get; set; }
  public decimal TotalOrderItemPoints { get; set; }
}

public class OrderStatusStatisticResponse
{
  public string Status { get; set; } = string.Empty;
  public int Count { get; set; }
}

public class MonthlyOrderStatisticResponse
{
  public int Year { get; set; }
  public int Month { get; set; }
  public int TotalOrders { get; set; }
  public int CompletedOrders { get; set; }
  public int CancelledOrders { get; set; }
}

public class ItemTypeStatisticResponse
{
  public int ItemTypeId { get; set; }
  public string ItemTypeName { get; set; } = string.Empty;
  public decimal TotalWeightKg { get; set; }
  public decimal TotalPoints { get; set; }
  public int OrdersCount { get; set; }
}

public class CancellationTypeStatisticResponse
{
  public string Type { get; set; } = string.Empty;
  public int Count { get; set; }
}

public class CancellationReasonStatisticResponse
{
  public string Reason { get; set; } = string.Empty;
  public int Count { get; set; }
}

// ============================================================
// Admin analytics
// ============================================================

public class PointsStatisticsResponse
{
  public decimal EarnedPoints { get; set; }
  public decimal SpentPoints { get; set; }
  public decimal NetPoints { get; set; }
  public int TransactionsCount { get; set; }
}

public class ProductStatisticsResponse
{
  public int ActiveProducts { get; set; }
  public int OutOfStockProducts { get; set; }
  public int PurchasedQuantity { get; set; }
  public decimal PointsSpentOnProducts { get; set; }
}

public class CenterStatisticResponse
{
  public int CenterId { get; set; }
  public string CenterName { get; set; } = string.Empty;

  public int TotalClients { get; set; }
  public int TotalEmployees { get; set; }
  public int TotalRegions { get; set; }

  public int TotalOrders { get; set; }
  public int PendingOrders { get; set; }
  public int InProgressOrders { get; set; }
  public int CompletedOrders { get; set; }
  public int CancelledOrders { get; set; }

  public decimal TotalWeightKg { get; set; }
  public decimal CompletionRate { get; set; }
}

public class TopProductStatisticResponse
{
  public int ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public int PurchasedQuantity { get; set; }
  public decimal PointsSpent { get; set; }
}

// ============================================================
// Manager operational analytics
// ============================================================

public class EmployeePerformanceStatisticResponse
{
  public string EmployeeId { get; set; } = string.Empty;
  public string EmployeeName { get; set; } = string.Empty;
  public bool IsActive { get; set; }

  public int TotalOrders { get; set; }
  public int AssignedOrders { get; set; }
  public int InProgressOrders { get; set; }
  public int CompletedOrders { get; set; }
  public int CancelledOrders { get; set; }

  public decimal TotalWeightKg { get; set; }
  public decimal CompletionRate { get; set; }
}

public class RegionPerformanceStatisticResponse
{
  public int RegionId { get; set; }
  public string RegionName { get; set; } = string.Empty;
  public bool IsActive { get; set; }
  public int TotalClients { get; set; }

  public int TotalOrders { get; set; }
  public int PendingOrders { get; set; }
  public int AssignedOrders { get; set; }
  public int InProgressOrders { get; set; }
  public int CompletedOrders { get; set; }
  public int CancelledOrders { get; set; }

  public decimal TotalWeightKg { get; set; }
  public decimal CompletionRate { get; set; }
}
