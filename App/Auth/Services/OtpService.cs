using JWT53.MyEnum.Email;
using Microsoft.AspNetCore.Identity;
using LapisApi.App.Auth.Dto;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Auth.Errors;
using LapisApi.App.Auth.Interfaces;
using LapisApi.App.Users.Errors;
using LapisApi.App.Users.Model;
using LapisApi.Helpers.Extensions;
using LapisApi.Helpers.Security;
using LapisApi.Shared.EmailTemplates;
using LapisApi.Shared.Services;

public class OtpService : IOtpService
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEmailService _emailService;
  private readonly IAttemptTrackerService _attemptTrackerService;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public OtpService(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IAttemptTrackerService attemptTrackerService,
    IHttpContextAccessor httpContextAccessor
  )
  {
    _userManager = userManager;
    _emailService = emailService;
    _attemptTrackerService = attemptTrackerService;
    _httpContextAccessor = httpContextAccessor;
  }

  public async Task<Result<object>> SendOtpAsync(SendOtpRequest model, string lang)
  {
    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null)
      return Result<object>.Failure(UserErrors.NotFound);

    var ip = _httpContextAccessor.HttpContext?.GetClientIp();

    var ipKey = $"otp:ip:{ip}";
    var emailKey = $"otp:email:{model.Email}";

    var isEmailLimited = await _attemptTrackerService.IsLimitedAsync(
      emailKey,
      maxAttempts: 1,
      window: TimeSpan.FromMinutes(10),
      lockoutDuration: TimeSpan.FromMinutes(10)
    );

    if (isEmailLimited)
    {
      var remaining = _attemptTrackerService.GetRemainingLockoutTime(emailKey);
      return Result<object>.Failure(AuthErrors.TooManyRequestsWithTime((int)remaining.TotalSeconds));
    }

    // 2. تحقق من حظر IP بالطريقة العادية
    if (await _attemptTrackerService.IsLockedAsync(ipKey))
      return Result<object>.Failure(AuthErrors.TooManyRequests);

    // 3. سجل المحاولات
    await _attemptTrackerService.RegisterAttemptAsync(ipKey);

    // 4. أرسل الكود
    var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
    await _emailService.SendOtpEmail(model.Email, model.Purpose, code, lang);

    return Result<object>.Success("✅");
  }


  public async Task<Result<object>> VerifyOtpAsync(string email, string code)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
      return Result<object>.Failure(UserErrors.NotFound);

    var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
    if (!isValid)
      return Result<object>.Failure(AuthErrors.InvalidVerificationCode);

    return Result<object>.Success("✅");
  }
}