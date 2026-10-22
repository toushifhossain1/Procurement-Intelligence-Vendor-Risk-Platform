import type { VendorRisk } from "../types/vendorRisk";

type JsonObject = Record<string, unknown>;

const LIST_ENDPOINTS = [
  "/api/vendor-risk",
  "/api/vendor-risks",
  "/api/vendors/risks",
  "/api/vendors/risk-assessments",
];

const DETAIL_ENDPOINTS = (vendorNo: string) => {
  const id = encodeURIComponent(vendorNo);

  return [
    `/api/vendors/${id}/risk-details`,
    `/api/vendors/${id}/risk`,
    `/api/vendor-risk/${id}`,
    `/api/vendor-risks/${id}`,
  ];
};

function isObject(value: unknown): value is JsonObject {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

function getValue(data: JsonObject, aliases: string[]): unknown {
  for (const alias of aliases) {
    const key = Object.keys(data).find(
      (item) => item.toLowerCase() === alias.toLowerCase(),
    );

    if (key !== undefined && data[key] !== undefined) {
      return data[key];
    }
  }

  return undefined;
}

function getString(data: JsonObject, aliases: string[], fallback = ""): string {
  const value = getValue(data, aliases);

  return typeof value === "string" || typeof value === "number"
    ? String(value)
    : fallback;
}

function getNumber(data: JsonObject, aliases: string[]): number | null {
  const value = getValue(data, aliases);

  if (value === null || value === undefined || value === "") {
    return null;
  }

  const parsed = typeof value === "number" ? value : Number(value);

  return Number.isFinite(parsed) ? parsed : null;
}

function getCount(data: JsonObject, aliases: string[]): number {
  return getNumber(data, aliases) ?? 0;
}

function getStringArray(data: JsonObject, aliases: string[]): string[] {
  const value = getValue(data, aliases);

  if (!Array.isArray(value)) return [];

  return value.filter((item): item is string => typeof item === "string");
}

function normalizeVendorRisk(value: unknown): VendorRisk {
  if (!isObject(value)) {
    throw new Error("The vendor risk API returned an invalid record.");
  }

  return {
    vendorNo: getString(value, ["vendorNo", "vendorNumber", "no"]),
    vendorName: getString(
      value,
      ["vendorName", "name", "displayName"],
      "Unnamed vendor",
    ),

    overallRiskScore: getNumber(value, ["overallRiskScore"]),
    riskClassification: getString(
      value,
      ["riskClassification", "classification"],
      "Insufficient Data",
    ),
    riskAssessmentStatus: getString(
      value,
      ["riskAssessmentStatus", "assessmentStatus", "status"],
      "Unknown",
    ),
    assessmentDate:
      getString(value, ["assessmentDate", "assessedAt", "lastAssessed"]) ||
      undefined,

    deliveryRiskScore: getNumber(value, ["deliveryRiskScore"]),
    financialRiskScore: getNumber(value, ["financialRiskScore"]),
    invoiceRiskScore: getNumber(value, ["invoiceRiskScore"]),
    reliabilityRiskScore: getNumber(value, ["reliabilityRiskScore"]),

    onTimeDeliveryRate: getNumber(value, ["onTimeDeliveryRate"]),
    orderFulfillmentRate: getNumber(value, ["orderFulfillmentRate"]),
    invoiceDiscrepancyRate: getNumber(value, ["invoiceDiscrepancyRate"]),
    overallPerformanceScore: getNumber(value, ["overallPerformanceScore"]),
    performanceRating: getString(
      value,
      ["performanceRating"],
      "Insufficient Data",
    ),

    outstandingBalanceLCY: getCount(value, ["outstandingBalanceLCY"]),
    overdueBalanceLCY: getCount(value, ["overdueBalanceLCY"]),
    overdueBalancePercentage: getNumber(value, ["overdueBalancePercentage"]),
    openLedgerEntryCount: getCount(value, ["openLedgerEntryCount"]),
    overdueLedgerEntryCount: getCount(value, ["overdueLedgerEntryCount"]),

    riskFactors: getStringArray(value, ["riskFactors"]),
    recommendedActions: getStringArray(value, ["recommendedActions"]),
  };
}

function extractArray(payload: unknown): unknown[] {
  if (Array.isArray(payload)) return payload;

  if (isObject(payload)) {
    for (const key of ["items", "data", "results", "vendors", "vendorRisks"]) {
      const value = getValue(payload, [key]);

      if (Array.isArray(value)) return value;
    }
  }

  throw new Error(
    "The vendor risk endpoint did not return a JSON array or a supported array wrapper.",
  );
}

async function requestJson(url: string): Promise<unknown> {
  const response = await fetch(url, {
    headers: { Accept: "application/json" },
  });

  if (!response.ok) {
    const error = new Error(`HTTP ${response.status} from ${url}`);
    Object.assign(error, { status: response.status });
    throw error;
  }

  return response.json();
}

function isMissingRoute(error: unknown): boolean {
  if (!isObject(error)) return false;

  return error.status === 404 || error.status === 405;
}

export async function getVendorRisks(): Promise<VendorRisk[]> {
  const routeErrors: string[] = [];

  for (const endpoint of LIST_ENDPOINTS) {
    try {
      const payload = await requestJson(endpoint);
      return extractArray(payload).map(normalizeVendorRisk);
    } catch (error) {
      if (!isMissingRoute(error)) {
        throw error;
      }

      routeErrors.push(endpoint);
    }
  }

  throw new Error(
    "Could not find the vendor risk list endpoint. Tried: " +
      routeErrors.join(", ") +
      ". Check Swagger and update LIST_ENDPOINTS in vendorRiskService.ts to match your controller route.",
  );
}

export async function getVendorRiskDetails(
  vendorNo: string,
): Promise<VendorRisk> {
  let lastError: unknown;

  for (const endpoint of DETAIL_ENDPOINTS(vendorNo)) {
    try {
      const payload = await requestJson(endpoint);

      if (isObject(payload)) {
        const nested = getValue(payload, ["data", "vendorRisk", "riskDetails"]);

        return normalizeVendorRisk(nested ?? payload);
      }

      throw new Error(
        "The vendor risk detail endpoint returned an invalid response.",
      );
    } catch (error) {
      lastError = error;

      if (!isMissingRoute(error)) {
        throw error;
      }
    }
  }

  throw lastError instanceof Error
    ? lastError
    : new Error(
        `Could not find a risk detail endpoint for vendor ${vendorNo}.`,
      );
}
