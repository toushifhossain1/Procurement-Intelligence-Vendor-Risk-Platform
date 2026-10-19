using IntegrationServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/vendor-performance")]
public class VendorPerformanceController : ControllerBase
{
    private readonly IVendorPerformanceService _performanceService;

    public VendorPerformanceController(
        IVendorPerformanceService performanceService)
    {
        _performanceService = performanceService;
    }

    // GET: api/vendor-performance
    [HttpGet]
    public async Task<IActionResult> GetAllVendorPerformance(
        CancellationToken cancellationToken)
    {
        var results =
            await _performanceService.GetAllVendorPerformanceAsync(
                cancellationToken);

        return Ok(results);
    }

    // GET: api/vendor-performance/{vendorNo}
    [HttpGet("{vendorNo}")]
    public async Task<IActionResult> GetVendorPerformance(
        string vendorNo,
        CancellationToken cancellationToken)
    {
        var result =
            await _performanceService.GetVendorPerformanceAsync(
                vendorNo,
                cancellationToken);

        if (result is null)
        {
            return NotFound(new
            {
                message = $"Vendor '{vendorNo}' was not found."
            });
        }

        return Ok(result);
    }
}