using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisApi.App.Orders.Dto.Request.Commands;
using SisApi.App.Orders.Dto.Request.Queries;
using SisApi.App.Orders.Interfaces;
namespace SisApi.App.Orders.Controller;


[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
  private readonly IOrdersService _ordersService;

  public OrdersController(IOrdersService OrdersService)
  {
    _ordersService = OrdersService;
  }
  [Authorize(Roles = "Client")]
  [HttpPost("add")]
  public async Task<IActionResult> Add([FromBody] OrdersCreateCommand command)
  {
    var result = await _ordersService.AddAsync(command);

    return result.ToActionResult(this);
  }
  
  [Authorize(Roles = "Admin,Manager")]
  [HttpPut("{id}/assign")]
  public async Task<IActionResult> AssignEmployee(
    int id,
    [FromBody] OrdersAssignEmployeeCommand command
  )
  {
    var result =
      await _ordersService.AssignEmployeeAsync(
        id,
        command
      );

    return result.ToActionResult(this);
  }
  
  [Authorize(Roles = "Employee")]
  [HttpPut("{id}/start")]
  public async Task<IActionResult> Start(
    int id
  )
  {
    var result =
      await _ordersService.StartAsync(id);

    return result.ToActionResult(this);
  }
  
  [Authorize]
  [HttpGet("get-all")]
  public async Task<IActionResult> GetAll([FromQuery] OrdersGetAllQuery query)
  {
    var result = await _ordersService.GetAllAsync(query);
    return result.ToActionResult(this);
  }
  
  [Authorize(Roles = "Employee")]
  [HttpPut("{id}/complete")]
  public async Task<IActionResult> Complete(
    int id,
    [FromBody] OrdersCompleteCommand command
  )
  {
    var result =
      await _ordersService.CompleteAsync(
        id,
        command
      );

    return result.ToActionResult(this);
  }
  
  [Authorize(Roles = "Admin,Manager,Employee,Client")]
  [HttpPut("{id}/cancel")]
  public async Task<IActionResult> Cancel(
    int id,
    [FromBody] OrdersCancelCommand command
  )
  {
    var result =
      await _ordersService.CancelAsync(
        id,
        command
      );

    return result.ToActionResult(this);
  }
}