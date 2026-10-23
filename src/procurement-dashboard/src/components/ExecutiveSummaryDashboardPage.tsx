import { BarChart3, ExternalLink } from "lucide-react";
import "./ExecutiveSummaryDashboardPage.css";

const reportUrl = import.meta.env.VITE_POWER_BI_REPORT_URL;

export default function ExecutiveSummaryDashboardPage() {
  return (
    <section className="executive-dashboard-page">
      <h1>Executive Summary Dashboard</h1>

      {reportUrl ? (
        <div className="executive-report-container">
          <iframe
            title="Procurement Executive Summary Power BI Report"
            src={reportUrl}
            className="executive-report-frame"
            allowFullScreen
          />
        </div>
      ) : (
        <div className="executive-report-placeholder">
          <BarChart3 size={30} />
          <h2>Connect your Power BI report</h2>
          <p>
            Add your Power BI embed URL to the VITE_POWER_BI_REPORT_URL setting
            in .env.local.
          </p>
        </div>
      )}

      {reportUrl && (
        <a href={reportUrl} target="_blank" rel="noopener noreferrer">
          <ExternalLink size={16} /> Open in Power BI
        </a>
      )}
    </section>
  );
}
