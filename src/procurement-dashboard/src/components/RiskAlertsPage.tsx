import { useCallback, useEffect, useMemo, useState } from "react";
import {
  AlertCircle,
  AlertTriangle,
  ArrowDownUp,
  Check,
  CheckCheck,
  Clock3,
  Eye,
  Filter,
  RefreshCw,
  Search,
  ShieldAlert,
  X,
  Zap,
} from "lucide-react";

import type { AlertSeverity, AlertStatus, RiskAlert } from "../types/riskAlert";

import type { GenerateAlertsResult } from "../types/riskAlert";

import {
  generateRiskAlerts,
  getRiskAlertById,
  getRiskAlerts,
  updateRiskAlertStatus,
} from "../services/riskAlertService";

type StatusFilter = "All" | AlertStatus;
type SeverityFilter = "All" | AlertSeverity;

function formatDate(value: string | null): string {
  if (!value) return "—";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return new Intl.DateTimeFormat("en-GB", {
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(date);
}

function formatRelativeDate(value: string | null): string {
  if (!value) return "Date unavailable";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) {
    return "Date unavailable";
  }

  const elapsed = Date.now() - date.getTime();
  const minutes = Math.floor(elapsed / 60_000);

  if (minutes < 1) return "Just now";
  if (minutes < 60) return `${minutes}m ago`;

  const hours = Math.floor(minutes / 60);

  if (hours < 24) return `${hours}h ago`;

  const days = Math.floor(hours / 24);

  if (days < 7) return `${days}d ago`;

  return formatDate(value);
}

function severityClass(severity: string): string {
  return `ra-severity--${severity.toLowerCase()}`;
}

function statusClass(status: string): string {
  return `ra-status--${status.toLowerCase().replace(/\s+/g, "-")}`;
}

function getSeverityIcon(severity: string) {
  if (severity === "Critical") {
    return <ShieldAlert size={15} />;
  }

  if (severity === "High") {
    return <AlertTriangle size={15} />;
  }

  return <AlertCircle size={15} />;
}

export default function RiskAlertsPage() {
  const [alerts, setAlerts] = useState<RiskAlert[]>([]);
  const [statusFilter, setStatusFilter] = useState<StatusFilter>("All");
  const [severityFilter, setSeverityFilter] = useState<SeverityFilter>("All");
  const [search, setSearch] = useState("");
  const [sortNewestFirst, setSortNewestFirst] = useState(true);

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [generating, setGenerating] = useState(false);
  const [actionLoading, setActionLoading] = useState(false);
  const [error, setError] = useState("");

  const [selectedAlert, setSelectedAlert] = useState<RiskAlert | null>(null);
  const [resolutionNotes, setResolutionNotes] = useState("");

  const [generationResult, setGenerationResult] =
    useState<GenerateAlertsResult | null>(null);

  const loadAlerts = useCallback(
    async (showRefresh = false) => {
      if (showRefresh) {
        setRefreshing(true);
      } else {
        setLoading(true);
      }

      setError("");

      try {
        const result = await getRiskAlerts(statusFilter, severityFilter);

        setAlerts(result);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Unable to load risk alerts.",
        );
      } finally {
        setLoading(false);
        setRefreshing(false);
      }
    },
    [statusFilter, severityFilter],
  );

  useEffect(() => {
    void loadAlerts();
  }, [loadAlerts]);

  const visibleAlerts = useMemo(() => {
    const query = search.trim().toLowerCase();

    return [...alerts]
      .filter((alert) => {
        if (!query) return true;

        return [
          alert.id,
          alert.vendorNo,
          alert.vendorName,
          alert.alertType,
          alert.title,
          alert.message,
          alert.severity,
          alert.status,
        ].some((value) => value.toLowerCase().includes(query));
      })
      .sort((a, b) => {
        const dateA = a.createdAt ? new Date(a.createdAt).getTime() : 0;

        const dateB = b.createdAt ? new Date(b.createdAt).getTime() : 0;

        return sortNewestFirst ? dateB - dateA : dateA - dateB;
      });
  }, [alerts, search, sortNewestFirst]);

  const counts = useMemo(() => {
    return {
      total: alerts.length,
      new: alerts.filter((a) => a.status === "New").length,
      acknowledged: alerts.filter((a) => a.status === "Acknowledged").length,
      critical: alerts.filter(
        (a) => a.severity === "Critical" && a.status !== "Resolved",
      ).length,
    };
  }, [alerts]);

  const handleGenerate = async () => {
    setGenerating(true);
    setError("");
    setGenerationResult(null);

    try {
      const result = await generateRiskAlerts();

      setGenerationResult(result);
      await loadAlerts(true);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Unable to generate alerts.",
      );
    } finally {
      setGenerating(false);
    }
  };

  const openDetails = async (alert: RiskAlert) => {
    setSelectedAlert(alert);
    setResolutionNotes(alert.resolutionNotes);
    setError("");

    try {
      const details = await getRiskAlertById(alert.id);
      setSelectedAlert(details);
      setResolutionNotes(details.resolutionNotes);
    } catch {
      // The list data remains available if detail retrieval fails.
    }
  };

  const handleStatusUpdate = async (status: AlertStatus) => {
    if (!selectedAlert) return;

    if (status === "Resolved" && !resolutionNotes.trim()) {
      setError("Please enter resolution notes before resolving this alert.");
      return;
    }

    setActionLoading(true);
    setError("");

    try {
      await updateRiskAlertStatus(
        selectedAlert.id,
        status,
        resolutionNotes.trim(),
      );

      setSelectedAlert(null);
      setResolutionNotes("");
      await loadAlerts(true);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Unable to update alert status.",
      );
    } finally {
      setActionLoading(false);
    }
  };

  const closeDetails = () => {
    setSelectedAlert(null);
    setResolutionNotes("");
  };

  return (
    <div className="ra-page">
      <header className="ra-page__header">
        <div>
          <div className="ra-eyebrow">PROCUREMENT INTELLIGENCE</div>

          <h1>Risk Alerts</h1>

          <p className="ra-page__subtitle">
            Monitor vendor risk events, investigate exceptions, and manage alert
            resolution.
          </p>
        </div>

        <div className="ra-page__actions">
          <button
            className="ra-button ra-button--secondary"
            onClick={() => void loadAlerts(true)}
            disabled={refreshing || loading}
          >
            <RefreshCw size={16} className={refreshing ? "ra-spin" : ""} />
            Refresh
          </button>

          <button
            className="ra-button ra-button--primary"
            onClick={() => void handleGenerate()}
            disabled={generating}
          >
            <Zap size={16} />
            {generating ? "Generating..." : "Generate Alerts"}
          </button>
        </div>
      </header>

      {error && (
        <div className="ra-error" role="alert">
          <AlertCircle size={18} />
          <span>{error}</span>
          <button
            type="button"
            aria-label="Dismiss error"
            onClick={() => setError("")}
          >
            <X size={16} />
          </button>
        </div>
      )}

      {generationResult && (
        <div className="ra-generation-result">
          <div className="ra-generation-result__icon">
            <CheckCheck size={20} />
          </div>

          <div className="ra-generation-result__content">
            <strong>Alert generation completed</strong>
            <p>
              Evaluated {generationResult.vendorsEvaluated} vendors; created{" "}
              {generationResult.alertsCreated} alerts; skipped{" "}
              {generationResult.duplicateAlertsSkipped} duplicates.
            </p>
          </div>

          <button
            type="button"
            aria-label="Dismiss generation result"
            onClick={() => setGenerationResult(null)}
          >
            <X size={17} />
          </button>
        </div>
      )}

      <section className="ra-summary-grid">
        <div className="ra-summary-card">
          <div className="ra-summary-card__top">
            <span>Total Alerts</span>
            <span className="ra-summary-card__icon ra-icon--blue">
              <AlertCircle size={19} />
            </span>
          </div>
          <strong>{loading ? "—" : counts.total}</strong>
          <span className="ra-summary-card__caption">
            Matching selected filters
          </span>
        </div>

        <div className="ra-summary-card">
          <div className="ra-summary-card__top">
            <span>New Alerts</span>
            <span className="ra-summary-card__icon ra-icon--red">
              <Zap size={19} />
            </span>
          </div>
          <strong>{loading ? "—" : counts.new}</strong>
          <span className="ra-summary-card__caption">
            Awaiting acknowledgment
          </span>
        </div>

        <div className="ra-summary-card">
          <div className="ra-summary-card__top">
            <span>Acknowledged</span>
            <span className="ra-summary-card__icon ra-icon--amber">
              <Clock3 size={19} />
            </span>
          </div>
          <strong>{loading ? "—" : counts.acknowledged}</strong>
          <span className="ra-summary-card__caption">Under review</span>
        </div>

        <div className="ra-summary-card">
          <div className="ra-summary-card__top">
            <span>Open Critical</span>
            <span className="ra-summary-card__icon ra-icon--purple">
              <ShieldAlert size={19} />
            </span>
          </div>
          <strong>{loading ? "—" : counts.critical}</strong>
          <span className="ra-summary-card__caption">
            Critical and unresolved
          </span>
        </div>
      </section>

      <section className="ra-list-panel">
        <div className="ra-list-panel__header">
          <div>
            <h2>Alert Register</h2>
            <p>Review, prioritize, and resolve procurement exceptions.</p>
          </div>

          <span className="ra-result-count">
            {visibleAlerts.length} result
            {visibleAlerts.length === 1 ? "" : "s"}
          </span>
        </div>

        <div className="ra-toolbar">
          <div className="ra-search">
            <Search size={17} />
            <input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Search vendor, alert type, or description..."
              aria-label="Search alerts"
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
          </div>

          <label className="ra-filter">
            <Filter size={15} />
            <span>Status</span>
            <select
              value={statusFilter}
              onChange={(event) =>
                setStatusFilter(event.target.value as StatusFilter)
              }
            >
              <option value="All">All statuses</option>
              <option value="New">New</option>
              <option value="Acknowledged">Acknowledged</option>
              <option value="Resolved">Resolved</option>
            </select>
          </label>

          <label className="ra-filter">
            <ShieldAlert size={15} />
            <span>Severity</span>
            <select
              value={severityFilter}
              onChange={(event) =>
                setSeverityFilter(event.target.value as SeverityFilter)
              }
            >
              <option value="All">All severities</option>
              <option value="Critical">Critical</option>
              <option value="High">High</option>
              <option value="Medium">Medium</option>
              <option value="Low">Low</option>
            </select>
          </label>

          <button
            type="button"
            className="ra-button ra-button--secondary ra-sort-button"
            onClick={() => setSortNewestFirst((current) => !current)}
            title="Change date sort order"
          >
            <ArrowDownUp size={15} />
            {sortNewestFirst ? "Newest first" : "Oldest first"}
          </button>
        </div>

        <div className="ra-table-wrapper">
          <table className="ra-table">
            <thead>
              <tr>
                <th>Alert</th>
                <th>Vendor</th>
                <th>Severity</th>
                <th>Status</th>
                <th>Created</th>
                <th className="ra-table__action-heading">Action</th>
              </tr>
            </thead>

            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6} className="ra-table__message">
                    <RefreshCw size={18} className="ra-spin" />
                    Loading risk alerts...
                  </td>
                </tr>
              ) : visibleAlerts.length === 0 ? (
                <tr>
                  <td colSpan={6} className="ra-table__message">
                    <div className="ra-empty">
                      <ShieldAlert size={30} />
                      <strong>No alerts found</strong>
                      <span>
                        Try changing your filters or generate alerts from the
                        current vendor risk data.
                      </span>
                    </div>
                  </td>
                </tr>
              ) : (
                visibleAlerts.map((alert) => (
                  <tr key={alert.id}>
                    <td>
                      <div className="ra-alert-cell">
                        <span
                          className={`ra-alert-cell__icon ${severityClass(alert.severity)}`}
                        >
                          {getSeverityIcon(alert.severity)}
                        </span>

                        <div>
                          <strong>{alert.title}</strong>
                          <span>{alert.alertType}</span>
                          <small>ID: {alert.id || "Unavailable"}</small>
                        </div>
                      </div>
                    </td>

                    <td>
                      <div className="ra-vendor-cell">
                        <strong>{alert.vendorName || "Unknown vendor"}</strong>
                        <span>{alert.vendorNo || "No vendor number"}</span>
                      </div>
                    </td>

                    <td>
                      <span
                        className={`ra-severity ${severityClass(alert.severity)}`}
                      >
                        {getSeverityIcon(alert.severity)}
                        {alert.severity}
                      </span>
                    </td>

                    <td>
                      <span
                        className={`ra-status ${statusClass(alert.status)}`}
                      >
                        {alert.status}
                      </span>
                    </td>

                    <td>
                      <div className="ra-date-cell">
                        <strong>{formatRelativeDate(alert.createdAt)}</strong>
                        <span>{formatDate(alert.createdAt)}</span>
                      </div>
                    </td>

                    <td>
                      <button
                        type="button"
                        className="ra-view-button"
                        onClick={() => void openDetails(alert)}
                      >
                        <Eye size={15} />
                        View
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        <footer className="ra-list-footer">
          <span>
            Showing {visibleAlerts.length} of {alerts.length} loaded alerts
          </span>
          <span>
            <span className="ra-live-dot" />
            Connected to procurement API
          </span>
        </footer>
      </section>

      {selectedAlert && (
        <div
          className="ra-modal-backdrop"
          onMouseDown={(event) => {
            if (event.target === event.currentTarget) {
              closeDetails();
            }
          }}
        >
          <section
            className="ra-modal"
            role="dialog"
            aria-modal="true"
            aria-labelledby="ra-modal-title"
          >
            <header className="ra-modal__header">
              <div>
                <span className="ra-eyebrow">ALERT DETAILS</span>
                <h2 id="ra-modal-title">{selectedAlert.title}</h2>
                <p>{selectedAlert.alertType}</p>
              </div>

              <button
                type="button"
                className="ra-icon-button"
                onClick={closeDetails}
                aria-label="Close alert details"
              >
                <X size={20} />
              </button>
            </header>

            <div className="ra-modal__body">
              <div className="ra-detail-badges">
                <span
                  className={`ra-severity ${severityClass(selectedAlert.severity)}`}
                >
                  {getSeverityIcon(selectedAlert.severity)}
                  {selectedAlert.severity} Severity
                </span>

                <span
                  className={`ra-status ${statusClass(selectedAlert.status)}`}
                >
                  {selectedAlert.status}
                </span>
              </div>

              <div className="ra-detail-grid">
                <div className="ra-detail-field">
                  <span>Vendor</span>
                  <strong>{selectedAlert.vendorName}</strong>
                </div>

                <div className="ra-detail-field">
                  <span>Vendor Number</span>
                  <strong>{selectedAlert.vendorNo || "—"}</strong>
                </div>

                <div className="ra-detail-field">
                  <span>Alert ID</span>
                  <strong>{selectedAlert.id || "—"}</strong>
                </div>

                <div className="ra-detail-field">
                  <span>Created At</span>
                  <strong>{formatDate(selectedAlert.createdAt)}</strong>
                </div>

                <div className="ra-detail-field">
                  <span>Acknowledged At</span>
                  <strong>{formatDate(selectedAlert.acknowledgedAt)}</strong>
                </div>

                <div className="ra-detail-field">
                  <span>Resolved At</span>
                  <strong>{formatDate(selectedAlert.resolvedAt)}</strong>
                </div>
              </div>

              <div className="ra-detail-description">
                <h3>Alert Description</h3>
                <p>{selectedAlert.message}</p>
              </div>

              {selectedAlert.status === "Resolved" && (
                <div className="ra-detail-description">
                  <h3>Resolution</h3>
                  <p>
                    {selectedAlert.resolutionNotes ||
                      "No resolution notes were provided."}
                  </p>

                  {selectedAlert.resolvedBy && (
                    <span>Resolved by: {selectedAlert.resolvedBy}</span>
                  )}
                </div>
              )}

              {selectedAlert.status !== "Resolved" && (
                <div className="ra-resolution-form">
                  <label htmlFor="ra-resolution-notes">
                    Resolution notes
                    {selectedAlert.status !== "Resolved" &&
                      " (required to resolve)"}
                  </label>

                  <textarea
                    id="ra-resolution-notes"
                    value={resolutionNotes}
                    onChange={(event) => setResolutionNotes(event.target.value)}
                    rows={4}
                    placeholder="Document the investigation, corrective action, or reason for resolution..."
                  />
                </div>
              )}
            </div>

            <footer className="ra-modal__footer">
              <button
                type="button"
                className="ra-button ra-button--secondary"
                onClick={closeDetails}
                disabled={actionLoading}
              >
                Close
              </button>

              {selectedAlert.status === "New" && (
                <button
                  type="button"
                  className="ra-button ra-button--primary"
                  onClick={() => void handleStatusUpdate("Acknowledged")}
                  disabled={actionLoading}
                >
                  <Check size={16} />
                  {actionLoading ? "Updating..." : "Acknowledge"}
                </button>
              )}

              {selectedAlert.status !== "Resolved" && (
                <button
                  type="button"
                  className="ra-button ra-button--resolve"
                  onClick={() => void handleStatusUpdate("Resolved")}
                  disabled={actionLoading}
                >
                  <CheckCheck size={16} />
                  {actionLoading ? "Resolving..." : "Resolve Alert"}
                </button>
              )}
            </footer>
          </section>
        </div>
      )}
    </div>
  );
}
