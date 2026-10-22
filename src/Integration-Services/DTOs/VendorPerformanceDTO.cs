namespace IntegrationServices.DTOs;

public class VendorPerformanceDTO
{
    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;

    public int PurchaseOrderCount { get; set; }
    public int ReceiptLineCount { get; set; }
    public int InvoiceCount { get; set; }
    public int EvaluatedInvoiceCount { get; set; }

    public decimal? OnTimeDeliveryRate { get; set; }
    public decimal? OrderFulfillmentRate { get; set; }
    public decimal? AverageLeadTimeDays { get; set; }
    public decimal? InvoiceDiscrepancyRate { get; set; }

    public decimal OutstandingBalanceLCY { get; set; }

    public decimal OverallPerformanceScore { get; set; }
    public string PerformanceRating { get; set; } =
        "Insufficient Data";
}