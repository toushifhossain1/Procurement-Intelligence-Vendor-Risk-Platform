import type {
  GenerateAlertsResult,
  RiskAlert,
  AlertStatus,
} from "../types/riskAlert";

const API_BASE = "/api/risk-alerts";

type ApiObject = Record<string, unknown>;

function normalizeKey(key: string): string {
  return key.replace(/[^a-z0-9]/gi, "").toLowerCase();
}

function getValue(obj: ApiObject, ...aliases: string[]): unknown {
  const normalized = new Map(
    Object.entries(obj).map(([key, value]) => [normalizeKey(key), value]),
  );

  for (const alias of aliases) {
    const value = normalized.get(normalizeKey(alias));

    if (value !== undefined && value !== null) {
      return value;
    }
  }

  return undefined;
}

function toStringValue(value: unknown, fallback = ""): string {
  if (value === null || value === undefined) {
    return fallback;
  }

  return String(value);
}

function toDateValue(value: unknown): string | null {
  if (value === null || value === undefined || value === "") {
    return null;
  }

  return String(value);
}

function normalizeAlert(value: unknown): RiskAlert {
  const obj = value as ApiObject;

  return {
    id: toStringValue(
      getValue(obj, "vendorRiskAlertID", "riskAlertId", "alertId", "id"),
    ),
    vendorNo: toStringValue(getValue(obj, "vendorNo", "vendorNumber")),
    vendorName: toStringValue(
      getValue(obj, "vendorName", "name"),
      "Unknown vendor",
    ),
    alertType: toStringValue(getValue(obj, "alertType", "type"), "Risk Alert"),
    severity: toStringValue(
      getValue(obj, "severity", "alertSeverity"),
      "Medium",
    ),
    status: toStringValue(getValue(obj, "status", "alertStatus"), "New"),
    title: toStringValue(
      getValue(obj, "title", "alertTitle", "subject"),
      toStringValue(getValue(obj, "alertType", "type"), "Risk Alert"),
    ),
    message: toStringValue(
      getValue(obj, "message", "description", "alertMessage", "details"),
      "No additional description provided.",
    ),
    createdAt: toDateValue(
      getValue(obj, "createdAt", "createdOn", "creationDate"),
    ),
    acknowledgedAt: toDateValue(getValue(obj, "acknowledgedAt")),
    resolvedAt: toDateValue(getValue(obj, "resolvedAt")),
    resolvedBy: toStringValue(getValue(obj, "resolvedBy")),
    resolutionNotes: toStringValue(getValue(obj, "resolutionNotes")),
  };
}

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    ...options,
    headers: {
      Accept: "application/json",
      ...(options?.body ? { "Content-Type": "application/json" } : {}),
      ...options?.headers,
    },
  });

  if (!response.ok) {
    const errorText = await response.text();

    throw new Error(
      errorText || `Request failed with status ${response.status}`,
    );
  }

  if (response.status === 204) {
    return undefined as T;
  }

  const text = await response.text();

  if (!text.trim()) {
    return undefined as T;
  }

  return JSON.parse(text) as T;
}

function extractAlertArray(response: unknown): unknown[] {
  if (Array.isArray(response)) {
    return response;
  }

  if (response && typeof response === "object") {
    const obj = response as ApiObject;

    const data = getValue(obj, "items", "data", "alerts", "results");

    if (Array.isArray(data)) {
      return data;
    }
  }

  return [];
}

export async function getRiskAlerts(
  status?: string,
  severity?: string,
): Promise<RiskAlert[]> {
  const params = new URLSearchParams();

  if (status && status !== "All") {
    params.set("status", status);
  }

  if (severity && severity !== "All") {
    params.set("severity", severity);
  }

  const query = params.toString();
  const url = query ? `${API_BASE}?${query}` : API_BASE;

  const response = await request<unknown>(url);

  return extractAlertArray(response).map(normalizeAlert);
}

export async function getRiskAlertById(id: string): Promise<RiskAlert> {
  const response = await request<unknown>(
    `${API_BASE}/${encodeURIComponent(id)}`,
  );

  return normalizeAlert(response);
}

export async function getVendorRiskAlerts(
  vendorNo: string,
): Promise<RiskAlert[]> {
  const response = await request<unknown>(
    `${API_BASE}/vendor/${encodeURIComponent(vendorNo)}`,
  );

  return extractAlertArray(response).map(normalizeAlert);
}

export async function updateRiskAlertStatus(
  id: string,
  status: AlertStatus,
  resolutionNotes = "",
  resolvedBy = "Procurement Dashboard User",
): Promise<void> {
  await request<unknown>(`${API_BASE}/${encodeURIComponent(id)}/status`, {
    method: "PATCH",
    body: JSON.stringify({
      status,
      resolvedBy,
      resolutionNotes,
    }),
  });
}

export async function generateRiskAlerts(): Promise<GenerateAlertsResult> {
  const response = await request<unknown>(`${API_BASE}/generate`, {
    method: "POST",
  });

  const obj = (response ?? {}) as ApiObject;

  return {
    vendorsEvaluated: Number(getValue(obj, "vendorsEvaluated") ?? 0),
    alertsCreated: Number(getValue(obj, "alertsCreated") ?? 0),
    duplicateAlertsSkipped: Number(
      getValue(obj, "duplicateAlertsSkipped") ?? 0,
    ),
    newAlerts: Number(getValue(obj, "newAlerts") ?? 0),
  };
}
