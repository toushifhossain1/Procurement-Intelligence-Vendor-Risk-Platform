using System.Text.Json.Serialization;

namespace IntegrationServices.DTOs;

public class PurchaseReceiptLineDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("documentNo")]
    public string DocumentNo { get; set; } = string.Empty;

    [JsonPropertyName("lineNo")]
    public int LineNo { get; set; }

    [JsonPropertyName("buyFromVendorNo")]
    public string BuyFromVendorNo { get; set; } = string.Empty;

    [JsonPropertyName("payToVendorNo")]
    public string PayToVendorNo { get; set; } = string.Empty;

    [JsonPropertyName("orderNo")]
    public string OrderNo { get; set; } = string.Empty;

    [JsonPropertyName("orderLineNo")]
    public int OrderLineNo { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("no")]
    public string No { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("quantityBase")]
    public decimal QuantityBase { get; set; }

    [JsonPropertyName("unitOfMeasureCode")]
    public string UnitOfMeasureCode { get; set; } = string.Empty;

    [JsonPropertyName("directUnitCost")]
    public decimal DirectUnitCost { get; set; }

    [JsonPropertyName("unitCost")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("unitCostLCY")]
    public decimal UnitCostLCY { get; set; }

    [JsonPropertyName("quantityReceivedNotInvoiced")]
    public decimal QuantityReceivedNotInvoiced { get; set; }

    [JsonPropertyName("quantityInvoiced")]
    public decimal QuantityInvoiced { get; set; }

    [JsonPropertyName("postingDate")]
    public DateTime PostingDate { get; set; }

    [JsonPropertyName("locationCode")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("expectedReceiptDate")]
    public DateTime ExpectedReceiptDate { get; set; }

    [JsonPropertyName("promisedReceiptDate")]
    public DateTime PromisedReceiptDate { get; set; }

    [JsonPropertyName("plannedReceiptDate")]
    public DateTime PlannedReceiptDate { get; set; }

    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonPropertyName("vendorOrderNo")]
    public string VendorOrderNo { get; set; } = string.Empty;

    [JsonPropertyName("vendorShipmentNo")]
    public string VendorShipmentNo { get; set; } = string.Empty;

    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }
}