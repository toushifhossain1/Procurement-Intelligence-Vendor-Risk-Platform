namespace IntegrationServices.Entities;

public class BCVendor
{
    public Guid BCVendorID { get; set; }

    public string VendorNo { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? CountryRegionCode { get; set; }

    public string? PhoneNo { get; set; }
    public string? Email { get; set; }

    public string? CurrencyCode { get; set; }

    public string? Blocked { get; set; }

    public decimal BalanceLCY { get; set; }

    public DateTime? LastModifiedDateTime { get; set; }

    public DateTime SyncedAt { get; set; }
}