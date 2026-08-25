using AutoMapper;
using SisApi.App.PointsTransactions.Dto.Response;
using SisApi.App.PointsTransactions.Model;

namespace SisApi.App.PointsTransactions.Dto.Mapping;

public class PointsTransactionsProfile : Profile
{
  public PointsTransactionsProfile()
  {
    CreateMap<PointsTransaction, PointsTransactionResponse>()
      .ForMember(
        destination => destination.ProductName,
        options => options.MapFrom(
          source => source.Product == null
            ? null
            : source.Product.Name
        )
      );
  }
}
