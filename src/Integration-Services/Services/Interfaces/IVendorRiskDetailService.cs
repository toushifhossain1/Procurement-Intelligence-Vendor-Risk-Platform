using IntegrationServices.DTOs;

namespace IntegrationServices.Services.Interfaces;

public interface IVendorRiskDetailService
{
    Task<VendorRiskDetailDTO?> GetVendorRiskDetailsAsync(
        string vendorNo,
        CancellationToken cancellationToken = default);
}