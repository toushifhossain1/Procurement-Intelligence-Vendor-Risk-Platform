using IntegrationServices.Data;
using IntegrationServices.DTOs;
using IntegrationServices.Entities;
using IntegrationServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Services;

public class VendorPerformanceService : IVendorPerformanceService
{
    private readonly ProcurementDbContext _context;

    // Provisional business rules for the initial prototype.
    private const decimal OnTimeWeight = 0.35m;
    private const decimal FulfillmentWeight = 0.30m;
    private const decimal LeadTimeWeight = 0.20m;
    private const decimal InvoiceAccuracyWeight = 0.15m;

    private const decimal LeadTimeBenchmarkDays = 14m;
    private const decimal InvoiceDiscrepancyTolerance = 0.01m;

    public VendorPerformanceService(ProcurementDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendorPerformanceDTO>>
        GetAllVendorPerformanceAsync(
            CancellationToken cancellationToken = default)
    {
        var vendors = await _context.BCVendors
            .AsNoTracking()
            .OrderBy(v => v.VendorNo)
            .ToListAsync(cancellationToken);

        var results = new List<VendorPerformanceDTO>(vendors.Count);

        foreach (var vendor in vendors)
        {
            var performance = await CalculatePerformanceAsync(
                vendor,
                cancellationToken);

            results.Add(performance);
        }

        return results;
    }

    public async Task<VendorPerformanceDTO?>
        GetVendorPerformanceAsync(
            string vendorNo,
            CancellationToken cancellationToken = default)
    {
        var vendor = await _context.BCVendors
            .AsNoTracking()
            .FirstOrDefaultAsync(
                v => v.VendorNo == vendorNo,
                cancellationToken);

        if (vendor is null)
        {
            return null;
        }

        return await CalculatePerformanceAsync(
            vendor,
            cancellationToken);
    }

    private async Task<VendorPerformanceDTO> CalculatePerformanceAsync(
        BCVendor vendor,
        CancellationToken cancellationToken)
    {
        var purchaseOrders = await _context.BCPurchaseOrders
            .AsNoTracking()
            .Where(po => po.VendorNo == vendor.VendorNo)
            .ToListAsync(cancellationToken);

        var orderNumbers = purchaseOrders
            .Select(po => po.PurchaseOrderNo)
            .Distinct()
            .ToList();

        var orderLines = await _context.BCPurchaseOrderLines
            .AsNoTracking()
            .Where(line =>
                line.VendorNo == vendor.VendorNo ||
                orderNumbers.Contains(line.PurchaseOrderNo))
            .ToListAsync(cancellationToken);

        // Ignore non-item lines and nonpositive quantities.
        var eligibleOrderLines = orderLines
            .Where(line =>
                line.Quantity.HasValue &&
                line.Quantity.Value > 0 &&
                (string.Equals(
                    line.Type,
                    "Item",
                    StringComparison.OrdinalIgnoreCase) ||
                 !string.IsNullOrWhiteSpace(line.ItemNo)))
            .ToList();

        var totalOrderedQuantity = eligibleOrderLines
            .Sum(line => line.Quantity ?? 0m);

        // QuantityReceived is the cumulative received quantity on
        // the purchase order line, avoiding double-counting multiple
        // partial receipt lines.
        var totalReceivedQuantity = eligibleOrderLines
            .Sum(line => Math.Max(
                0m,
                (line.Quantity ?? 0m) -
                (line.OutstandingQuantity ?? 0m)));

        // Use the explicit received quantity when available.
        totalReceivedQuantity = eligibleOrderLines
            .Sum(line => Math.Max(
                0m,
                Math.Min(
                    line.Quantity ?? 0m,
                    line.QuantityReceived ??
                    ((line.Quantity ?? 0m) -
                     (line.OutstandingQuantity ?? 0m)))));

        var fulfillmentRate = totalOrderedQuantity > 0m
            ? Math.Clamp(
                totalReceivedQuantity / totalOrderedQuantity * 100m,
                0m,
                100m)
            : 0m;

        var receipts = await _context.BCPurchaseReceiptLines
            .AsNoTracking()
            .Where(line =>
                line.BuyFromVendorNo == vendor.VendorNo ||
                line.PayToVendorNo == vendor.VendorNo)
            .ToListAsync(cancellationToken);

        // If receipt lines don't carry a usable vendor number,
        // associate them with purchase orders belonging to this vendor.
        if (receipts.Count == 0 && orderNumbers.Count > 0)
        {
            receipts = await _context.BCPurchaseReceiptLines
                .AsNoTracking()
                .Where(line =>
                    line.PurchaseOrderNo != null &&
                    orderNumbers.Contains(line.PurchaseOrderNo))
                .ToListAsync(cancellationToken);
        }

        var eligibleReceiptLines = receipts
            .Where(line =>
                line.Quantity.HasValue &&
                line.Quantity.Value > 0 &&
                line.PostingDate.HasValue)
            .ToList();

        var onTimeEligibleCount = eligibleReceiptLines
            .Count(line =>
                line.ExpectedReceiptDate.HasValue ||
                line.PromisedReceiptDate.HasValue ||
                line.PlannedReceiptDate.HasValue);

        var onTimeCount = eligibleReceiptLines.Count(line =>
        {
            var expectedDate =
                line.PromisedReceiptDate ??
                line.ExpectedReceiptDate ??
                line.PlannedReceiptDate;

            return expectedDate.HasValue &&
                   line.PostingDate!.Value.Date <= expectedDate.Value.Date;
        });

        var onTimeRate = onTimeEligibleCount > 0
            ? (decimal)onTimeCount / onTimeEligibleCount * 100m
            : 0m;

        var leadTimes = eligibleReceiptLines
            .Where(line =>
                line.OrderDate.HasValue &&
                line.OrderDate.Value.Date <= line.PostingDate!.Value.Date)
            .Select(line => (decimal)(
                line.PostingDate!.Value.Date -
                line.OrderDate!.Value.Date).TotalDays)
            .ToList();

        decimal? averageLeadTime = leadTimes.Count > 0
            ? leadTimes.Average()
            : null;

        var invoices = await _context.BCPurchaseInvoices
            .AsNoTracking()
            .Where(invoice => invoice.VendorNo == vendor.VendorNo)
            .ToListAsync(cancellationToken);

        var invoiceNumbers = invoices
            .Select(invoice => invoice.InvoiceNo)
            .Distinct()
            .ToList();

        var invoiceLines = await _context.BCPurchaseInvoiceLines
            .AsNoTracking()
            .Where(line =>
                line.VendorNo == vendor.VendorNo ||
                invoiceNumbers.Contains(line.InvoiceNo))
            .ToListAsync(cancellationToken);

        var invoiceDiscrepancyCount = 0;

        foreach (var invoice in invoices)
        {
            var linesForInvoice = invoiceLines
                .Where(line => line.InvoiceNo == invoice.InvoiceNo)
                .ToList();

            if (linesForInvoice.Count == 0)
            {
                continue;
            }

            // Invoice line amounts may exclude VAT. Compare the
            // header Amount to line totals first; do not compare
            // line totals directly with AmountIncludingVAT.
            if (invoice.Amount.HasValue)
            {
                var lineTotal = linesForInvoice.Sum(line =>
                    line.LineAmount ?? line.Amount ?? 0m);

                if (Math.Abs(invoice.Amount.Value - lineTotal) >
                    InvoiceDiscrepancyTolerance)
                {
                    invoiceDiscrepancyCount++;
                }
            }
        }

        var evaluatedInvoiceCount = invoices.Count(invoice =>
            invoice.Amount.HasValue &&
            invoiceLines.Any(line =>
                line.InvoiceNo == invoice.InvoiceNo));

        var discrepancyRate = evaluatedInvoiceCount > 0
            ? (decimal)invoiceDiscrepancyCount /
              evaluatedInvoiceCount * 100m
            : 0m;

        var ledgerEntries = await _context.BCVendorLedgerEntries
            .AsNoTracking()
            .Where(entry => entry.VendorNo == vendor.VendorNo)
            .ToListAsync(cancellationToken);

        var outstandingBalance = ledgerEntries
            .Where(entry => entry.Open == true)
            .Sum(entry => entry.RemainingAmountLCY ?? 0m);

        // Score only the dimensions for which data is available.
        // This avoids silently treating missing measurements as
        // poor performance.
        decimal weightedScore = 0m;
        decimal availableWeight = 0m;

        if (onTimeEligibleCount > 0)
        {
            weightedScore += onTimeRate * OnTimeWeight;
            availableWeight += OnTimeWeight;
        }

        if (totalOrderedQuantity > 0m)
        {
            weightedScore += fulfillmentRate * FulfillmentWeight;
            availableWeight += FulfillmentWeight;
        }

        if (averageLeadTime.HasValue)
        {
            var leadTimeScore = Math.Clamp(
                (LeadTimeBenchmarkDays - averageLeadTime.Value) /
                LeadTimeBenchmarkDays * 100m,
                0m,
                100m);

            weightedScore += leadTimeScore * LeadTimeWeight;
            availableWeight += LeadTimeWeight;
        }

        if (evaluatedInvoiceCount > 0)
        {
            var invoiceAccuracy = Math.Clamp(
                100m - discrepancyRate,
                0m,
                100m);

            weightedScore += invoiceAccuracy * InvoiceAccuracyWeight;
            availableWeight += InvoiceAccuracyWeight;
        }

        var overallScore = availableWeight > 0m
            ? weightedScore / availableWeight
            : 0m;

        var rating = availableWeight == 0m
            ? "Insufficient Data"
            : overallScore >= 85m
                ? "Excellent"
                : overallScore >= 70m
                    ? "Good"
                    : overallScore >= 50m
                        ? "Fair"
                        : "Poor";

        return new VendorPerformanceDTO
        {
            VendorNo = vendor.VendorNo,
            VendorName = vendor.VendorName,

            PurchaseOrderCount = purchaseOrders.Count,
            ReceiptLineCount = receipts.Count,
            InvoiceCount = invoices.Count,

            OnTimeDeliveryRate = Math.Round(onTimeRate, 2),
            OrderFulfillmentRate = Math.Round(fulfillmentRate, 2),
            AverageLeadTimeDays = averageLeadTime.HasValue
                ? Math.Round(averageLeadTime.Value, 2)
                : null,

            InvoiceDiscrepancyRate = Math.Round(discrepancyRate, 2),
            OutstandingBalanceLCY = Math.Round(outstandingBalance, 2),

            OverallPerformanceScore = Math.Round(overallScore, 2),
            PerformanceRating = rating
        };
    }
}