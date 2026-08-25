using LapisApi.Helpers.Responses;
using SisApi.App.Orders.Dto.Request.Commands;
using SisApi.App.Orders.Dto.Request.Queries;
using SisApi.App.Orders.Dto.Response;

namespace SisApi.App.Orders.Interfaces;

public interface IOrdersService
{
  Task<Result<OrdersResponse>> AddAsync(
    OrdersCreateCommand command
  );

  Task<Result<IEnumerable<OrdersResponse>>> GetAllAsync(
    OrdersGetAllQuery query
  );

  // Task<Result<OrdersResponse>> GetByIdAsync(
  //   int id
  // );
  //
  // Task<Result<object>> DeleteAsync(
  //   int id
  // );
  //
  
  Task<Result<OrdersResponse>> StartAsync(int id);
  Task<Result<OrdersResponse>> AssignEmployeeAsync(
    int id,
    OrdersAssignEmployeeCommand command
  );
  
  Task<Result<OrdersResponse>> CompleteAsync(
    int id,
    OrdersCompleteCommand command
  );
  
  Task<Result<OrdersResponse>> CancelAsync(
    int id,
    OrdersCancelCommand command
  );
}