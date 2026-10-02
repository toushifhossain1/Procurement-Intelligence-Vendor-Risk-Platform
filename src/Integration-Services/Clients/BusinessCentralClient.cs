using System.Text.Json;
using IntegrationServices.DTOs;
using IntegrationServices.Models;
using Microsoft.Extensions.Options;

namespace IntegrationServices.Clients;

public class BusinessCentralClient : IBusinessCentralClient
{
    private readonly HttpClient _httpClient;
    private readonly BusinessCentralOptions _options;

    public BusinessCentralClient(
        HttpClient httpClient,
        IOptions<BusinessCentralOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<List<VendorDTO>> GetVendorsAsync()
    {
        var endpoint =
            $"Company('{Uri.EscapeDataString(_options.CompanyName)}')/Vendor_API";

        var response = await _httpClient.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var result =
            JsonSerializer.Deserialize<BusinessCentralResponse<VendorDTO>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return result?.Value ?? [];
    }
}