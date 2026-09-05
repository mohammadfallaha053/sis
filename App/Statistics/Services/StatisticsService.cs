using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.Users.Errors;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Auth.Enums;
using SisApi.App.Categories.Model;
using SisApi.App.Centers.Errors;
using SisApi.App.Centers.Model;
using SisApi.App.Orders.Enums;
using SisApi.App.Orders.Model;
using SisApi.App.PointsTransactions.Enums;
using SisApi.App.PointsTransactions.Model;
using SisApi.App.Products.Model;
using SisApi.App.Regions.Model;
using SisApi.App.Statistics.Dto.Request.Queries;
using SisApi.App.Statistics.Dto.Response;
using SisApi.App.Statistics.Errors;
using SisApi.App.Statistics.Interfaces;
using SisApi.App.Users.Model;
using SisApi.Data;

namespace SisApi.App.Statistics.Services;

public class StatisticsService : IStatisticsService
{
  private readonly ApplicationDbContext _dbContext;
  private readonly IClaimService _claimService;

  public StatisticsService(
    ApplicationDbContext dbContext,
    IClaimService claimService
  )
  {
    _dbContext = dbContext;
    _claimService = claimService;
  }

  public async Task<Result<StatisticsResponse>> GetAsync(
    StatisticsGetQuery query
  )
  {
    // ============================================================
    // 1. Validation + current user
    // ============================================================

    if (
      query.FromDate.HasValue
      && query.ToDate.HasValue
      && query.FromDate.Value.Date > query.ToDate.Value.Date
    )
    {
      return Result<StatisticsResponse>.Failure(
        StatisticsErrors.InvalidDateRange
      );
    }

    var currentUserId = _claimService.GetUserId();

    if (string.IsNullOrWhiteSpace(currentUserId))
    {
      return Result<StatisticsResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    var currentUser =
      await _dbContext
        .Set<ApplicationUser>()
        .AsNoTracking()
        .FirstOrDefaultAsync(user => user.Id == currentUserId);

    if (currentUser == null)
    {
      return Result<StatisticsResponse>.Failure(
        UserErrors.NotFound
      );
    }

    if (
      currentUser.Role != RoleEnum.Admin
      && currentUser.Role != RoleEnum.Manager
    )
    {
      return Result<StatisticsResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    // Manager is ALWAYS forced to their own center.
    // Admin may optionally filter by center.
    int? centerId = null;

    if (currentUser.Role == RoleEnum.Manager)
    {
      if (!currentUser.CenterId.HasValue)
      {
        return Result<StatisticsResponse>.Failure(
          CentersErrors.NoCenterForThisManagerYet
        );
      }

      centerId = currentUser.CenterId.Value;
    }
    else if (query.CenterId.HasValue)
    {
      centerId = query.CenterId.Value;
    }

    var fromDate = query.FromDate?.Date;
    var toDateExclusive = query.ToDate?.Date.AddDays(1);
    var top = Math.Clamp(query.Top, 1, 20);

    // ============================================================
    // 2. Scoped Orders - common source for Admin and Manager
    // ============================================================

    IQueryable<Order> ordersQuery =
      _dbContext
        .Set<Order>()
        .AsNoTracking();

    if (centerId.HasValue)
    {
      ordersQuery =
        ordersQuery.Where(order => order.CenterId == centerId.Value);
    }

    if (fromDate.HasValue)
    {
      ordersQuery =
        ordersQuery.Where(order => order.CreatedAt >= fromDate.Value);
    }

    if (toDateExclusive.HasValue)
    {
      ordersQuery =
        ordersQuery.Where(order => order.CreatedAt < toDateExclusive.Value);
    }

    var scopedOrderIds =
      ordersQuery.Select(order => order.Id);

    var completedOrderIds =
      ordersQuery
        .Where(order => order.Status == OrderStatusEnum.Completed)
        .Select(order => order.Id);

    IQueryable<OrderItem> completedItemsQuery =
      _dbContext
        .Set<OrderItem>()
        .AsNoTracking()
        .Where(orderItem => completedOrderIds.Contains(orderItem.OrderId));

    // ============================================================
    // 3. Common - Orders summary
    // ============================================================

    var orderStatusCounts =
      await ordersQuery
        .GroupBy(order => order.Status)
        .Select(group => new
        {
          Status = group.Key,
          Count = group.Count()
        })
        .ToListAsync();

    int CountStatus(OrderStatusEnum status) =>
      orderStatusCounts
        .FirstOrDefault(item => item.Status == status)
        ?.Count ?? 0;

    var totalOrders = orderStatusCounts.Sum(item => item.Count);
    var pendingOrders = CountStatus(OrderStatusEnum.Pending);
    var assignedOrders = CountStatus(OrderStatusEnum.Assigned);
    var inProgressOrders = CountStatus(OrderStatusEnum.InProgress);
    var completedOrders = CountStatus(OrderStatusEnum.Completed);
    var cancelledOrders = CountStatus(OrderStatusEnum.Cancelled);

    var completionRate =
      totalOrders == 0
        ? 0
        : Math.Round(
          (decimal)completedOrders * 100m / totalOrders,
          2
        );

    var cancellationRate =
      totalOrders == 0
        ? 0
        : Math.Round(
          (decimal)cancelledOrders * 100m / totalOrders,
          2
        );

    var averageCompletionMinutes =
      await ordersQuery
        .Where(order =>
          order.Status == OrderStatusEnum.Completed
          && order.CompletedAt.HasValue
        )
        .Select(order =>
          (double?)EF.Functions.DateDiffMinute(
            order.CreatedAt,
            order.CompletedAt!.Value
          )
        )
        .AverageAsync() ?? 0d;

    var averageCompletionHours =
      Math.Round((decimal)averageCompletionMinutes / 60m, 2);

    // ============================================================
    // 4. Common - Collection
    // Only COMPLETED orders represent collected material.
    // ============================================================

    var totalWeightKg =
      await completedItemsQuery.SumAsync(
        orderItem => orderItem.WeightKg ?? 0m
      );

    var totalOrderItemPoints =
      await completedItemsQuery.SumAsync(
        orderItem => orderItem.Points
      );

    var averageWeightPerCompletedOrder =
      completedOrders == 0
        ? 0
        : Math.Round(totalWeightKg / completedOrders, 3);

    // ============================================================
    // 5. Common - Orders by month
    // ============================================================

    var ordersByMonth =
      await ordersQuery
        .GroupBy(order => new
        {
          order.CreatedAt.Year,
          order.CreatedAt.Month
        })
        .Select(group => new MonthlyOrderStatisticResponse
        {
          Year = group.Key.Year,
          Month = group.Key.Month,
          TotalOrders = group.Count(),
          CompletedOrders = group.Count(order =>
            order.Status == OrderStatusEnum.Completed
          ),
          CancelledOrders = group.Count(order =>
            order.Status == OrderStatusEnum.Cancelled
          )
        })
        .OrderBy(item => item.Year)
        .ThenBy(item => item.Month)
        .ToListAsync();

    // ============================================================
    // 6. Common - Item types / collected materials
    // ============================================================

    var itemTypes =
      await completedItemsQuery
        .GroupBy(orderItem => new
        {
          orderItem.ItemTypeId,
          orderItem.ItemType.Name
        })
        .Select(group => new ItemTypeStatisticResponse
        {
          ItemTypeId = group.Key.ItemTypeId,
          ItemTypeName = group.Key.Name,
          TotalWeightKg = group.Sum(item => item.WeightKg ?? 0m),
          TotalPoints = group.Sum(item => item.Points),
          OrdersCount = group
            .Select(item => item.OrderId)
            .Distinct()
            .Count()
        })
        .OrderByDescending(item => item.TotalWeightKg)
        .ToListAsync();

    // ============================================================
    // 7. Common - Cancellation analytics
    // ============================================================

    var cancellationTypeRaw =
      await ordersQuery
        .Where(order => order.Status == OrderStatusEnum.Cancelled)
        .GroupBy(order => order.CancellationType)
        .Select(group => new
        {
          Type = group.Key,
          Count = group.Count()
        })
        .ToListAsync();

    var cancellationsByType =
      Enum
        .GetValues<OrderCancellationTypeEnum>()
        .Select(type => new CancellationTypeStatisticResponse
        {
          Type = type.ToString(),
          Count = cancellationTypeRaw
            .FirstOrDefault(item => item.Type == type)
            ?.Count ?? 0
        })
        .ToList();

    var unknownCancellationCount =
      cancellationTypeRaw
        .FirstOrDefault(item => !item.Type.HasValue)
        ?.Count ?? 0;

    if (unknownCancellationCount > 0)
    {
      cancellationsByType.Add(
        new CancellationTypeStatisticResponse
        {
          Type = "Unknown",
          Count = unknownCancellationCount
        }
      );
    }

    var topCancellationReasons =
      await ordersQuery
        .Where(order =>
          order.Status == OrderStatusEnum.Cancelled
          && order.CancellationReason != null
          && order.CancellationReason != string.Empty
        )
        .GroupBy(order => order.CancellationReason!)
        .Select(group => new CancellationReasonStatisticResponse
        {
          Reason = group.Key,
          Count = group.Count()
        })
        .OrderByDescending(item => item.Count)
        .Take(top)
        .ToListAsync();

    // ============================================================
    // 8. Build COMMON response first
    // ============================================================

    var data = new StatisticsResponse
    {
      GeneratedAt = DateTime.UtcNow,
      FromDate = fromDate,
      ToDate = query.ToDate?.Date,
      ViewerRole = currentUser.Role.ToString(),
      CenterId = centerId,

      Common = new CommonStatisticsResponse
      {
        Orders = new OrderStatisticsResponse
        {
          Total = totalOrders,
          Pending = pendingOrders,
          Assigned = assignedOrders,
          InProgress = inProgressOrders,
          Completed = completedOrders,
          Cancelled = cancelledOrders,
          CompletionRate = completionRate,
          CancellationRate = cancellationRate,
          AverageCompletionHours = averageCompletionHours
        },

        Collection = new CollectionStatisticsResponse
        {
          TotalWeightKg = totalWeightKg,
          AverageWeightPerCompletedOrderKg = averageWeightPerCompletedOrder,
          TotalOrderItemPoints = totalOrderItemPoints
        },

        OrdersByStatus = Enum
          .GetValues<OrderStatusEnum>()
          .Select(status => new OrderStatusStatisticResponse
          {
            Status = status.ToString(),
            Count = CountStatus(status)
          })
          .ToList(),

        OrdersByMonth = ordersByMonth,
        ItemTypes = itemTypes,
        CancellationsByType = cancellationsByType,
        TopCancellationReasons = topCancellationReasons
      }
    };

    // ============================================================
    // 9. MANAGER DASHBOARD
    // Operational center statistics only.
    // NO Categories, Products, TopProducts, PurchasedQuantity,
    // or product-points spending are returned to Manager.
    // ============================================================

    if (currentUser.Role == RoleEnum.Manager)
    {
      var managerCenterId = centerId!.Value;

      var center =
        await _dbContext
          .Set<Center>()
          .AsNoTracking()
          .FirstOrDefaultAsync(item => item.Id == managerCenterId);

      if (center == null)
      {
        return Result<StatisticsResponse>.Failure(
          CentersErrors.NotFound
        );
      }

      var centerRegionsQuery =
        _dbContext
          .Set<Region>()
          .AsNoTracking()
          .Where(region => region.CenterId == managerCenterId);

      var centerRegionIds =
        centerRegionsQuery.Select(region => region.Id);

      var centerEmployeesQuery =
        _dbContext
          .Set<ApplicationUser>()
          .AsNoTracking()
          .Where(user =>
            user.Role == RoleEnum.Employee
            && user.CenterId == managerCenterId
          );

      var totalClients =
        await _dbContext
          .Set<ApplicationUser>()
          .AsNoTracking()
          .CountAsync(user =>
            user.Role == RoleEnum.Client
            && user.RegionId.HasValue
            && centerRegionIds.Contains(user.RegionId.Value)
          );

      var totalEmployees = await centerEmployeesQuery.CountAsync();
      var activeEmployees = await centerEmployeesQuery.CountAsync(user => user.IsActive);
      var totalRegions = await centerRegionsQuery.CountAsync();

      // ----------------------------------------------------------
      // Employee performance
      // ----------------------------------------------------------

      var employeeRows =
        await centerEmployeesQuery
          .Select(employee => new
          {
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.IsActive
          })
          .ToListAsync();

      var employeeOrderGroups =
        await ordersQuery
          .Where(order => order.EmployeeId != null)
          .GroupBy(order => order.EmployeeId!)
          .Select(group => new
          {
            EmployeeId = group.Key,
            TotalOrders = group.Count(),
            AssignedOrders = group.Count(order =>
              order.Status == OrderStatusEnum.Assigned
            ),
            InProgressOrders = group.Count(order =>
              order.Status == OrderStatusEnum.InProgress
            ),
            CompletedOrders = group.Count(order =>
              order.Status == OrderStatusEnum.Completed
            ),
            CancelledOrders = group.Count(order =>
              order.Status == OrderStatusEnum.Cancelled
            )
          })
          .ToListAsync();

      var employeeWeightGroups =
        await completedItemsQuery
          .Where(item => item.Order.EmployeeId != null)
          .GroupBy(item => item.Order.EmployeeId!)
          .Select(group => new
          {
            EmployeeId = group.Key,
            TotalWeightKg = group.Sum(item => item.WeightKg ?? 0m)
          })
          .ToListAsync();

      var employeeStatistics =
        employeeRows
          .Select(employee =>
          {
            var orders = employeeOrderGroups
              .FirstOrDefault(item => item.EmployeeId == employee.Id);

            var weight = employeeWeightGroups
              .FirstOrDefault(item => item.EmployeeId == employee.Id)
              ?.TotalWeightKg ?? 0m;

            var employeeTotalOrders = orders?.TotalOrders ?? 0;
            var employeeCompletedOrders = orders?.CompletedOrders ?? 0;

            return new EmployeePerformanceStatisticResponse
            {
              EmployeeId = employee.Id,
              EmployeeName = $"{employee.FirstName} {employee.LastName}".Trim(),
              IsActive = employee.IsActive,
              TotalOrders = employeeTotalOrders,
              AssignedOrders = orders?.AssignedOrders ?? 0,
              InProgressOrders = orders?.InProgressOrders ?? 0,
              CompletedOrders = employeeCompletedOrders,
              CancelledOrders = orders?.CancelledOrders ?? 0,
              TotalWeightKg = weight,
              CompletionRate = employeeTotalOrders == 0
                ? 0
                : Math.Round(
                  (decimal)employeeCompletedOrders * 100m / employeeTotalOrders,
                  2
                )
            };
          })
          .OrderByDescending(item => item.TotalOrders)
          .ThenByDescending(item => item.CompletedOrders)
          .ToList();

      // ----------------------------------------------------------
      // Region performance
      // ----------------------------------------------------------

      var regionRows =
        await centerRegionsQuery
          .Select(region => new
          {
            region.Id,
            region.Name,
            region.IsActive
          })
          .ToListAsync();

      var clientsByRegion =
        await _dbContext
          .Set<ApplicationUser>()
          .AsNoTracking()
          .Where(user =>
            user.Role == RoleEnum.Client
            && user.RegionId.HasValue
            && centerRegionIds.Contains(user.RegionId.Value)
          )
          .GroupBy(user => user.RegionId!.Value)
          .Select(group => new
          {
            RegionId = group.Key,
            Count = group.Count()
          })
          .ToListAsync();

      var regionOrderGroups =
        await ordersQuery
          .GroupBy(order => order.RegionId)
          .Select(group => new
          {
            RegionId = group.Key,
            TotalOrders = group.Count(),
            PendingOrders = group.Count(order =>
              order.Status == OrderStatusEnum.Pending
            ),
            AssignedOrders = group.Count(order =>
              order.Status == OrderStatusEnum.Assigned
            ),
            InProgressOrders = group.Count(order =>
              order.Status == OrderStatusEnum.InProgress
            ),
            CompletedOrders = group.Count(order =>
              order.Status == OrderStatusEnum.Completed
            ),
            CancelledOrders = group.Count(order =>
              order.Status == OrderStatusEnum.Cancelled
            )
          })
          .ToListAsync();

      var regionWeightGroups =
        await completedItemsQuery
          .GroupBy(item => item.Order.RegionId)
          .Select(group => new
          {
            RegionId = group.Key,
            TotalWeightKg = group.Sum(item => item.WeightKg ?? 0m)
          })
          .ToListAsync();

      var regionStatistics =
        regionRows
          .Select(region =>
          {
            var orders = regionOrderGroups
              .FirstOrDefault(item => item.RegionId == region.Id);

            var regionTotalOrders = orders?.TotalOrders ?? 0;
            var regionCompletedOrders = orders?.CompletedOrders ?? 0;

            return new RegionPerformanceStatisticResponse
            {
              RegionId = region.Id,
              RegionName = region.Name,
              IsActive = region.IsActive,
              TotalClients = clientsByRegion
                .FirstOrDefault(item => item.RegionId == region.Id)
                ?.Count ?? 0,
              TotalOrders = regionTotalOrders,
              PendingOrders = orders?.PendingOrders ?? 0,
              AssignedOrders = orders?.AssignedOrders ?? 0,
              InProgressOrders = orders?.InProgressOrders ?? 0,
              CompletedOrders = regionCompletedOrders,
              CancelledOrders = orders?.CancelledOrders ?? 0,
              TotalWeightKg = regionWeightGroups
                .FirstOrDefault(item => item.RegionId == region.Id)
                ?.TotalWeightKg ?? 0m,
              CompletionRate = regionTotalOrders == 0
                ? 0
                : Math.Round(
                  (decimal)regionCompletedOrders * 100m / regionTotalOrders,
                  2
                )
            };
          })
          .OrderByDescending(item => item.TotalOrders)
          .ToList();

      data.Manager = new ManagerStatisticsResponse
      {
        Center = new CenterSummaryResponse
        {
          Id = center.Id,
          Name = center.Name
        },

        General = new ManagerGeneralStatisticsResponse
        {
          TotalClients = totalClients,
          TotalEmployees = totalEmployees,
          ActiveEmployees = activeEmployees,
          TotalRegions = totalRegions
        },

        Employees = employeeStatistics,
        Regions = regionStatistics
      };

      return Result<StatisticsResponse>.Success(data);
    }

    // ============================================================
    // 10. ADMIN DASHBOARD
    // Global management + catalog + points + center performance.
    // ============================================================

    IQueryable<ApplicationUser> adminUsersQuery =
      _dbContext
        .Set<ApplicationUser>()
        .AsNoTracking();

    IQueryable<Center> adminCentersQuery =
      _dbContext
        .Set<Center>()
        .AsNoTracking();

    IQueryable<Region> adminRegionsQuery =
      _dbContext
        .Set<Region>()
        .AsNoTracking();

    if (centerId.HasValue)
    {
      adminCentersQuery =
        adminCentersQuery.Where(center => center.Id == centerId.Value);

      adminRegionsQuery =
        adminRegionsQuery.Where(region => region.CenterId == centerId.Value);

      var filteredRegionIds =
        adminRegionsQuery.Select(region => region.Id);

      adminUsersQuery =
        adminUsersQuery.Where(user =>
          (
            user.Role == RoleEnum.Employee
            && user.CenterId == centerId.Value
          )
          ||
          (
            user.Role == RoleEnum.Manager
            && user.CenterId == centerId.Value
          )
          ||
          (
            user.Role == RoleEnum.Client
            && user.RegionId.HasValue
            && filteredRegionIds.Contains(user.RegionId.Value)
          )
        );
    }

    var adminTotalClients =
      await adminUsersQuery.CountAsync(user => user.Role == RoleEnum.Client);

    var adminTotalEmployees =
      await adminUsersQuery.CountAsync(user => user.Role == RoleEnum.Employee);

    var adminTotalCenters = await adminCentersQuery.CountAsync();
    var adminTotalRegions = await adminRegionsQuery.CountAsync();

    // Catalog is global Admin data; Manager never receives it.
    var totalCategories =
      await _dbContext
        .Set<Category>()
        .AsNoTracking()
        .CountAsync();

    var productsQuery =
      _dbContext
        .Set<Product>()
        .AsNoTracking();

    var totalProducts = await productsQuery.CountAsync();
    var activeProducts = await productsQuery.CountAsync(product => product.IsActive);
    var outOfStockProducts = await productsQuery.CountAsync(product => product.StockQuantity <= 0);

    // ------------------------------------------------------------
    // Admin points statistics
    // ------------------------------------------------------------

    IQueryable<PointsTransaction> pointsQuery =
      _dbContext
        .Set<PointsTransaction>()
        .AsNoTracking();

    if (fromDate.HasValue)
    {
      pointsQuery =
        pointsQuery.Where(transaction => transaction.CreatedAt >= fromDate.Value);
    }

    if (toDateExclusive.HasValue)
    {
      pointsQuery =
        pointsQuery.Where(transaction => transaction.CreatedAt < toDateExclusive.Value);
    }

    if (centerId.HasValue)
    {
      var scopedClientIds =
        _dbContext
          .Set<ApplicationUser>()
          .AsNoTracking()
          .Where(user =>
            user.Role == RoleEnum.Client
            && user.RegionId.HasValue
            && _dbContext
              .Set<Region>()
              .Any(region =>
                region.Id == user.RegionId.Value
                && region.CenterId == centerId.Value
              )
          )
          .Select(user => user.Id);

      pointsQuery =
        pointsQuery.Where(transaction =>
          (
            transaction.OrderId.HasValue
            && scopedOrderIds.Contains(transaction.OrderId.Value)
          )
          ||
          (
            transaction.Type == PointsTransactionTypeEnum.ProductPurchase
            && scopedClientIds.Contains(transaction.ClientId)
          )
        );
    }

    var earnedPoints =
      await pointsQuery
        .Where(transaction =>
          transaction.Type == PointsTransactionTypeEnum.OrderEarned
        )
        .SumAsync(transaction => transaction.Points);

    var spentPoints =
      await pointsQuery
        .Where(transaction =>
          transaction.Type == PointsTransactionTypeEnum.ProductPurchase
        )
        .SumAsync(transaction => transaction.Points);

    var pointsTransactionsCount = await pointsQuery.CountAsync();

    var purchasedQuantity =
      await pointsQuery
        .Where(transaction =>
          transaction.Type == PointsTransactionTypeEnum.ProductPurchase
        )
        .SumAsync(transaction => transaction.Quantity ?? 0);

    // ------------------------------------------------------------
    // Admin center performance
    // ------------------------------------------------------------

    var adminCenterRows =
      await adminCentersQuery
        .Select(center => new
        {
          center.Id,
          center.Name
        })
        .ToListAsync();

    var centerOrderGroups =
      await ordersQuery
        .GroupBy(order => order.CenterId)
        .Select(group => new
        {
          CenterId = group.Key,
          TotalOrders = group.Count(),
          PendingOrders = group.Count(order =>
            order.Status == OrderStatusEnum.Pending
          ),
          InProgressOrders = group.Count(order =>
            order.Status == OrderStatusEnum.InProgress
          ),
          CompletedOrders = group.Count(order =>
            order.Status == OrderStatusEnum.Completed
          ),
          CancelledOrders = group.Count(order =>
            order.Status == OrderStatusEnum.Cancelled
          )
        })
        .ToListAsync();

    var centerWeightGroups =
      await completedItemsQuery
        .GroupBy(item => item.Order.CenterId)
        .Select(group => new
        {
          CenterId = group.Key,
          TotalWeightKg = group.Sum(item => item.WeightKg ?? 0m)
        })
        .ToListAsync();

    var employeeCountByCenter =
      await _dbContext
        .Set<ApplicationUser>()
        .AsNoTracking()
        .Where(user =>
          user.Role == RoleEnum.Employee
          && user.CenterId.HasValue
        )
        .GroupBy(user => user.CenterId!.Value)
        .Select(group => new
        {
          CenterId = group.Key,
          Count = group.Count()
        })
        .ToListAsync();

    var regionCountByCenter =
      await _dbContext
        .Set<Region>()
        .AsNoTracking()
        .GroupBy(region => region.CenterId)
        .Select(group => new
        {
          CenterId = group.Key,
          Count = group.Count()
        })
        .ToListAsync();

    var clientsByCenter =
      await (
        from user in _dbContext.Set<ApplicationUser>().AsNoTracking()
        join region in _dbContext.Set<Region>().AsNoTracking()
          on user.RegionId equals (int?)region.Id
        where user.Role == RoleEnum.Client
        group user by region.CenterId into groupItems
        select new
        {
          CenterId = groupItems.Key,
          Count = groupItems.Count()
        }
      ).ToListAsync();

    var centerStatistics =
      adminCenterRows
        .Select(center =>
        {
          var orders = centerOrderGroups
            .FirstOrDefault(item => item.CenterId == center.Id);

          var centerTotalOrders = orders?.TotalOrders ?? 0;
          var centerCompletedOrders = orders?.CompletedOrders ?? 0;

          return new CenterStatisticResponse
          {
            CenterId = center.Id,
            CenterName = center.Name,
            TotalClients = clientsByCenter
              .FirstOrDefault(item => item.CenterId == center.Id)
              ?.Count ?? 0,
            TotalEmployees = employeeCountByCenter
              .FirstOrDefault(item => item.CenterId == center.Id)
              ?.Count ?? 0,
            TotalRegions = regionCountByCenter
              .FirstOrDefault(item => item.CenterId == center.Id)
              ?.Count ?? 0,
            TotalOrders = centerTotalOrders,
            PendingOrders = orders?.PendingOrders ?? 0,
            InProgressOrders = orders?.InProgressOrders ?? 0,
            CompletedOrders = centerCompletedOrders,
            CancelledOrders = orders?.CancelledOrders ?? 0,
            TotalWeightKg = centerWeightGroups
              .FirstOrDefault(item => item.CenterId == center.Id)
              ?.TotalWeightKg ?? 0m,
            CompletionRate = centerTotalOrders == 0
              ? 0
              : Math.Round(
                (decimal)centerCompletedOrders * 100m / centerTotalOrders,
                2
              )
          };
        })
        .OrderByDescending(item => item.TotalOrders)
        .ToList();

    // ------------------------------------------------------------
    // Admin top purchased products
    // ------------------------------------------------------------

    var topProducts =
      await pointsQuery
        .Where(transaction =>
          transaction.Type == PointsTransactionTypeEnum.ProductPurchase
          && transaction.ProductId.HasValue
        )
        .GroupBy(transaction => new
        {
          ProductId = transaction.ProductId!.Value,
          ProductName = transaction.Product!.Name
        })
        .Select(group => new TopProductStatisticResponse
        {
          ProductId = group.Key.ProductId,
          ProductName = group.Key.ProductName,
          PurchasedQuantity = group.Sum(item => item.Quantity ?? 0),
          PointsSpent = group.Sum(item => item.Points)
        })
        .OrderByDescending(item => item.PurchasedQuantity)
        .ThenByDescending(item => item.PointsSpent)
        .Take(top)
        .ToListAsync();

    data.Admin = new AdminStatisticsResponse
    {
      General = new AdminGeneralStatisticsResponse
      {
        TotalClients = adminTotalClients,
        TotalEmployees = adminTotalEmployees,
        TotalCenters = adminTotalCenters,
        TotalRegions = adminTotalRegions,
        TotalCategories = totalCategories,
        TotalProducts = totalProducts
      },

      Points = new PointsStatisticsResponse
      {
        EarnedPoints = earnedPoints,
        SpentPoints = spentPoints,
        NetPoints = earnedPoints - spentPoints,
        TransactionsCount = pointsTransactionsCount
      },

      Products = new ProductStatisticsResponse
      {
        ActiveProducts = activeProducts,
        OutOfStockProducts = outOfStockProducts,
        PurchasedQuantity = purchasedQuantity,
        PointsSpentOnProducts = spentPoints
      },

      Centers = centerStatistics,
      TopProducts = topProducts
    };

    return Result<StatisticsResponse>.Success(data);
  }
}
