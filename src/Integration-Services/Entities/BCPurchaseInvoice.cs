namespace IntegrationServices.Entities;

public class BCPurchaseInvoice
{
    public Guid BCPurchaseInvoiceID { get; set; }

    public string InvoiceNo { get; set; } = string.Empty;

    public string? VendorNo { get; set; }
    public string? VendorName { get; set; }

    public string? PurchaseOrderNo { get; set; }

    public string? VendorInvoiceNo { get; set; }

    public DateTime? PostingDate { get; set; }
    public DateTime? DocumentDate { get; set; }
    public DateTime? DueDate { get; set; }

    public string? CurrencyCode { get; set; }
    public string? PaymentTermsCode { get; set; }
    public string? LocationCode { get; set; }

    public decimal? Amount { get; set; }
    public decimal? AmountIncludingVAT { get; set; }

    public DateTime? LastModifiedDateTime { get; set; }

    public DateTime SyncedAt { get; set; }
}