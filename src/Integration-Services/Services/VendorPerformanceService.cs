using IntegrationServices.Data;
using IntegrationServices.DTOs;
using IntegrationServices.Entities;
using IntegrationServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Services;

public class VendorPerformanceService : IVendorPerformanceService
{
    private readonly ProcurementDbContext _context;

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
            cancellationToken.ThrowIfCancellationRequested();

            results.Add(await CalculatePerformanceAsync(
                vendor,
                cancellationToken));
        }

        return results;
    }

    public async Task<VendorPerformanceDTO?>
        GetVendorPerformanceAsync(
            string vendorNo,
            CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(vendorNo))
        {
            return null;
        }

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
        // 1. Purchase orders and eligible purchase lines.

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

        // Count item lines only. If Type is missing in imported data,
        // ItemNo is used as a fallback. Explicit non-item types are excluded.
        var eligibleOrderLines = orderLines
            .Where(line =>
                line.Quantity.HasValue &&
                line.Quantity.Value > 0m &&
                (
                    string.Equals(
                        line.Type,
                        "Item",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    (
                        string.IsNullOrWhiteSpace(line.Type) &&
                        !string.IsNullOrWhiteSpace(line.ItemNo)
                    )
                ))
            .ToList();

        var totalOrderedQuantity = eligibleOrderLines
            .Sum(line => line.Quantity ?? 0m);

        // QuantityReceived is cumulative. Do not sum receipt lines here,
        // because partial receipts could otherwise be double-counted.
        var totalReceivedQuantity = eligibleOrderLines
            .Sum(line =>
            {
                var ordered = line.Quantity ?? 0m;
                var received = line.QuantityReceived
                    ?? Math.Max(
                        0m,
                        ordered - (line.OutstandingQuantity ?? 0m));

                return Math.Clamp(received, 0m, ordered);
            });

        decimal? fulfillmentRate = totalOrderedQuantity > 0m
            ? Math.Round(
                Math.Clamp(
                    totalReceivedQuantity / totalOrderedQuantity * 100m,
                    0m,
                    100m),
                2)
            : null;

        // 2. Receipt-based delivery and lead-time measurements.

        var receipts = await _context.BCPurchaseReceiptLines
            .AsNoTracking()
            .Where(line =>
                line.BuyFromVendorNo == vendor.VendorNo ||
                line.PayToVendorNo == vendor.VendorNo)
            .ToListAsync(cancellationToken);

        // Fallback for imported receipt lines without usable vendor numbers.
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
                line.Quantity.Value > 0m &&
                line.PostingDate.HasValue)
            .ToList();

        var datedReceiptLines = eligibleReceiptLines
            .Where(line =>
                line.PromisedReceiptDate.HasValue ||
                line.ExpectedReceiptDate.HasValue ||
                line.PlannedReceiptDate.HasValue)
            .ToList();

        var onTimeCount = datedReceiptLines.Count(line =>
        {
            var expectedDate =
                line.PromisedReceiptDate ??
                line.ExpectedReceiptDate ??
                line.PlannedReceiptDate;

            return expectedDate.HasValue &&
                   line.PostingDate!.Value.Date <= expectedDate.Value.Date;
        });

        decimal? onTimeRate = datedReceiptLines.Count > 0
            ? Math.Round(
                (decimal)onTimeCount / datedReceiptLines.Count * 100m,
                2)
            : null;

        var leadTimes = eligibleReceiptLines
            .Where(line =>
                line.OrderDate.HasValue &&
                line.OrderDate.Value.Date <= line.PostingDate!.Value.Date)
            .Select(line => (decimal)(
                line.PostingDate!.Value.Date -
                line.OrderDate!.Value.Date).TotalDays)
            .ToList();

        decimal? averageLeadTime = leadTimes.Count > 0
            ? Math.Round(leadTimes.Average(), 2)
            : null;

        // 3. Invoice discrepancy measurement.

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

        var evaluatedInvoiceCount = 0;
        var invoiceDiscrepancyCount = 0;

        foreach (var invoice in invoices)
        {
            var linesForInvoice = invoiceLines
                .Where(line =>
                    line.InvoiceNo == invoice.InvoiceNo &&
                    (
                        line.VendorNo == vendor.VendorNo ||
                        string.IsNullOrWhiteSpace(line.VendorNo)
                    ))
                .ToList();

            // Do not count invoices that cannot be compared.
            if (!invoice.Amount.HasValue || linesForInvoice.Count == 0)
            {
                continue;
            }

            evaluatedInvoiceCount++;

            var lineTotal = linesForInvoice.Sum(line =>
                line.LineAmount ?? line.Amount ?? 0m);

            if (Math.Abs(invoice.Amount.Value - lineTotal) >
                InvoiceDiscrepancyTolerance)
            {
                invoiceDiscrepancyCount++;
            }
        }

        decimal? discrepancyRate = evaluatedInvoiceCount > 0
            ? Math.Round(
                (decimal)invoiceDiscrepancyCount /
                evaluatedInvoiceCount * 100m,
                2)
            : null;

        // 4. Outstanding balances.
        // Use positive remaining amounts on open entries to represent
        // outstanding supplier payables consistently with risk scoring.

        var ledgerEntries = await _context.BCVendorLedgerEntries
            .AsNoTracking()
            .Where(entry => entry.VendorNo == vendor.VendorNo)
            .ToListAsync(cancellationToken);

        var outstandingBalance = ledgerEntries
            .Where(entry =>
                entry.Open == true &&
                (entry.RemainingAmountLCY ?? 0m) > 0m)
            .Sum(entry => entry.RemainingAmountLCY ?? 0m);

        // 5. Weighted performance score.
        // Missing dimensions do not contribute to the denominator.

        decimal weightedScore = 0m;
        decimal availableWeight = 0m;

        if (onTimeRate.HasValue)
        {
            weightedScore += onTimeRate.Value * OnTimeWeight;
            availableWeight += OnTimeWeight;
        }

        if (fulfillmentRate.HasValue)
        {
            weightedScore += fulfillmentRate.Value * FulfillmentWeight;
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

        if (discrepancyRate.HasValue)
        {
            var invoiceAccuracy = Math.Clamp(
                100m - discrepancyRate.Value,
                0m,
                100m);

            weightedScore += invoiceAccuracy * InvoiceAccuracyWeight;
            availableWeight += InvoiceAccuracyWeight;
        }

        var overallScore = availableWeight > 0m
            ? Math.Round(weightedScore / availableWeight, 2)
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
            EvaluatedInvoiceCount = evaluatedInvoiceCount,

            OnTimeDeliveryRate = onTimeRate,
            OrderFulfillmentRate = fulfillmentRate,
            AverageLeadTimeDays = averageLeadTime,
            InvoiceDiscrepancyRate = discrepancyRate,

            OutstandingBalanceLCY = Math.Round(outstandingBalance, 2),

            OverallPerformanceScore = overallScore,
            PerformanceRating = rating
        };
    }
}