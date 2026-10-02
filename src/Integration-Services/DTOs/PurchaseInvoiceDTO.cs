using System.Text.Json.Serialization;

namespace IntegrationServices.DTOs;

public class PurchaseInvoiceDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("no")]
    public string No { get; set; } = string.Empty;

    [JsonPropertyName("buyFromVendorNo")]
    public string BuyFromVendorNo { get; set; } = string.Empty;

    [JsonPropertyName("buyFromVendorName")]
    public string BuyFromVendorName { get; set; } = string.Empty;

    [JsonPropertyName("orderNo")]
    public string OrderNo { get; set; } = string.Empty;

    [JsonPropertyName("VendorInvoiceNo")]
    public string VendorInvoiceNo { get; set; } = string.Empty;

    [JsonPropertyName("postingDate")]
    public DateTime PostingDate { get; set; }

    [JsonPropertyName("documentDate")]
    public DateTime DocumentDate { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime DueDate { get; set; }

    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("paymentTermsCode")]
    public string PaymentTermsCode { get; set; } = string.Empty;

    [JsonPropertyName("locationCode")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("amountIncludingVAT")]
    public decimal AmountIncludingVAT { get; set; }

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }
}