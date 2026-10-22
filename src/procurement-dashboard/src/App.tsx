import { useCallback, useEffect, useState } from "react";
import {
  Activity,
  AlertCircle,
  AlertTriangle,
  ArrowDownRight,
  ArrowUpRight,
  Bell,
  Boxes,
  Building2,
  CheckCircle2,
  ChevronDown,
  Clock3,
  LayoutDashboard,
  LoaderCircle,
  RefreshCw,
  ShieldAlert,
  ShieldCheck,
  Users,
} from "lucide-react";

import { getDashboardSummary } from "./services/dashboardService";
import type { DashboardSummary } from "./types/dashboard";
import "./App.css";

type NavigationItem = {
  label: string;
  icon: typeof LayoutDashboard;
  active?: boolean;
};

const navigation: NavigationItem[] = [
  { label: "Overview", icon: LayoutDashboard, active: true },
  { label: "Vendor Risk", icon: ShieldAlert },
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
  return new Intl.NumberFormat("en-US").format(value);
}

function formatUpdatedAt(value?: string): string {
  if (!value) return "Latest API response";

  const date = new Date(value);

  if (Number.isNaN(date.getTime())) return "Latest API response";

  return date.toLocaleString(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
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
  const percentage = total > 0 ? Math.min((count / total) * 100, 100) : 0;

  return (
    <div className="risk-row">
      <div className="risk-row-label">
        <span className={`risk-dot ${color}`} />
        <span>{label}</span>
      </div>

      <div className="risk-bar-track">
        <div
          className={`risk-bar-fill ${color}`}
          style={{ width: `${percentage}%` }}
        />
      </div>

      <span className="risk-count">{formatNumber(count)}</span>
    </div>
  );
}

function App() {
  const [dashboard, setDashboard] =
    useState<DashboardSummary>(initialDashboard);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [lastUpdated, setLastUpdated] = useState<string | undefined>();

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
      setLastUpdated(result.generatedAt);
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "Unable to load dashboard data.",
      );
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    void loadDashboard();
  }, [loadDashboard]);

  const highRiskTotal =
    dashboard.highRiskVendors + dashboard.criticalRiskVendors;

  const riskTotal =
    dashboard.lowRiskVendors +
    dashboard.mediumRiskVendors +
    dashboard.highRiskVendors +
    dashboard.criticalRiskVendors +
    dashboard.insufficientDataVendors;

  const riskCoverage =
    dashboard.totalVendors > 0
      ? Math.round(
          ((dashboard.totalVendors - dashboard.insufficientDataVendors) /
            dashboard.totalVendors) *
            100,
        )
      : 0;

  const kpis = [
    {
      label: "Total Vendors",
      value: dashboard.totalVendors,
      detail: "Vendors in the dataset",
      icon: Building2,
      tone: "blue",
    },
    {
      label: "High / Critical Risk",
      value: highRiskTotal,
      detail: "Require closer review",
      icon: ShieldAlert,
      tone: "red",
    },
    {
      label: "Active Risk Alerts",
      value: dashboard.activeAlerts,
      detail: `${formatNumber(dashboard.newAlerts)} new alerts`,
      icon: Bell,
      tone: "amber",
    },
    {
      label: "Risk Coverage",
      value: `${riskCoverage}%`,
      detail: `${formatNumber(dashboard.insufficientDataVendors)} vendors need data`,
      icon: ShieldCheck,
      tone: "green",
    },
  ];

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-mark">
            <Boxes size={23} strokeWidth={2.2} />
          </div>
          <div className="brand-copy">
            <span className="brand-name">PROCUREWISE</span>
            <span className="brand-subtitle">INTELLIGENCE PLATFORM</span>
          </div>
        </div>

        <div className="workspace-label">WORKSPACE</div>

        <nav className="sidebar-nav" aria-label="Main navigation">
          {navigation.map((item) => {
            const Icon = item.icon;

            return (
              <button
                type="button"
                className={`nav-item ${item.active ? "active" : ""}`}
                key={item.label}
                onClick={() => {
                  if (!item.active) {
                    window.alert(
                      `${item.label} will be implemented in a later feature.`,
                    );
                  }
                }}
                aria-current={item.active ? "page" : undefined}
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

        <div className="sidebar-divider" />

        <div className="workspace-label">SYSTEM</div>

        <div className="system-status">
          <span className="status-indicator" />
          <div>
            <strong>API connection</strong>
            <span>{error ? "Connection issue" : "Local environment"}</span>
          </div>
        </div>

        <div className="sidebar-footer">
          <div className="profile-avatar">PI</div>
          <div className="profile-copy">
            <strong>Procurement Team</strong>
            <span>Platform workspace</span>
          </div>
          <ChevronDown size={16} className="profile-chevron" />
        </div>
      </aside>

      <main className="main-content">
        <header className="topbar">
          <div className="breadcrumbs">
            <span>Workspace</span>
            <span className="breadcrumb-separator">/</span>
            <strong>Overview</strong>
          </div>

          <div className="topbar-actions">
            <span className="environment-badge">
              <span className="status-indicator" />
              Development
            </span>

            <button
              className="icon-button"
              type="button"
              title="Refresh dashboard"
              aria-label="Refresh dashboard"
              onClick={() => void loadDashboard(true)}
              disabled={loading || refreshing}
            >
              <RefreshCw size={17} className={refreshing ? "spin" : ""} />
            </button>

            <div className="topbar-user">
              <div className="profile-avatar small">PI</div>
            </div>
          </div>
        </header>

        <div className="page-content">
          <section className="page-heading">
            <div>
              <div className="eyebrow">PROCUREMENT INTELLIGENCE</div>
              <h1>Overview dashboard</h1>
              <p>
                Monitor vendor risk, procurement performance, and outstanding
                alerts from one place.
              </p>
            </div>

            <div className="updated-label">
              <Clock3 size={15} />
              <span>Updated: {formatUpdatedAt(lastUpdated)}</span>
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

          <section className="kpi-grid" aria-label="Key performance indicators">
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
                  <strong>{formatNumber(dashboard.acknowledgedAlerts)}</strong>
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
                  <strong>{formatNumber(dashboard.highSeverityAlerts)}</strong>
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
            <article className="panel insight-card">
              <div className="insight-icon blue">
                <Users size={20} />
              </div>
              <div className="insight-copy">
                <h3>Vendor risk management</h3>
                <p>
                  Review vendor classifications, understand risk factors, and
                  investigate vendors requiring attention.
                </p>
                <span className="coming-label">Next implementation phase</span>
              </div>
              <ArrowUpRight size={18} className="insight-arrow" />
            </article>

            <article className="panel insight-card">
              <div className="insight-icon green">
                <Activity size={20} />
              </div>
              <div className="insight-copy">
                <h3>Procurement analytics</h3>
                <p>
                  Explore vendor performance rankings and identify outstanding
                  financial exposure.
                </p>
                <span className="coming-label">Next implementation phase</span>
              </div>
              <ArrowDownRight size={18} className="insight-arrow" />
            </article>
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
        </div>
      </main>
    </div>
  );
}

export default App;
