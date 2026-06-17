namespace LapisApi.Helpers;

using Microsoft.AspNetCore.Identity;

public static class SecretCodeHasher
{
  private static readonly PasswordHasher<object> _hasher = new();

  public static string Hash(string secretCode)
  {
    return _hasher.HashPassword(null, secretCode);
  }

  public static bool Verify(string hashedCode, string plainCode)
  {
    var result = _hasher.VerifyHashedPassword(null, hashedCode, plainCode);
    return result == PasswordVerificationResult.Success;
  }
}