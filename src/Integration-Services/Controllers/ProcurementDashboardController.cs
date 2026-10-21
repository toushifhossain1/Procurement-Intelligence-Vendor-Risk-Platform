using IntegrationServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/dashboard")]
public class ProcurementDashboardController : ControllerBase
{
    private readonly IProcurementDashboardService _dashboardService;

    public ProcurementDashboardController(
        IProcurementDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardService.GetDashboardAsync(
            cancellationToken);

        return Ok(dashboard);
    }
}