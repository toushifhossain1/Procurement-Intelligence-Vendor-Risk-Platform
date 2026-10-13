using IntegrationServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseInvoicesController : ControllerBase
{
    private readonly ProcurementDbContext _context;

    public PurchaseInvoicesController(ProcurementDbContext context)
    {
        _context = context;
    }

    // GET: api/purchaseinvoices?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest(new
            {
                message = "Page must be >= 1 and pageSize must be 1-100."
            });

        var query = _context.BCPurchaseInvoices.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderBy(x => x.BCPurchaseInvoiceID)
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

    // GET: api/purchaseinvoices/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetInvoiceById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var invoice = await _context.BCPurchaseInvoices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.BCPurchaseInvoiceID == id,
                cancellationToken);

        return invoice is null
            ? NotFound(new { message = "Purchase invoice not found." })
            : Ok(invoice);
    }
}