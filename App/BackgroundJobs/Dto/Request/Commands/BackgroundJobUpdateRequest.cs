using System.ComponentModel.DataAnnotations;

namespace LapisApi.App.BackgroundJobs.Dto;

public class BackgroundJobUpdateRequest
{
  public string? Note { get; set; }
}