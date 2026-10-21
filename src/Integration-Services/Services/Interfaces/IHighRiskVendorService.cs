using IntegrationServices.DTOs;

namespace IntegrationServices.Services.Interfaces;

public interface IHighRiskVendorService
{
    Task<List<HighRiskVendorDTO>> GetHighRiskVendorsAsync(
        string? classification = null,
        CancellationToken cancellationToken = default);
}