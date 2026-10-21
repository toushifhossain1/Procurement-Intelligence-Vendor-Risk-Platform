using IntegrationServices.DTOs;
using IntegrationServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/risk-alerts")]
public class VendorRiskAlertsController : ControllerBase
{
    private readonly IVendorRiskAlertService _alertService;

    public VendorRiskAlertsController(
        IVendorRiskAlertService alertService)
    {
        _alertService = alertService;
    }

    // POST: api/risk-alerts/generate
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateAlerts(
        CancellationToken cancellationToken)
    {
        var result = await _alertService.GenerateAlertsAsync(
            cancellationToken);

        return Ok(result);
    }

    // GET: api/risk-alerts
    // Optional filters: ?status=New&severity=High
    [HttpGet]
    public async Task<IActionResult> GetAllAlerts(
        [FromQuery] string? status,
        [FromQuery] string? severity,
        CancellationToken cancellationToken)
    {
        var alerts = await _alertService.GetAllAlertsAsync(
            status,
            severity,
            cancellationToken);

        return Ok(alerts);
    }

    // GET: api/risk-alerts/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAlertById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var alert = await _alertService.GetAlertByIdAsync(
            id,
            cancellationToken);

        if (alert is null)
        {
            return NotFound(new
            {
                message = $"Risk alert '{id}' was not found."
            });
        }

        return Ok(alert);
    }

    // GET: api/risk-alerts/vendor/{vendorNo}
    [HttpGet("vendor/{vendorNo}")]
    public async Task<IActionResult> GetAlertsByVendor(
        string vendorNo,
        CancellationToken cancellationToken)
    {
        var alerts = await _alertService.GetAlertsByVendorAsync(
            vendorNo,
            cancellationToken);

        return Ok(alerts);
    }

    // PATCH: api/risk-alerts/{id}/status
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateAlertStatus(
        Guid id,
        [FromBody] UpdateRiskAlertStatusDTO request,
        CancellationToken cancellationToken)
    {
        try
        {
            var alert = await _alertService.UpdateAlertStatusAsync(
                id,
                request.Status,
                request.ResolvedBy,
                request.ResolutionNotes,
                cancellationToken);

            if (alert is null)
            {
                return NotFound(new
                {
                    message = $"Risk alert '{id}' was not found."
                });
            }

            return Ok(alert);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new
            {
                message = exception.Message
            });
        }
    }
}