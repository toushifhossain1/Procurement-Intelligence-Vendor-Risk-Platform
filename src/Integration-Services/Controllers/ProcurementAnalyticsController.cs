using IntegrationServices.DTOs;
using IntegrationServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/analytics")]
public class ProcurementAnalyticsController : ControllerBase
{
    private readonly IProcurementAnalyticsService _analyticsService;

    public ProcurementAnalyticsController(
        IProcurementAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("vendor-performance")]
    [ProducesResponseType(
        typeof(List<VendorPerformanceRankingDTO>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetVendorPerformanceRankings(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var rankings = await _analyticsService
                .GetVendorPerformanceRankingsAsync(
                    limit,
                    cancellationToken);

            return Ok(rankings);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("financial-exposure")]
    [ProducesResponseType(
        typeof(FinancialExposureDTO),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFinancialExposure(
        [FromQuery] string? sortBy = null,
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var exposure = await _analyticsService
                .GetFinancialExposureAsync(
                    sortBy,
                    limit,
                    cancellationToken);

            return Ok(exposure);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}