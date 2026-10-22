export type RiskClassification =
  | "Low"
  | "Medium"
  | "High"
  | "Critical"
  | "Insufficient Data"
  | string;

export interface VendorRisk {
  vendorNo: string;
  vendorName: string;
  overallRiskScore: number | null;
  riskClassification: RiskClassification;
  riskAssessmentStatus: string;
  assessmentDate?: string;

  deliveryRiskScore: number | null;
  financialRiskScore: number | null;
  invoiceRiskScore: number | null;
  reliabilityRiskScore: number | null;

  onTimeDeliveryRate: number | null;
  orderFulfillmentRate: number | null;
  invoiceDiscrepancyRate: number | null;
  overallPerformanceScore: number | null;
  performanceRating: string;

  outstandingBalanceLCY: number;
  overdueBalanceLCY: number;
  overdueBalancePercentage: number | null;
  openLedgerEntryCount: number;
  overdueLedgerEntryCount: number;

  riskFactors: string[];
  recommendedActions: string[];
}
