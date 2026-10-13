using IntegrationServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseReceiptsController : ControllerBase
{
    private readonly ProcurementDbContext _context;

    public PurchaseReceiptsController(ProcurementDbContext context)
    {
        _context = context;
    }

    // GET: api/purchasereceipts?page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetReceipts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest(new
            {
                message = "Page must be >= 1 and pageSize must be 1-100."
            });

        var query = _context.BCPurchaseReceipts.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .OrderBy(x => x.BCPurchaseReceiptID)
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

    // GET: api/purchasereceipts/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReceiptById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var receipt = await _context.BCPurchaseReceipts
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.BCPurchaseReceiptID == id,
                cancellationToken);

        return receipt is null
            ? NotFound(new { message = "Purchase receipt not found." })
            : Ok(receipt);
    }
}