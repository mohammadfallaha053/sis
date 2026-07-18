using LapisApi.App.Users.Dto;
using LapisApi.App.Users.Dto.Request.Commands;
using LapisApi.App.Users.Dto.Request.Queries;
using SisApi.App.Users.Dto.Request.Commands;
using SisApi.App.Users.Dto.Response;
namespace SisApi.App.Users.Interfaces;

public interface IUserService
{
  Task<Result<IEnumerable<UserResponse>>> GetAllUsersAsync(UserGetAllQuery getAllQuery);
  Task<Result<UserResponse>> InsertUserAsync(CreateUserRequest request);
  Task<Result<object>> UpdateUserAsync(UpdateUserRequest request);
  Task<Result<object>> DisableUserAsync(DisableAgentRequest request);
  Task<Result<object>> AddContactUsAsync(ContactUsCommand request);
  Task<Result<object>> ChangePasswordAsync(ChangePasswordRequest request);
  Task<bool> UserExistsAsync(string userId);
  Task<Result<UserResponse>> GetUserByIdAsync(string id);
}