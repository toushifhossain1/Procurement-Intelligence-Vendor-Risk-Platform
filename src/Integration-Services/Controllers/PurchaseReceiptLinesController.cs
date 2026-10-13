using IntegrationServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseReceiptLinesController : ControllerBase
{
    private readonly ProcurementDbContext _context;

    public PurchaseReceiptLinesController(ProcurementDbContext context)
    {
        _context = context;
    }

    // GET: api/purchasereceiptlines?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetReceiptLines(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest(new
            {
                message = "Page must be >= 1 and pageSize must be 1-100."
            });

        var query = _context.BCPurchaseReceiptLines.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderBy(x => x.BCPurchaseReceiptLineID)
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

    // GET: api/purchasereceiptlines/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReceiptLineById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var line = await _context.BCPurchaseReceiptLines
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.BCPurchaseReceiptLineID == id,
                cancellationToken);

        return line is null
            ? NotFound(new { message = "Purchase receipt line not found." })
            : Ok(line);
    }
}