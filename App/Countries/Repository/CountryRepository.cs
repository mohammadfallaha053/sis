using GenericRepository.Interfaces;
using GenericRepository.Repositories;
using Microsoft.EntityFrameworkCore;
using LapisApi.App.Cities.Model;
using LapisApi.Data;
using LapisApi.Data.Models;
using LapisApi.Interfaces.Cities;
namespace LapisApi.Repository;

public class CityRepository : GenericRepository<City>, ICityRepository
{
  public CityRepository(ApplicationDbContext context) : base(context)
  {
  }
  
}