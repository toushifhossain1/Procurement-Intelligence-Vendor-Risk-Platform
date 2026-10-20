using IntegrationServices.DTOs;

namespace IntegrationServices.Services.Interfaces;

public interface IVendorRiskAlertService
{
    Task<GenerateRiskAlertsResultDTO> GenerateAlertsAsync(
        CancellationToken cancellationToken = default);

    Task<List<VendorRiskAlertDTO>> GetAllAlertsAsync(
        string? status = null,
        string? severity = null,
        CancellationToken cancellationToken = default);

    Task<VendorRiskAlertDTO?> GetAlertByIdAsync(
        Guid alertId,
        CancellationToken cancellationToken = default);

    Task<List<VendorRiskAlertDTO>> GetAlertsByVendorAsync(
        string vendorNo,
        CancellationToken cancellationToken = default);

    Task<VendorRiskAlertDTO?> UpdateAlertStatusAsync(
        Guid alertId,
        string status,
        string? resolvedBy = null,
        string? resolutionNotes = null,
        CancellationToken cancellationToken = default);
}