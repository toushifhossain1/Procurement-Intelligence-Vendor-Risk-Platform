namespace IntegrationServices.Entities;

public class BCPurchaseOrder
{
    public Guid BCPurchaseOrderID { get; set; }

    public string? DocumentType { get; set; }

    public string PurchaseOrderNo { get; set; } = string.Empty;

    public string VendorNo { get; set; } = string.Empty;
    public string? VendorName { get; set; }

    public DateTime? OrderDate { get; set; }
    public DateTime? PostingDate { get; set; }
    public DateTime? ExpectedReceiptDate { get; set; }

    public string? CurrencyCode { get; set; }
    public string? PaymentTermsCode { get; set; }
    public string? LocationCode { get; set; }

    public string? Status { get; set; }

    public decimal? Amount { get; set; }
    public decimal? AmountIncludingVAT { get; set; }

    public DateTime? LastModifiedDateTime { get; set; }

    public DateTime SyncedAt { get; set; }
}