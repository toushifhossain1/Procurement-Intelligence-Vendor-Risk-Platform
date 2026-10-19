namespace IntegrationServices.DTOs;

public class VendorRiskDTO
{
    public string VendorNo { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

    // Overall risk assessment
    public decimal? OverallRiskScore { get; set; }

    public string RiskClassification { get; set; } = "Insufficient Data";

    public string RiskAssessmentStatus { get; set; } = "Insufficient Data";

    // Individual risk dimensions
    public decimal? DeliveryRiskScore { get; set; }

    public decimal? FinancialRiskScore { get; set; }

    public decimal? InvoiceRiskScore { get; set; }

    public decimal? ReliabilityRiskScore { get; set; }

    // Delivery indicators
    public decimal? OnTimeDeliveryRate { get; set; }

    public decimal? OrderFulfillmentRate { get; set; }

    // Financial indicators
    public decimal OutstandingBalanceLCY { get; set; }

    public decimal OverdueBalanceLCY { get; set; }

    public decimal? OverdueBalancePercentage { get; set; }

    public int OpenLedgerEntryCount { get; set; }

    public int OverdueLedgerEntryCount { get; set; }

    // Invoice indicators
    public decimal? InvoiceDiscrepancyRate { get; set; }

    // Performance indicator
    public decimal? OverallPerformanceScore { get; set; }

    public string PerformanceRating { get; set; } = "Insufficient Data";

    // Explanations
    public List<string> RiskFactors { get; set; } = [];

    public List<string> RecommendedActions { get; set; } = [];

    // Assessment metadata
    public DateTime AssessmentDate { get; set; }
}