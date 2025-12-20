namespace PktApp.Core.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalCompanies { get; set; }
    public int ActiveCompanies { get; set; }
    public int TotalContacts { get; set; }
    public int TotalLeads { get; set; }
    public int ActiveLeads { get; set; }
    public int TotalOpportunities { get; set; }
    public int OpenOpportunities { get; set; }
    public int WonOpportunities { get; set; }
    public int LostOpportunities { get; set; }
    public decimal TotalOpportunityValue { get; set; }
    public decimal WonOpportunityValue { get; set; }
    public decimal OpenOpportunityValue { get; set; }
    public int TotalActivities { get; set; }
    public int OverdueActivities { get; set; }
    public int TodayActivities { get; set; }
    public double WinRate { get; set; }
    public decimal AverageDealSize { get; set; }
}
