namespace SisApi.App.Orders.Dto.Request.Commands;

public class OrdersCompleteCommand
{
  public List<OrderItemWeightCommand> Items { get; set; } = [];
}

public class OrderItemWeightCommand
{
  public int OrderItemId { get; set; }

  public decimal WeightKg { get; set; }
}