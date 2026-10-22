using IntegrationServices.DTOs;
using IntegrationServices.Services.Interfaces;

namespace IntegrationServices.Services;

public class ProcurementAnalyticsService : IProcurementAnalyticsService
{
    private readonly IVendorPerformanceService _vendorPerformanceService;
    private readonly IVendorRiskService _vendorRiskService;

    public ProcurementAnalyticsService(
        IVendorPerformanceService vendorPerformanceService,
        IVendorRiskService vendorRiskService)
    {
        _vendorPerformanceService = vendorPerformanceService;
        _vendorRiskService = vendorRiskService;
    }

    public async Task<List<VendorPerformanceRankingDTO>>
        GetVendorPerformanceRankingsAsync(
            int limit = 10,
            CancellationToken cancellationToken = default)
    {
        ValidateLimit(limit);

        var performances = await _vendorPerformanceService
            .GetAllVendorPerformanceAsync(cancellationToken);

        return performances
            .Where(vendor =>
                !string.Equals(
                    vendor.PerformanceRating,
                    "Insufficient Data",
                    StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(vendor => vendor.OverallPerformanceScore)
            .ThenBy(vendor => vendor.VendorName)
            .Take(limit)
            .Select((vendor, index) => new VendorPerformanceRankingDTO
            {
                Rank = index + 1,
                VendorNo = vendor.VendorNo,
                VendorName = vendor.VendorName,
                OverallPerformanceScore = vendor.OverallPerformanceScore,
                PerformanceRating = vendor.PerformanceRating,
                OnTimeDeliveryRate = vendor.OnTimeDeliveryRate,
                OrderFulfillmentRate = vendor.OrderFulfillmentRate,
                AverageLeadTimeDays = vendor.AverageLeadTimeDays,
                InvoiceDiscrepancyRate = vendor.InvoiceDiscrepancyRate,
                PurchaseOrderCount = vendor.PurchaseOrderCount,
                ReceiptLineCount = vendor.ReceiptLineCount,
                InvoiceCount = vendor.InvoiceCount
            })
            .ToList();
    }

    public async Task<FinancialExposureDTO> GetFinancialExposureAsync(
        string? sortBy = null,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        ValidateLimit(limit);

        var normalizedSortBy = string.IsNullOrWhiteSpace(sortBy)
            ? "OverdueBalanceLCY"
            : sortBy.Trim();

        var sortByOutstanding = string.Equals(
            normalizedSortBy,
            "OutstandingBalanceLCY",
            StringComparison.OrdinalIgnoreCase);

        var sortByOverdue = string.Equals(
            normalizedSortBy,
            "OverdueBalanceLCY",
            StringComparison.OrdinalIgnoreCase);

        if (!sortByOutstanding && !sortByOverdue)
        {
            throw new ArgumentException(
                "sortBy must be 'OverdueBalanceLCY' or " +
                "'OutstandingBalanceLCY'.",
                nameof(sortBy));
        }

        var risks = await _vendorRiskService
            .GetAllVendorRisksAsync(cancellationToken);

        // Aggregate across all vendors, not just the displayed top N.
        var totalOutstandingBalance = risks.Sum(
            vendor => vendor.OutstandingBalanceLCY);

        var totalOverdueBalance = risks.Sum(
            vendor => vendor.OverdueBalanceLCY);

        var overduePercentage = totalOutstandingBalance > 0m
            ? Math.Round(
                totalOverdueBalance / totalOutstandingBalance * 100m,
                2)
            : 0m;

        var vendorsWithOutstandingBalance = risks
            .Count(vendor => vendor.OutstandingBalanceLCY > 0m);

        var vendorsWithOverdueBalance = risks
            .Count(vendor => vendor.OverdueBalanceLCY > 0m);

        // The detail list only contains vendors with actual positive exposure.
        var vendorsWithExposure = risks
            .Where(vendor =>
                vendor.OutstandingBalanceLCY > 0m ||
                vendor.OverdueBalanceLCY > 0m);

        var orderedVendors = sortByOutstanding
            ? vendorsWithExposure
                .OrderByDescending(vendor => vendor.OutstandingBalanceLCY)
                .ThenBy(vendor => vendor.VendorName)
            : vendorsWithExposure
                .OrderByDescending(vendor => vendor.OverdueBalanceLCY)
                .ThenBy(vendor => vendor.VendorName);

        var vendorExposures = orderedVendors
            .Take(limit)
            .Select(vendor => new VendorFinancialExposureDTO
            {
                VendorNo = vendor.VendorNo,
                VendorName = vendor.VendorName,
                OutstandingBalanceLCY = vendor.OutstandingBalanceLCY,
                OverdueBalanceLCY = vendor.OverdueBalanceLCY,
                OverdueBalancePercentage =
                    vendor.OverdueBalancePercentage,
                OpenLedgerEntryCount = vendor.OpenLedgerEntryCount,
                OverdueLedgerEntryCount = vendor.OverdueLedgerEntryCount,
                RiskClassification = vendor.RiskClassification
            })
            .ToList();

        return new FinancialExposureDTO
        {
            TotalOutstandingBalanceLCY = totalOutstandingBalance,
            TotalOverdueBalanceLCY = totalOverdueBalance,
            OverdueBalancePercentage = overduePercentage,
            VendorsWithOutstandingBalance =
                vendorsWithOutstandingBalance,
            VendorsWithOverdueBalance = vendorsWithOverdueBalance,
            Vendors = vendorExposures,
            GeneratedAt = DateTime.UtcNow
        };
    }

    private static void ValidateLimit(int limit)
    {
        if (limit < 1 || limit > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Limit must be between 1 and 100.");
        }
    }
}