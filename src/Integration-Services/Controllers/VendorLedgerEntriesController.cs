using IntegrationServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorLedgerEntriesController : ControllerBase
{
    private readonly ProcurementDbContext _context;

    public VendorLedgerEntriesController(ProcurementDbContext context)
    {
        _context = context;
    }

    // GET: api/vendorledgerentries?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetLedgerEntries(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest(new
            {
                message = "Page must be >= 1 and pageSize must be 1-100."
            });

        var query = _context.BCVendorLedgerEntries.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderBy(x => x.BCVendorLedgerEntryID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            data,
            pagination = new
            {
                page,
                pageSize,
                totalCount,
                totalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize),
                hasPreviousPage = page > 1,
                hasNextPage = page * (long)pageSize < totalCount
            }
        });
    }

    // GET: api/vendorledgerentries/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLedgerEntryById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var entry = await _context.BCVendorLedgerEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.BCVendorLedgerEntryID == id,
                cancellationToken);

        return entry is null
            ? NotFound(new { message = "Vendor ledger entry not found." })
            : Ok(entry);
    }
}