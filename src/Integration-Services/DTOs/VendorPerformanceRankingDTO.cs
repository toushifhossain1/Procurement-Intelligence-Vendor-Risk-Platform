using IntegrationServices.DTOs;

namespace IntegrationServices.DTOs;

public class VendorPerformanceRankingDTO
{
    public int Rank { get; set; }

    public string VendorNo { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

    public decimal? OverallPerformanceScore { get; set; }

    public string PerformanceRating { get; set; } = string.Empty;

    public decimal? OnTimeDeliveryRate { get; set; }

    public decimal? OrderFulfillmentRate { get; set; }

    public decimal? AverageLeadTimeDays { get; set; }

    public decimal? InvoiceDiscrepancyRate { get; set; }

    public int PurchaseOrderCount { get; set; }

    public int ReceiptLineCount { get; set; }

    public int InvoiceCount { get; set; }
}