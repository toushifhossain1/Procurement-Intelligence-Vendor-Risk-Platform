using System.Text.Json.Serialization;

namespace IntegrationServices.DTOs;

public class PurchaseOrderDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("documentType")]
    public string DocumentType { get; set; } = string.Empty;

    [JsonPropertyName("no")]
    public string No { get; set; } = string.Empty;

    [JsonPropertyName("buyFromVendorNo")]
    public string BuyFromVendorNo { get; set; } = string.Empty;

    [JsonPropertyName("buyFromVendorName")]
    public string BuyFromVendorName { get; set; } = string.Empty;

    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonPropertyName("postingDate")]
    public DateTime PostingDate { get; set; }

    [JsonPropertyName("expectedReceiptDate")]
    public DateTime ExpectedReceiptDate { get; set; }

    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("paymentTermsCode")]
    public string PaymentTermsCode { get; set; } = string.Empty;

    [JsonPropertyName("locationCode")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("amountIncludingVAT")]
    public decimal AmountIncludingVAT { get; set; }

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }
}