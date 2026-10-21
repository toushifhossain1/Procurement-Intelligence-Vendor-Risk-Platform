using IntegrationServices.DTOs;

namespace IntegrationServices.DTOs;

public class VendorRiskDetailDTO
{
    public VendorRiskDTO RiskAssessment { get; set; } = new();

    public List<VendorRiskAlertDTO> Alerts { get; set; } = [];

    public int TotalAlerts { get; set; }

    public int ActiveAlerts { get; set; }

    public int ResolvedAlerts { get; set; }

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}