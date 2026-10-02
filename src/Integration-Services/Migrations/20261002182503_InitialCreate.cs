using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integration_Services.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BCPurchaseInvoiceLines",
                columns: table => new
                {
                    BCPurchaseInvoiceLineID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LineAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ExpectedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCPurchaseInvoiceLines", x => x.BCPurchaseInvoiceLineID);
                });

            migrationBuilder.CreateTable(
                name: "BCPurchaseInvoices",
                columns: table => new
                {
                    BCPurchaseInvoiceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VendorInvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTermsCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AmountIncludingVAT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCPurchaseInvoices", x => x.BCPurchaseInvoiceID);
                });

            migrationBuilder.CreateTable(
                name: "BCPurchaseOrderLines",
                columns: table => new
                {
                    BCPurchaseOrderLineID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitOfMeasureCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    QuantityBase = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    OutstandingQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    QuantityReceived = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    QuantityInvoiced = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LineAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ExpectedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PromisedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCPurchaseOrderLines", x => x.BCPurchaseOrderLineID);
                });

            migrationBuilder.CreateTable(
                name: "BCPurchaseOrders",
                columns: table => new
                {
                    BCPurchaseOrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTermsCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AmountIncludingVAT = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCPurchaseOrders", x => x.BCPurchaseOrderID);
                });

            migrationBuilder.CreateTable(
                name: "BCPurchaseReceiptLines",
                columns: table => new
                {
                    BCPurchaseReceiptLineID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    BuyFromVendorNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PayToVendorNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PurchaseOrderLineNo = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    QuantityBase = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    UnitOfMeasureCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DirectUnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    UnitCostLCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    QuantityReceivedNotInvoiced = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    QuantityInvoiced = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PromisedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedReceiptDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VendorOrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorShipmentNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCPurchaseReceiptLines", x => x.BCPurchaseReceiptLineID);
                });

            migrationBuilder.CreateTable(
                name: "BCPurchaseReceipts",
                columns: table => new
                {
                    BCPurchaseReceiptID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiptNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseOrderNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCPurchaseReceipts", x => x.BCPurchaseReceiptID);
                });

            migrationBuilder.CreateTable(
                name: "BCVendorLedgerEntries",
                columns: table => new
                {
                    BCVendorLedgerEntryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryNo = table.Column<int>(type: "int", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreditAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AmountLCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DebitAmountLCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreditAmountLCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentDiscountDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OriginalAmountLCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AppliesToDocType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppliesToDocNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Open = table.Column<bool>(type: "bit", nullable: true),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    RemainingAmountLCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    GlobalDimension1Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlobalDimension2Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethodCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCVendorLedgerEntries", x => x.BCVendorLedgerEntryID);
                });

            migrationBuilder.CreateTable(
                name: "BCVendors",
                columns: table => new
                {
                    BCVendorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VendorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryRegionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Blocked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BalanceLCY = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LastModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BCVendors", x => x.BCVendorID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoiceLines_InvoiceNo_LineNo",
                table: "BCPurchaseInvoiceLines",
                columns: new[] { "InvoiceNo", "LineNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoiceLines_LastModifiedDateTime",
                table: "BCPurchaseInvoiceLines",
                column: "LastModifiedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoiceLines_PostingDate",
                table: "BCPurchaseInvoiceLines",
                column: "PostingDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoiceLines_VendorNo",
                table: "BCPurchaseInvoiceLines",
                column: "VendorNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoices_DueDate",
                table: "BCPurchaseInvoices",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoices_InvoiceNo",
                table: "BCPurchaseInvoices",
                column: "InvoiceNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoices_LastModifiedDateTime",
                table: "BCPurchaseInvoices",
                column: "LastModifiedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoices_PostingDate",
                table: "BCPurchaseInvoices",
                column: "PostingDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoices_PurchaseOrderNo",
                table: "BCPurchaseInvoices",
                column: "PurchaseOrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseInvoices_VendorNo",
                table: "BCPurchaseInvoices",
                column: "VendorNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrderLines_ExpectedReceiptDate",
                table: "BCPurchaseOrderLines",
                column: "ExpectedReceiptDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrderLines_LastModifiedDateTime",
                table: "BCPurchaseOrderLines",
                column: "LastModifiedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrderLines_PurchaseOrderNo_LineNo",
                table: "BCPurchaseOrderLines",
                columns: new[] { "PurchaseOrderNo", "LineNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrderLines_VendorNo",
                table: "BCPurchaseOrderLines",
                column: "VendorNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrders_ExpectedReceiptDate",
                table: "BCPurchaseOrders",
                column: "ExpectedReceiptDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrders_LastModifiedDateTime",
                table: "BCPurchaseOrders",
                column: "LastModifiedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrders_OrderDate",
                table: "BCPurchaseOrders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrders_PurchaseOrderNo",
                table: "BCPurchaseOrders",
                column: "PurchaseOrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseOrders_VendorNo",
                table: "BCPurchaseOrders",
                column: "VendorNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceiptLines_BuyFromVendorNo",
                table: "BCPurchaseReceiptLines",
                column: "BuyFromVendorNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceiptLines_LastModifiedDateTime",
                table: "BCPurchaseReceiptLines",
                column: "LastModifiedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceiptLines_PostingDate",
                table: "BCPurchaseReceiptLines",
                column: "PostingDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceiptLines_PurchaseOrderNo",
                table: "BCPurchaseReceiptLines",
                column: "PurchaseOrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceiptLines_ReceiptNo_LineNo",
                table: "BCPurchaseReceiptLines",
                columns: new[] { "ReceiptNo", "LineNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceipts_LastModifiedDateTime",
                table: "BCPurchaseReceipts",
                column: "LastModifiedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceipts_PostingDate",
                table: "BCPurchaseReceipts",
                column: "PostingDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceipts_PurchaseOrderNo",
                table: "BCPurchaseReceipts",
                column: "PurchaseOrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceipts_ReceiptNo",
                table: "BCPurchaseReceipts",
                column: "ReceiptNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BCPurchaseReceipts_VendorNo",
                table: "BCPurchaseReceipts",
                column: "VendorNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCVendorLedgerEntries_DocumentNo",
                table: "BCVendorLedgerEntries",
                column: "DocumentNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCVendorLedgerEntries_DueDate",
                table: "BCVendorLedgerEntries",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCVendorLedgerEntries_EntryNo",
                table: "BCVendorLedgerEntries",
                column: "EntryNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BCVendorLedgerEntries_LastModifiedDateTime",
                table: "BCVendorLedgerEntries",
                column: "LastModifiedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BCVendorLedgerEntries_Open",
                table: "BCVendorLedgerEntries",
                column: "Open");

            migrationBuilder.CreateIndex(
                name: "IX_BCVendorLedgerEntries_PostingDate",
                table: "BCVendorLedgerEntries",
                column: "PostingDate");

            migrationBuilder.CreateIndex(
                name: "IX_BCVendorLedgerEntries_VendorNo",
                table: "BCVendorLedgerEntries",
                column: "VendorNo");

            migrationBuilder.CreateIndex(
                name: "IX_BCVendors_LastModifiedDateTime",
                table: "BCVendors",
                column: "LastModifiedDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_BCVendors_VendorNo",
                table: "BCVendors",
                column: "VendorNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BCPurchaseInvoiceLines");

            migrationBuilder.DropTable(
                name: "BCPurchaseInvoices");

            migrationBuilder.DropTable(
                name: "BCPurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "BCPurchaseOrders");

            migrationBuilder.DropTable(
                name: "BCPurchaseReceiptLines");

            migrationBuilder.DropTable(
                name: "BCPurchaseReceipts");

            migrationBuilder.DropTable(
                name: "BCVendorLedgerEntries");

            migrationBuilder.DropTable(
                name: "BCVendors");
        }
    }
}
