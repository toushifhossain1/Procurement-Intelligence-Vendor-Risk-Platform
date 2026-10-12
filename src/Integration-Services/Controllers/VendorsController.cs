using IntegrationServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly ProcurementDbContext _context;

    public VendorsController(ProcurementDbContext context)
    {
        _context = context;
    }

    // GET: api/vendors
    [HttpGet]
    public async Task<IActionResult> GetVendors(
        CancellationToken cancellationToken)
    {
        var vendors = await _context.BCVendors
            .AsNoTracking()
            .OrderBy(v => v.VendorName)
            .ToListAsync(cancellationToken);

        return Ok(vendors);
    }

    // GET: api/vendors/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVendorById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var vendor = await _context.BCVendors
            .AsNoTracking()
            .FirstOrDefaultAsync(
                v => v.BCVendorID == id,
                cancellationToken);

        if (vendor is null)
        {
            return NotFound(new
            {
                message = $"Vendor with ID '{id}' was not found."
            });
        }

        return Ok(vendor);
    }
}