using IntegrationServices.DTOs;
using IntegrationServices.Services.Interfaces;

namespace IntegrationServices.Services;

public class VendorRiskDetailService : IVendorRiskDetailService
{
    private readonly IVendorRiskService _vendorRiskService;
    private readonly IVendorRiskAlertService _vendorRiskAlertService;

    public VendorRiskDetailService(
        IVendorRiskService vendorRiskService,
        IVendorRiskAlertService vendorRiskAlertService)
    {
        _vendorRiskService = vendorRiskService;
        _vendorRiskAlertService = vendorRiskAlertService;
    }

    public async Task<VendorRiskDetailDTO?> GetVendorRiskDetailsAsync(
        string vendorNo,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(vendorNo))
        {
            throw new ArgumentException(
                "Vendor number cannot be empty.",
                nameof(vendorNo));
        }

        // Retrieve the vendor's risk assessment.
        var riskAssessment = await _vendorRiskService
            .GetVendorRiskAsync(vendorNo, cancellationToken);

        // Return null when no risk assessment exists.
        if (riskAssessment is null)
        {
            return null;
        }

        // Retrieve the vendor's complete alert history.
        var alerts = await _vendorRiskAlertService
            .GetAlertsByVendorAsync(vendorNo, cancellationToken);

        var activeAlerts = alerts.Count(x =>
            x.Status == "New" ||
            x.Status == "Acknowledged");

        var resolvedAlerts = alerts.Count(x =>
            x.Status == "Resolved");

        return new VendorRiskDetailDTO
        {
            RiskAssessment = riskAssessment,

            Alerts = alerts,

            TotalAlerts = alerts.Count,

            ActiveAlerts = activeAlerts,

            ResolvedAlerts = resolvedAlerts,

            GeneratedAt = DateTime.UtcNow
        };
    }
}