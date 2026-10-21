using IntegrationServices.DTOs;
using IntegrationServices.Services.Interfaces;

namespace IntegrationServices.Services;

public class ProcurementDashboardService : IProcurementDashboardService
{
    private readonly IVendorRiskService _vendorRiskService;
    private readonly IVendorRiskAlertService _vendorRiskAlertService;

    public ProcurementDashboardService(
        IVendorRiskService vendorRiskService,
        IVendorRiskAlertService vendorRiskAlertService)
    {
        _vendorRiskService = vendorRiskService;
        _vendorRiskAlertService = vendorRiskAlertService;
    }

    public async Task<ProcurementDashboardDTO> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        // Retrieve vendor risk assessments.
        var vendorRisks = await _vendorRiskService
            .GetAllVendorRisksAsync(cancellationToken);

        // Retrieve all risk alerts.
        var alerts = await _vendorRiskAlertService
            .GetAllAlertsAsync(
                cancellationToken: cancellationToken);

        var dashboard = new ProcurementDashboardDTO
        {
            GeneratedAt = DateTime.UtcNow,

            // Vendor Summary
            TotalVendors = vendorRisks.Count,

            // Risk Distribution
            LowRiskVendors = vendorRisks.Count(
                x => x.RiskClassification == "Low"),

            MediumRiskVendors = vendorRisks.Count(
                x => x.RiskClassification == "Medium"),

            HighRiskVendors = vendorRisks.Count(
                x => x.RiskClassification == "High"),

            CriticalRiskVendors = vendorRisks.Count(
                x => x.RiskClassification == "Critical"),

            InsufficientDataVendors = vendorRisks.Count(
                x => x.RiskAssessmentStatus == "Insufficient Data"),

            // Active Alerts
            TotalActiveAlerts = alerts.Count(
                x => x.Status == "New" ||
                     x.Status == "Acknowledged"),

            NewAlerts = alerts.Count(
                x => x.Status == "New"),

            AcknowledgedAlerts = alerts.Count(
                x => x.Status == "Acknowledged"),

            // Alert Severity
            CriticalAlerts = alerts.Count(
                x => x.Severity == "Critical" &&
                     (x.Status == "New" ||
                      x.Status == "Acknowledged")),

            HighSeverityAlerts = alerts.Count(
                x => x.Severity == "High" &&
                     (x.Status == "New" ||
                      x.Status == "Acknowledged")),

            MediumSeverityAlerts = alerts.Count(
                x => x.Severity == "Medium" &&
                     (x.Status == "New" ||
                      x.Status == "Acknowledged"))
        };

        return dashboard;
    }
}