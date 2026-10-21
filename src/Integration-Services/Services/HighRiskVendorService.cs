using IntegrationServices.DTOs;
using IntegrationServices.Services.Interfaces;

namespace IntegrationServices.Services;

public class HighRiskVendorService : IHighRiskVendorService
{
    private readonly IVendorRiskService _vendorRiskService;
    private readonly IVendorRiskAlertService _vendorRiskAlertService;

    public HighRiskVendorService(
        IVendorRiskService vendorRiskService,
        IVendorRiskAlertService vendorRiskAlertService)
    {
        _vendorRiskService = vendorRiskService;
        _vendorRiskAlertService = vendorRiskAlertService;
    }

    public async Task<List<HighRiskVendorDTO>> GetHighRiskVendorsAsync(
        string? classification = null,
        CancellationToken cancellationToken = default)
    {
        // Retrieve existing vendor risk assessments.
        var vendorRisks = await _vendorRiskService
            .GetAllVendorRisksAsync(cancellationToken);

        // Retrieve alerts, including their current statuses.
        var alerts = await _vendorRiskAlertService
            .GetAllAlertsAsync(cancellationToken: cancellationToken);

        // Keep only active alerts.
        var activeAlerts = alerts
            .Where(x =>
                x.Status == "New" ||
                x.Status == "Acknowledged")
            .ToList();

        // Include only High- and Critical-risk vendors.
        var highRiskVendors = vendorRisks
            .Where(x =>
                x.RiskClassification == "High" ||
                x.RiskClassification == "Critical");

        // Apply an optional classification filter.
        if (!string.IsNullOrWhiteSpace(classification))
        {
            var normalizedClassification = classification.Trim();

            if (!string.Equals(
                    normalizedClassification,
                    "High",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(
                    normalizedClassification,
                    "Critical",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Classification must be either 'High' or 'Critical'.",
                    nameof(classification));
            }

            highRiskVendors = highRiskVendors.Where(x =>
                string.Equals(
                    x.RiskClassification,
                    normalizedClassification,
                    StringComparison.OrdinalIgnoreCase));
        }

        // Build the response.
        var result = highRiskVendors
            .Select(vendor =>
            {
                var vendorAlerts = activeAlerts
                    .Where(alert => alert.VendorNo == vendor.VendorNo)
                    .ToList();

                return new HighRiskVendorDTO
                {
                    VendorNo = vendor.VendorNo,
                    VendorName = vendor.VendorName,

                    OverallRiskScore = vendor.OverallRiskScore,
                    RiskClassification = vendor.RiskClassification,

                    DeliveryRiskScore = vendor.DeliveryRiskScore,
                    FinancialRiskScore = vendor.FinancialRiskScore,
                    InvoiceRiskScore = vendor.InvoiceRiskScore,
                    ReliabilityRiskScore = vendor.ReliabilityRiskScore,

                    RiskFactors = vendor.RiskFactors.ToList(),
                    RecommendedActions = vendor.RecommendedActions.ToList(),

                    AssessmentDate = vendor.AssessmentDate,

                    ActiveAlertCount = vendorAlerts.Count,

                    CriticalAlertCount = vendorAlerts.Count(
                        x => x.Severity == "Critical"),

                    HighSeverityAlertCount = vendorAlerts.Count(
                        x => x.Severity == "High"),

                    MediumSeverityAlertCount = vendorAlerts.Count(
                        x => x.Severity == "Medium")
                };
            })
            .OrderByDescending(x =>
                x.RiskClassification == "Critical" ? 2 : 1)
            .ThenByDescending(x => x.OverallRiskScore ?? -1m)
            .ThenBy(x => x.VendorName)
            .ToList();

        return result;
    }
}