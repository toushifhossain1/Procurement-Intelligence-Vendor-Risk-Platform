namespace IntegrationServices.DTOs;

public class UpdateRiskAlertStatusDTO
{
    public string Status { get; set; } = string.Empty;

    public string? ResolvedBy { get; set; }

    public string? ResolutionNotes { get; set; }
}