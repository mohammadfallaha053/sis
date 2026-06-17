using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.BackgroundJobs.Enums;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.BackgroundJobs.Jobs.Payloads;
using LapisApi.App.Centers.Errors;
using LapisApi.App.MediaFiles.Dto;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.MediaFiles.Interfaces;
using LapisApi.App.Users.Dto;
using LapisApi.App.Users.Dto.Request.Commands;
using LapisApi.App.Users.Dto.Request.Queries;
using LapisApi.App.Users.Dto.Response;
using LapisApi.App.Users.Enums;
using LapisApi.App.Users.Errors;
using LapisApi.App.Users.Interfaces;
using LapisApi.App.Users.Model;
using LapisApi.Data;
using LapisApi.Data.Interfaces;
using LapisApi.Helpers;
using LapisApi.Helpers.Responses;
using LapisApi.Interfaces.Auth;
using LapisApi.Shared.Errors;
using LapisApi.Shared.Services;
namespace LapisApi.App.Users.Services;

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
      // queryBuilder: o => o.Include(o => o.MediaFiles)
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
    var isAdmin = await _claimService.IsAdminAsync();
    if (!isAdmin)
      return Result<object>.Failure(AuthErrors.Unauthorized);

    var user = await _unitOfWork.Users.GetFirstOrDefaultAsync(
      predicate: u => u.Email == request.Email
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


  public async Task<Result<UserResponse>> InsertAgentAsync(CreateAgentRequest request)
  {
    var existingUser = await _userManager.FindByEmailAsync(request.Email);
    if (existingUser != null)
    {
      return
        Result<UserResponse>.Failure(UserErrors.EmailAlreadyUsed);
    }

    var center = await _unitOfWork.Centers.GetByIdAsync(request.CenterId);
    if (center == null)
    {
      return
        Result<UserResponse>.Failure(CenterErrors.NotFound);
    }


    var user = new ApplicationUser
    {
      FirstName = "FirstName",
      PhoneNumber = null,
      LastName = "LastName",
      UserName = request.Email,
      Email = request.Email,
      CreatedAt = DateTime.UtcNow,
      CenterId = request.CenterId,
      IsActive = true,
      EmailConfirmed = true,
      Role = RoleEnum.Client
    };

    var result = await _userManager.CreateAsync(user, "123456");
    if (!result.Succeeded)
    {
      return
        Result<UserResponse>.Failure(SharedErrors.CreateFailed);
    }

    var roleResult =
      await _userManager.AddToRoleAsync(
        user,
        nameof(RoleEnum.Client)
      );

    var response = new UserResponse()
    {
      Id = user.Id,
      Email = user.Email,
      FirstName = user.FirstName,
      LastName = user.LastName,
      PhoneNumber = user.PhoneNumber,
      CreatedAt = user.CreatedAt,
      Role = user.Role.ToString(),
    };

    center.AgentsCount++;

    await _unitOfWork.SaveChangesAsync();

    return Result<UserResponse>.Success(response);
  }

  public async Task<bool> UserExistsAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    return user != null;
  }
}