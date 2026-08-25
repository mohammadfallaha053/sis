using AutoMapper;
using LapisApi.App.ItemTypes.Dto.Request.Commands;
using SisApi.App.ItemTypes.Dto.Request.Commands;
using SisApi.App.ItemTypes.Dto.Response;
using SisApi.App.ItemTypes.Model;
namespace SisApi.App.ItemTypes.Dto.Mapping;

public class ItemTypesProfile : Profile
{
  public ItemTypesProfile()
  {
    CreateMap<ItemTypesCreateCommand, ItemType>();
    CreateMap<ItemTypesUpdateCommand, ItemType>();
    CreateMap<ItemType, ItemTypesResponse>();
  }
}