using IntegrationServices.DTOs;

namespace IntegrationServices.Services.Interfaces;

public interface IProcurementAnalyticsService
{
    Task<List<VendorPerformanceRankingDTO>>
        GetVendorPerformanceRankingsAsync(
            int limit = 10,
            CancellationToken cancellationToken = default);

    Task<FinancialExposureDTO> GetFinancialExposureAsync(
        string? sortBy = null,
        int limit = 10,
        CancellationToken cancellationToken = default);
}