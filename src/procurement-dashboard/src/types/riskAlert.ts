export type AlertStatus = "New" | "Acknowledged" | "Resolved";

export type AlertSeverity = "Critical" | "High" | "Medium" | "Low";

export interface RiskAlert {
  id: string;
  vendorNo: string;
  vendorName: string;
  alertType: string;
  severity: AlertSeverity | string;
  status: AlertStatus | string;
  title: string;
  message: string;
  createdAt: string | null;
  acknowledgedAt: string | null;
  resolvedAt: string | null;
  resolvedBy: string;
  resolutionNotes: string;
}

export interface GenerateAlertsResult {
  vendorsEvaluated: number;
  alertsCreated: number;
  duplicateAlertsSkipped: number;
  newAlerts: number;
}
