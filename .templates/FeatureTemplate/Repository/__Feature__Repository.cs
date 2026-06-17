using GenericRepository.Repositories;
using TransfersApi.App.__Feature__s.Interfaces;
using TransfersApi.App.__Feature__s.Model;
using TransfersApi.Data;
namespace TransfersApi.App.__Feature__s.Repository;

public class __Feature__Repository : GenericRepository<__Feature__>, I__Feature__Repository
{
  public __Feature__Repository(ApplicationDbContext context) : base(context)
  {
  }
  
}