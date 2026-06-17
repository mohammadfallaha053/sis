using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LapisApi.App.Auth.Enums;
using LapisApi.App.Users.Model;
using LapisApi.OptionConfigurations;
namespace LapisApi.Shared.Providers
{
  public class JwtProvider
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _JwtOptions;

    public JwtProvider(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions)
    {
      _userManager = userManager;
      _JwtOptions = jwtOptions.Value;
    }

    public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
      var userClaims = await _userManager.GetClaimsAsync(user);
      var roles = await _userManager.GetRolesAsync(user);

      var roleClaims = roles.Select(role => new Claim("roles", role)).ToList();

      var claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim("uid", user.Id)
      };

      if (roles.Contains(nameof(RoleEnum.Client)) && !string.IsNullOrEmpty(user.CenterId))
      {
        claims.Add(new Claim("centerId", user.CenterId));
      }

      claims.AddRange(userClaims);
      claims.AddRange(roleClaims);

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtOptions.Key));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      return new JwtSecurityToken(
        issuer: _JwtOptions.Issuer,
        audience: _JwtOptions.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_JwtOptions.ExpiryMinutes),
        signingCredentials: credentials
      );
    }


    public string GenerateRefreshToken()
    {
      return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = false,
        ValidIssuer = _JwtOptions.Issuer,
        ValidAudience = _JwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JwtOptions.Key))
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

      if (securityToken is not JwtSecurityToken jwtToken ||
          !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
      {
        throw new SecurityTokenException("Invalid token");
      }

      return principal;
    }
  }
}