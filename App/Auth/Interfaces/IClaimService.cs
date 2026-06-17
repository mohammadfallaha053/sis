namespace LapisApi.App.Auth.Interfaces;

public interface IClaimService
{
  string? GetUserId();
  string? GetCenterId();
  string? GetEmail();
  bool IsAuthenticated();

  Task<bool> IsAdminAsync();
 
  IEnumerable<string> GetRoles();
}