export interface VendorPerformanceRanking {
  rank: number;
  vendorNo: string;
  vendorName: string;
  overallPerformanceScore: number | null;
  performanceRating: string;
  onTimeDeliveryRate: number | null;
  orderFulfillmentRate: number | null;
  averageLeadTimeDays: number | null;
  invoiceDiscrepancyRate: number | null;
  purchaseOrderCount: number;
  receiptLineCount: number;
  invoiceCount: number;
}

export interface VendorFinancialExposure {
  vendorNo: string;
  vendorName: string;
  outstandingBalanceLCY: number;
  overdueBalanceLCY: number;
  overdueBalancePercentage: number;
  openLedgerEntryCount: number;
  overdueLedgerEntryCount: number;
  riskClassification: string;
}

export interface FinancialExposureSummary {
  totalOutstandingBalanceLCY: number;
  totalOverdueBalanceLCY: number;
  overdueBalancePercentage: number;
  vendorsWithOutstandingBalance: number;
  vendorsWithOverdueBalance: number;
  vendors: VendorFinancialExposure[];
  generatedAt?: string;
}
