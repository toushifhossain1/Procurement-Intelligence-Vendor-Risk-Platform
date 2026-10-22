import { useCallback, useEffect, useMemo, useState } from "react";
import {
  Activity,
  AlertCircle,
  AlertTriangle,
  ArrowDownRight,
  ArrowUpRight,
  Bell,
  CheckCircle2,
  Clock3,
  LayoutDashboard,
  LoaderCircle,
  Menu,
  ShieldCheck,
  Users,
  X,
} from "lucide-react";

import ProcurementAnalyticsPage from "./components/ProcurementAnalyticsPage";
import VendorRiskPage from "./components/VendorRiskPage";
import { getDashboardSummary } from "./services/dashboardService";
import type { DashboardSummary } from "./types/dashboard";
import "./App.css";

type NavigationItem = {
  label: string;
  icon: typeof LayoutDashboard;
};

const navigationItems: NavigationItem[] = [
  { label: "Overview", icon: LayoutDashboard },
  { label: "Vendor Risk", icon: Users },
  { label: "Procurement Analytics", icon: Activity },
  { label: "Risk Alerts", icon: Bell },
];

const initialDashboard: DashboardSummary = {
  totalVendors: 0,
  lowRiskVendors: 0,
  mediumRiskVendors: 0,
  highRiskVendors: 0,
  criticalRiskVendors: 0,
  insufficientDataVendors: 0,
  activeAlerts: 0,
  newAlerts: 0,
  acknowledgedAlerts: 0,
  criticalAlerts: 0,
  highSeverityAlerts: 0,
};

function formatNumber(value: number): string {
  return new Intl.NumberFormat("en-US", {
    maximumFractionDigits: 0,
  }).format(value);
}

function formatUpdatedAt(value: string | null): string {
  if (!value) return "Not yet updated";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) return "Not available";

  return date.toLocaleString(undefined, {
    day: "2-digit",
    month: "short",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

function RiskBar({
  label,
  count,
  total,
  color,
}: {
  label: string;
  count: number;
  total: number;
  color: string;
}) {
  const percentage =
    total > 0 ? Math.min(100, Math.max(0, (count / total) * 100)) : 0;

  return (
    <div className="risk-bar-row">
      <div className="risk-bar-heading">
        <span className="risk-bar-label">
          <span className={`risk-legend-dot ${color}`} />
          {label}
        </span>
        <strong>{formatNumber(count)}</strong>
      </div>

      <div
        className="risk-bar-track"
        role="progressbar"
        aria-label={label}
        aria-valuemin={0}
        aria-valuemax={100}
        aria-valuenow={Math.round(percentage)}
      >
        <div
          className={`risk-bar-fill ${color}`}
          style={{ width: `${percentage}%` }}
        />
      </div>
    </div>
  );
}

export default function App() {
  const [selectedPage, setSelectedPage] = useState("Overview");
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const [dashboard, setDashboard] =
    useState<DashboardSummary>(initialDashboard);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [lastUpdated, setLastUpdated] = useState<string | null>(null);

  const loadDashboard = useCallback(async (isRefresh = false) => {
    if (isRefresh) {
      setRefreshing(true);
    } else {
      setLoading(true);
    }

    setError(null);

    try {
      const result = await getDashboardSummary();

      setDashboard(result);
      setLastUpdated(result.generatedAt ?? new Date().toISOString());
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "An unexpected error occurred while loading dashboard data.",
      );
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    void loadDashboard();
  }, [loadDashboard]);

  const riskTotal =
    dashboard.lowRiskVendors +
    dashboard.mediumRiskVendors +
    dashboard.highRiskVendors +
    dashboard.criticalRiskVendors +
    dashboard.insufficientDataVendors;

  const highRiskTotal =
    dashboard.highRiskVendors + dashboard.criticalRiskVendors;

  const assessedCount =
    dashboard.totalVendors - dashboard.insufficientDataVendors;

  const riskCoverage =
    dashboard.totalVendors > 0
      ? Math.round((Math.max(0, assessedCount) / dashboard.totalVendors) * 100)
      : 0;

  const kpis = useMemo(
    () => [
      {
        label: "Total Vendors",
        value: dashboard.totalVendors,
        detail: "Vendors in the procurement dataset",
        icon: Users,
        tone: "blue",
      },
      {
        label: "High / Critical Risk",
        value: highRiskTotal,
        detail: "Vendors requiring closer review",
        icon: AlertTriangle,
        tone: "red",
      },
      {
        label: "Active Risk Alerts",
        value: dashboard.activeAlerts,
        detail: "Alerts not yet resolved",
        icon: Bell,
        tone: "amber",
      },
      {
        label: "Risk Coverage",
        value: `${riskCoverage}%`,
        detail: "Vendors with sufficient risk data",
        icon: ShieldCheck,
        tone: "green",
      },
    ],
    [dashboard, highRiskTotal, riskCoverage],
  );

  const handleNavigation = (label: string) => {
    if (
      label === "Overview" ||
      label === "Vendor Risk" ||
      label === "Procurement Analytics"
    ) {
      setSelectedPage(label);
      setSidebarOpen(false);
      return;
    }

    window.alert(`${label} will be implemented in a later feature.`);
    setSidebarOpen(false);
  };

  return (
    <div className="app-shell">
      {sidebarOpen && (
        <button
          type="button"
          className="sidebar-backdrop"
          aria-label="Close navigation"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      <aside className={`sidebar ${sidebarOpen ? "sidebar-open" : ""}`}>
        <div className="brand">
          <div className="brand-mark">
            <ShieldCheck size={23} />
          </div>

          <div className="brand-copy">
            <strong>Procurement</strong>
            <span>INTELLIGENCE PLATFORM</span>
          </div>

          <button
            type="button"
            className="mobile-close-button"
            aria-label="Close navigation"
            onClick={() => setSidebarOpen(false)}
          >
            <X size={19} />
          </button>
        </div>

        <div className="sidebar-section-label">WORKSPACE</div>

        <nav className="main-navigation" aria-label="Main navigation">
          {navigationItems.map((item) => {
            const Icon = item.icon;
            const isActive = selectedPage === item.label;

            return (
              <button
                type="button"
                key={item.label}
                className={`nav-item ${isActive ? "active" : ""}`}
                onClick={() => handleNavigation(item.label)}
                aria-current={isActive ? "page" : undefined}
              >
                <Icon size={18} />
                <span>{item.label}</span>

                {item.label === "Risk Alerts" && dashboard.activeAlerts > 0 && (
                  <span className="nav-count">{dashboard.activeAlerts}</span>
                )}
              </button>
            );
          })}
        </nav>

        <div className="sidebar-bottom">
          <div className="sidebar-status">
            <span className="status-indicator" />
            <div>
              <strong>Platform status</strong>
              <span>Local development</span>
            </div>
          </div>

          <div className="sidebar-footer">
            Procurement Intelligence
            <br />
            Vendor Risk Platform
          </div>
        </div>
      </aside>

      <div className="main-area">
        <header className="topbar">
          <div className="topbar-left">
            <button
              type="button"
              className="mobile-menu-button"
              aria-label="Open navigation"
              onClick={() => setSidebarOpen(true)}
            >
              <Menu size={20} />
            </button>

            <div className="breadcrumb">
              <span>Workspace</span>
              <span className="breadcrumb-separator">/</span>
              <strong>{selectedPage}</strong>
            </div>
          </div>

          <div className="topbar-right">
            <div className="environment-badge">
              <span className="environment-dot" />
              Development
            </div>

            <div className="user-avatar" title="Local development user">
              TH
            </div>
          </div>
        </header>

        <main className="page-content">
          {selectedPage === "Vendor Risk" ? (
            <VendorRiskPage />
          ) : selectedPage === "Procurement Analytics" ? (
            <ProcurementAnalyticsPage />
          ) : (
            <>
              <section className="page-heading">
                <div>
                  <div className="eyebrow">PROCUREMENT INTELLIGENCE</div>
                  <h1>Overview dashboard</h1>
                  <p>
                    Monitor vendor risk, procurement performance, and
                    outstanding alerts from one place.
                  </p>
                </div>

                <div className="updated-label">
                  <Clock3 size={15} />
                  <span>Updated: {formatUpdatedAt(lastUpdated)}</span>
                  <button
                    type="button"
                    className="refresh-icon-button"
                    aria-label="Refresh dashboard"
                    onClick={() => void loadDashboard(true)}
                    disabled={loading || refreshing}
                  >
                    <LoaderCircle
                      size={15}
                      className={refreshing ? "spin" : ""}
                    />
                  </button>
                </div>
              </section>

              {error && (
                <section className="error-banner" role="alert">
                  <AlertCircle size={20} />
                  <div>
                    <strong>Dashboard data could not be loaded</strong>
                    <p>{error}</p>
                    <span>
                      Confirm that the .NET API is running and that GET
                      /api/dashboard works in Swagger.
                    </span>
                  </div>

                  <button
                    type="button"
                    className="retry-button"
                    onClick={() => void loadDashboard(true)}
                    disabled={refreshing}
                  >
                    Retry
                  </button>
                </section>
              )}

              <section className="section-heading">
                <div>
                  <h2>Business overview</h2>
                  <p>Key indicators across your vendor portfolio</p>
                </div>

                <span className="live-label">
                  <span className="status-indicator" />
                  API-connected metrics
                </span>
              </section>

              <section
                className="kpi-grid"
                aria-label="Key performance indicators"
              >
                {kpis.map((kpi) => {
                  const Icon = kpi.icon;

                  return (
                    <article className="kpi-card" key={kpi.label}>
                      <div className="kpi-card-top">
                        <span className="kpi-label">{kpi.label}</span>

                        <div className={`kpi-icon ${kpi.tone}`}>
                          <Icon size={19} />
                        </div>
                      </div>

                      <div className="kpi-value">
                        {loading ? (
                          <span className="skeleton-value" />
                        ) : typeof kpi.value === "number" ? (
                          formatNumber(kpi.value)
                        ) : (
                          kpi.value
                        )}
                      </div>

                      <div className="kpi-detail">{kpi.detail}</div>
                    </article>
                  );
                })}
              </section>

              <section className="insights-grid">
                <article className="panel risk-panel">
                  <div className="panel-header">
                    <div>
                      <h2>Vendor risk distribution</h2>
                      <p>Current risk classification breakdown</p>
                    </div>

                    <div className="panel-icon">
                      <ShieldCheck size={19} />
                    </div>
                  </div>

                  <div className="risk-total">
                    <div>
                      <span className="muted-label">Classified vendors</span>
                      <div className="risk-total-number">
                        {loading ? "—" : formatNumber(riskTotal)}
                      </div>
                    </div>

                    <span className="subtle-pill">
                      {loading ? "Loading" : `${riskCoverage}% assessed`}
                    </span>
                  </div>

                  <div className="risk-bars">
                    <RiskBar
                      label="Low risk"
                      count={dashboard.lowRiskVendors}
                      total={riskTotal}
                      color="low"
                    />
                    <RiskBar
                      label="Medium risk"
                      count={dashboard.mediumRiskVendors}
                      total={riskTotal}
                      color="medium"
                    />
                    <RiskBar
                      label="High risk"
                      count={dashboard.highRiskVendors}
                      total={riskTotal}
                      color="high"
                    />
                    <RiskBar
                      label="Critical risk"
                      count={dashboard.criticalRiskVendors}
                      total={riskTotal}
                      color="critical"
                    />
                    <RiskBar
                      label="Insufficient data"
                      count={dashboard.insufficientDataVendors}
                      total={riskTotal}
                      color="unknown"
                    />
                  </div>

                  <div className="panel-note">
                    <AlertTriangle size={16} />
                    <span>
                      {highRiskTotal > 0
                        ? `${formatNumber(highRiskTotal)} vendors are classified as high or critical risk.`
                        : "No high or critical risk vendors are currently reported by the API."}
                    </span>
                  </div>
                </article>

                <article className="panel alerts-panel">
                  <div className="panel-header">
                    <div>
                      <h2>Risk alert summary</h2>
                      <p>Current alert workload</p>
                    </div>

                    <div className="panel-icon amber">
                      <Bell size={19} />
                    </div>
                  </div>

                  <div className="alert-highlight">
                    <div className="alert-highlight-copy">
                      <span>Active alerts</span>
                      <strong>
                        {loading ? "—" : formatNumber(dashboard.activeAlerts)}
                      </strong>
                      <small>Awaiting final resolution</small>
                    </div>

                    <div className="alert-highlight-icon">
                      <AlertTriangle size={24} />
                    </div>
                  </div>

                  <div className="alert-stat-list">
                    <div className="alert-stat-row">
                      <div className="alert-stat-name">
                        <span className="alert-status-dot new" />
                        <span>New alerts</span>
                      </div>
                      <strong>{formatNumber(dashboard.newAlerts)}</strong>
                      <span className="alert-status-tag new-tag">New</span>
                    </div>

                    <div className="alert-stat-row">
                      <div className="alert-stat-name">
                        <span className="alert-status-dot acknowledged" />
                        <span>Acknowledged</span>
                      </div>
                      <strong>
                        {formatNumber(dashboard.acknowledgedAlerts)}
                      </strong>
                      <span className="alert-status-tag acknowledged-tag">
                        In progress
                      </span>
                    </div>

                    <div className="alert-stat-row">
                      <div className="alert-stat-name">
                        <span className="alert-status-dot critical" />
                        <span>Critical severity</span>
                      </div>
                      <strong>{formatNumber(dashboard.criticalAlerts)}</strong>
                      <span className="alert-status-tag critical-tag">
                        Critical
                      </span>
                    </div>

                    <div className="alert-stat-row">
                      <div className="alert-stat-name">
                        <span className="alert-status-dot high" />
                        <span>High severity</span>
                      </div>
                      <strong>
                        {formatNumber(dashboard.highSeverityAlerts)}
                      </strong>
                      <span className="alert-status-tag high-tag">High</span>
                    </div>
                  </div>

                  <div className="alert-footer">
                    <span>
                      <CheckCircle2 size={15} />
                      Alert metrics from the backend
                    </span>
                  </div>
                </article>
              </section>

              <section className="bottom-grid">
                <button
                  type="button"
                  className="panel insight-card"
                  onClick={() => handleNavigation("Vendor Risk")}
                >
                  <div className="insight-icon blue">
                    <Users size={20} />
                  </div>

                  <div className="insight-copy">
                    <h3>Vendor risk management</h3>
                    <p>
                      Review vendor classifications, understand risk factors,
                      and investigate vendors requiring attention.
                    </p>
                    <span className="coming-label">
                      Open vendor risk register
                    </span>
                  </div>

                  <ArrowUpRight size={18} className="insight-arrow" />
                </button>

                <button
                  type="button"
                  className="panel insight-card"
                  onClick={() => handleNavigation("Procurement Analytics")}
                >
                  <div className="insight-icon green">
                    <Activity size={20} />
                  </div>

                  <div className="insight-copy">
                    <h3>Procurement analytics</h3>
                    <p>
                      Explore vendor performance rankings and identify
                      outstanding financial exposure.
                    </p>
                    <span className="coming-label">
                      Explore procurement analytics
                    </span>
                  </div>

                  <ArrowDownRight size={18} className="insight-arrow" />
                </button>
              </section>

              <footer className="page-footer">
                <span>Procurement Intelligence &amp; Vendor Risk Platform</span>
                <span>Development build · Data supplied by the .NET API</span>
              </footer>

              {loading && (
                <div className="loading-indicator" role="status">
                  <LoaderCircle size={15} className="spin" />
                  Connecting to dashboard API…
                </div>
              )}
            </>
          )}
        </main>
      </div>
    </div>
  );
}
