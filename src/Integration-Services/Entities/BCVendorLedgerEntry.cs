namespace IntegrationServices.Entities;

public class BCVendorLedgerEntry
{
    public Guid BCVendorLedgerEntryID { get; set; }

    public int EntryNo { get; set; }

    public string? VendorNo { get; set; }
    public string? VendorName { get; set; }

    public DateTime? PostingDate { get; set; }

    public string? DocumentType { get; set; }
    public string? DocumentNo { get; set; }

    public string? Description { get; set; }

    public string? CurrencyCode { get; set; }

    public decimal? Amount { get; set; }
    public decimal? DebitAmount { get; set; }
    public decimal? CreditAmount { get; set; }

    public decimal? AmountLCY { get; set; }
    public decimal? DebitAmountLCY { get; set; }
    public decimal? CreditAmountLCY { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? PaymentDiscountDate { get; set; }

    public decimal? OriginalAmount { get; set; }
    public decimal? OriginalAmountLCY { get; set; }

    public string? AppliesToDocType { get; set; }
    public string? AppliesToDocNo { get; set; }

    public bool? Open { get; set; }

    public decimal? RemainingAmount { get; set; }
    public decimal? RemainingAmountLCY { get; set; }

    public string? GlobalDimension1Code { get; set; }
    public string? GlobalDimension2Code { get; set; }

    public string? PaymentMethodCode { get; set; }

    public DateTime? LastModifiedDateTime { get; set; }

    public DateTime SyncedAt { get; set; }
}