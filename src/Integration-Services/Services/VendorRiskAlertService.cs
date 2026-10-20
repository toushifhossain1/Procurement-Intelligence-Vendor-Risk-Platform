using IntegrationServices.Data;
using IntegrationServices.DTOs;
using IntegrationServices.Entities;
using IntegrationServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Services;

public class VendorRiskAlertService : IVendorRiskAlertService
{
    private readonly ProcurementDbContext _context;
    private readonly IVendorRiskService _vendorRiskService;
    private readonly IConfiguration _configuration;

    private const string NewStatus = "New";
    private const string AcknowledgedStatus = "Acknowledged";
    private const string ResolvedStatus = "Resolved";

    public VendorRiskAlertService(
        ProcurementDbContext context,
        IVendorRiskService vendorRiskService,
        IConfiguration configuration)
    {
        _context = context;
        _vendorRiskService = vendorRiskService;
        _configuration = configuration;
    }

    public async Task<GenerateRiskAlertsResultDTO> GenerateAlertsAsync(
        CancellationToken cancellationToken = default)
    {
        var result = new GenerateRiskAlertsResultDTO();

        var overdueThreshold =
            _configuration.GetValue<decimal>(
                "VendorRiskAlerts:OverdueBalanceThresholdLCY",
                100000m);

        var deliveryThreshold =
            _configuration.GetValue<decimal>(
                "VendorRiskAlerts:OnTimeDeliveryThreshold",
                80m);

        var invoiceThreshold =
            _configuration.GetValue<decimal>(
                "VendorRiskAlerts:InvoiceDiscrepancyThreshold",
                10m);

        var risks = await _vendorRiskService.GetAllVendorRisksAsync(
            cancellationToken);

        result.VendorsEvaluated = risks.Count;

        foreach (var risk in risks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (risk.RiskAssessmentStatus == "Insufficient Data")
            {
                continue;
            }

            var candidates = new List<VendorRiskAlert>();

            // -------------------------------------------------
            // 1. Critical vendor risk
            // -------------------------------------------------

            if (risk.RiskClassification == "Critical")
            {
                candidates.Add(CreateAlert(
                    risk,
                    alertType: "CriticalVendorRisk",
                    severity: "Critical",
                    title: "Critical Vendor Risk Detected",
                    description:
                        $"Vendor {risk.VendorNo} has a critical " +
                        $"risk score of {risk.OverallRiskScore:N2}."));
            }

            // -------------------------------------------------
            // 2. High vendor risk
            // -------------------------------------------------

            else if (risk.RiskClassification == "High")
            {
                candidates.Add(CreateAlert(
                    risk,
                    alertType: "HighVendorRisk",
                    severity: "High",
                    title: "High Vendor Risk Detected",
                    description:
                        $"Vendor {risk.VendorNo} has a high " +
                        $"risk score of {risk.OverallRiskScore:N2}."));
            }

            // -------------------------------------------------
            // 3. Overdue balance
            // -------------------------------------------------

            if (risk.OverdueBalanceLCY > overdueThreshold)
            {
                candidates.Add(CreateAlert(
                    risk,
                    alertType: "OverduePayment",
                    severity: "High",
                    title: "Vendor Overdue Balance Exceeded",
                    description:
                        $"Vendor {risk.VendorNo} has an overdue " +
                        $"balance of {risk.OverdueBalanceLCY:N2} LCY, " +
                        $"exceeding the configured threshold of " +
                        $"{overdueThreshold:N2} LCY."));
            }

            // -------------------------------------------------
            // 4. Poor delivery performance
            // -------------------------------------------------

            if (risk.OnTimeDeliveryRate.HasValue &&
                risk.OnTimeDeliveryRate.Value < deliveryThreshold)
            {
                candidates.Add(CreateAlert(
                    risk,
                    alertType: "PoorDeliveryPerformance",
                    severity: "Medium",
                    title: "Poor On-Time Delivery Performance",
                    description:
                        $"Vendor {risk.VendorNo} has an on-time " +
                        $"delivery rate of " +
                        $"{risk.OnTimeDeliveryRate.Value:N2}%, " +
                        $"below the configured threshold of " +
                        $"{deliveryThreshold:N2}%."));
            }

            // -------------------------------------------------
            // 5. Invoice discrepancy
            // -------------------------------------------------

            if (risk.InvoiceDiscrepancyRate.HasValue &&
                risk.InvoiceDiscrepancyRate.Value > invoiceThreshold)
            {
                candidates.Add(CreateAlert(
                    risk,
                    alertType: "InvoiceDiscrepancy",
                    severity: "Medium",
                    title: "High Invoice Discrepancy Rate",
                    description:
                        $"Vendor {risk.VendorNo} has an invoice " +
                        $"discrepancy rate of " +
                        $"{risk.InvoiceDiscrepancyRate.Value:N2}%, " +
                        $"above the configured threshold of " +
                        $"{invoiceThreshold:N2}%."));
            }

            // -------------------------------------------------
            // 6. Persist new alerts and prevent duplicates.
            //
            // An alert is considered a duplicate if the same
            // vendor and alert type already have an unresolved
            // alert (New or Acknowledged).
            // -------------------------------------------------

            foreach (var candidate in candidates)
            {
                var existingAlert = await _context.VendorRiskAlerts
                    .AsNoTracking()
                    .AnyAsync(
                        alert =>
                            alert.VendorNo == candidate.VendorNo &&
                            alert.AlertType == candidate.AlertType &&
                            (alert.Status == NewStatus ||
                             alert.Status == AcknowledgedStatus),
                        cancellationToken);

                if (existingAlert)
                {
                    result.DuplicateAlertsSkipped++;
                    continue;
                }

                _context.VendorRiskAlerts.Add(candidate);

                result.NewAlerts.Add(MapToDTO(candidate));
                result.AlertsCreated++;
            }
        }

        if (result.AlertsCreated > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

    public async Task<List<VendorRiskAlertDTO>> GetAllAlertsAsync(
        string? status = null,
        string? severity = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.VendorRiskAlerts
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(
                alert => alert.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            query = query.Where(
                alert => alert.Severity == severity);
        }

        var alerts = await query
            .OrderByDescending(alert => alert.CreatedAt)
            .ToListAsync(cancellationToken);

        return alerts.Select(MapToDTO).ToList();
    }

    public async Task<VendorRiskAlertDTO?> GetAlertByIdAsync(
        Guid alertId,
        CancellationToken cancellationToken = default)
    {
        var alert = await _context.VendorRiskAlerts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.VendorRiskAlertID == alertId,
                cancellationToken);

        return alert is null ? null : MapToDTO(alert);
    }

    public async Task<List<VendorRiskAlertDTO>> GetAlertsByVendorAsync(
        string vendorNo,
        CancellationToken cancellationToken = default)
    {
        var alerts = await _context.VendorRiskAlerts
            .AsNoTracking()
            .Where(alert => alert.VendorNo == vendorNo)
            .OrderByDescending(alert => alert.CreatedAt)
            .ToListAsync(cancellationToken);

        return alerts.Select(MapToDTO).ToList();
    }

    public async Task<VendorRiskAlertDTO?> UpdateAlertStatusAsync(
        Guid alertId,
        string status,
        string? resolvedBy = null,
        string? resolutionNotes = null,
        CancellationToken cancellationToken = default)
    {
        var alert = await _context.VendorRiskAlerts
            .FirstOrDefaultAsync(
                item => item.VendorRiskAlertID == alertId,
                cancellationToken);

        if (alert is null)
        {
            return null;
        }

        if (status == AcknowledgedStatus)
        {
            if (alert.Status != NewStatus)
            {
                throw new InvalidOperationException(
                    "Only New alerts can be acknowledged.");
            }

            alert.Status = AcknowledgedStatus;
            alert.AcknowledgedAt = DateTime.UtcNow;
        }
        else if (status == ResolvedStatus)
        {
            if (alert.Status == ResolvedStatus)
            {
                throw new InvalidOperationException(
                    "This alert is already resolved.");
            }

            if (alert.Status != NewStatus &&
                alert.Status != AcknowledgedStatus)
            {
                throw new InvalidOperationException(
                    "This alert cannot be resolved from its current status.");
            }

            alert.Status = ResolvedStatus;
            alert.ResolvedAt = DateTime.UtcNow;
            alert.ResolvedBy = resolvedBy;
            alert.ResolutionNotes = resolutionNotes;
        }
        else
        {
            throw new ArgumentException(
                "Status must be Acknowledged or Resolved.",
                nameof(status));
        }

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDTO(alert);
    }

    private static VendorRiskAlert CreateAlert(
        VendorRiskDTO risk,
        string alertType,
        string severity,
        string title,
        string description)
    {
        return new VendorRiskAlert
        {
            VendorNo = risk.VendorNo,
            VendorName = risk.VendorName,
            AlertType = alertType,
            Severity = severity,
            Title = title,
            Description = description,
            Status = NewStatus,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static VendorRiskAlertDTO MapToDTO(
        VendorRiskAlert alert)
    {
        return new VendorRiskAlertDTO
        {
            VendorRiskAlertID = alert.VendorRiskAlertID,
            VendorNo = alert.VendorNo,
            VendorName = alert.VendorName,
            AlertType = alert.AlertType,
            Severity = alert.Severity,
            Title = alert.Title,
            Description = alert.Description,
            Status = alert.Status,
            CreatedAt = alert.CreatedAt,
            AcknowledgedAt = alert.AcknowledgedAt,
            ResolvedAt = alert.ResolvedAt,
            ResolvedBy = alert.ResolvedBy,
            ResolutionNotes = alert.ResolutionNotes
        };
    }
}