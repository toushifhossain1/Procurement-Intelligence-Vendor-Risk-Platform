import type {
  DashboardApiResponse,
  DashboardSummary,
} from "../types/dashboard";

const DASHBOARD_ENDPOINT = "/api/dashboard";

function readNumber(data: DashboardApiResponse, keys: string[]): number {
  for (const key of keys) {
    const matchedKey = Object.keys(data).find(
      (item) => item.toLowerCase() === key.toLowerCase(),
    );

    if (matchedKey !== undefined) {
      const value = data[matchedKey];

      if (typeof value === "number" && Number.isFinite(value)) {
        return value;
      }

      if (typeof value === "string" && value.trim() !== "") {
        const parsed = Number(value);

        if (Number.isFinite(parsed)) {
          return parsed;
        }
      }
    }
  }

  return 0;
}

function readDate(
  data: DashboardApiResponse,
  keys: string[],
): string | undefined {
  for (const key of keys) {
    const matchedKey = Object.keys(data).find(
      (item) => item.toLowerCase() === key.toLowerCase(),
    );

    if (matchedKey !== undefined) {
      const value = data[matchedKey];

      if (typeof value === "string" && value.length > 0) {
        return value;
      }
    }
  }

  return undefined;
}

export async function getDashboardSummary(): Promise<DashboardSummary> {
  const response = await fetch(DASHBOARD_ENDPOINT);

  if (!response.ok) {
    throw new Error(`Dashboard API returned HTTP ${response.status}.`);
  }

  const data: DashboardApiResponse = await response.json();

  return {
    totalVendors: readNumber(data, [
      "totalVendors",
      "totalVendorCount",
      "vendorCount",
    ]),
    lowRiskVendors: readNumber(data, [
      "lowRiskVendors",
      "lowRiskVendorCount",
      "lowRiskCount",
    ]),
    mediumRiskVendors: readNumber(data, [
      "mediumRiskVendors",
      "mediumRiskVendorCount",
      "mediumRiskCount",
    ]),
    highRiskVendors: readNumber(data, [
      "highRiskVendors",
      "highRiskVendorCount",
      "highRiskCount",
    ]),
    criticalRiskVendors: readNumber(data, [
      "criticalRiskVendors",
      "criticalRiskVendorCount",
      "criticalRiskCount",
    ]),
    insufficientDataVendors: readNumber(data, [
      "insufficientDataVendors",
      "insufficientDataVendorCount",
      "insufficientDataCount",
    ]),
    activeAlerts: readNumber(data, ["activeAlerts", "activeAlertCount"]),
    newAlerts: readNumber(data, ["newAlerts", "newAlertCount"]),
    acknowledgedAlerts: readNumber(data, [
      "acknowledgedAlerts",
      "acknowledgedAlertCount",
    ]),
    criticalAlerts: readNumber(data, ["criticalAlerts", "criticalAlertCount"]),
    highSeverityAlerts: readNumber(data, [
      "highSeverityAlerts",
      "highSeverityAlertCount",
    ]),
    generatedAt: readDate(data, ["generatedAt", "lastUpdated", "generatedOn"]),
  };
}
