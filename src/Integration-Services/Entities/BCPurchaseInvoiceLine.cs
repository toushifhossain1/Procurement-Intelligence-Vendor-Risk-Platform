namespace IntegrationServices.Entities;

public class BCPurchaseInvoiceLine
{
    public Guid BCPurchaseInvoiceLineID { get; set; }

    public string InvoiceNo { get; set; } = string.Empty;
    public int LineNo { get; set; }

    public string? VendorNo { get; set; }

    public string? Type { get; set; }

    public string? ItemNo { get; set; }
    public string? Description { get; set; }

    public string? LocationCode { get; set; }
    public string? UnitOfMeasure { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? UnitCost { get; set; }

    public decimal? LineAmount { get; set; }
    public decimal? Amount { get; set; }

    public DateTime? ExpectedReceiptDate { get; set; }
    public DateTime? PostingDate { get; set; }

    public DateTime? LastModifiedDateTime { get; set; }

    public DateTime SyncedAt { get; set; }
}