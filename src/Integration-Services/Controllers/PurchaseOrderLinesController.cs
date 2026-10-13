using IntegrationServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntegrationServices.Entities;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrderLinesController : ControllerBase
{
    private readonly ProcurementDbContext _context;

    public PurchaseOrderLinesController(ProcurementDbContext context)
    {
        _context = context;
    }

    // GET: api/purchaseorderlines
    // Example:
    // ?page=1&pageSize=10
    // ?purchaseOrderNo=PO-1001
    // ?vendorNo=V0001
    // ?search=steel
    // ?sortBy=quantity&sortOrder=desc

    [HttpGet]
    public async Task<IActionResult> GetPurchaseOrderLines(
        [FromQuery] string? purchaseOrderNo,
        [FromQuery] string? vendorNo,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "purchaseOrderNo",
        [FromQuery] string sortOrder = "asc",
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            return BadRequest(new
            {
                message = "Page must be greater than or equal to 1."
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                message = "Page size must be between 1 and 100."
            });
        }

        var allowedSortFields = new[]
        {
            "purchaseOrderNo",
            "lineNo",
            "vendorNo",
            "description",
            "quantity",
            "quantityReceived",
            "quantityInvoiced",
            "lineAmount"
        };

        if (!allowedSortFields.Contains(
                sortBy,
                StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Invalid sortBy value.",
                allowedValues = allowedSortFields
            });
        }

        if (!sortOrder.Equals(
                "asc",
                StringComparison.OrdinalIgnoreCase)
            && !sortOrder.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "sortOrder must be 'asc' or 'desc'."
            });
        }

        var query = _context.BCPurchaseOrderLines
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(purchaseOrderNo))
        {
            var orderNo = purchaseOrderNo.Trim();

            query = query.Where(line =>
                line.PurchaseOrderNo == orderNo);
        }

        if (!string.IsNullOrWhiteSpace(vendorNo))
        {
            var vendorNumber = vendorNo.Trim();

            query = query.Where(line =>
                line.VendorNo == vendorNumber);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(line =>
                (line.Description != null &&
                 line.Description.Contains(term)) ||
                (line.ItemNo != null &&
                 line.ItemNo.Contains(term)) ||
                (line.PurchaseOrderNo != null &&
                 line.PurchaseOrderNo.Contains(term)));
        }

        var totalCount = await query.CountAsync(
            cancellationToken);

        var descending = sortOrder.Equals(
            "desc",
            StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLowerInvariant() switch
        {
            "purchaseorderno" => descending
                ? query.OrderByDescending(line => line.PurchaseOrderNo)
                : query.OrderBy(line => line.PurchaseOrderNo),

            "lineno" => descending
                ? query.OrderByDescending(line => line.LineNo)
                : query.OrderBy(line => line.LineNo),

            "vendorno" => descending
                ? query.OrderByDescending(line => line.VendorNo)
                : query.OrderBy(line => line.VendorNo),

            "description" => descending
                ? query.OrderByDescending(line => line.Description)
                : query.OrderBy(line => line.Description),

            "quantity" => descending
                ? query.OrderByDescending(line => line.Quantity)
                : query.OrderBy(line => line.Quantity),

            "quantityreceived" => descending
                ? query.OrderByDescending(line => line.QuantityReceived)
                : query.OrderBy(line => line.QuantityReceived),

            "quantityinvoiced" => descending
                ? query.OrderByDescending(line => line.QuantityInvoiced)
                : query.OrderBy(line => line.QuantityInvoiced),

            "lineamount" => descending
                ? query.OrderByDescending(line => line.LineAmount)
                : query.OrderBy(line => line.LineAmount),

            _ => query.OrderBy(line => line.PurchaseOrderNo)
        };

        if (query is IOrderedQueryable<BCPurchaseOrderLine> orderedQuery)
        {
            query = orderedQuery.ThenBy(line => line.BCPurchaseOrderLineID);
        }
        else
        {
            query = query.OrderBy(line => line.BCPurchaseOrderLineID);
        }

        var lines = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            data = lines,
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

    // GET: api/purchaseorderlines/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPurchaseOrderLineById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var line = await _context.BCPurchaseOrderLines
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.BCPurchaseOrderLineID == id,
                cancellationToken);

        if (line is null)
        {
            return NotFound(new
            {
                message =
                    $"Purchase order line with ID '{id}' was not found."
            });
        }

        return Ok(line);
    }
}