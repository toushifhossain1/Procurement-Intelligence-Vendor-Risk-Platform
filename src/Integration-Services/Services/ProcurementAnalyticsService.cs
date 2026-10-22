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
        if (limit < 1 || limit > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Limit must be between 1 and 100.");
        }

        var performances = await _vendorPerformanceService
            .GetAllVendorPerformanceAsync(cancellationToken);

        var rankedVendors = performances
            .OrderByDescending(x => x.OverallPerformanceScore)
            .ThenBy(x => x.VendorName)
            .Take(limit)
            .Select((vendor, index) => new VendorPerformanceRankingDTO
            {
                Rank = index + 1,

                VendorNo = vendor.VendorNo,
                VendorName = vendor.VendorName,

                OverallPerformanceScore =
                    vendor.OverallPerformanceScore,

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

        return rankedVendors;
    }

    public async Task<FinancialExposureDTO> GetFinancialExposureAsync(
        string? sortBy = null,
        int limit = 10,
        CancellationToken cancellationToken = default)
    {
        if (limit < 1 || limit > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Limit must be between 1 and 100.");
        }

        var normalizedSortBy = string.IsNullOrWhiteSpace(sortBy)
            ? "OverdueBalanceLCY"
            : sortBy.Trim();

        if (!string.Equals(
                normalizedSortBy,
                "OverdueBalanceLCY",
                StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(
                normalizedSortBy,
                "OutstandingBalanceLCY",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "sortBy must be 'OverdueBalanceLCY' or " +
                "'OutstandingBalanceLCY'.",
                nameof(sortBy));
        }

        var risks = await _vendorRiskService
            .GetAllVendorRisksAsync(cancellationToken);

        var totalOutstandingBalance = risks.Sum(
            x => x.OutstandingBalanceLCY);

        var totalOverdueBalance = risks.Sum(
            x => x.OverdueBalanceLCY);

        var overduePercentage = totalOutstandingBalance > 0
            ? Math.Round(
                totalOverdueBalance / totalOutstandingBalance * 100m,
                2)
            : 0m;

        var vendorsWithOutstandingBalance = risks
            .Where(x => x.OutstandingBalanceLCY > 0)
            .ToList();

        var vendorsWithOverdueBalance = risks
            .Where(x => x.OverdueBalanceLCY > 0)
            .ToList();

        var orderedVendors = string.Equals(
            normalizedSortBy,
            "OutstandingBalanceLCY",
            StringComparison.OrdinalIgnoreCase)
            ? risks
                .OrderByDescending(x => x.OutstandingBalanceLCY)
                .ThenBy(x => x.VendorName)
            : risks
                .OrderByDescending(x => x.OverdueBalanceLCY)
                .ThenBy(x => x.VendorName);

        var vendorExposures = orderedVendors
            .Take(limit)
            .Select(vendor => new VendorFinancialExposureDTO
            {
                VendorNo = vendor.VendorNo,
                VendorName = vendor.VendorName,

                OutstandingBalanceLCY =
                    vendor.OutstandingBalanceLCY,

                OverdueBalanceLCY =
                    vendor.OverdueBalanceLCY,

                OverdueBalancePercentage =
                    vendor.OverdueBalancePercentage,

                OpenLedgerEntryCount =
                    vendor.OpenLedgerEntryCount,

                OverdueLedgerEntryCount =
                    vendor.OverdueLedgerEntryCount,

                RiskClassification =
                    vendor.RiskClassification
            })
            .ToList();

        return new FinancialExposureDTO
        {
            TotalOutstandingBalanceLCY =
                totalOutstandingBalance,

            TotalOverdueBalanceLCY =
                totalOverdueBalance,

            OverdueBalancePercentage =
                overduePercentage,

            VendorsWithOutstandingBalance =
                vendorsWithOutstandingBalance.Count,

            VendorsWithOverdueBalance =
                vendorsWithOverdueBalance.Count,

            Vendors = vendorExposures,

            GeneratedAt = DateTime.UtcNow
        };
    }
}