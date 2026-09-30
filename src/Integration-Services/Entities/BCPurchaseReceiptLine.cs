namespace IntegrationServices.Entities;

public class BCPurchaseReceiptLine
{
    public Guid BCPurchaseReceiptLineID { get; set; }

    public string ReceiptNo { get; set; } = string.Empty;
    public int LineNo { get; set; }

    public string? BuyFromVendorNo { get; set; }
    public string? PayToVendorNo { get; set; }

    public string? PurchaseOrderNo { get; set; }
    public int? PurchaseOrderLineNo { get; set; }

    public string? Type { get; set; }

    public string? ItemNo { get; set; }
    public string? Description { get; set; }

    public decimal? Quantity { get; set; }
    public decimal? QuantityBase { get; set; }

    public string? UnitOfMeasureCode { get; set; }

    public decimal? DirectUnitCost { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? UnitCostLCY { get; set; }

    public decimal? QuantityReceivedNotInvoiced { get; set; }
    public decimal? QuantityInvoiced { get; set; }

    public DateTime? PostingDate { get; set; }
    public string? LocationCode { get; set; }

    public DateTime? ExpectedReceiptDate { get; set; }
    public DateTime? PromisedReceiptDate { get; set; }
    public DateTime? PlannedReceiptDate { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? VendorOrderNo { get; set; }
    public string? VendorShipmentNo { get; set; }

    public string? CurrencyCode { get; set; }

    public DateTime? LastModifiedDateTime { get; set; }

    public DateTime SyncedAt { get; set; }
}