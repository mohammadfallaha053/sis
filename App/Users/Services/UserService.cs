using AutoMapper;
using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.BackgroundJobs.Enums;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.BackgroundJobs.Jobs.Payloads;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.Users.Dto;
using LapisApi.App.Users.Dto.Request.Commands;
using LapisApi.App.Users.Dto.Request.Queries;
using LapisApi.App.Users.Enums;
using LapisApi.App.Users.Errors;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LapisApi.Shared.Errors;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SisApi.App.Auth.Enums;
using SisApi.App.Centers.Errors;
using SisApi.App.MediaFiles.Interfaces;
using SisApi.App.Users.Dto.Request.Commands;
using SisApi.App.Users.Dto.Response;
using SisApi.App.Users.Interfaces;
using SisApi.App.Users.Model;
using SisApi.Data;
using SisApi.Data.Interfaces;
using System.Linq.Expressions;
namespace SisApi.App.Users.Services;

public class UserService : IUserService
{
  private readonly long _maxImageSize = 2 * 1024 * 1024; // 2 ميجابايت

  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly ApplicationDbContext _context;
  private readonly IWebHostEnvironment _environment;
  private readonly IMapper _mapper;
  private readonly IClaimService _claimService;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IFileService _fileService;
  private readonly IBackgroundJobService _backgroundJobService;


  public UserService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ApplicationDbContext context,
    IWebHostEnvironment environment,
    IClaimService claimService,
    IUnitOfWork unitOfWork,
    IFileService fileService,
    IBackgroundJobService backgroundJobService
  )
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _context = context;
    _environment = environment;
    _claimService = claimService;
    _unitOfWork = unitOfWork;
    _fileService = fileService;
    _backgroundJobService = backgroundJobService;
  }

  public async Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(UserGetAllQuery getAllQuery)
  {
    Expression<Func<ApplicationUser, bool>> predicate = user =>
      string.IsNullOrEmpty(getAllQuery.Search) ||
      user.FirstName.Contains(getAllQuery.Search) ||
      user.LastName.Contains(getAllQuery.Search) ||
      user.Email.Contains(getAllQuery.Search);

    predicate = predicate.And(user => user.Role != RoleEnum.Admin);
    
    var isAdmin = await _claimService.IsAdminAsync();
    if (!isAdmin)
    {
      var curentUserId = _claimService.GetUserId();
      var currentUser = await _userManager.FindByIdAsync(curentUserId);
      predicate = predicate.And(user => user.CenterId == currentUser.CenterId);
      predicate = predicate.And(user => user.Role == RoleEnum.Employee);
    }
    
    if (getAllQuery.IsActive != null)
    {
      predicate = predicate.And(user => user.IsActive == getAllQuery.IsActive);
    }


    var sortFunc = SortHelper.BuildSort<ApplicationUser, UserSortField>(getAllQuery.Sort);

    var pagedResult = await _unitOfWork.Users.GetPagedAsync(
      predicate: predicate,
      pageNumber: getAllQuery.PageNumber,
      pageSize: getAllQuery.PageSize,
      sort: sortFunc
    );

    var result =
      pagedResult
        .Data
        .Select(
          user => new UserResponse
          {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber!,
            CreatedAt = user.CreatedAt,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            Image = null
          }
        ).ToList();

    var paging = new AppPaging
    {
      PageNumber = getAllQuery.PageNumber,
      PageSize = getAllQuery.PageSize,
      TotalRecords = pagedResult.TotalRecords
    };

    return Result<IEnumerable<UserResponse>>.Success(result, paging);
  }

  public async Task<Result<object>> UpdateUserAsync(UpdateUserRequest request)
  {
    var userId = _claimService.GetUserId();
    if (userId == null)
      return Result<object>.Failure(AuthErrors.Unauthorized);

    var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(
      predicate: u => u.Id == userId
      // queryBuilder: o => o.Include(o => o.MediaFiles)
    );

    if (user == null)
      return Result<object>.Failure(UserErrors.NotFound);

    var oldFileIds =
      await _fileService.GetFilesByEntityAsync(
        entityId: user.Id,
        entityType: AttachmentEntityType.User
      );

    int? oldFileId = oldFileIds.Count == 0 ? null : oldFileIds.FirstOrDefault().Id;

    var fileResult = await _fileService.ProcessFileUpdateAsync(
      newFileId: request.FileId,
      oldFileId: oldFileId,
      entityType: AttachmentEntityType.User,
      entityId: user.Id
    );

    if (!fileResult.IsSuccess)
      return Result<object>.Failure(fileResult.Error);

    // تعديل بيانات المستخدم
    user.FirstName = request.FirstName;
    user.LastName = request.LastName;
    user.PhoneNumber = request.PhoneNumber;


    await _unitOfWork.Users.UpdateAsync(user);
    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(null);
  }

  public async Task<Result<object>> DisableUserAsync(DisableAgentRequest request)
  {
    var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(
      predicate: u => u.Id == request.UserId
    );
    // تعديل بيانات المستخدم
    user.IsActive = user.IsActive ? false : true;

    await _unitOfWork.Users.UpdateAsync(user);
    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(new
      {
        user.IsActive
      }
    );
  }
  public async Task<Result<object>> AddContactUsAsync(ContactUsCommand request)
  {
    await _backgroundJobService.EnqueueAsync(
      jobType: BackgroundJobTypes.SendEmailAfterContactUs,
      payload: new SendEmailAfterContactUsPayload
      {
        Email = request.Email,
        FullName = request.FullName,
        PhoneNumber = request.PhoneNumber,
        JobType = request.JobType,
        Message = request.Message,
        IsAgent = request.IsAgent,
      }
    );

    await _unitOfWork.SaveChangesAsync();

    return Result<object>.Success(null);
  }

  public async Task<Result<UserResponse>> GetUserByIdAsync(string id)
  {
    var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(u => u.Id == id);

    if (user == null)
    {
      return Result<UserResponse>.Failure(UserErrors.NotFound);
    }

    var mediaFiles = await _fileService.GetFilesByEntityAsync(
      entityId: id,
      entityType: AttachmentEntityType.User
    );

    var dto = new UserResponse
    {
      Id = user.Id,
      Email = user.Email!,
      FirstName = user.FirstName,
      LastName = user.LastName,
      PhoneNumber = user.PhoneNumber!,
      CreatedAt = user.CreatedAt,
      Role = user.Role.ToString(),
      IsActive = user.IsActive,
      Image = mediaFiles.FirstOrDefault()
    };

    return Result<UserResponse>.Success(dto);
  }

  public async Task<int> GetTotalUsersCountAsync()
  {
    return await _userManager.Users.CountAsync();
  }
  public async Task<int> GetUsersCountByRoleAsync(string roleName)
  {
    var role = await _roleManager.FindByNameAsync(roleName);
    if (role == null)
    {
      throw new Exception($"Role '{roleName}' not found.");
    }

    var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
    return usersInRole.Count;
  }

  public async Task<Result<object>> ChangePasswordAsync(ChangePasswordRequest request)
  {
    var userId = _claimService.GetUserId();

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
    {
      Result<object>.Failure(UserErrors.NotFound)
        ;
    }

    var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
    if (!result.Succeeded)
    {
      return
        Result<object>.Failure(UserErrors.PasswordChangeFailed);
    }

    return Result<object>.Success(null);
  }
  
public async Task<Result<UserResponse>> InsertUserAsync(
  CreateUserRequest request
)
{
  var existingUser =
    await _userManager.FindByEmailAsync(request.Email);

  if (existingUser is not null)
  {
    return Result<UserResponse>.Failure(
      UserErrors.EmailAlreadyUsed
    );
  }

  var currentUserId = _claimService.GetUserId();

  var currentUser =
    await _userManager.FindByIdAsync(currentUserId);

  if (currentUser is null)
  {
    return Result<UserResponse>.Failure(
      UserErrors.NotFound
    );
  }

  if (!currentUser.IsActive)
  {
    return Result<UserResponse>.Failure(
      UserErrors.InactiveUser
    );
  }

  var isAdmin =
    await _userManager.IsInRoleAsync(
      currentUser,
      RoleEnum.Admin.ToString()
    );

  var isManager =
    await _userManager.IsInRoleAsync(
      currentUser,
      RoleEnum.Manager.ToString()
    );

  /*
   * حماية إضافية داخل الخدمة.
   * لا نعتمد فقط على Authorize الموجود في Controller.
   */
  if (!isAdmin && !isManager)
  {
    return Result<UserResponse>.Failure(
      UserErrors.NotAllowedToCreateUsers
    );
  }

  RoleEnum newUserRole;
  int? centerId;

  if (isAdmin)
  {
    /*
     * الأدمن ينشئ مديرًا.
     * يسمح بإنشاء المدير دون مركز،
     * ثم يتم تعيينه لمركز لاحقًا.
     */
    newUserRole = RoleEnum.Manager;
    centerId = null;
  }
  else
  {
    /*
     * المدير ينشئ موظفًا حصراً.
     */
    newUserRole = RoleEnum.Employee;

    /*
     * لا نسمح للمدير بإنشاء موظف
     * إن لم يكن مرتبطًا بمركز.
     */
    if (!currentUser.CenterId.HasValue)
    {
      return Result<UserResponse>.Failure(
        CentersErrors.NoCenterForThisManagerYet
      );
    }

    var center =
      await _unitOfWork.Centers.GetFirstOrDefaultAsync(
        item => item.Id == currentUser.CenterId.Value
      );

    if (center is null)
    {
      return Result<UserResponse>.Failure(
        CentersErrors.NotFound
      );
    }

    /*
     * لا يكفي أن يكون المستخدم تابعًا للمركز؛
     * يجب أن يكون المدير المعين رسميًا لهذا المركز.
     */
    if (center.ManagerId != currentUser.Id)
    {
      return Result<UserResponse>.Failure(
        CentersErrors.UserIsNotCenterManager
      );
    }

    centerId = center.Id;
  }

  await using var transaction =
    await _unitOfWork.BeginTransactionAsync();

  try
  {
    var user = new ApplicationUser
    {
      FirstName = "FirstName",
      LastName = "LastName",
      PhoneNumber = null,

      UserName = request.Email,
      Email = request.Email,

      CreatedAt = DateTime.UtcNow,

      /*
       * Manager أنشأه Admin:
       * CenterId = null.
       *
       * Employee أنشأه Manager:
       * CenterId = مركز المدير.
       */
      CenterId = centerId,

      IsActive = true,
      EmailConfirmed = true,
      Role = newUserRole
    };

    var createResult =
      await _userManager.CreateAsync(
        user,
        "123456"
      );

    if (!createResult.Succeeded)
    {
      await transaction.RollbackAsync();

      return Result<UserResponse>.Failure(
        SharedErrors.CreateFailed
      );
    }

    var roleResult =
      await _userManager.AddToRoleAsync(
        user,
        newUserRole.ToString()
      );

    /*
     * في كودك السابق لم يتم فحص نتيجة إضافة الدور.
     */
    if (!roleResult.Succeeded)
    {
      await transaction.RollbackAsync();

      return Result<UserResponse>.Failure(
        UserErrors.AddRoleFailed
      );
    }

    await _unitOfWork.SaveChangesAsync();
    await transaction.CommitAsync();

    var response = new UserResponse
    {
      Id = user.Id,
      Email = user.Email,
      FirstName = user.FirstName,
      LastName = user.LastName,
      PhoneNumber = user.PhoneNumber,
      CreatedAt = user.CreatedAt,
      Role = user.Role.ToString(),
      CenterId = user.CenterId
    };

    return Result<UserResponse>.Success(response);
  }
  catch
  {
    await transaction.RollbackAsync();
    throw;
  }
}
  public async Task<bool> UserExistsAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    return user != null;
  }
}