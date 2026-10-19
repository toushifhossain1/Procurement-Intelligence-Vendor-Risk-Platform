using IntegrationServices.DTOs;

namespace IntegrationServices.Services.Interfaces;

public interface IVendorPerformanceService
{
    Task<List<VendorPerformanceDTO>> GetAllVendorPerformanceAsync(
        CancellationToken cancellationToken = default);

    Task<VendorPerformanceDTO?> GetVendorPerformanceAsync(
        string vendorNo,
        CancellationToken cancellationToken = default);
}