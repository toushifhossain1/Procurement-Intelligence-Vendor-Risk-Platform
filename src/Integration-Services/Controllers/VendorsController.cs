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
    // Example: api/vendors?page=1&pageSize=10
    // Example: api/vendors?search=ber
    // Example: api/vendors?blocked=true
    // Example: api/vendors?city=Dhaka&countryRegionCode=BD
    // Example: api/vendors?sortBy=name&sortOrder=asc
    [HttpGet]
    public async Task<IActionResult> GetVendors(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] string? countryRegionCode,
        [FromQuery] bool? blocked,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "name",
        [FromQuery] string sortOrder = "asc",
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

        // Validate sorting.
        var allowedSortFields = new[]
        {
            "name",
            "no",
            "city",
            "countryregioncode",
            "balance"
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

        // Start with the SQL-backed vendor query.
        var query = _context.BCVendors
            .AsNoTracking()
            .AsQueryable();

        // Search by vendor number, name, email, or phone.
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(v =>
                (v.VendorNo != null && v.VendorNo.Contains(term)) ||
                (v.VendorName != null && v.VendorName.Contains(term)) ||
                (v.Email != null && v.Email.Contains(term)) ||
                (v.PhoneNo != null && v.PhoneNo.Contains(term)));
        }

        // Filter by city.
        if (!string.IsNullOrWhiteSpace(city))
        {
            var cityFilter = city.Trim();

            query = query.Where(v =>
                v.City != null && v.City == cityFilter);
        }

        // Filter by country/region code.
        if (!string.IsNullOrWhiteSpace(countryRegionCode))
        {
            var countryFilter = countryRegionCode.Trim();

            query = query.Where(v =>
                v.CountryRegionCode != null &&
                v.CountryRegionCode == countryFilter);
        }

        // BC's Blocked field may contain values such as:
        // " ", "Payment", or "All".
        // Any nonblank value means the vendor is blocked.
        if (blocked.HasValue)
        {
            if (blocked.Value)
            {
                query = query.Where(v =>
                    v.Blocked != null &&
                    v.Blocked.Trim() != "");
            }
            else
            {
                query = query.Where(v =>
                    v.Blocked == null ||
                    v.Blocked.Trim() == "");
            }
        }

        // Count the filtered records before pagination.
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting.
        var descending = sortOrder.Equals(
            "desc",
            StringComparison.OrdinalIgnoreCase);

        query = sortBy.ToLowerInvariant() switch
        {
            "name" => descending
                ? query.OrderByDescending(v => v.VendorName)
                : query.OrderBy(v => v.VendorName),

            "no" => descending
                ? query.OrderByDescending(v => v.VendorNo)
                : query.OrderBy(v => v.VendorNo),

            "city" => descending
                ? query.OrderByDescending(v => v.City)
                : query.OrderBy(v => v.City),

            "countryregioncode" => descending
                ? query.OrderByDescending(v => v.CountryRegionCode)
                : query.OrderBy(v => v.CountryRegionCode),

            "balance" => descending
                ? query.OrderByDescending(v => v.BalanceLCY)
                : query.OrderBy(v => v.BalanceLCY),

            _ => query.OrderBy(v => v.VendorName)
        };

        // Apply pagination.
        var vendors = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        // Return data and pagination metadata.
        return Ok(new
        {
            data = vendors,
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