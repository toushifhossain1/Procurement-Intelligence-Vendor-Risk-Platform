namespace IntegrationServices.DTOs;

public class HighRiskVendorDTO
{
    public string VendorNo { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

    public decimal? OverallRiskScore { get; set; }

    public string RiskClassification { get; set; } = string.Empty;

    public decimal? DeliveryRiskScore { get; set; }

    public decimal? FinancialRiskScore { get; set; }

    public decimal? InvoiceRiskScore { get; set; }

    public decimal? ReliabilityRiskScore { get; set; }

    public List<string> RiskFactors { get; set; } = [];

    public List<string> RecommendedActions { get; set; } = [];

    public DateTime AssessmentDate { get; set; }

    public int ActiveAlertCount { get; set; }

    public int CriticalAlertCount { get; set; }

    public int HighSeverityAlertCount { get; set; }

    public int MediumSeverityAlertCount { get; set; }
}