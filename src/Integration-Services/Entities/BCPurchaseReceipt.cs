namespace IntegrationServices.Entities;

public class BCPurchaseReceipt
{
    public Guid BCPurchaseReceiptID { get; set; }

    public string ReceiptNo { get; set; } = string.Empty;

    public string? VendorNo { get; set; }
    public string? VendorName { get; set; }

    public string? PurchaseOrderNo { get; set; }

    public DateTime? PostingDate { get; set; }
    public DateTime? DocumentDate { get; set; }

    public string? LocationCode { get; set; }
    public string? CurrencyCode { get; set; }

    public DateTime? LastModifiedDateTime { get; set; }

    public DateTime SyncedAt { get; set; }
}