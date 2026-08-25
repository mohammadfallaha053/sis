using AutoMapper;
using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.Users.Errors;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Auth.Enums;
using SisApi.App.Centers.Errors;
using SisApi.App.ItemTypes.Errors;
using SisApi.App.Orders.Dto.Request.Commands;
using SisApi.App.Orders.Dto.Request.Queries;
using SisApi.App.Orders.Dto.Response;
using SisApi.App.Orders.Enums;
using SisApi.App.Orders.Errors;
using SisApi.App.Orders.Interfaces;
using SisApi.App.Orders.Model;
using SisApi.App.PointsTransactions.Enums;
using SisApi.App.PointsTransactions.Model;
using SisApi.App.Regions.Errors;
using SisApi.Data.Interfaces;
using SisApi.Shared.Errors;
using System.Linq.Expressions;

namespace SisApi.App.Orders.Services;

public class OrdersService : IOrdersService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;

  public OrdersService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimService claimService
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _claimService = claimService;
  }

  public async Task<Result<OrdersResponse>> AddAsync(
    OrdersCreateCommand command
  )
  {
    // ==========================================
    // 1. يجب اختيار مادة واحدة على الأقل
    // ==========================================
    if (
      command.ItemTypeIds == null ||
      command.ItemTypeIds.Count == 0
    )
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.ItemsRequired
      );
    }

    await using var transaction =
      await _unitOfWork.BeginTransactionAsync();

    try
    {
      // ==========================================
      // 2. معرفة العميل الحالي
      // ==========================================
      var clientId = _claimService.GetUserId();

      if (string.IsNullOrWhiteSpace(clientId))
      {
        await transaction.RollbackAsync();

        return Result<OrdersResponse>.Failure(
          AuthErrors.Unauthorized
        );
      }

      var client =
        await _unitOfWork.Users.GetFirstOrDefaultAsync(
          predicate: user => user.Id == clientId
        );

      if (client == null)
      {
        await transaction.RollbackAsync();

        return Result<OrdersResponse>.Failure(
          UserErrors.NotFound
        );
      }

      // ==========================================
      // 3. حماية إضافية:
      // إنشاء الطلب للعميل فقط
      // ==========================================
      if (client.Role != RoleEnum.Client)
      {
        await transaction.RollbackAsync();

        return Result<OrdersResponse>.Failure(
          AuthErrors.Unauthorized
        );
      }

      // ==========================================
      // 4. العميل يجب أن يكون محدد منطقة
      // ==========================================
      if (!client.RegionId.HasValue)
      {
        await transaction.RollbackAsync();

        return Result<OrdersResponse>.Failure(
          RegionErrors.Required
        );
      }

      // ==========================================
      // 5. التأكد أن المنطقة موجودة
      // ==========================================
      var region =
        await _unitOfWork.Regions.GetByIdAsync(
          client.RegionId.Value
        );

      if (region == null)
      {
        await transaction.RollbackAsync();

        return Result<OrdersResponse>.Failure(
          RegionErrors.NotFound
        );
      }

      // ==========================================
      // 6. المنطقة يجب أن تكون فعالة
      // ==========================================
      if (!region.IsActive)
      {
        await transaction.RollbackAsync();

        return Result<OrdersResponse>.Failure(
          RegionErrors.Inactive
        );
      }

      // ==========================================
      // 7. التأكد أن المنطقة تابعة لمركز صالح
      // ==========================================
      var center =
        await _unitOfWork.Centers.GetByIdAsync(
          region.CenterId
        );

      if (center == null || !center.IsActive)
      {
        await transaction.RollbackAsync();

        return Result<OrdersResponse>.Failure(
          RegionErrors.HasNoServiceCenter
        );
      }

      // ==========================================
      // 8. التأكد من وجود موقع استلام
      // ==========================================
      if (
        !client.Lat.HasValue ||
        !client.Long.HasValue
      )
      {
        await transaction.RollbackAsync();

        return Result<OrdersResponse>.Failure(
          OrdersErrors.PickupLocationRequired
        );
      }

      // ==========================================
      // 9. تجهيز عناصر الطلب
      // ==========================================
      var orderItems = new List<OrderItem>();

      foreach (
        var itemTypeId in command.ItemTypeIds.Distinct()
      )
      {
        var itemType =
          await _unitOfWork.ItemTypes.GetByIdAsync(
            itemTypeId
          );

        if (itemType == null)
        {
          await transaction.RollbackAsync();

          return Result<OrdersResponse>.Failure(
            ItemTypesErrors.NotFound
          );
        }

        orderItems.Add(
          new OrderItem
          {
            ItemTypeId = itemType.Id,
            PointsPerKg = itemType.PointsPerKg,
            WeightKg = null,
            Points = 0
          }
        );
      }

      // ==========================================
      // 10. إنشاء الطلب
      // ==========================================
      var model = new Order
      {
        ClientId = client.Id,

        RegionId = region.Id,
        CenterId = center.Id,
        
        EmployeeId = null,

        Status = OrderStatusEnum.Pending,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,

        OrderItems = orderItems
      };

      model =
        await _unitOfWork.Orders.AddAsync(model);

      await _unitOfWork.SaveChangesAsync();

      await transaction.CommitAsync();

      var data =
        _mapper.Map<OrdersResponse>(model);

      return Result<OrdersResponse>.Success(data);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }

public async Task<Result<IEnumerable<OrdersResponse>>> GetAllAsync(
  OrdersGetAllQuery query
)
{
  // ==========================================
  // 1. المستخدم الحالي
  // ==========================================
  var currentUserId =
    _claimService.GetUserId();

  if (string.IsNullOrWhiteSpace(currentUserId))
  {
    return Result<IEnumerable<OrdersResponse>>.Failure(
      AuthErrors.Unauthorized
    );
  }

  var currentUser =
    await _unitOfWork.Users.GetFirstOrDefaultAsync(
      predicate: user => user.Id == currentUserId
    );

  if (currentUser == null)
  {
    return Result<IEnumerable<OrdersResponse>>.Failure(
      UserErrors.NotFound
    );
  }

  // ==========================================
  // 2. البحث الأساسي
  // ==========================================
  Expression<Func<Order, bool>> predicate =
    order =>
      string.IsNullOrEmpty(query.Search)
      ||
      order.Status.ToString().Contains(query.Search);

  // ==========================================
  // 3. فلترة الطلبات حسب الدور
  // ==========================================
  switch (currentUser.Role)
  {
    // ========================================
    // Admin
    // يرى جميع الطلبات
    // ويستطيع الفلترة حسب المركز
    // ========================================
    case RoleEnum.Admin:
    {
      if (query.CenterId.HasValue)
      {
        predicate =
          predicate.And(
            order =>
              order.CenterId == query.CenterId.Value
          );
      }

      break;
    }

    // ========================================
    // Manager
    // يرى طلبات مركزه فقط
    // ========================================
    case RoleEnum.Manager:
    {
      if (!currentUser.CenterId.HasValue)
      {
        return Result<IEnumerable<OrdersResponse>>.Failure(
          CentersErrors.NoCenterForThisManagerYet
        );
      }

      var managerCenterId =
        currentUser.CenterId.Value;

      predicate =
        predicate.And(
          order =>
            order.CenterId == managerCenterId
        );

      break;
    }

    // ========================================
    // Employee
    // يرى الطلبات المعينة له فقط
    // ========================================
    case RoleEnum.Employee:
    {
      predicate =
        predicate.And(
          order =>
            order.EmployeeId == currentUser.Id
        );

      break;
    }

    // ========================================
    // Client
    // يرى طلباته فقط
    // ========================================
    case RoleEnum.Client:
    {
      predicate =
        predicate.And(
          order =>
            order.ClientId == currentUser.Id
        );

      break;
    }

    default:
      return Result<IEnumerable<OrdersResponse>>.Failure(
        AuthErrors.Unauthorized
      );
  }

  // ==========================================
  // 4. فلترة حسب حالة الطلب
  // متاحة لجميع الأدوار
  // لكن ضمن نطاق صلاحياتهم فقط
  // ==========================================
  if (query.Status.HasValue)
  {
    predicate =
      predicate.And(
        order =>
          order.Status == query.Status.Value
      );
  }

  // ==========================================
  // 5. فلترة IsActive
  // ==========================================
  if (query.IsActive.HasValue)
  {
    predicate =
      predicate.And(
        order =>
          order.IsActive == query.IsActive.Value
      );
  }

  // ==========================================
  // 6. جلب البيانات
  // ==========================================
  var pagedResult =
    await _unitOfWork.Orders.GetPagedAsync(
      predicate: predicate,
      pageNumber: query.PageNumber,
      pageSize: query.PageSize,
      queryBuilder: orders => orders
        .Include(order => order.OrderItems)
        .ThenInclude(orderItem => orderItem.ItemType)
    );

  var orders =
    pagedResult.Data.ToList();

  var data =
    _mapper.Map<List<OrdersResponse>>(orders);

  var paging =
    new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

  return Result<IEnumerable<OrdersResponse>>.Success(
    data,
    paging
  );
}

  // public async Task<Result<OrdersResponse>> GetByIdAsync(
  //   int id
  // )
  // {
  //   var order =
  //     await _unitOfWork.Orders
  //       .GetFirstOrDefaultAsync(
  //         predicate: order => order.Id == id,
  //         queryBuilder: orders => orders
  //           .Include(order => order.OrderItems)
  //           .ThenInclude(orderItem => orderItem.ItemType)
  //       );
  //
  //   if (order == null)
  //   {
  //     return Result<OrdersResponse>.Failure(
  //       OrdersErrors.NotFound
  //     );
  //   }
  //
  //   var data =
  //     _mapper.Map<OrdersResponse>(order);
  //
  //   return Result<OrdersResponse>.Success(data);
  // }
  //
  // public async Task<Result<object>> DeleteAsync(
  //   int id
  // )
  // {
  //   await using var transaction =
  //     await _unitOfWork.BeginTransactionAsync();
  //
  //   try
  //   {
  //     var order =
  //       await _unitOfWork.Orders.GetByIdAsync(id);
  //
  //     if (order == null)
  //     {
  //       await transaction.RollbackAsync();
  //
  //       return Result<object>.Failure(
  //         OrdersErrors.NotFound
  //       );
  //     }
  //
  //     await _unitOfWork.Orders.RemoveAsync(order);
  //
  //     await _unitOfWork.SaveChangesAsync();
  //
  //     await transaction.CommitAsync();
  //
  //     return Result<object>.Success(null);
  //   }
  //   catch
  //   {
  //     await transaction.RollbackAsync();
  //     throw;
  //   }
  // }

  public async Task<Result<OrdersResponse>> AssignEmployeeAsync(
    int id,
    OrdersAssignEmployeeCommand command
  )
  {
    // ==========================================
    // 1. التحقق من EmployeeId
    // ==========================================
    if (string.IsNullOrWhiteSpace(command.EmployeeId))
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.EmployeeRequired
      );
    }

    // ==========================================
    // 2. المستخدم الحالي
    // ==========================================
    var currentUserId =
      _claimService.GetUserId();

    if (string.IsNullOrWhiteSpace(currentUserId))
    {
      return Result<OrdersResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    var currentUser =
      await _unitOfWork.Users.GetFirstOrDefaultAsync(
        predicate: user => user.Id == currentUserId
      );

    if (currentUser == null)
    {
      return Result<OrdersResponse>.Failure(
        UserErrors.NotFound
      );
    }

    // ==========================================
    // 3. فقط Admin أو Manager
    // ==========================================
    if (
      currentUser.Role != RoleEnum.Admin &&
      currentUser.Role != RoleEnum.Manager
    )
    {
      return Result<OrdersResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    // ==========================================
    // 4. جلب الطلب
    // ==========================================
    var order =
      await _unitOfWork.Orders.GetFirstOrDefaultAsync(
        predicate: order => order.Id == id,
        queryBuilder: orders => orders
          .Include(order => order.OrderItems)
          .ThenInclude(orderItem => orderItem.ItemType)
      );

    if (order == null)
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.NotFound
      );
    }

    // ==========================================
    // 5. يسمح بالتعيين فقط للطلب Pending
    // ==========================================
    if (order.Status != OrderStatusEnum.Pending)
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.CannotAssignAtCurrentStatus
      );
    }

    // ==========================================
    // 6. إذا كان المستخدم Manager
    // يجب أن يكون الطلب تابعًا لمركزه
    // ==========================================
    if (currentUser.Role == RoleEnum.Manager)
    {
      if (!currentUser.CenterId.HasValue)
      {
        return Result<OrdersResponse>.Failure(
          CentersErrors.NoCenterForThisManagerYet
        );
      }

      if (currentUser.CenterId.Value != order.CenterId)
      {
        return Result<OrdersResponse>.Failure(
          AuthErrors.Unauthorized
        );
      }

      // ========================================
      // حماية إضافية:
      // التأكد أنه المدير الرسمي لهذا المركز
      // ========================================
      var center =
        await _unitOfWork.Centers.GetByIdAsync(
          order.CenterId
        );

      if (center == null)
      {
        return Result<OrdersResponse>.Failure(
          CentersErrors.NotFound
        );
      }

      if (center.ManagerId != currentUser.Id)
      {
        return Result<OrdersResponse>.Failure(
          CentersErrors.UserIsNotCenterManager
        );
      }
    }

    // ==========================================
    // 7. جلب الموظف
    // ==========================================
    var employee =
      await _unitOfWork.Users.GetFirstOrDefaultAsync(
        predicate: user =>
          user.Id == command.EmployeeId
      );

    if (employee == null)
    {
      return Result<OrdersResponse>.Failure(
        UserErrors.NotFound
      );
    }

    // ==========================================
    // 8. يجب أن يكون Employee
    // ==========================================
    if (employee.Role != RoleEnum.Employee)
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.EmployeeMustBelongToOrderCenter
      );
    }

    // ==========================================
    // 9. الموظف يجب أن يكون فعال
    // ==========================================
    if (!employee.IsActive)
    {
      return Result<OrdersResponse>.Failure(
        UserErrors.InactiveUser
      );
    }

    // ==========================================
    // 10. الموظف يجب أن يتبع لنفس مركز الطلب
    // ==========================================
    if (
      !employee.CenterId.HasValue ||
      employee.CenterId.Value != order.CenterId
    )
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.EmployeeMustBelongToOrderCenter
      );
    }

    // ==========================================
    // 11. تعيين الموظف
    // ==========================================
    order.EmployeeId = employee.Id;

    order.Status =
      OrderStatusEnum.Assigned;

    await _unitOfWork.Orders.UpdateAsync(order);

    await _unitOfWork.SaveChangesAsync();

    // ==========================================
    // 12. Response
    // ==========================================
    var data =
      _mapper.Map<OrdersResponse>(order);

    return Result<OrdersResponse>.Success(data);
  }

  public async Task<Result<OrdersResponse>> StartAsync(
    int id
  )
  {
    // ==========================================
    // 1. المستخدم الحالي
    // ==========================================
    var currentUserId =
      _claimService.GetUserId();

    if (string.IsNullOrWhiteSpace(currentUserId))
    {
      return Result<OrdersResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    var currentUser =
      await _unitOfWork.Users.GetFirstOrDefaultAsync(
        predicate: user => user.Id == currentUserId
      );

    if (currentUser == null)
    {
      return Result<OrdersResponse>.Failure(
        UserErrors.NotFound
      );
    }

    // ==========================================
    // 2. فقط Employee يستطيع بدء الطلب
    // ==========================================
    if (currentUser.Role != RoleEnum.Employee)
    {
      return Result<OrdersResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    // ==========================================
    // 3. الموظف يجب أن يكون فعالًا
    // ==========================================
    if (!currentUser.IsActive)
    {
      return Result<OrdersResponse>.Failure(
        UserErrors.InactiveUser
      );
    }

    // ==========================================
    // 4. جلب الطلب مع عناصره
    // ==========================================
    var order =
      await _unitOfWork.Orders.GetFirstOrDefaultAsync(
        predicate: order => order.Id == id,
        queryBuilder: orders => orders
          .Include(order => order.OrderItems)
          .ThenInclude(orderItem => orderItem.ItemType)
      );

    if (order == null)
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.NotFound
      );
    }

    // ==========================================
    // 5. الطلب يجب أن يكون Assigned
    // ==========================================
    if (order.Status != OrderStatusEnum.Assigned)
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.CannotStartAtCurrentStatus
      );
    }

    // ==========================================
    // 6. يجب أن يكون الطلب معينًا
    // لهذا الموظف تحديدًا
    // ==========================================
    if (
      string.IsNullOrWhiteSpace(order.EmployeeId) ||
      order.EmployeeId != currentUser.Id
    )
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.NotAssignedToCurrentEmployee
      );
    }

    // ==========================================
    // 7. حماية إضافية:
    // الموظف يجب أن يبقى تابعًا لمركز الطلب
    // ==========================================
    if (
      !currentUser.CenterId.HasValue ||
      currentUser.CenterId.Value != order.CenterId
    )
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.EmployeeMustBelongToOrderCenter
      );
    }

    // ==========================================
    // 8. بدء الطلب
    // ==========================================
    order.Status =
      OrderStatusEnum.InProgress;

    await _unitOfWork.Orders.UpdateAsync(order);

    await _unitOfWork.SaveChangesAsync();

    // ==========================================
    // 9. Response
    // ==========================================
    var data =
      _mapper.Map<OrdersResponse>(order);

    return Result<OrdersResponse>.Success(data);
  }

public async Task<Result<OrdersResponse>> CompleteAsync(
  int id,
  OrdersCompleteCommand command
)
{
  var currentUserId =
    _claimService.GetUserId();

  if (string.IsNullOrWhiteSpace(currentUserId))
  {
    return Result<OrdersResponse>.Failure(
      AuthErrors.Unauthorized
    );
  }

  var currentUser =
    await _unitOfWork.Users.GetFirstOrDefaultAsync(
      predicate: user => user.Id == currentUserId
    );

  if (currentUser == null)
  {
    return Result<OrdersResponse>.Failure(
      UserErrors.NotFound
    );
  }

  // فقط الموظف يستطيع إنهاء الطلب
  if (currentUser.Role != RoleEnum.Employee)
  {
    return Result<OrdersResponse>.Failure(
      AuthErrors.Unauthorized
    );
  }

  if (!currentUser.IsActive)
  {
    return Result<OrdersResponse>.Failure(
      UserErrors.InactiveUser
    );
  }

  var order =
    await _unitOfWork.Orders.GetFirstOrDefaultAsync(
      predicate: order => order.Id == id,
      queryBuilder: orders => orders
        .Include(order => order.OrderItems)
        .ThenInclude(orderItem => orderItem.ItemType)
    );

  if (order == null)
  {
    return Result<OrdersResponse>.Failure(
      OrdersErrors.NotFound
    );
  }

  // يجب أن يكون الطلب InProgress
  if (order.Status != OrderStatusEnum.InProgress)
  {
    return Result<OrdersResponse>.Failure(
      OrdersErrors.CannotCompleteAtCurrentStatus
    );
  }

  // نفس الموظف المعين على الطلب فقط
  if (
    string.IsNullOrWhiteSpace(order.EmployeeId) ||
    order.EmployeeId != currentUser.Id
  )
  {
    return Result<OrdersResponse>.Failure(
      OrdersErrors.NotAssignedToCurrentEmployee
    );
  }

  // الموظف يجب أن يكون من نفس مركز الطلب
  if (
    !currentUser.CenterId.HasValue ||
    currentUser.CenterId.Value != order.CenterId
  )
  {
    return Result<OrdersResponse>.Failure(
      OrdersErrors.EmployeeMustBelongToOrderCenter
    );
  }

  // يجب إرسال العناصر
  if (
    command.Items == null ||
    command.Items.Count == 0
  )
  {
    return Result<OrdersResponse>.Failure(
      OrdersErrors.CompleteItemsMismatch
    );
  }

  // ==========================================
  // التحقق من عدم تكرار نفس OrderItem
  // ==========================================

  var submittedIds =
    command.Items
      .Select(item => item.OrderItemId)
      .ToList();

  if (
    submittedIds.Distinct().Count() !=
    submittedIds.Count
  )
  {
    return Result<OrdersResponse>.Failure(
      OrdersErrors.CompleteItemsMismatch
    );
  }

  // ==========================================
  // التأكد من إرسال جميع عناصر الطلب بالضبط
  // ==========================================

  var actualIds =
    order.OrderItems
      .Select(item => item.Id)
      .OrderBy(itemId => itemId)
      .ToList();

  var requestedIds =
    submittedIds
      .OrderBy(itemId => itemId)
      .ToList();

  if (!actualIds.SequenceEqual(requestedIds))
  {
    return Result<OrdersResponse>.Failure(
      OrdersErrors.CompleteItemsMismatch
    );
  }

  // ==========================================
  // الوزن يجب أن يكون أكبر من صفر
  // ==========================================

  if (
    command.Items.Any(
      item => item.WeightKg <= 0
    )
  )
  {
    return Result<OrdersResponse>.Failure(
      OrdersErrors.InvalidItemWeight
    );
  }

  // ==========================================
  // Transaction
  // ==========================================

  await using var transaction =
    await _unitOfWork.BeginTransactionAsync();

  try
  {
    // ========================================
    // جلب الزبون صاحب الطلب
    // ========================================

    var client =
      await _unitOfWork.Users.GetFirstOrDefaultAsync(
        predicate: user =>
          user.Id == order.ClientId
      );

    if (client == null)
    {
      await transaction.RollbackAsync();

      return Result<OrdersResponse>.Failure(
        UserErrors.NotFound
      );
    }

    // ========================================
    // حساب نقاط كل مادة حسب الوزن
    // ========================================

    foreach (var orderItem in order.OrderItems)
    {
      var input =
        command.Items.First(
          item =>
            item.OrderItemId == orderItem.Id
        );

      orderItem.WeightKg =
        input.WeightKg;

      orderItem.Points =
        input.WeightKg *
        orderItem.PointsPerKg;
    }

    // ========================================
    // مجموع النقاط المكتسبة من الطلب
    // ========================================

    var totalPoints =
      order.OrderItems.Sum(
        orderItem => orderItem.Points
      );

    // ========================================
    // تحديث رصيد الزبون
    // ========================================

    var balanceBefore =
      client.PointsBalance;

    client.PointsBalance +=
      totalPoints;

    // ========================================
    // إنشاء حركة النقاط
    // ========================================

    var now =
      DateTime.UtcNow;

    var pointsTransaction =
      new PointsTransaction()
      {
        ClientId =
          client.Id,

        Type =
          PointsTransactionTypeEnum.OrderEarned,

        Points =
          totalPoints,

        BalanceBefore =
          balanceBefore,

        BalanceAfter =
          client.PointsBalance,

        OrderId =
          order.Id,

        ProductId =
          null,

        Quantity =
          null,

        CreatedAt =
          now
      };

    await _unitOfWork.PointsTransactions.AddAsync(
      pointsTransaction
    );

    // ========================================
    // إنهاء الطلب
    // ========================================

    order.Status =
      OrderStatusEnum.Completed;

    order.CompletedAt =
      now;

    // ========================================
    // حفظ كل شيء مرة واحدة
    // ========================================

    await _unitOfWork.SaveChangesAsync();

    await transaction.CommitAsync();

    // ========================================
    // Response
    // ========================================

    var data =
      _mapper.Map<OrdersResponse>(order);

    return Result<OrdersResponse>.Success(
      data
    );
  }
  catch
  {
    await transaction.RollbackAsync();

    throw;
  }
}

  public async Task<Result<OrdersResponse>> CancelAsync(
    int id,
    OrdersCancelCommand command
  )
  {
    // ==========================================
    // 1. التحقق من سبب الإلغاء
    // ==========================================
    if (string.IsNullOrWhiteSpace(command.Reason))
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.CancellationReasonRequired
      );
    }

    // ==========================================
    // 2. المستخدم الحالي
    // ==========================================
    var currentUserId =
      _claimService.GetUserId();

    if (string.IsNullOrWhiteSpace(currentUserId))
    {
      return Result<OrdersResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    var currentUser =
      await _unitOfWork.Users.GetFirstOrDefaultAsync(
        predicate: user => user.Id == currentUserId
      );

    if (currentUser == null)
    {
      return Result<OrdersResponse>.Failure(
        UserErrors.NotFound
      );
    }

    // ==========================================
    // 3. المستخدم يجب أن يكون فعالًا
    // ==========================================
    if (!currentUser.IsActive)
    {
      return Result<OrdersResponse>.Failure(
        UserErrors.InactiveUser
      );
    }

    // ==========================================
    // 4. الأدوار التي يسمح لها بالإلغاء
    // ==========================================
    if (
      currentUser.Role != RoleEnum.Admin &&
      currentUser.Role != RoleEnum.Manager &&
      currentUser.Role != RoleEnum.Employee &&
      currentUser.Role != RoleEnum.Client
    )
    {
      return Result<OrdersResponse>.Failure(
        AuthErrors.Unauthorized
      );
    }

    // ==========================================
    // 5. جلب الطلب
    // ==========================================
    var order =
      await _unitOfWork.Orders.GetFirstOrDefaultAsync(
        predicate: order => order.Id == id,
        queryBuilder: orders => orders
          .Include(order => order.OrderItems)
          .ThenInclude(orderItem => orderItem.ItemType)
      );

    if (order == null)
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.NotFound
      );
    }

    // ==========================================
    // 6. الطلب المكتمل أو الملغى
    // لا يمكن إلغاؤه
    // ==========================================
    if (
      order.Status == OrderStatusEnum.Completed ||
      order.Status == OrderStatusEnum.Cancelled
    )
    {
      return Result<OrdersResponse>.Failure(
        OrdersErrors.CannotCancelAtCurrentStatus
      );
    }

    OrderCancellationTypeEnum cancellationType;

    // ==========================================
    // 7. Client
    // ==========================================
    if (currentUser.Role == RoleEnum.Client)
    {
      // العميل يستطيع إلغاء طلبه فقط
      if (order.ClientId != currentUser.Id)
      {
        return Result<OrdersResponse>.Failure(
          AuthErrors.Unauthorized
        );
      }

      // فقط قبل أن يبدأ الموظف العمل
      if (
        order.Status != OrderStatusEnum.Pending &&
        order.Status != OrderStatusEnum.Assigned
      )
      {
        return Result<OrdersResponse>.Failure(
          OrdersErrors.CannotCancelAtCurrentStatus
        );
      }

      cancellationType =
        OrderCancellationTypeEnum.Client;
    }

    // ==========================================
    // 8. Manager
    // ==========================================
    else if (currentUser.Role == RoleEnum.Manager)
    {
      // المدير يجب أن يكون مرتبطًا بمركز
      if (!currentUser.CenterId.HasValue)
      {
        return Result<OrdersResponse>.Failure(
          CentersErrors.NoCenterForThisManagerYet
        );
      }

      // الطلب يجب أن يتبع مركز المدير
      if (
        currentUser.CenterId.Value != order.CenterId
      )
      {
        return Result<OrdersResponse>.Failure(
          AuthErrors.Unauthorized
        );
      }

      var center =
        await _unitOfWork.Centers.GetByIdAsync(
          order.CenterId
        );

      if (center == null)
      {
        return Result<OrdersResponse>.Failure(
          CentersErrors.NotFound
        );
      }

      // التأكد أنه المدير الفعلي للمركز
      if (center.ManagerId != currentUser.Id)
      {
        return Result<OrdersResponse>.Failure(
          CentersErrors.UserIsNotCenterManager
        );
      }

      // المدير يلغي فقط قبل بدء التنفيذ
      if (
        order.Status != OrderStatusEnum.Pending &&
        order.Status != OrderStatusEnum.Assigned
      )
      {
        return Result<OrdersResponse>.Failure(
          OrdersErrors.CannotCancelAtCurrentStatus
        );
      }

      cancellationType =
        OrderCancellationTypeEnum.Manager;
    }

    // ==========================================
    // 9. Employee
    // ==========================================
    else if (currentUser.Role == RoleEnum.Employee)
    {
      // الطلب يجب أن يكون معينًا لهذا الموظف
      if (
        string.IsNullOrWhiteSpace(order.EmployeeId) ||
        order.EmployeeId != currentUser.Id
      )
      {
        return Result<OrdersResponse>.Failure(
          OrdersErrors.NotAssignedToCurrentEmployee
        );
      }

      // الموظف يجب أن يبقى تابعًا لنفس المركز
      if (
        !currentUser.CenterId.HasValue ||
        currentUser.CenterId.Value != order.CenterId
      )
      {
        return Result<OrdersResponse>.Failure(
          OrdersErrors.EmployeeMustBelongToOrderCenter
        );
      }

      // الموظف يستطيع الإلغاء بعد التعيين
      // أو بعد بدء تنفيذ الطلب
      if (
        order.Status != OrderStatusEnum.Assigned &&
        order.Status != OrderStatusEnum.InProgress
      )
      {
        return Result<OrdersResponse>.Failure(
          OrdersErrors.CannotCancelAtCurrentStatus
        );
      }

      cancellationType =
        OrderCancellationTypeEnum.Employee;
    }

    // ==========================================
    // 10. Admin
    // ==========================================
    else
    {
      // وصلنا إلى هنا يعني Admin.
      // Completed و Cancelled تم منعهم مسبقًا.
      cancellationType =
        OrderCancellationTypeEnum.Admin;
    }

    // ==========================================
    // 11. تنفيذ الإلغاء
    // ==========================================
    order.Status =
      OrderStatusEnum.Cancelled;
    
    order.CanceledAt =
      DateTime.UtcNow;

    // الجهة تتحدد تلقائيًا
    order.CancellationType =
      cancellationType;

    // السبب يأتي كنص من المستخدم
    order.CancellationReason =
      command.Reason.Trim();

    await _unitOfWork.SaveChangesAsync();

    // ==========================================
    // 12. Response
    // ==========================================
    var data =
      _mapper.Map<OrdersResponse>(order);

    return Result<OrdersResponse>.Success(data);
  }

}