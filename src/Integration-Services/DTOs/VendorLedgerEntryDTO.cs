using System.Text.Json.Serialization;

namespace IntegrationServices.DTOs;

public class VendorLedgerEntryDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("entryNo")]
    public int EntryNo { get; set; }

    [JsonPropertyName("vendorNo")]
    public string VendorNo { get; set; } = string.Empty;

    [JsonPropertyName("postingDate")]
    public DateTime PostingDate { get; set; }

    [JsonPropertyName("documentType")]
    public string DocumentType { get; set; } = string.Empty;

    [JsonPropertyName("documentNo")]
    public string DocumentNo { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("debitAmount")]
    public decimal DebitAmount { get; set; }

    [JsonPropertyName("creditAmount")]
    public decimal CreditAmount { get; set; }

    [JsonPropertyName("amountLCY")]
    public decimal AmountLCY { get; set; }

    [JsonPropertyName("debitAmountLCY")]
    public decimal DebitAmountLCY { get; set; }

    [JsonPropertyName("creditAmountLCY")]
    public decimal CreditAmountLCY { get; set; }

    [JsonPropertyName("dueDate")]
    public DateTime DueDate { get; set; }

    [JsonPropertyName("pmtDiscountDate")]
    public DateTime PmtDiscountDate { get; set; }

    [JsonPropertyName("originalAmount")]
    public decimal OriginalAmount { get; set; }

    [JsonPropertyName("originalAmtLCY")]
    public decimal OriginalAmtLCY { get; set; }

    [JsonPropertyName("appliesToDocType")]
    public string AppliesToDocType { get; set; } = string.Empty;

    [JsonPropertyName("appliesToDocNo")]
    public string AppliesToDocNo { get; set; } = string.Empty;

    [JsonPropertyName("open")]
    public bool Open { get; set; }

    [JsonPropertyName("remainingAmount")]
    public decimal RemainingAmount { get; set; }

    [JsonPropertyName("remainingAmtLCY")]
    public decimal RemainingAmtLCY { get; set; }

    [JsonPropertyName("globalDimension1Code")]
    public string GlobalDimension1Code { get; set; } = string.Empty;

    [JsonPropertyName("globalDimension2Code")]
    public string GlobalDimension2Code { get; set; } = string.Empty;

    [JsonPropertyName("paymentMethodCode")]
    public string PaymentMethodCode { get; set; } = string.Empty;

    [JsonPropertyName("vendorName")]
    public string VendorName { get; set; } = string.Empty;

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; }
}