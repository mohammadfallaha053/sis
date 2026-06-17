using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using LapisApi.OptionConfigurations;
namespace LapisApi.Helpers.Security;

public class SecretCodeSignatureHelper : ISecretCodeSignatureHelper
{
  private readonly string _secretKey;

  public SecretCodeSignatureHelper(IOptions<SecuritySettings> options)
  {
    _secretKey = options.Value.HmacSecretKey;
  }

  public string GenerateSignature(string code, string nationalId, string firstName, string lastName)
  {
    var input = $"{code}:{nationalId}:{firstName}:{lastName}";
    var keyBytes = Encoding.UTF8.GetBytes(_secretKey);
    var inputBytes = Encoding.UTF8.GetBytes(input);

    using var hmac = new HMACSHA256(keyBytes);
    var hashBytes = hmac.ComputeHash(inputBytes);

    return Convert.ToBase64String(hashBytes);
  }

  public bool VerifySignature(string code, string nationalId, string firstName, string lastName, string signature)
  {
    var expected = GenerateSignature(code, nationalId, firstName, lastName);
    return expected == signature;
  }
}
