using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integration_Services.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorRiskAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorRiskAlerts",
                columns: table => new
                {
                    VendorRiskAlertID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorRiskAlerts", x => x.VendorRiskAlertID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorRiskAlerts_CreatedAt",
                table: "VendorRiskAlerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRiskAlerts_Severity",
                table: "VendorRiskAlerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRiskAlerts_Status",
                table: "VendorRiskAlerts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorRiskAlerts_VendorNo",
                table: "VendorRiskAlerts",
                column: "VendorNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorRiskAlerts");
        }
    }
}
