using IntegrationServices.Data;
using IntegrationServices.DTOs;
using IntegrationServices.Entities;
using IntegrationServices.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IntegrationServices.Services;

public class VendorRiskService : IVendorRiskService
{
    private readonly ProcurementDbContext _context;
    private readonly IVendorPerformanceService _performanceService;

    private const decimal DeliveryRiskWeight = 0.30m;
    private const decimal FinancialRiskWeight = 0.30m;
    private const decimal InvoiceRiskWeight = 0.20m;
    private const decimal ReliabilityRiskWeight = 0.20m;

    public VendorRiskService(
        ProcurementDbContext context,
        IVendorPerformanceService performanceService)
    {
        _context = context;
        _performanceService = performanceService;
    }

    public async Task<List<VendorRiskDTO>> GetAllVendorRisksAsync(
        CancellationToken cancellationToken = default)
    {
        var vendors = await _context.BCVendors
            .AsNoTracking()
            .OrderBy(v => v.VendorNo)
            .ToListAsync(cancellationToken);

        var results = new List<VendorRiskDTO>(vendors.Count);

        foreach (var vendor in vendors)
        {
            cancellationToken.ThrowIfCancellationRequested();

            results.Add(await CalculateVendorRiskAsync(
                vendor,
                cancellationToken));
        }

        return results
            .OrderByDescending(r => r.OverallRiskScore ?? -1m)
            .ThenBy(r => r.VendorNo)
            .ToList();
    }

    public async Task<VendorRiskDTO?> GetVendorRiskAsync(
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

        return await CalculateVendorRiskAsync(
            vendor,
            cancellationToken);
    }

    private async Task<VendorRiskDTO> CalculateVendorRiskAsync(
        BCVendor vendor,
        CancellationToken cancellationToken)
    {
        var result = new VendorRiskDTO
        {
            VendorNo = vendor.VendorNo,
            VendorName = vendor.VendorName,
            AssessmentDate = DateTime.UtcNow
        };

        var performance =
            await _performanceService.GetVendorPerformanceAsync(
                vendor.VendorNo,
                cancellationToken);

        if (performance is not null)
        {
            result.OnTimeDeliveryRate = performance.OnTimeDeliveryRate;
            result.OrderFulfillmentRate = performance.OrderFulfillmentRate;
            result.InvoiceDiscrepancyRate =
                performance.InvoiceDiscrepancyRate;

            result.OverallPerformanceScore =
                performance.OverallPerformanceScore;

            result.PerformanceRating = performance.PerformanceRating;
        }

        // 1. Delivery risk.
        // Late delivery and incomplete fulfillment are measured only
        // when their corresponding performance data is available.

        decimal deliveryWeightedScore = 0m;
        decimal availableDeliveryWeight = 0m;

        if (performance?.OnTimeDeliveryRate is decimal onTimeRate)
        {
            deliveryWeightedScore +=
                Math.Clamp(100m - onTimeRate, 0m, 100m) * 0.60m;

            availableDeliveryWeight += 0.60m;
        }

        if (performance?.OrderFulfillmentRate is decimal fulfillmentRate)
        {
            deliveryWeightedScore +=
                Math.Clamp(100m - fulfillmentRate, 0m, 100m) * 0.40m;

            availableDeliveryWeight += 0.40m;
        }

        if (availableDeliveryWeight > 0m)
        {
            result.DeliveryRiskScore = Math.Round(
                deliveryWeightedScore / availableDeliveryWeight,
                2);
        }

        // 2. Financial risk.
        // Exposure consists of positive remaining amounts on open entries.

        var ledgerEntries = await _context.BCVendorLedgerEntries
            .AsNoTracking()
            .Where(entry => entry.VendorNo == vendor.VendorNo)
            .ToListAsync(cancellationToken);

        var openLedgerEntries = ledgerEntries
            .Where(entry =>
                entry.Open == true &&
                (entry.RemainingAmountLCY ?? 0m) > 0m)
            .ToList();

        var overdueLedgerEntries = openLedgerEntries
            .Where(entry =>
                entry.DueDate.HasValue &&
                entry.DueDate.Value.Date < DateTime.UtcNow.Date)
            .ToList();

        var outstandingBalance = openLedgerEntries
            .Sum(entry => entry.RemainingAmountLCY ?? 0m);

        var overdueBalance = overdueLedgerEntries
            .Sum(entry => entry.RemainingAmountLCY ?? 0m);

        result.OutstandingBalanceLCY = Math.Round(
            outstandingBalance, 2);

        result.OverdueBalanceLCY = Math.Round(
            overdueBalance, 2);

        result.OpenLedgerEntryCount = openLedgerEntries.Count;
        result.OverdueLedgerEntryCount = overdueLedgerEntries.Count;

        // Ledger history is considered financial data even when all
        // balances are settled. No ledger history means unknown risk.
        if (ledgerEntries.Count > 0)
        {
            var overduePercentage = outstandingBalance > 0m
                ? Math.Clamp(
                    overdueBalance / outstandingBalance * 100m,
                    0m,
                    100m)
                : 0m;

            result.OverdueBalancePercentage =
                Math.Round(overduePercentage, 2);

            result.FinancialRiskScore =
                Math.Round(overduePercentage, 2);
        }

        // 3. Invoice risk.
        // An invoice count alone does not prove that discrepancies
        // could be evaluated. Use the explicit evaluated invoice count.

        if (performance is not null &&
            performance.EvaluatedInvoiceCount > 0 &&
            performance.InvoiceDiscrepancyRate is decimal discrepancyRate)
        {
            result.InvoiceRiskScore = Math.Clamp(
                discrepancyRate,
                0m,
                100m);
        }

        // 4. Reliability risk.
        // Only score reliability when performance has usable data.

        if (performance is not null &&
            !string.Equals(
                performance.PerformanceRating,
                "Insufficient Data",
                StringComparison.OrdinalIgnoreCase))
        {
            result.ReliabilityRiskScore = Math.Clamp(
                100m - performance.OverallPerformanceScore,
                0m,
                100m);
        }

        // 5. Weighted overall risk.
        // Missing dimensions are excluded and remaining weights normalized.

        decimal weightedRiskTotal = 0m;
        decimal availableWeightTotal = 0m;

        AddRiskDimension(
            result.DeliveryRiskScore,
            DeliveryRiskWeight,
            ref weightedRiskTotal,
            ref availableWeightTotal);

        AddRiskDimension(
            result.FinancialRiskScore,
            FinancialRiskWeight,
            ref weightedRiskTotal,
            ref availableWeightTotal);

        AddRiskDimension(
            result.InvoiceRiskScore,
            InvoiceRiskWeight,
            ref weightedRiskTotal,
            ref availableWeightTotal);

        AddRiskDimension(
            result.ReliabilityRiskScore,
            ReliabilityRiskWeight,
            ref weightedRiskTotal,
            ref availableWeightTotal);

        if (availableWeightTotal > 0m)
        {
            result.OverallRiskScore = Math.Round(
                weightedRiskTotal / availableWeightTotal,
                2);

            result.RiskClassification =
                ClassifyRisk(result.OverallRiskScore.Value);

            result.RiskAssessmentStatus =
                availableWeightTotal == 1.00m
                    ? "Complete"
                    : "Partial";
        }
        else
        {
            result.OverallRiskScore = null;
            result.RiskClassification = "Insufficient Data";
            result.RiskAssessmentStatus = "Insufficient Data";
        }

        AddRiskFactors(result);
        AddRecommendedActions(result);

        return result;
    }

    private static void AddRiskDimension(
        decimal? score,
        decimal weight,
        ref decimal weightedTotal,
        ref decimal availableWeight)
    {
        if (!score.HasValue)
        {
            return;
        }

        weightedTotal += score.Value * weight;
        availableWeight += weight;
    }

    private static string ClassifyRisk(decimal score)
    {
        if (score < 25m) return "Low";
        if (score < 50m) return "Medium";
        if (score < 75m) return "High";

        return "Critical";
    }

    private static void AddRiskFactors(VendorRiskDTO result)
    {
        if (result.DeliveryRiskScore >= 50m)
        {
            result.RiskFactors.Add(
                "The vendor has significant delivery or fulfillment risk.");
        }
        else if (result.DeliveryRiskScore >= 25m)
        {
            result.RiskFactors.Add(
                "The vendor has some delivery or fulfillment concerns.");
        }

        if (result.OverdueBalanceLCY > 0m)
        {
            result.RiskFactors.Add(
                $"The vendor has an overdue ledger balance of " +
                $"{result.OverdueBalanceLCY:N2} in local currency.");

            if (result.OverdueBalancePercentage >= 50m)
            {
                result.RiskFactors.Add(
                    "A significant proportion of the vendor's " +
                    "outstanding balance is overdue.");
            }
        }

        if (result.InvoiceRiskScore >= 25m)
        {
            result.RiskFactors.Add(
                "The vendor has a notable invoice discrepancy rate.");
        }

        if (result.ReliabilityRiskScore >= 50m)
        {
            result.RiskFactors.Add(
                "The vendor's overall performance indicates " +
                "reliability concerns.");
        }

        if (result.RiskFactors.Count == 0)
        {
            result.RiskFactors.Add(
                result.RiskAssessmentStatus == "Insufficient Data"
                    ? "There is insufficient transaction data to " +
                      "identify and evaluate vendor risk."
                    : "No significant risk indicators were identified " +
                      "by the current assessment rules.");
        }
    }

    private static void AddRecommendedActions(VendorRiskDTO result)
    {
        if (result.RiskAssessmentStatus == "Insufficient Data")
        {
            result.RecommendedActions.Add(
                "Collect sufficient purchase, receipt, invoice, " +
                "and ledger history before making a risk decision.");

            return;
        }

        if (result.DeliveryRiskScore >= 25m)
        {
            result.RecommendedActions.Add(
                "Review late deliveries and fulfillment gaps " +
                "with the procurement team.");
        }

        if (result.OverdueBalanceLCY > 0m)
        {
            result.RecommendedActions.Add(
                "Review overdue ledger entries and confirm their " +
                "status with the finance team.");
        }

        if (result.InvoiceRiskScore >= 25m)
        {
            result.RecommendedActions.Add(
                "Investigate invoice discrepancies and strengthen " +
                "purchase order, receipt, and invoice matching.");
        }

        if (result.ReliabilityRiskScore >= 30m)
        {
            result.RecommendedActions.Add(
                "Arrange a vendor performance review and agree " +
                "on corrective actions where necessary.");
        }

        if (result.RiskClassification == "Critical")
        {
            result.RecommendedActions.Add(
                "Escalate the vendor for urgent procurement " +
                "and management review.");
        }
        else if (result.RiskClassification == "High")
        {
            result.RecommendedActions.Add(
                "Prioritize this vendor for a formal risk review.");
        }

        if (result.RecommendedActions.Count == 0)
        {
            result.RecommendedActions.Add(
                "Continue routine monitoring and reassess " +
                "the vendor as new transaction data becomes available.");
        }
    }
}