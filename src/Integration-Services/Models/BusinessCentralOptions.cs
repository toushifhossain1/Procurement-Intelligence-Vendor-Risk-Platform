namespace IntegrationServices.Models;

public class BusinessCentralOptions
{
    public string BaseUrl { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string TenantId { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}