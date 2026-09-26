using System.Text.Json.Serialization;

namespace IntegrationServices.DTOs;

public class PurchaseOrderLineDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("documentType")]
    public string DocumentType { get; set; } = string.Empty;

    [JsonPropertyName("documentNo")]
    public string DocumentNo { get; set; } = string.Empty;

    [JsonPropertyName("lineNo")]
    public int LineNo { get; set; }

    [JsonPropertyName("buyFromVendorNo")]
    public string BuyFromVendorNo { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("no")]
    public string No { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("locationCode")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("unitOfMeasureCode")]
    public string UnitOfMeasureCode { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("quantityBase")]
    public decimal QuantityBase { get; set; }

    [JsonPropertyName("outstandingQuantity")]
    public decimal OutstandingQuantity { get; set; }

    [JsonPropertyName("quantityReceived")]
    public decimal QuantityReceived { get; set; }

    [JsonPropertyName("quantityInvoiced")]
    public decimal QuantityInvoiced { get; set; }

    [JsonPropertyName("unitCost")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("lineAmount")]
    public decimal LineAmount { get; set; }

    [JsonPropertyName("expectedReceiptDate")]
    public DateTime ExpectedReceiptDate { get; set; }

    [JsonPropertyName("plannedReceiptDate")]
    public DateTime PlannedReceiptDate { get; set; }

    [JsonPropertyName("promisedReceiptDate")]
    public DateTime PromisedReceiptDate { get; set; }

    [JsonPropertyName("requestedReceiptDate")]
    public DateTime RequestedReceiptDate { get; set; }

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }
}