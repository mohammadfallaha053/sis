namespace LapisApi.App.Settings.Dto.Response;

public class GetStatisticResponse
{
  public int TotalCenters { get; set; }
  public int ActiveCenters { get; set; }
  public int InactiveCenters { get; set; }

  public int TotalRegions { get; set; }
  public int TotalCities { get; set; }

  public int TotalUsers { get; set; }
  public int ActiveUsers { get; set; }
  public int InactiveUsers { get; set; }

  public int TotalAgents { get; set; }
  public int ActiveAgents { get; set; }
  public int InactiveAgents { get; set; }

  public int TotalTransactions { get; set; }
  public int ReceivedTransfers { get; set; }
  public int ReadyTransfers { get; set; }
  public int RejectedTransfers { get; set; }
  public int PendingApprovalTransfers { get; set; }
  public int PendingPaymentTransfers { get; set; }
}