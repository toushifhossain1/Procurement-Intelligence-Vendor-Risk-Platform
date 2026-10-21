namespace IntegrationServices.DTOs;

public class ProcurementDashboardDTO
{
    // Vendor Summary
    public int TotalVendors { get; set; }

    // Risk Distribution
    public int LowRiskVendors { get; set; }
    public int MediumRiskVendors { get; set; }
    public int HighRiskVendors { get; set; }
    public int CriticalRiskVendors { get; set; }
    public int InsufficientDataVendors { get; set; }

    // Alert Summary
    public int TotalActiveAlerts { get; set; }
    public int NewAlerts { get; set; }
    public int AcknowledgedAlerts { get; set; }
    public int CriticalAlerts { get; set; }
    public int HighSeverityAlerts { get; set; }
    public int MediumSeverityAlerts { get; set; }

    // Dashboard Metadata
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}