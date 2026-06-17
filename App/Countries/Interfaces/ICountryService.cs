using LapisApi.App.Cities.Dto;
using LapisApi.App.Cities.Dto.Request.Commands;
namespace LapisApi.Interfaces.Cities;

public interface ICityService
{
  Task<Result<CityResponse>> AddCityAsync(CityCreateCommand dto);
  Task<Result<IEnumerable<CityResponse>>> GetAllCitiesAsync(CityGetAllQuery CityGetAllQuery);
  Task<Result<CityResponse>> GetCityByIdAsync(int id);
  Task<Result<CityResponse>> UpdateCityAsync(int id, UpdateCityCommand CityCommand);
  Task<Result<object>> DeleteCityAsync(int id);
  Task<Result<IEnumerable<CityAutoCompleteResponse>>> GetAutoComplete(CityGetAutoCompleteQuery query);
}