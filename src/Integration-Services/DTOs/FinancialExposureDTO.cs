namespace IntegrationServices.DTOs;

public class FinancialExposureDTO
{
    public decimal TotalOutstandingBalanceLCY { get; set; }

    public decimal TotalOverdueBalanceLCY { get; set; }

    public decimal OverdueBalancePercentage { get; set; }

    public int VendorsWithOutstandingBalance { get; set; }

    public int VendorsWithOverdueBalance { get; set; }

    public List<VendorFinancialExposureDTO> Vendors { get; set; } = [];

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}