using LapisApi.App.BackgroundJobs.Enums;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.Data.Interfaces;
namespace LapisApi.App.BackgroundJobs.Jobs.Handlers;

public class CleanBackgroundJobHandler : IBackgroundJobHandler
{
  private readonly IUnitOfWork _unitOfWork;
  public string JobType => BackgroundJobTypes.CleanBackgroundJobHandler;

  public CleanBackgroundJobHandler(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }
  public async Task HandleAsync(string payloadJson)
  {
    var backgrounds =
      await _unitOfWork.BackgroundJobs.GetAllAsync(
        o => o.CreatedAt < DateTime.UtcNow.AddDays(-1)
             &&
             o.Status == JobStatus.Completed
      );
    try
    {
      await _unitOfWork.BackgroundJobs.RemoveRange(backgrounds);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }

    await _unitOfWork.SaveChangesAsync();
  }
}