using IntegrationServices.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntegrationServices.Entities;

namespace IntegrationServices.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly ProcurementDbContext _context;

    public PurchaseOrdersController(ProcurementDbContext context)
    {
        _context = context;
    }

    // GET: api/purchaseorders
    // Example: api/purchaseorders?page=1&pageSize=10
    // Example: api/purchaseorders?search=PO-1001
    // Example: api/purchaseorders?vendorNo=V0001
    // Example: api/purchaseorders?fromDate=2026-01-01&toDate=2026-12-31
    // Example: api/purchaseorders?sortBy=amount&sortOrder=desc

    [HttpGet]
    public async Task<IActionResult> GetPurchaseOrders(
        [FromQuery] string? search,
        [FromQuery] string? vendorNo,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "orderDate",
        [FromQuery] string sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        // Validate pagination.
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

        // Validate the date range.
        if (fromDate.HasValue &&
            toDate.HasValue &&
            fromDate.Value.Date > toDate.Value.Date)
        {
            return BadRequest(new
            {
                message = "fromDate cannot be later than toDate."
            });
        }

        // Validate sorting.
        var allowedSortFields = new[]
        {
            "orderNo",
            "vendorNo",
            "vendorName",
            "orderDate",
            "postingDate",
            "amount"
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

        // Start with purchase orders stored in SQL Server.
        var query = _context.BCPurchaseOrders
            .AsNoTracking()
            .AsQueryable();

        // Search by purchase order number, vendor number,
        // or vendor name.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(po =>
                (po.PurchaseOrderNo != null &&
                 po.PurchaseOrderNo.Contains(term)) ||
                (po.VendorNo != null &&
                 po.VendorNo.Contains(term)) ||
                (po.VendorName != null &&
                 po.VendorName.Contains(term)));
        }

        // Filter by exact vendor number.
        if (!string.IsNullOrWhiteSpace(vendorNo))
        {
            var vendorFilter = vendorNo.Trim();

            query = query.Where(po =>
                po.VendorNo == vendorFilter);
        }

        // Filter from the beginning of fromDate.
        if (fromDate.HasValue)
        {
            var startDate = fromDate.Value.Date;

            query = query.Where(po =>
                po.OrderDate >= startDate);
        }

        // Include the entire toDate, including its time.
        if (toDate.HasValue)
        {
            var endDateExclusive = toDate.Value.Date.AddDays(1);

            query = query.Where(po =>
                po.OrderDate < endDateExclusive);
        }

        // Count matching records before pagination.
        var totalCount = await query.CountAsync(
            cancellationToken);

        var descending = sortOrder.Equals(
            "desc",
            StringComparison.OrdinalIgnoreCase);

        // Apply sorting.
        IOrderedQueryable<BCPurchaseOrder> orderedQuery = sortBy.ToLowerInvariant() switch
        {
            "orderno" => descending
                ? query.OrderByDescending(po => po.PurchaseOrderNo)
                : query.OrderBy(po => po.PurchaseOrderNo),

            "vendorno" => descending
                ? query.OrderByDescending(po => po.VendorNo)
                : query.OrderBy(po => po.VendorNo),

            "vendorname" => descending
                ? query.OrderByDescending(po => po.VendorName)
                : query.OrderBy(po => po.VendorName),

            "orderdate" => descending
                ? query.OrderByDescending(po => po.OrderDate)
                : query.OrderBy(po => po.OrderDate),

            "postingdate" => descending
                ? query.OrderByDescending(po => po.PostingDate)
                : query.OrderBy(po => po.PostingDate),

            "amount" => descending
                ? query.OrderByDescending(
                    po => po.AmountIncludingVAT)
                : query.OrderBy(
                    po => po.AmountIncludingVAT),

            _ => query.OrderByDescending(po => po.OrderDate)
        };

        // Use the GUID as a secondary sort key for stable paging.
        orderedQuery = orderedQuery.ThenBy(po => po.BCPurchaseOrderID);

        // Apply pagination.
        var purchaseOrders = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            data = purchaseOrders,
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

    // GET: api/purchaseorders/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPurchaseOrderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var purchaseOrder = await _context.BCPurchaseOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(
                po => po.BCPurchaseOrderID == id,
                cancellationToken);

        if (purchaseOrder is null)
        {
            return NotFound(new
            {
                message =
                    $"Purchase order with ID '{id}' was not found."
            });
        }

        return Ok(purchaseOrder);
    }
}