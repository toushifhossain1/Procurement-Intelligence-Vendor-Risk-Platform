import { useCallback, useEffect, useMemo, useState } from "react";
import {
  AlertCircle,
  ArrowDownUp,
  Building2,
  CheckCircle2,
  Clock3,
  Eye,
  FileWarning,
  RefreshCw,
  Search,
  ShieldAlert,
  ShieldCheck,
  SlidersHorizontal,
  TrendingUp,
  X,
} from "lucide-react";

import {
  getVendorRiskDetails,
  getVendorRisks,
} from "../services/vendorRiskService";
import type { VendorRisk } from "../types/vendorRisk";

type RiskFilter =
  | "All"
  | "Low"
  | "Medium"
  | "High"
  | "Critical"
  | "Insufficient Data";

type SortMode = "risk-desc" | "risk-asc" | "name-asc" | "vendor-asc";

const riskFilters: RiskFilter[] = [
  "All",
  "Low",
  "Medium",
  "High",
  "Critical",
  "Insufficient Data",
];

function normalizeText(value: string): string {
  return value.trim().toLowerCase();
}

function classificationKey(value: string): string {
  const normalized = normalizeText(value);

  if (normalized.includes("critical")) return "critical";
  if (normalized === "high") return "high";
  if (normalized === "medium") return "medium";
  if (normalized === "low") return "low";
  if (normalized.includes("insufficient")) return "unknown";

  return "unknown";
}

function classificationLabel(value: string): string {
  if (classificationKey(value) === "unknown") return "Insufficient Data";

  return value;
}

function formatScore(value: number | null): string {
  return value === null ? "—" : value.toFixed(1);
}

function formatPercent(value: number | null): string {
  return value === null ? "—" : `${value.toFixed(1)}%`;
}

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("en-BD", {
    style: "currency",
    currency: "BDT",
    maximumFractionDigits: 0,
  }).format(value);
}

function formatDate(value?: string): string {
  if (!value) return "Not available";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) return "Not available";

  return date.toLocaleDateString(undefined, {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });
}

function RiskBadge({ value }: { value: string }) {
  const key = classificationKey(value);

  return (
    <span className={`vr-risk-badge ${key}`}>
      <span className="vr-badge-dot" />
      {classificationLabel(value)}
    </span>
  );
}

function AssessmentBadge({ value }: { value: string }) {
  const normalized = normalizeText(value);
  const type =
    normalized === "complete"
      ? "complete"
      : normalized === "partial"
        ? "partial"
        : "insufficient";

  return (
    <span className={`vr-assessment-badge ${type}`}>{value || "Unknown"}</span>
  );
}

function ScoreBar({ label, value }: { label: string; value: number | null }) {
  const width = value === null ? 0 : Math.max(0, Math.min(value, 100));

  return (
    <div className="vr-score-row">
      <div className="vr-score-row-heading">
        <span>{label}</span>
        <strong>{formatScore(value)}</strong>
      </div>
      <div className="vr-score-track">
        <div className="vr-score-fill" style={{ width: `${width}%` }} />
      </div>
    </div>
  );
}

function MetricCard({
  label,
  value,
  description,
}: {
  label: string;
  value: string;
  description?: string;
}) {
  return (
    <div className="vr-detail-metric">
      <span>{label}</span>
      <strong>{value}</strong>
      {description && <small>{description}</small>}
    </div>
  );
}

function VendorDetailPanel({
  vendor,
  onClose,
}: {
  vendor: VendorRisk;
  onClose: () => void;
}) {
  return (
    <div
      className="vr-modal-backdrop"
      role="presentation"
      onMouseDown={(event) => {
        if (event.target === event.currentTarget) onClose();
      }}
    >
      <section
        className="vr-detail-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="vr-detail-title"
      >
        <header className="vr-detail-header">
          <div className="vr-detail-heading-icon">
            <Building2 size={22} />
          </div>

          <div className="vr-detail-title-copy">
            <span className="vr-detail-kicker">VENDOR RISK PROFILE</span>
            <h2 id="vr-detail-title">{vendor.vendorName}</h2>
            <p>Vendor ID: {vendor.vendorNo}</p>
          </div>

          <button
            className="vr-close-button"
            type="button"
            onClick={onClose}
            aria-label="Close vendor details"
          >
            <X size={20} />
          </button>
        </header>

        <div className="vr-detail-body">
          <section className="vr-detail-summary">
            <div className="vr-overall-score">
              <span>Overall risk score</span>
              <strong>
                {vendor.overallRiskScore === null
                  ? "—"
                  : formatScore(vendor.overallRiskScore)}
                <small>/100</small>
              </strong>
              <RiskBadge value={vendor.riskClassification} />
            </div>

            <div className="vr-detail-assessment">
              <span>Assessment status</span>
              <AssessmentBadge value={vendor.riskAssessmentStatus} />
              <span className="vr-detail-date">
                <Clock3 size={13} />
                {formatDate(vendor.assessmentDate)}
              </span>
            </div>
          </section>

          <section className="vr-detail-section">
            <div className="vr-detail-section-heading">
              <h3>Risk dimensions</h3>
              <span>Higher scores indicate greater risk</span>
            </div>

            <div className="vr-score-list">
              <ScoreBar
                label="Delivery risk"
                value={vendor.deliveryRiskScore}
              />
              <ScoreBar
                label="Financial risk"
                value={vendor.financialRiskScore}
              />
              <ScoreBar label="Invoice risk" value={vendor.invoiceRiskScore} />
              <ScoreBar
                label="Reliability risk"
                value={vendor.reliabilityRiskScore}
              />
            </div>
          </section>

          <section className="vr-detail-section">
            <div className="vr-detail-section-heading">
              <h3>Performance indicators</h3>
              <span>From the existing performance assessment</span>
            </div>

            <div className="vr-detail-metric-grid">
              <MetricCard
                label="On-time delivery"
                value={formatPercent(vendor.onTimeDeliveryRate)}
              />
              <MetricCard
                label="Order fulfillment"
                value={formatPercent(vendor.orderFulfillmentRate)}
              />
              <MetricCard
                label="Invoice discrepancy"
                value={formatPercent(vendor.invoiceDiscrepancyRate)}
              />
              <MetricCard
                label="Performance score"
                value={formatScore(vendor.overallPerformanceScore)}
                description={vendor.performanceRating}
              />
            </div>
          </section>

          <section className="vr-detail-section">
            <div className="vr-detail-section-heading">
              <h3>Financial exposure</h3>
              <span>Local currency amounts</span>
            </div>

            <div className="vr-detail-metric-grid">
              <MetricCard
                label="Outstanding balance"
                value={formatCurrency(vendor.outstandingBalanceLCY)}
                description={`${vendor.openLedgerEntryCount} open ledger entries`}
              />
              <MetricCard
                label="Overdue balance"
                value={formatCurrency(vendor.overdueBalanceLCY)}
                description={`${vendor.overdueLedgerEntryCount} overdue entries`}
              />
              <MetricCard
                label="Overdue percentage"
                value={formatPercent(vendor.overdueBalancePercentage)}
              />
            </div>
          </section>

          <section className="vr-detail-section">
            <div className="vr-detail-section-heading">
              <h3>Risk factors</h3>
              <FileWarning size={17} />
            </div>

            {vendor.riskFactors.length > 0 ? (
              <ul className="vr-detail-list risk-factor-list">
                {vendor.riskFactors.map((factor, index) => (
                  <li key={`${index}-${factor}`}>
                    <AlertCircle size={16} />
                    <span>{factor}</span>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="vr-detail-empty">
                No risk factors were supplied by the API.
              </p>
            )}
          </section>

          <section className="vr-detail-section">
            <div className="vr-detail-section-heading">
              <h3>Recommended actions</h3>
              <CheckCircle2 size={17} />
            </div>

            {vendor.recommendedActions.length > 0 ? (
              <ul className="vr-detail-list recommended-list">
                {vendor.recommendedActions.map((action, index) => (
                  <li key={`${index}-${action}`}>
                    <CheckCircle2 size={16} />
                    <span>{action}</span>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="vr-detail-empty">
                No recommended actions were supplied by the API.
              </p>
            )}
          </section>
        </div>

        <footer className="vr-detail-footer">
          <span>Risk calculations are provided by the backend.</span>
          <button type="button" className="vr-primary-button" onClick={onClose}>
            Close details
          </button>
        </footer>
      </section>
    </div>
  );
}

export default function VendorRiskPage() {
  const [vendors, setVendors] = useState<VendorRisk[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState<RiskFilter>("All");
  const [sort, setSort] = useState<SortMode>("risk-desc");
  const [selectedVendor, setSelectedVendor] = useState<VendorRisk | null>(null);
  const [detailLoading, setDetailLoading] = useState(false);
  const [detailError, setDetailError] = useState<string | null>(null);

  const loadVendors = useCallback(async (isRefresh = false) => {
    if (isRefresh) setRefreshing(true);
    else setLoading(true);

    setError(null);

    try {
      const result = await getVendorRisks();
      setVendors(result);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Unable to load vendor risk data.",
      );
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    void loadVendors();
  }, [loadVendors]);

  const counts = useMemo(() => {
    const result: Record<RiskFilter, number> = {
      All: vendors.length,
      Low: 0,
      Medium: 0,
      High: 0,
      Critical: 0,
      "Insufficient Data": 0,
    };

    for (const vendor of vendors) {
      const key = classificationKey(vendor.riskClassification);

      if (key === "low") result.Low++;
      else if (key === "medium") result.Medium++;
      else if (key === "high") result.High++;
      else if (key === "critical") result.Critical++;
      else result["Insufficient Data"]++;
    }

    return result;
  }, [vendors]);

  const filteredVendors = useMemo(() => {
    const term = normalizeText(search);

    const filtered = vendors.filter((vendor) => {
      const matchesSearch =
        !term ||
        normalizeText(vendor.vendorNo).includes(term) ||
        normalizeText(vendor.vendorName).includes(term);

      const matchesRisk =
        filter === "All" ||
        (filter === "Insufficient Data"
          ? classificationKey(vendor.riskClassification) === "unknown"
          : classificationKey(vendor.riskClassification) ===
            classificationKey(filter));

      return matchesSearch && matchesRisk;
    });

    return filtered.sort((a, b) => {
      switch (sort) {
        case "risk-asc":
          return (a.overallRiskScore ?? -1) - (b.overallRiskScore ?? -1);
        case "name-asc":
          return a.vendorName.localeCompare(b.vendorName);
        case "vendor-asc":
          return a.vendorNo.localeCompare(b.vendorNo);
        default:
          return (b.overallRiskScore ?? -1) - (a.overallRiskScore ?? -1);
      }
    });
  }, [vendors, search, filter, sort]);

  const openVendorDetails = async (vendor: VendorRisk) => {
    setSelectedVendor(vendor);
    setDetailError(null);
    setDetailLoading(true);

    try {
      const details = await getVendorRiskDetails(vendor.vendorNo);
      setSelectedVendor(details);
    } catch (err) {
      setDetailError(
        err instanceof Error
          ? err.message
          : "Could not load the vendor's full risk profile.",
      );
      // Keep the list record visible as a fallback if detail routing differs.
    } finally {
      setDetailLoading(false);
    }
  };

  const highRiskCount = counts.High + counts.Critical;
  const assessedCount = vendors.length - counts["Insufficient Data"];
  const coverage =
    vendors.length > 0 ? Math.round((assessedCount / vendors.length) * 100) : 0;

  return (
    <div className="vendor-risk-page">
      <div className="vr-page-heading">
        <div>
          <div className="eyebrow">PROCUREMENT INTELLIGENCE</div>
          <h1>Vendor risk management</h1>
          <p>
            Assess vendor exposure, investigate risk drivers, and prioritize
            procurement reviews.
          </p>
        </div>

        <button
          type="button"
          className="vr-secondary-button"
          onClick={() => void loadVendors(true)}
          disabled={loading || refreshing}
        >
          <RefreshCw size={16} className={refreshing ? "spin" : ""} />
          Refresh
        </button>
      </div>

      <div className="vr-summary-grid">
        <article className="vr-summary-card">
          <span>Total vendors</span>
          <div className="vr-summary-value">
            {loading ? "—" : vendors.length.toLocaleString()}
          </div>
          <small>Vendor risk assessments</small>
          <Building2 size={19} />
        </article>

        <article className="vr-summary-card danger">
          <span>High / critical risk</span>
          <div className="vr-summary-value">
            {loading ? "—" : highRiskCount.toLocaleString()}
          </div>
          <small>Prioritize for review</small>
          <ShieldAlert size={19} />
        </article>

        <article className="vr-summary-card success">
          <span>Risk coverage</span>
          <div className="vr-summary-value">
            {loading ? "—" : `${coverage}%`}
          </div>
          <small>{counts["Insufficient Data"]} need more data</small>
          <ShieldCheck size={19} />
        </article>

        <article className="vr-summary-card warning">
          <span>Critical vendors</span>
          <div className="vr-summary-value">
            {loading ? "—" : counts.Critical.toLocaleString()}
          </div>
          <small>Highest risk classification</small>
          <TrendingUp size={19} />
        </article>
      </div>

      {error && (
        <div className="vr-error-banner" role="alert">
          <AlertCircle size={20} />
          <div>
            <strong>Unable to load the vendor register</strong>
            <p>{error}</p>
            <span>
              Open Swagger and confirm the route that returns the full vendor
              risk assessment list. Then update LIST_ENDPOINTS in
              vendorRiskService.ts if necessary.
            </span>
          </div>
          <button
            type="button"
            className="vr-secondary-button"
            onClick={() => void loadVendors(true)}
            disabled={refreshing}
          >
            Retry
          </button>
        </div>
      )}

      <section className="vr-register-panel">
        <div className="vr-register-heading">
          <div>
            <h2>Vendor register</h2>
            <p>Search, filter, and inspect individual risk assessments.</p>
          </div>
          <div className="vr-register-total">
            {filteredVendors.length.toLocaleString()} records
          </div>
        </div>

        <div
          className="vr-filter-tabs"
          aria-label="Filter by risk classification"
        >
          {riskFilters.map((item) => (
            <button
              type="button"
              key={item}
              className={`vr-filter-tab ${filter === item ? "active" : ""} ${
                item === "Critical" ? "critical-tab" : ""
              }`}
              onClick={() => setFilter(item)}
              aria-pressed={filter === item}
            >
              {item}
              <span>{counts[item]}</span>
            </button>
          ))}
        </div>

        <div className="vr-toolbar">
          <label className="vr-search">
            <Search size={17} />
            <input
              type="search"
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Search vendor number or name..."
              aria-label="Search vendors"
            />
            {search && (
              <button
                type="button"
                aria-label="Clear search"
                onClick={() => setSearch("")}
              >
                <X size={15} />
              </button>
            )}
          </label>

          <label className="vr-sort-control">
            <SlidersHorizontal size={15} />
            <span>Sort by</span>
            <select
              value={sort}
              onChange={(event) => setSort(event.target.value as SortMode)}
              aria-label="Sort vendors"
            >
              <option value="risk-desc">Highest risk</option>
              <option value="risk-asc">Lowest risk</option>
              <option value="name-asc">Vendor name</option>
              <option value="vendor-asc">Vendor number</option>
            </select>
            <ArrowDownUp size={14} />
          </label>
        </div>

        <div className="vr-table-wrap">
          <table className="vr-table">
            <thead>
              <tr>
                <th>Vendor</th>
                <th>Overall risk</th>
                <th>Classification</th>
                <th>Assessment</th>
                <th>Performance</th>
                <th>Overdue balance</th>
                <th aria-label="Actions" />
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={7} className="vr-state-cell">
                    <RefreshCw size={19} className="spin" />
                    Loading vendor risk assessments…
                  </td>
                </tr>
              ) : filteredVendors.length === 0 ? (
                <tr>
                  <td colSpan={7} className="vr-state-cell">
                    <Search size={20} />
                    <strong>No matching vendors found</strong>
                    <span>
                      {vendors.length === 0
                        ? "The API returned no vendor assessments."
                        : "Try another search term or risk classification."}
                    </span>
                    {(search || filter !== "All") && (
                      <button
                        type="button"
                        className="vr-link-button"
                        onClick={() => {
                          setSearch("");
                          setFilter("All");
                        }}
                      >
                        Clear filters
                      </button>
                    )}
                  </td>
                </tr>
              ) : (
                filteredVendors.map((vendor) => (
                  <tr key={vendor.vendorNo}>
                    <td>
                      <div className="vr-vendor-cell">
                        <div className="vr-vendor-avatar">
                          {vendor.vendorName.trim().charAt(0).toUpperCase() ||
                            "V"}
                        </div>
                        <div>
                          <strong>{vendor.vendorName}</strong>
                          <span>{vendor.vendorNo}</span>
                        </div>
                      </div>
                    </td>
                    <td>
                      <div className="vr-score-cell">
                        <strong>{formatScore(vendor.overallRiskScore)}</strong>
                        <span>/100</span>
                      </div>
                    </td>
                    <td>
                      <RiskBadge value={vendor.riskClassification} />
                    </td>
                    <td>
                      <AssessmentBadge value={vendor.riskAssessmentStatus} />
                    </td>
                    <td>
                      <div className="vr-performance-cell">
                        <strong>
                          {formatScore(vendor.overallPerformanceScore)}
                        </strong>
                        <span>{vendor.performanceRating}</span>
                      </div>
                    </td>
                    <td>
                      <div className="vr-balance-cell">
                        <strong>
                          {formatCurrency(vendor.overdueBalanceLCY)}
                        </strong>
                        <span>
                          {formatPercent(vendor.overdueBalancePercentage)}{" "}
                          overdue
                        </span>
                      </div>
                    </td>
                    <td>
                      <button
                        type="button"
                        className="vr-view-button"
                        onClick={() => void openVendorDetails(vendor)}
                        aria-label={`View risk details for ${vendor.vendorName}`}
                      >
                        <Eye size={15} />
                        Details
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        <div className="vr-table-footer">
          <span>
            Showing {filteredVendors.length} of {vendors.length} vendor
            {vendors.length === 1 ? "" : "s"}
          </span>
          <span>
            <ShieldCheck size={14} />
            Risk scores calculated by the backend
          </span>
        </div>
      </section>

      <div className="vr-disclaimer">
        <AlertCircle size={16} />
        <p>
          Risk classifications support procurement review; they should be
          interpreted alongside source data, vendor context, and your
          organization's approved procurement policies.
        </p>
      </div>

      {selectedVendor && (
        <VendorDetailPanel
          vendor={selectedVendor}
          onClose={() => {
            setSelectedVendor(null);
            setDetailError(null);
          }}
        />
      )}

      {selectedVendor && detailLoading && (
        <div className="vr-detail-loading" role="status">
          <RefreshCw size={15} className="spin" />
          Loading latest vendor details…
        </div>
      )}

      {selectedVendor && detailError && !detailLoading && (
        <div className="vr-detail-error" role="alert">
          <AlertCircle size={16} />
          <span>
            Could not refresh full detail data. Showing the risk data available
            in the register. {detailError}
          </span>
          <button
            type="button"
            onClick={() => void openVendorDetails(selectedVendor)}
          >
            Retry
          </button>
        </div>
      )}
    </div>
  );
}
