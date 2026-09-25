using System.Text.Json.Serialization;

namespace IntegrationServices.DTOs;

public class BusinessCentralResponse<T>
{
    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = [];
}