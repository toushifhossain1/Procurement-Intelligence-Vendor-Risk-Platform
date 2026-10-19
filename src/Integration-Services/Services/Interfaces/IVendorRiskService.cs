using IntegrationServices.DTOs;

namespace IntegrationServices.Services.Interfaces;

public interface IVendorRiskService
{
    Task<List<VendorRiskDTO>> GetAllVendorRisksAsync(
        CancellationToken cancellationToken = default);

    Task<VendorRiskDTO?> GetVendorRiskAsync(
        string vendorNo,
        CancellationToken cancellationToken = default);
}