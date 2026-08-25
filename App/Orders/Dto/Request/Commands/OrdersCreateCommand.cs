namespace SisApi.App.Orders.Dto.Request.Commands;

public class OrdersCreateCommand
{
  public List<int> ItemTypeIds { get; set; } = [];
}