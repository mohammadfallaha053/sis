using AutoMapper;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.MediaFiles.Dto;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.Centers.Enums;
using LapisApi.App.Users.Errors;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Auth.Enums;
using SisApi.App.Centers.Dto.Request.Commands;
using SisApi.App.Centers.Dto.Request.Queries;
using SisApi.App.Centers.Dto.Response;
using SisApi.App.Centers.Errors;
using SisApi.App.Centers.Interfaces;
using SisApi.App.Centers.Model;
using SisApi.App.MediaFiles.Interfaces;
using SisApi.App.Users.Interfaces;
using SisApi.App.Users.Model;
using SisApi.Data.Interfaces;
using System.Linq.Expressions;
namespace LapisApi.App.Centers.Services;

public class CentersService : ICentersService
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;
  private readonly IUserService _userService;
  private readonly IFileService _fileService;
  private readonly UserManager<ApplicationUser> _userManager;


  public CentersService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IClaimService claimService,
    IUserService userService,
    IFileService fileService,
    UserManager<ApplicationUser> userManager
  )
  {
    _unitOfWork = unitOfWork;
    _mapper = mapper;
    _claimService = claimService;
    _userService = userService;
    _fileService = fileService;
    _userManager = userManager;
  }

  public async Task<Result<CentersResponse>> AddAsync(
    CentersCreateCommand command
  )
  {
    await using var transaction =
      await _unitOfWork.BeginTransactionAsync();

    try
    {
      var model = _mapper.Map<Center>(command);

      /*
       * المركز يمكن إنشاؤه دون مدير.
       * كما أن AutoMapper يتجاهل ManagerId.
       */
      model.ManagerId = null;
      model.Manager = null;

      model =
        await _unitOfWork.Centers.AddAsync(model);

      /*
       * نحفظ أولًا حتى يحصل المركز على Id.
       */
      await _unitOfWork.SaveChangesAsync();

      /*
       * إذا تم إرسال ManagerId نتحقق منه ثم نربطه.
       * إذا كان null يبقى المركز دون مدير.
       */
      var assignManagerResult =
        await ApplyManagerAsync(
          center: model,
          managerId: command.ManagerId,
          clearManagerWhenNull: false
        );

      if (!assignManagerResult.IsSuccess)
      {
        await transaction.RollbackAsync();

        return Result<CentersResponse>.Failure(
          assignManagerResult.Error
        );
      }

      await _unitOfWork.SaveChangesAsync();

      var createdCenter =
        await _unitOfWork.Centers.GetFirstOrDefaultAsync(
          center => center.Id == model.Id,
          queryBuilder: query =>
            query.Include(center => center.Manager)
        );

      await transaction.CommitAsync();

      var data =
        _mapper.Map<CentersResponse>(createdCenter);

      return Result<CentersResponse>.Success(data);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
  public async Task<Result<CentersResponse>> UpdateAsync(
    int id,
    CentersUpdateCommand command
  )
  {
    await using var transaction =
      await _unitOfWork.BeginTransactionAsync();

    try
    {
      var center =
        await _unitOfWork.Centers.GetFirstOrDefaultAsync(
          item => item.Id == id,
          queryBuilder: query =>
            query.Include(item => item.Manager)
        );

      if (center is null)
      {
        await transaction.RollbackAsync();

        return Result<CentersResponse>.Failure(
          CentersErrors.NotFound
        );
      }

      /*
       * ManagerId لن يتم تغييره بواسطة AutoMapper
       * بسبب Ignore الموجود في CentersProfile.
       */
      _mapper.Map(command, center);

      var assignManagerResult =
        await ApplyManagerAsync(
          center: center,
          managerId: command.ManagerId,
          clearManagerWhenNull: true
        );

      if (!assignManagerResult.IsSuccess)
      {
        await transaction.RollbackAsync();

        return Result<CentersResponse>.Failure(
          assignManagerResult.Error
        );
      }

      await _unitOfWork.Centers.UpdateAsync(center);
      await _unitOfWork.SaveChangesAsync();

      var updatedCenter =
        await _unitOfWork.Centers.GetFirstOrDefaultAsync(
          item => item.Id == center.Id,
          queryBuilder: query =>
            query.Include(item => item.Manager)
        );

      await transaction.CommitAsync();

      var data =
        _mapper.Map<CentersResponse>(updatedCenter);

      return Result<CentersResponse>.Success(data);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
  public async Task<Result<IEnumerable<CentersResponse>>> GetAllAsync(
    CentersGetAllQuery query
  )
  {
    Expression<Func<Center, bool>> predicate =
      c =>
      (
        string.IsNullOrEmpty(query.Search)
      );

    if (query.IsActive != null)
    {
      predicate = predicate.And(c => c.IsActive == query.IsActive);
    }

    var sortFunc =
      SortHelper.BuildSort<Center, CentersSortFieldEnum>(
        query.Sort
      );

    var pagedResult =
      await _unitOfWork.Centers.GetPagedAsync(
        predicate: predicate,
        pageNumber: query.PageNumber,
        pageSize: query.PageSize,
        sort: sortFunc,
        queryBuilder:
        o => o.Include(o => o.Manager)
      );

    var specialists = pagedResult.Data.ToList();

    var data = _mapper.Map<List<CentersResponse>>(specialists);

    var paging = new AppPaging
    {
      PageNumber = query.PageNumber,
      PageSize = query.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<CentersResponse>>.Success(data, paging);
  }
  public async Task<Result<CentersResponse>> GetByIdAsync(int id)
  {
    var Centers =
      await _unitOfWork.Centers
        .GetFirstOrDefaultAsync(
          o => o.Id == id,
          queryBuilder:
          o => o.Include(o => o.Manager)
        );

    if (Centers == null)
    {
      return Result<CentersResponse>.Failure(CentersErrors.NotFound);
    }

    var data = _mapper.Map<CentersResponse>(Centers);

    return Result<CentersResponse>.Success(data);
  }

  public async Task<Result<object>> DeleteAsync(int id)
  {
    await using var transaction = await _unitOfWork.BeginTransactionAsync();

    try
    {
      var Centers =
        await _unitOfWork.Centers.GetByIdAsync(id);

      if (Centers == null)
      {
        await transaction.RollbackAsync();
        return Result<object>.Failure(CentersErrors.NotFound);
      }

      var files =
        await _fileService.GetFilesByEntityAsync(
          entityId: Centers.Id.ToString(),
          entityType: AttachmentEntityType.Center
        );

      foreach (var file in files)
      {
        var deleteFileResult =
          await _fileService.DeleteFileAsync(file.Id);

        if (!deleteFileResult.IsSuccess)
        {
          await transaction.RollbackAsync();
          return Result<object>.Failure(deleteFileResult.Error);
        }
      }

      await _unitOfWork.Centers.RemoveAsync(Centers);

      await _unitOfWork.SaveChangesAsync();

      await transaction.CommitAsync();

      return Result<object>.Success(null);
    }
    catch
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
  
  private async Task<Result<object>> ApplyManagerAsync(
    Center center,
    string? managerId,
    bool clearManagerWhenNull
  )
  {
    /*
     * عند إنشاء مركز دون مدير:
     * لا نفعل شيئًا.
     *
     * عند تحديث المركز وإرسال null:
     * نزيل المدير الحالي.
     */
    if (string.IsNullOrWhiteSpace(managerId))
    {
      if (clearManagerWhenNull)
      {
        center.ManagerId = null;
        center.Manager = null;
      }

      return Result<object>.Success(null);
    }

    var manager =
      await _unitOfWork.Users.GetByIdAsync(managerId);

    if (manager is null)
    {
      return Result<object>.Failure(
        UserErrors.NotFound
      );
    }

    var isManager =
      await _userManager.IsInRoleAsync(
        manager,
        RoleEnum.Manager.ToString()
      );

    if (!isManager)
    {
      return Result<object>.Failure(
        CentersErrors.UserIsNotManager
      );
    }

    /*
     * لا ننقل المستخدم تلقائيًا من مركز إلى آخر.
     * يجب أولًا فك ارتباطه بالمركز السابق.
     */
    if (
      manager.CenterId.HasValue &&
      manager.CenterId.Value != center.Id
    )
    {
      return Result<object>.Failure(
        CentersErrors.ManagerBelongsToAnotherCenter
      );
    }

    /*
     * التأكد أنه ليس مديرًا لمركز آخر.
     */
    var managedCenter =
      await _unitOfWork.Centers.GetFirstOrDefaultAsync(
        currentCenter =>
          currentCenter.ManagerId == manager.Id &&
          currentCenter.Id != center.Id
      );

    if (managedCenter is not null)
    {
      return Result<object>.Failure(
        CentersErrors.ManagerAlreadyAssigned
      );
    }

    /*
     * المدير يجب أن يكون أيضًا موظفًا تابعًا للمركز.
     */
    manager.CenterId = center.Id;

    center.ManagerId = manager.Id;
    center.Manager = manager;

    await _unitOfWork.Users.UpdateAsync(manager);

    return Result<object>.Success(null);
  }
}