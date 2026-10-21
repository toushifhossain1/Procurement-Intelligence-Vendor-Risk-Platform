using IntegrationServices.DTOs;
using IntegrationServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/vendors/{vendorNo}/risk-details")]
public class VendorRiskDetailsController : ControllerBase
{
    private readonly IVendorRiskDetailService _vendorRiskDetailService;

    public VendorRiskDetailsController(
        IVendorRiskDetailService vendorRiskDetailService)
    {
        _vendorRiskDetailService = vendorRiskDetailService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(VendorRiskDetailDTO),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVendorRiskDetails(
        [FromRoute] string vendorNo,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(vendorNo))
        {
            return BadRequest(new
            {
                message = "Vendor number is required."
            });
        }

        var details = await _vendorRiskDetailService
            .GetVendorRiskDetailsAsync(
                vendorNo,
                cancellationToken);

        if (details is null)
        {
            return NotFound(new
            {
                message = $"No risk assessment was found for vendor '{vendorNo}'."
            });
        }

        return Ok(details);
    }
}