import type {
  FinancialExposureSummary,
  VendorFinancialExposure,
  VendorPerformanceRanking,
} from "../types/procurementAnalytics";

function getProperty(source: Record<string, unknown>, name: string): unknown {
  const key = Object.keys(source).find(
    (candidate) => candidate.toLowerCase() === name.toLowerCase(),
  );

  return key ? source[key] : undefined;
}

function asRecord(value: unknown): Record<string, unknown> {
  if (typeof value !== "object" || value === null || Array.isArray(value)) {
    return {};
  }

  return value as Record<string, unknown>;
}

function asNumber(value: unknown): number | null {
  if (value === null || value === undefined || value === "") {
    return null;
  }

  const parsed = Number(value);
  return Number.isFinite(parsed) ? parsed : null;
}

function numberOrZero(value: unknown): number {
  return asNumber(value) ?? 0;
}

function asString(value: unknown, fallback = ""): string {
  return typeof value === "string" || typeof value === "number"
    ? String(value)
    : fallback;
}

async function getJson<T>(url: string): Promise<T> {
  const response = await fetch(url);

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || `Request failed with status ${response.status}`);
  }

  return response.json() as Promise<T>;
}

function mapPerformance(value: unknown): VendorPerformanceRanking {
  const source = asRecord(value);

  return {
    rank: numberOrZero(getProperty(source, "rank")),
    vendorNo: asString(getProperty(source, "vendorNo")),
    vendorName: asString(getProperty(source, "vendorName"), "Unknown vendor"),
    overallPerformanceScore: asNumber(
      getProperty(source, "overallPerformanceScore"),
    ),
    performanceRating: asString(
      getProperty(source, "performanceRating"),
      "Not rated",
    ),
    onTimeDeliveryRate: asNumber(getProperty(source, "onTimeDeliveryRate")),
    orderFulfillmentRate: asNumber(getProperty(source, "orderFulfillmentRate")),
    averageLeadTimeDays: asNumber(getProperty(source, "averageLeadTimeDays")),
    invoiceDiscrepancyRate: asNumber(
      getProperty(source, "invoiceDiscrepancyRate"),
    ),
    purchaseOrderCount: numberOrZero(getProperty(source, "purchaseOrderCount")),
    receiptLineCount: numberOrZero(getProperty(source, "receiptLineCount")),
    invoiceCount: numberOrZero(getProperty(source, "invoiceCount")),
  };
}

function mapFinancialVendor(value: unknown): VendorFinancialExposure {
  const source = asRecord(value);

  return {
    vendorNo: asString(getProperty(source, "vendorNo")),
    vendorName: asString(getProperty(source, "vendorName"), "Unknown vendor"),
    outstandingBalanceLCY: numberOrZero(
      getProperty(source, "outstandingBalanceLCY"),
    ),
    overdueBalanceLCY: numberOrZero(getProperty(source, "overdueBalanceLCY")),
    overdueBalancePercentage: numberOrZero(
      getProperty(source, "overdueBalancePercentage"),
    ),
    openLedgerEntryCount: numberOrZero(
      getProperty(source, "openLedgerEntryCount"),
    ),
    overdueLedgerEntryCount: numberOrZero(
      getProperty(source, "overdueLedgerEntryCount"),
    ),
    riskClassification: asString(
      getProperty(source, "riskClassification"),
      "Unassessed",
    ),
  };
}

export async function getVendorPerformance(
  limit = 10,
): Promise<VendorPerformanceRanking[]> {
  const data = await getJson<unknown>(
    `/api/analytics/vendor-performance?limit=${limit}`,
  );

  if (!Array.isArray(data)) {
    throw new Error("The vendor-performance API did not return a list.");
  }

  return data.map(mapPerformance);
}

export async function getFinancialExposure(
  sortBy: "OverdueBalanceLCY" | "OutstandingBalanceLCY" = "OverdueBalanceLCY",
  limit = 10,
): Promise<FinancialExposureSummary> {
  const data = await getJson<unknown>(
    `/api/analytics/financial-exposure?sortBy=${sortBy}&limit=${limit}`,
  );

  const source = asRecord(data);
  const vendorsValue = getProperty(source, "vendors");

  return {
    totalOutstandingBalanceLCY: numberOrZero(
      getProperty(source, "totalOutstandingBalanceLCY"),
    ),
    totalOverdueBalanceLCY: numberOrZero(
      getProperty(source, "totalOverdueBalanceLCY"),
    ),
    overdueBalancePercentage: numberOrZero(
      getProperty(source, "overdueBalancePercentage"),
    ),
    vendorsWithOutstandingBalance: numberOrZero(
      getProperty(source, "vendorsWithOutstandingBalance"),
    ),
    vendorsWithOverdueBalance: numberOrZero(
      getProperty(source, "vendorsWithOverdueBalance"),
    ),
    vendors: Array.isArray(vendorsValue)
      ? vendorsValue.map(mapFinancialVendor)
      : [],
    generatedAt: asString(getProperty(source, "generatedAt")) || undefined,
  };
}
