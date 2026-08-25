using AutoMapper;
using SisApi.App.Orders.Dto.Response;
using SisApi.App.Orders.Model;

namespace SisApi.App.Orders.Dto.Mapping;

public class OrdersProfile : Profile
{
  public OrdersProfile()
  {
    CreateMap<Order, OrdersResponse>();

    CreateMap<OrderItem, OrderItemResponse>()
      .ForMember(
        destination => destination.ItemTypeName,
        options => options.MapFrom(
          source => source.ItemType.Name
        )
      );
  }
}