namespace LapisApi.App.BackgroundJobs.Interfaces;

public interface IBackgroundJobHandler
{
  string JobType { get; }
  Task HandleAsync(string payloadJson);
}