namespace IntegrationServices.Entities;

public class VendorRiskAlert
{
    public Guid VendorRiskAlertID { get; set; } = Guid.NewGuid();

    public string VendorNo { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

    public string AlertType { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "New";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? AcknowledgedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? ResolvedBy { get; set; }

    public string? ResolutionNotes { get; set; }
}