import { useCallback, useEffect, useMemo, useState } from "react";
import {
  AlertCircle,
  BarChart3,
  CircleDollarSign,
  Clock3,
  RefreshCw,
  ShieldAlert,
  TrendingUp,
  Wallet,
} from "lucide-react";

import {
  getFinancialExposure,
  getVendorPerformance,
} from "../services/procurementAnalyticsService";

import type {
  FinancialExposureSummary,
  VendorFinancialExposure,
  VendorPerformanceRanking,
} from "../types/procurementAnalytics";

type AnalyticsTab = "performance" | "financial";
type FinancialSort = "OverdueBalanceLCY" | "OutstandingBalanceLCY";

const numberFormat = new Intl.NumberFormat("en-BD", {
  maximumFractionDigits: 1,
});

const moneyFormat = new Intl.NumberFormat("en-BD", {
  maximumFractionDigits: 0,
});

function formatMoney(value: number): string {
  return `৳${moneyFormat.format(value)}`;
}

function formatPercent(value: number | null): string {
  return value === null ? "—" : `${numberFormat.format(value)}%`;
}

function formatDays(value: number | null): string {
  return value === null ? "—" : `${numberFormat.format(value)} days`;
}

function scoreLabel(value: number | null): string {
  return value === null ? "N/A" : `${numberFormat.format(value)}/100`;
}

function ratingClass(rating: string): string {
  const normalized = rating.toLowerCase();

  if (normalized === "excellent") return "pa-rating pa-rating--excellent";
  if (normalized === "good") return "pa-rating pa-rating--good";
  if (normalized === "fair") return "pa-rating pa-rating--fair";
  if (normalized === "poor") return "pa-rating pa-rating--poor";

  return "pa-rating pa-rating--neutral";
}

function riskClass(risk: string): string {
  const normalized = risk.toLowerCase();

  if (normalized === "critical") return "pa-risk pa-risk--critical";
  if (normalized === "high") return "pa-risk pa-risk--high";
  if (normalized === "medium") return "pa-risk pa-risk--medium";
  if (normalized === "low") return "pa-risk pa-risk--low";

  return "pa-risk pa-risk--neutral";
}

function SectionError({
  message,
  onRetry,
}: {
  message: string;
  onRetry: () => void;
}) {
  return (
    <div className="pa-error">
      <AlertCircle size={20} />
      <div>
        <strong>Unable to load this section</strong>
        <p>{message}</p>
        <button className="pa-button pa-button--secondary" onClick={onRetry}>
          <RefreshCw size={15} />
          Retry
        </button>
      </div>
    </div>
  );
}

export default function ProcurementAnalyticsPage() {
  const [activeTab, setActiveTab] = useState<AnalyticsTab>("performance");

  const [performance, setPerformance] = useState<VendorPerformanceRanking[]>(
    [],
  );
  const [financial, setFinancial] = useState<FinancialExposureSummary | null>(
    null,
  );

  const [performanceLoading, setPerformanceLoading] = useState(true);
  const [financialLoading, setFinancialLoading] = useState(true);
  const [performanceError, setPerformanceError] = useState("");
  const [financialError, setFinancialError] = useState("");
  const [financialSort, setFinancialSort] =
    useState<FinancialSort>("OverdueBalanceLCY");
  const [limit, setLimit] = useState(10);
  const [lastUpdated, setLastUpdated] = useState<Date | null>(null);

  const loadPerformance = useCallback(async () => {
    setPerformanceLoading(true);
    setPerformanceError("");

    try {
      const result = await getVendorPerformance(limit);
      setPerformance(result);
    } catch (error) {
      setPerformanceError(
        error instanceof Error ? error.message : "Unexpected API error.",
      );
    } finally {
      setPerformanceLoading(false);
    }
  }, [limit]);

  const loadFinancial = useCallback(async () => {
    setFinancialLoading(true);
    setFinancialError("");

    try {
      const result = await getFinancialExposure(financialSort, limit);
      setFinancial(result);
    } catch (error) {
      setFinancialError(
        error instanceof Error ? error.message : "Unexpected API error.",
      );
    } finally {
      setFinancialLoading(false);
    }
  }, [financialSort, limit]);

  const refreshAll = useCallback(async () => {
    await Promise.all([loadPerformance(), loadFinancial()]);
    setLastUpdated(new Date());
  }, [loadPerformance, loadFinancial]);

  useEffect(() => {
    void refreshAll();
  }, [refreshAll]);

  const performanceSummary = useMemo(() => {
    const rated = performance.filter(
      (vendor) => vendor.overallPerformanceScore !== null,
    );

    const averageScore =
      rated.length > 0
        ? rated.reduce(
            (total, vendor) => total + (vendor.overallPerformanceScore ?? 0),
            0,
          ) / rated.length
        : null;

    const excellentOrGood = rated.filter((vendor) => {
      const rating = vendor.performanceRating.toLowerCase();
      return rating === "excellent" || rating === "good";
    }).length;

    const poorCount = rated.filter(
      (vendor) => vendor.performanceRating.toLowerCase() === "poor",
    ).length;

    return {
      averageScore,
      ratedCount: rated.length,
      excellentOrGood,
      poorCount,
    };
  }, [performance]);

  const highestExposure = useMemo(
    () =>
      Math.max(
        1,
        ...(financial?.vendors.map((vendor) =>
          financialSort === "OverdueBalanceLCY"
            ? vendor.overdueBalanceLCY
            : vendor.outstandingBalanceLCY,
        ) ?? []),
      ),
    [financial, financialSort],
  );

  function displayedBalance(vendor: VendorFinancialExposure): number {
    return financialSort === "OverdueBalanceLCY"
      ? vendor.overdueBalanceLCY
      : vendor.outstandingBalanceLCY;
  }

  return (
    <div className="pa-page">
      <div className="pa-page-heading">
        <div>
          <div className="pa-eyebrow">PROCUREMENT INTELLIGENCE</div>
          <h1>Procurement Analytics</h1>
          <p>
            Understand supplier performance, financial exposure, and where
            procurement attention may be needed.
          </p>
        </div>

        <button
          className="pa-button pa-button--primary"
          onClick={() => void refreshAll()}
          disabled={performanceLoading || financialLoading}
        >
          <RefreshCw
            size={16}
            className={
              performanceLoading || financialLoading ? "pa-spinning" : ""
            }
          />
          Refresh data
        </button>
      </div>

      <div className="pa-toolbar">
        <div className="pa-tabs" role="tablist" aria-label="Analytics sections">
          <button
            className={activeTab === "performance" ? "active" : ""}
            role="tab"
            aria-selected={activeTab === "performance"}
            onClick={() => setActiveTab("performance")}
          >
            <TrendingUp size={17} />
            Vendor Performance
          </button>
          <button
            className={activeTab === "financial" ? "active" : ""}
            role="tab"
            aria-selected={activeTab === "financial"}
            onClick={() => setActiveTab("financial")}
          >
            <Wallet size={17} />
            Financial Exposure
          </button>
        </div>

        <label className="pa-limit">
          Show
          <select
            value={limit}
            onChange={(event) => setLimit(Number(event.target.value))}
          >
            <option value={5}>Top 5</option>
            <option value={10}>Top 10</option>
            <option value={25}>Top 25</option>
            <option value={50}>Top 50</option>
            <option value={100}>Top 100</option>
          </select>
          vendors
        </label>
      </div>

      {activeTab === "performance" && (
        <section className="pa-section">
          {performanceError ? (
            <SectionError
              message={performanceError}
              onRetry={() => void loadPerformance()}
            />
          ) : (
            <>
              <div className="pa-kpi-grid">
                <article className="pa-kpi">
                  <div className="pa-kpi__icon pa-kpi__icon--blue">
                    <BarChart3 size={20} />
                  </div>
                  <span>Vendors ranked</span>
                  <strong>
                    {performanceLoading ? "…" : performance.length}
                  </strong>
                  <small>In the current ranking</small>
                </article>

                <article className="pa-kpi">
                  <div className="pa-kpi__icon pa-kpi__icon--green">
                    <TrendingUp size={20} />
                  </div>
                  <span>Average performance</span>
                  <strong>
                    {performanceLoading
                      ? "…"
                      : scoreLabel(performanceSummary.averageScore)}
                  </strong>
                  <small>Among vendors with a score</small>
                </article>

                <article className="pa-kpi">
                  <div className="pa-kpi__icon pa-kpi__icon--purple">
                    <ShieldAlert size={20} />
                  </div>
                  <span>Good or excellent</span>
                  <strong>
                    {performanceLoading
                      ? "…"
                      : performanceSummary.excellentOrGood}
                  </strong>
                  <small>Ranked vendors</small>
                </article>

                <article className="pa-kpi">
                  <div className="pa-kpi__icon pa-kpi__icon--orange">
                    <Clock3 size={20} />
                  </div>
                  <span>Poor performance</span>
                  <strong>
                    {performanceLoading ? "…" : performanceSummary.poorCount}
                  </strong>
                  <small>Vendors rated Poor</small>
                </article>
              </div>

              <div className="pa-panel">
                <div className="pa-panel-heading">
                  <div>
                    <h2>Performance ranking</h2>
                    <p>
                      Compare delivery, fulfillment, lead time, and invoice
                      discrepancy metrics.
                    </p>
                  </div>
                  <span className="pa-record-count">
                    {performance.length} vendors
                  </span>
                </div>

                {performanceLoading ? (
                  <div className="pa-state">
                    <RefreshCw className="pa-spinning" size={22} />
                    Loading performance data…
                  </div>
                ) : performance.length === 0 ? (
                  <div className="pa-state">
                    No vendor performance records were returned by the API.
                  </div>
                ) : (
                  <>
                    <div className="pa-performance-highlights">
                      {performance.slice(0, 5).map((vendor) => (
                        <div
                          className="pa-vendor-highlight"
                          key={vendor.vendorNo}
                        >
                          <div className="pa-vendor-highlight__top">
                            <span className="pa-rank">#{vendor.rank}</span>
                            <span
                              className={ratingClass(vendor.performanceRating)}
                            >
                              {vendor.performanceRating}
                            </span>
                          </div>
                          <strong title={vendor.vendorName}>
                            {vendor.vendorName}
                          </strong>
                          <small>{vendor.vendorNo}</small>
                          <div className="pa-score">
                            <span
                              className="pa-score__fill"
                              style={{
                                width: `${Math.max(
                                  0,
                                  Math.min(
                                    vendor.overallPerformanceScore ?? 0,
                                    100,
                                  ),
                                )}%`,
                              }}
                            />
                          </div>
                          <div className="pa-vendor-highlight__score">
                            <strong>
                              {scoreLabel(vendor.overallPerformanceScore)}
                            </strong>
                            <span>Overall score</span>
                          </div>
                        </div>
                      ))}
                    </div>

                    <div className="pa-table-wrap">
                      <table className="pa-table">
                        <thead>
                          <tr>
                            <th>Rank</th>
                            <th>Vendor</th>
                            <th>Score</th>
                            <th>Rating</th>
                            <th>On-time delivery</th>
                            <th>Order fulfillment</th>
                            <th>Avg. lead time</th>
                            <th>Invoice discrepancy</th>
                          </tr>
                        </thead>
                        <tbody>
                          {performance.map((vendor) => (
                            <tr key={vendor.vendorNo}>
                              <td>
                                <span className="pa-rank">#{vendor.rank}</span>
                              </td>
                              <td>
                                <strong>{vendor.vendorName}</strong>
                                <small>{vendor.vendorNo}</small>
                              </td>
                              <td>
                                <strong>
                                  {scoreLabel(vendor.overallPerformanceScore)}
                                </strong>
                              </td>
                              <td>
                                <span
                                  className={ratingClass(
                                    vendor.performanceRating,
                                  )}
                                >
                                  {vendor.performanceRating}
                                </span>
                              </td>
                              <td>
                                {formatPercent(vendor.onTimeDeliveryRate)}
                              </td>
                              <td>
                                {formatPercent(vendor.orderFulfillmentRate)}
                              </td>
                              <td>{formatDays(vendor.averageLeadTimeDays)}</td>
                              <td>
                                {formatPercent(vendor.invoiceDiscrepancyRate)}
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>

                    <div className="pa-table-footnote">
                      A lower invoice discrepancy rate is generally preferable.
                      Lead time should be interpreted alongside delivery and
                      fulfillment performance.
                    </div>
                  </>
                )}
              </div>
            </>
          )}
        </section>
      )}

      {activeTab === "financial" && (
        <section className="pa-section">
          {financialError ? (
            <SectionError
              message={financialError}
              onRetry={() => void loadFinancial()}
            />
          ) : (
            <>
              <div className="pa-kpi-grid">
                <article className="pa-kpi">
                  <div className="pa-kpi__icon pa-kpi__icon--blue">
                    <CircleDollarSign size={20} />
                  </div>
                  <span>Total outstanding</span>
                  <strong className="pa-money-value">
                    {financialLoading
                      ? "…"
                      : formatMoney(financial?.totalOutstandingBalanceLCY ?? 0)}
                  </strong>
                  <small>Open vendor balances</small>
                </article>

                <article className="pa-kpi">
                  <div className="pa-kpi__icon pa-kpi__icon--orange">
                    <AlertCircle size={20} />
                  </div>
                  <span>Total overdue</span>
                  <strong className="pa-money-value">
                    {financialLoading
                      ? "…"
                      : formatMoney(financial?.totalOverdueBalanceLCY ?? 0)}
                  </strong>
                  <small>Overdue vendor balances</small>
                </article>

                <article className="pa-kpi">
                  <div className="pa-kpi__icon pa-kpi__icon--purple">
                    <BarChart3 size={20} />
                  </div>
                  <span>Overdue percentage</span>
                  <strong>
                    {financialLoading
                      ? "…"
                      : formatPercent(financial?.overdueBalancePercentage ?? 0)}
                  </strong>
                  <small>Of total outstanding exposure</small>
                </article>

                <article className="pa-kpi">
                  <div className="pa-kpi__icon pa-kpi__icon--red">
                    <ShieldAlert size={20} />
                  </div>
                  <span>Vendors with overdue balances</span>
                  <strong>
                    {financialLoading
                      ? "…"
                      : (financial?.vendorsWithOverdueBalance ?? 0)}
                  </strong>
                  <small>Require financial review</small>
                </article>
              </div>

              <div className="pa-panel">
                <div className="pa-panel-heading pa-panel-heading--wrap">
                  <div>
                    <h2>Vendor financial exposure</h2>
                    <p>
                      Prioritize vendors with the largest balances for review.
                    </p>
                  </div>

                  <label className="pa-limit">
                    Rank by
                    <select
                      value={financialSort}
                      onChange={(event) =>
                        setFinancialSort(event.target.value as FinancialSort)
                      }
                    >
                      <option value="OverdueBalanceLCY">Overdue balance</option>
                      <option value="OutstandingBalanceLCY">
                        Outstanding balance
                      </option>
                    </select>
                  </label>
                </div>

                {financialLoading ? (
                  <div className="pa-state">
                    <RefreshCw className="pa-spinning" size={22} />
                    Loading financial exposure…
                  </div>
                ) : !financial || financial.vendors.length === 0 ? (
                  <div className="pa-state">
                    No financial exposure records were returned by the API.
                  </div>
                ) : (
                  <>
                    <div className="pa-exposure-chart">
                      {financial.vendors.slice(0, 8).map((vendor, index) => {
                        const amount = displayedBalance(vendor);
                        const width = (amount / highestExposure) * 100;

                        return (
                          <div
                            className="pa-exposure-row"
                            key={vendor.vendorNo}
                          >
                            <div className="pa-exposure-row__label">
                              <span className="pa-exposure-row__rank">
                                {index + 1}
                              </span>
                              <div>
                                <strong title={vendor.vendorName}>
                                  {vendor.vendorName}
                                </strong>
                                <small>{vendor.vendorNo}</small>
                              </div>
                            </div>
                            <div className="pa-exposure-row__bar">
                              <div
                                className="pa-exposure-row__fill"
                                style={{ width: `${width}%` }}
                              />
                            </div>
                            <strong className="pa-exposure-row__amount">
                              {formatMoney(amount)}
                            </strong>
                          </div>
                        );
                      })}
                    </div>

                    <div className="pa-table-wrap">
                      <table className="pa-table">
                        <thead>
                          <tr>
                            <th>Vendor</th>
                            <th>Outstanding balance</th>
                            <th>Overdue balance</th>
                            <th>Overdue %</th>
                            <th>Open entries</th>
                            <th>Overdue entries</th>
                            <th>Risk classification</th>
                          </tr>
                        </thead>
                        <tbody>
                          {financial.vendors.map((vendor) => (
                            <tr key={vendor.vendorNo}>
                              <td>
                                <strong>{vendor.vendorName}</strong>
                                <small>{vendor.vendorNo}</small>
                              </td>
                              <td>
                                <strong>
                                  {formatMoney(vendor.outstandingBalanceLCY)}
                                </strong>
                              </td>
                              <td>
                                <strong
                                  className={
                                    vendor.overdueBalanceLCY > 0
                                      ? "pa-overdue-amount"
                                      : ""
                                  }
                                >
                                  {formatMoney(vendor.overdueBalanceLCY)}
                                </strong>
                              </td>
                              <td>
                                {formatPercent(vendor.overdueBalancePercentage)}
                              </td>
                              <td>{vendor.openLedgerEntryCount}</td>
                              <td>{vendor.overdueLedgerEntryCount}</td>
                              <td>
                                <span
                                  className={riskClass(
                                    vendor.riskClassification,
                                  )}
                                >
                                  {vendor.riskClassification}
                                </span>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </>
                )}
              </div>
            </>
          )}
        </section>
      )}

      <footer className="pa-footer">
        <span>
          <Clock3 size={14} />
          {lastUpdated
            ? `Last refreshed ${lastUpdated.toLocaleTimeString()}`
            : "Waiting for data"}
        </span>
        <span>Source: Procurement analytics API</span>
      </footer>
    </div>
  );
}
