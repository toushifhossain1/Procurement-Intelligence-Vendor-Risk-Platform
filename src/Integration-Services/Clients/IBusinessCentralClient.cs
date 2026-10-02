using IntegrationServices.DTOs;

namespace IntegrationServices.Clients;

public interface IBusinessCentralClient
{
    Task<List<VendorDTO>> GetVendorsAsync();
}