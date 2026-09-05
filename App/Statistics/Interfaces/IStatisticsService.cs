using SisApi.App.Statistics.Dto.Request.Queries;
using SisApi.App.Statistics.Dto.Response;

namespace SisApi.App.Statistics.Interfaces;

public interface IStatisticsService
{
  Task<Result<StatisticsResponse>> GetAsync(StatisticsGetQuery query);
}
