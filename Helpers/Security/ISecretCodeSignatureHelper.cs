namespace LapisApi.Helpers.Security;

public interface ISecretCodeSignatureHelper
{
  string GenerateSignature(string code, string nationalId, string firstName, string lastName);
  bool VerifySignature(string code, string nationalId, string firstName, string lastName, string signature);
}