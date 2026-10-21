using IntegrationServices.DTOs;

namespace IntegrationServices.Services.Interfaces;

public interface IProcurementDashboardService
{
    Task<ProcurementDashboardDTO> GetDashboardAsync(
        CancellationToken cancellationToken = default);
}