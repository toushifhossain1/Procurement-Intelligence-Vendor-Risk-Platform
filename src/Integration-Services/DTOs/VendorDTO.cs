using System.Text.Json.Serialization;

namespace IntegrationServices.DTOs;

public class VendorDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("no.")]
    public string No { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("countryRegion code")]
    public string CountryRegionCode { get; set; } = string.Empty;

    [JsonPropertyName("phoneNo")]
    public string PhoneNo { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("vatRegistrationNo")]
    public string VatRegistrationNo { get; set; } = string.Empty;

    [JsonPropertyName("blocked")]
    public bool Blocked { get; set; } = false;

    [JsonPropertyName("balanceLCY")]
    public decimal BalanceLCY { get; set; } = 0;

    [JsonPropertyName("lastModifiedDateTime")]
    public DateTime LastModifiedDateTime { get; set; } = DateTime.MinValue;
}