export interface DashboardSummary {
  totalVendors: number;
  lowRiskVendors: number;
  mediumRiskVendors: number;
  highRiskVendors: number;
  criticalRiskVendors: number;
  insufficientDataVendors: number;
  activeAlerts: number;
  newAlerts: number;
  acknowledgedAlerts: number;
  criticalAlerts: number;
  highSeverityAlerts: number;
  generatedAt?: string;
}

export type DashboardApiResponse = Record<string, unknown>;
