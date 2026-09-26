using System.Text.Json.Serialization;

namespace IntegrationServices.DTOs;

public class PurchaseInvoiceLineDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

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

    [JsonPropertyName("unitOfMeasure")]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("unitCost")]
    public decimal UnitCost { get; set; }

    [JsonPropertyName("lineAmount")]
    public decimal LineAmount { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("expectedReceiptDate")]
    public DateTime ExpectedReceiptDate { get; set; }

    [JsonPropertyName("postingDate")]
    public DateTime PostingDate { get; set; }

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }
}