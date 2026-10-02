namespace IntegrationServices.Entities;

public class BCPurchaseOrderLine
{
    public Guid BCPurchaseOrderLineID { get; set; }

    public string? DocumentType { get; set; }

    public string PurchaseOrderNo { get; set; } = string.Empty;
    public int LineNo { get; set; }

    public string? VendorNo { get; set; }

    public string? Type { get; set; }

    public string? ItemNo { get; set; }
    public string? Description { get; set; }

    public string? LocationCode { get; set; }
    public string? UnitOfMeasureCode { get; set; }

    public decimal? Quantity { get; set; }
    public decimal? QuantityBase { get; set; }

    public decimal? OutstandingQuantity { get; set; }
    public decimal? QuantityReceived { get; set; }
    public decimal? QuantityInvoiced { get; set; }

    public decimal? UnitCost { get; set; }
    public decimal? LineAmount { get; set; }

    public DateTime? ExpectedReceiptDate { get; set; }
    public DateTime? PlannedReceiptDate { get; set; }
    public DateTime? PromisedReceiptDate { get; set; }
    public DateTime? RequestedReceiptDate { get; set; }

    public DateTime? LastModifiedDateTime { get; set; }

    public DateTime SyncedAt { get; set; }
}