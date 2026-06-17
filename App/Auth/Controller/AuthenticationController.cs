using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LapisApi.App.Auth.Dto;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Auth.Interfaces;
using LapisApi.Dto.Auth;
using LapisApi.Filter;
using LapisApi.Interfaces.Auth;
namespace LapisApi.App.Auth.Controller
{
  [Route("api/auth")]
  [ApiController]
  public class AuthenticationController : ControllerBase
  {
    private readonly IAuthService _authService;
    private readonly IOtpService _otpService;

    public AuthenticationController(
      IAuthService authService,
      IOtpService otpService
    )
    {
      _authService = authService;
      _otpService = otpService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
      var result = await _authService.RegisterAsync(model);
      return result.ToActionResult(this);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
      var result = await _authService.LoginAsync(model);
      return result.ToActionResult(this);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
      var acceptLanguage = Request.Headers["Accept-Language"].FirstOrDefault() ?? "en";
      var result = await _authService.ConfirmEmailAsync(request, acceptLanguage);
      return result.ToActionResult(this);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordByOtp([FromBody] ResetPasswordRequest model)
    {
      var result = await _authService.ResetPasswordByOtpAsync(model);
      return result.ToActionResult(this);
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest model)
    {
      var lang = HttpContext.Request.Headers["Accept-Language"].ToString() ?? "en";
      var result = await _otpService.SendOtpAsync(model, lang);
      return result.ToActionResult(this);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpRequest request)
    {
      var result = await _otpService.VerifyOtpAsync(request.Email, request.Code);
      return result.ToActionResult(this);
    }
    [ServiceFilter(typeof(ActiveUserAuthorizationFilter))]
    [Authorize(Roles = nameof(RoleEnum.Client))]
    [HttpPost("toggle-2fa")]
    public async Task<IActionResult> ToggleTwoFactor([FromBody] ToggleTwoFactorRequest request)
    {
      var result = await _authService.ToggleTwoFactorAsync(request);
      return result.ToActionResult(this);
    }
  }
}