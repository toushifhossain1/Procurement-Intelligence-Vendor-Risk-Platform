using IntegrationServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/vendor-risk")]
public class VendorRiskController : ControllerBase
{
    private readonly IVendorRiskService _riskService;

    public VendorRiskController(IVendorRiskService riskService)
    {
        _riskService = riskService;
    }

    // GET: api/vendor-risk
    [HttpGet]
    public async Task<IActionResult> GetAllVendorRisks(
        CancellationToken cancellationToken)
    {
        var results = await _riskService.GetAllVendorRisksAsync(
            cancellationToken);

        return Ok(results);
    }

    // GET: api/vendor-risk/V0001
    [HttpGet("{vendorNo}")]
    public async Task<IActionResult> GetVendorRisk(
        string vendorNo,
        CancellationToken cancellationToken)
    {
        var result = await _riskService.GetVendorRiskAsync(
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