using LapisApi.App.Auth.Dto;
using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.MediaFiles.Enums;
using LapisApi.App.Users.Errors;
using LapisApi.Dto.Client;
using LapisApi.Helpers.Security;
using LapisApi.Interfaces.Auth;
using LapisApi.OptionConfigurations;
using LapisApi.Shared.Errors;
using LapisApi.Shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SisApi.App.Auth.Dto;
using SisApi.App.Auth.Enums;
using SisApi.App.Centers.Errors;
using SisApi.App.MediaFiles.Interfaces;
using SisApi.App.Users.Model;
using SisApi.Data;
using SisApi.Shared.Providers;
using System.IdentityModel.Tokens.Jwt;
namespace SisApi.App.Auth.Services;

public class AuthService : IAuthService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly IOptions<JwtOptions> _jwt;
  private readonly ApplicationDbContext _context;
  private readonly JwtProvider _jwtProvider;
  private readonly IFileService _fileService;
  private readonly IEmailService _emailService;
  private readonly IOtpService _otpService;
  private readonly IClaimService _claimService;
  private readonly IAttemptTrackerService _attemptTrackerService;

  public AuthService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ApplicationDbContext context,
    JwtProvider jwtProvider,
    IFileService fileService,
    IEmailService emailService,
    IOtpService otpService,
    IClaimService claimService,
    IAttemptTrackerService attemptTrackerService
  )
  {
    _userManager = userManager;
    _roleManager = roleManager;
    _context = context;
    _jwtProvider = jwtProvider;
    _fileService = fileService;
    _emailService = emailService;
    _otpService = otpService;
    _claimService = claimService;
    _attemptTrackerService = attemptTrackerService;
  }

  public async Task<Result<ClientRegisterResponse>> RegisterAsync(RegisterRequest model)
  {
    var date = DateTime.UtcNow;
    var userByEmail = await _userManager.FindByEmailAsync(model.Email);

    if (userByEmail is not null)
    {
      return
        Result<ClientRegisterResponse>.Failure(UserErrors.EmailAlreadyUsed);
    }

    var user = new ApplicationUser
    {
      CreatedAt = date,
      UserName = model.Email,
      Email = model.Email,
      FirstName = model.FirstName,
      LastName = model.LastName,
      PhoneNumber = model.PhoneNumber,
      IsActive = true,
      Role = RoleEnum.Client
    };

    var result = await _userManager.CreateAsync(user, model.Password);
    var result2 =
      await _userManager
        .AddToRoleAsync(
          user,
          nameof(RoleEnum.Client)
        );

    if (!result.Succeeded || !result2.Succeeded)
      Result<ClientRegisterResponse>.Failure(SharedErrors.CreateFailed);

    //var jwtSecurityToken = await _jwtProvider.CreateJwtToken(user);

    var data = new ClientRegisterResponse
    {
      Id = user.Id,
      Email = user.Email,
      EmailConfirmed = user.EmailConfirmed,
      Role = nameof(RoleEnum.Client),
      PhoneNumber = user.PhoneNumber,
      CreatedAt = user.CreatedAt
    };

    return Result<ClientRegisterResponse>.Success(data);
  }

  public async Task<Result<AuthResponse>> LoginAsync(LoginRequest model)
  {
    var user = await _userManager.Users
      .Include(o => o.Center)
      .FirstOrDefaultAsync(x => x.Email == model.Email); //await _userManager.FindByEmailAsyncElementType;

    var isLocked = await _attemptTrackerService.IsLockedAsync(model.Email);
    if (isLocked)
    {
      var waitTime = _attemptTrackerService.GetRemainingLockoutTime(model.Email);
      return Result<AuthResponse>.Failure(UserErrors.AccountLocked(waitTime));
    }


    if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
    {
      await _attemptTrackerService.RegisterAttemptAsync(model.Email);
      return
        Result<AuthResponse>.Failure(UserErrors.EmailOrPasswordIncorrect);
    }

    await _attemptTrackerService.ResetAttemptsAsync(model.Email);

    // if (user.EmailConfirmed == false)
    // {
    //   return
    //     Result<AuthResponse>.Failure(AuthErrors.EmailNotConfirmed);
    // }
    
    
    if (user.IsActive == false)
    {
      return Result<AuthResponse>.Failure(UserErrors.AccountDisabled);
    }
    
    
    var isManger = await _userManager.IsInRoleAsync(user, nameof(RoleEnum.Manager));
    if (isManger && user.CenterId is null)
    {
      {
        return Result<AuthResponse>.Failure(CentersErrors.NoCenterForThisManagerYet);
      }
    }

    var jwtSecurityToken = await _jwtProvider.CreateJwtToken(user);
    var rolesList = await _userManager.GetRolesAsync(user);

    var role = rolesList.FirstOrDefault();

    var files =
      await _fileService
        .GetFilesByEntityAsync(
          user.Id,
          AttachmentEntityType.User
        );

    var result =
      new AuthResponse
      {
        Id = user.Id,
        Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
        Email = user.Email,
        ExpiresOn = jwtSecurityToken.ValidTo,
        Role = role,
        PhoneNumber = user.PhoneNumber,
        Image = files.FirstOrDefault()
      };

    return Result<AuthResponse>.Success(result);
  }

  public async Task<Result<object>> ConfirmEmailAsync(ConfirmEmailRequest request, string acceptLanguage)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user == null)
      return Result<object>.Failure(UserErrors.NotFound);

    var result = await _otpService.VerifyOtpAsync(request.Email, request.Code);
    if (!result.IsSuccess)
    {
      return result;
    }


    user.EmailConfirmed = true;
    await _userManager.UpdateAsync(user);

    return Result<object>.Success("✅");
  }
  public async Task<Result<object>> ResetPasswordByOtpAsync(ResetPasswordRequest model)
  {
    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null)
      return Result<object>.Failure(UserErrors.NotFound);

    var result = await _otpService.VerifyOtpAsync(model.Email, model.Code);
    if (!result.IsSuccess)
      return result;

    var resetResult = await _userManager.RemovePasswordAsync(user);
    if (!resetResult.Succeeded)
      return Result<object>.Failure(UserErrors.PasswordResetFailed);

    var addPassResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
    if (!addPassResult.Succeeded)
      return Result<object>.Failure(UserErrors.PasswordResetFailed);

    return Result<object>.Success("✅");
  }


  public async Task<Result<bool>> ToggleTwoFactorAsync(ToggleTwoFactorRequest request)
  {
    var userId = _claimService.GetUserId();
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null)
      return Result<bool>.Failure(UserErrors.NotFound);

    //
    // var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
    // if (!passwordValid)
    //   return Result<bool>.Failure(UserErrors.EmailOrPasswordIncorrect);

    user.TwoFactorEnabled = !user.TwoFactorEnabled;
    await _userManager.UpdateAsync(user);

    return Result<bool>.Success(user.TwoFactorEnabled);
  }
}