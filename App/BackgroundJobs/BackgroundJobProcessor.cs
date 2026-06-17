using LapisApi.App.BackgroundJobs.Enums;
using LapisApi.App.BackgroundJobs.Interfaces;
using LapisApi.App.BackgroundJobs.Jobs.Payloads;
using LapisApi.App.MediaFiles.Interfaces;
using LapisApi.Data.Interfaces;
namespace LapisApi.App.BackgroundJobs;

public class BackgroundJobProcessor
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IBackgroundJobService _backgroundJobService;
  private readonly IFileService _fileService;

  private readonly IEnumerable<IBackgroundJobHandler> _handlers;

  public BackgroundJobProcessor(
    IUnitOfWork unitOfWork,
    IEnumerable<IBackgroundJobHandler> handlers,
    IBackgroundJobService backgroundJobService,
    IFileService fileService
  )
  {
    _unitOfWork = unitOfWork;
    _handlers = handlers;
    _backgroundJobService = backgroundJobService;
    _fileService = fileService;
  }

  public async Task ProcessJobsAsync()
  {
    Console.WriteLine($"[BackgroundJobProcessor] Starting job processing at {DateTime.Now}");

    var result = await _unitOfWork.BackgroundJobs.GetPagedAsync(
      predicate: o => o.Status == JobStatus.Pending,
      pageNumber: 1,
      pageSize: 5,
      sort: q => q.OrderBy(o => o.CreatedAt)
    );

    var jobs = result.Data;

    foreach (var job in jobs)
    {
      try
      {
        job.Status = JobStatus.InProgress;
        job.LastAttemptAt = DateTime.UtcNow;

        var handler = _handlers.FirstOrDefault(h => h.JobType == job.JobType);
        if (handler == null)
          throw new Exception($"No handler for job type: {job.JobType}");

        await handler.HandleAsync(job.PayloadJson);

        job.Status = JobStatus.Completed;
      }
      catch (Exception ex)
      {
        job.RetryCount++;
        job.Status = job.RetryCount >= 5 ? JobStatus.Failed : JobStatus.Pending;
        job.ErrorMessage = ex.Message;
      }

      await _unitOfWork.SaveChangesAsync();
    }
  }

  public async Task ExecuteOnDayAsync()
  {
    await _backgroundJobService.EnqueueAsync(
      BackgroundJobTypes.CleanBackgroundJobHandler,
      new CleanBackgroundJobPayload()
    );
    await _unitOfWork.SaveChangesAsync();
  }

  public async Task ExecuteOnHourAsync()
  {
    await _fileService.ClearTempFilesAsync();
  }
}