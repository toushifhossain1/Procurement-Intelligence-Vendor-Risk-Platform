namespace IntegrationServices.DTOs;

public class GenerateRiskAlertsResultDTO
{
    public int VendorsEvaluated { get; set; }

    public int AlertsCreated { get; set; }

    public int DuplicateAlertsSkipped { get; set; }

    public List<VendorRiskAlertDTO> NewAlerts { get; set; } = [];
}