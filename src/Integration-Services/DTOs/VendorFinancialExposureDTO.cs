namespace IntegrationServices.DTOs;

public class VendorFinancialExposureDTO
{
    public string VendorNo { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

    public decimal OutstandingBalanceLCY { get; set; }

    public decimal OverdueBalanceLCY { get; set; }

    public decimal? OverdueBalancePercentage { get; set; }

    public int OpenLedgerEntryCount { get; set; }

    public int OverdueLedgerEntryCount { get; set; }

    public string RiskClassification { get; set; } = string.Empty;
}