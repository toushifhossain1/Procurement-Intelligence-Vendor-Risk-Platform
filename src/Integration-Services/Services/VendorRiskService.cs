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

    // Initial business-rule weights.
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
            .OrderBy(vendor => vendor.VendorNo)
            .ToListAsync(cancellationToken);

        var results = new List<VendorRiskDTO>(vendors.Count);

        foreach (var vendor in vendors)
        {
            var risk = await CalculateVendorRiskAsync(
                vendor,
                cancellationToken);

            results.Add(risk);
        }

        return results
            .OrderByDescending(risk => risk.OverallRiskScore ?? -1m)
            .ThenBy(risk => risk.VendorNo)
            .ToList();
    }

    public async Task<VendorRiskDTO?> GetVendorRiskAsync(
        string vendorNo,
        CancellationToken cancellationToken = default)
    {
        var vendor = await _context.BCVendors
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.VendorNo == vendorNo,
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

        // ---------------------------------------------------------
        // 1. Obtain the vendor's performance assessment.
        // ---------------------------------------------------------

        var performance =
            await _performanceService.GetVendorPerformanceAsync(
                vendor.VendorNo,
                cancellationToken);

        if (performance is not null)
        {
            result.OnTimeDeliveryRate =
                performance.OnTimeDeliveryRate;

            result.OrderFulfillmentRate =
                performance.OrderFulfillmentRate;

            result.InvoiceDiscrepancyRate =
                performance.InvoiceDiscrepancyRate;

            result.OverallPerformanceScore =
                performance.OverallPerformanceScore;

            result.PerformanceRating =
                performance.PerformanceRating;
        }

        // ---------------------------------------------------------
        // 2. Calculate Delivery Risk.
        //
        // Delivery Risk =
        //     60% x Late Delivery Percentage
        //   + 40% x Incomplete Fulfillment Percentage
        // ---------------------------------------------------------

        var hasDeliveryData =
            performance is not null &&
            (
                performance.ReceiptLineCount > 0 ||
                performance.PurchaseOrderCount > 0
            );

        if (hasDeliveryData)
        {
            var lateDeliveryPercentage =
                performance!.ReceiptLineCount > 0
                    ? 100m - performance.OnTimeDeliveryRate
                    : (decimal?)null;

            var incompleteFulfillmentPercentage =
                performance.PurchaseOrderCount > 0
                    ? 100m - performance.OrderFulfillmentRate
                    : (decimal?)null;

            decimal deliveryWeightedScore = 0m;
            decimal availableDeliveryWeight = 0m;

            if (lateDeliveryPercentage.HasValue)
            {
                deliveryWeightedScore +=
                    Math.Clamp(
                        lateDeliveryPercentage.Value,
                        0m,
                        100m) * 0.60m;

                availableDeliveryWeight += 0.60m;
            }

            if (incompleteFulfillmentPercentage.HasValue)
            {
                deliveryWeightedScore +=
                    Math.Clamp(
                        incompleteFulfillmentPercentage.Value,
                        0m,
                        100m) * 0.40m;

                availableDeliveryWeight += 0.40m;
            }

            if (availableDeliveryWeight > 0m)
            {
                result.DeliveryRiskScore = Math.Round(
                    deliveryWeightedScore /
                    availableDeliveryWeight,
                    2);
            }
        }

        // ---------------------------------------------------------
        // 3. Calculate Financial Risk.
        //
        // Financial Risk is based on the percentage of outstanding
        // open ledger balances that are overdue.
        //
        // Overdue Percentage =
        //     Overdue Balance / Outstanding Balance x 100
        //
        // We do not assign an arbitrary monetary threshold because
        // a large balance can be normal for a large supplier.
        // ---------------------------------------------------------

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

        result.OutstandingBalanceLCY =
            Math.Round(outstandingBalance, 2);

        result.OverdueBalanceLCY =
            Math.Round(overdueBalance, 2);

        result.OpenLedgerEntryCount =
            openLedgerEntries.Count;

        result.OverdueLedgerEntryCount =
            overdueLedgerEntries.Count;

        var hasFinancialData = ledgerEntries.Count > 0;

        if (hasFinancialData)
        {
            if (outstandingBalance > 0m)
            {
                var overduePercentage = Math.Clamp(
                    overdueBalance / outstandingBalance * 100m,
                    0m,
                    100m);

                result.OverdueBalancePercentage =
                    Math.Round(overduePercentage, 2);

                result.FinancialRiskScore =
                    Math.Round(overduePercentage, 2);
            }
            else
            {
                // Ledger history exists, but no positive open
                // balance remains under the current rule.
                result.OverdueBalancePercentage = 0m;
                result.FinancialRiskScore = 0m;
            }
        }

        // ---------------------------------------------------------
        // 4. Calculate Invoice Risk.
        //
        // The performance engine supplies the invoice discrepancy
        // rate. A 35% discrepancy rate produces a risk score of 35.
        // ---------------------------------------------------------

        var hasInvoiceData =
            performance is not null &&
            performance.InvoiceCount > 0;

        if (hasInvoiceData)
        {
            result.InvoiceRiskScore = Math.Clamp(
                performance!.InvoiceDiscrepancyRate,
                0m,
                100m);
        }

        // ---------------------------------------------------------
        // 5. Calculate Reliability Risk.
        //
        // Reliability Risk = 100 - Overall Performance Score
        // ---------------------------------------------------------

        var hasPerformanceData =
            performance is not null &&
            !string.Equals(
                performance.PerformanceRating,
                "Insufficient Data",
                StringComparison.OrdinalIgnoreCase);

        if (hasPerformanceData)
        {
            result.ReliabilityRiskScore = Math.Clamp(
                100m - performance!.OverallPerformanceScore,
                0m,
                100m);
        }

        // ---------------------------------------------------------
        // 6. Calculate the overall weighted risk score.
        //
        // Missing dimensions are excluded and the remaining
        // weights are normalized.
        // ---------------------------------------------------------

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

        // ---------------------------------------------------------
        // 7. Identify the risk factors.
        // ---------------------------------------------------------

        AddRiskFactors(result);

        // ---------------------------------------------------------
        // 8. Generate recommended actions.
        // ---------------------------------------------------------

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
        if (score < 25m)
        {
            return "Low";
        }

        if (score < 50m)
        {
            return "Medium";
        }

        if (score < 75m)
        {
            return "High";
        }

        return "Critical";
    }

    private static void AddRiskFactors(VendorRiskDTO result)
    {
        // Delivery
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

        // Financial
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

        // Invoice
        if (result.InvoiceRiskScore >= 25m)
        {
            result.RiskFactors.Add(
                "The vendor has a notable invoice discrepancy rate.");
        }

        // Reliability
        if (result.ReliabilityRiskScore >= 50m)
        {
            result.RiskFactors.Add(
                "The vendor's overall performance indicates " +
                "reliability concerns.");
        }

        if (result.RiskFactors.Count == 0)
        {
            if (result.RiskAssessmentStatus == "Insufficient Data")
            {
                result.RiskFactors.Add(
                    "There is insufficient transaction data " +
                    "to identify and evaluate vendor risk.");
            }
            else
            {
                result.RiskFactors.Add(
                    "No significant risk indicators were identified " +
                    "by the current assessment rules.");
            }
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

        // Delivery
        if (result.DeliveryRiskScore >= 25m)
        {
            result.RecommendedActions.Add(
                "Review late deliveries and fulfillment gaps " +
                "with the procurement team.");
        }

        // Financial
        if (result.OverdueBalanceLCY > 0m)
        {
            result.RecommendedActions.Add(
                "Review overdue ledger entries and confirm their " +
                "status with the finance team.");
        }

        // Invoice
        if (result.InvoiceRiskScore >= 25m)
        {
            result.RecommendedActions.Add(
                "Investigate invoice discrepancies and strengthen " +
                "purchase order, receipt, and invoice matching.");
        }

        // Reliability
        if (result.ReliabilityRiskScore >= 30m)
        {
            result.RecommendedActions.Add(
                "Arrange a vendor performance review and agree " +
                "on corrective actions where necessary.");
        }

        // Overall classification
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