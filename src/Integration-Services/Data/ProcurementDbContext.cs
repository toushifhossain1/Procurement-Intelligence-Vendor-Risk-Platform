using Microsoft.EntityFrameworkCore;
using IntegrationServices.Entities;

namespace IntegrationServices.Data;

public class ProcurementDbContext : DbContext
{
    public ProcurementDbContext(DbContextOptions<ProcurementDbContext> options)
        : base(options)
    {
    }

    public DbSet<BCVendor> BCVendors => Set<BCVendor>();

    public DbSet<BCPurchaseOrder> BCPurchaseOrders => Set<BCPurchaseOrder>();

    public DbSet<BCPurchaseOrderLine> BCPurchaseOrderLines => Set<BCPurchaseOrderLine>();

    public DbSet<BCPurchaseReceipt> BCPurchaseReceipts => Set<BCPurchaseReceipt>();

    public DbSet<BCPurchaseReceiptLine> BCPurchaseReceiptLines => Set<BCPurchaseReceiptLine>();

    public DbSet<BCPurchaseInvoice> BCPurchaseInvoices => Set<BCPurchaseInvoice>();

    public DbSet<BCPurchaseInvoiceLine> BCPurchaseInvoiceLines => Set<BCPurchaseInvoiceLine>();

    public DbSet<BCVendorLedgerEntry> BCVendorLedgerEntries => Set<BCVendorLedgerEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // ========================================================
    // BC Vendor
    // ========================================================

    modelBuilder.Entity<BCVendor>(entity =>
    {
        entity.HasKey(x => x.BCVendorID);

        entity.Property(x => x.BalanceLCY)
            .HasPrecision(18, 2);

        entity.HasIndex(x => x.VendorNo)
            .IsUnique();

        entity.HasIndex(x => x.LastModifiedDateTime);
    });


    // ========================================================
    // BC Purchase Order
    // ========================================================

    modelBuilder.Entity<BCPurchaseOrder>(entity =>
    {
        entity.HasKey(x => x.BCPurchaseOrderID);

        entity.Property(x => x.Amount)
            .HasPrecision(18, 2);

        entity.Property(x => x.AmountIncludingVAT)
            .HasPrecision(18, 2);

        entity.HasIndex(x => x.PurchaseOrderNo)
            .IsUnique();

        entity.HasIndex(x => x.VendorNo);
        entity.HasIndex(x => x.OrderDate);
        entity.HasIndex(x => x.ExpectedReceiptDate);
        entity.HasIndex(x => x.LastModifiedDateTime);
    });


    // ========================================================
    // BC Purchase Order Line
    // ========================================================

    modelBuilder.Entity<BCPurchaseOrderLine>(entity =>
    {
        entity.HasKey(x => x.BCPurchaseOrderLineID);

        entity.Property(x => x.Quantity)
            .HasPrecision(18, 4);

        entity.Property(x => x.QuantityBase)
            .HasPrecision(18, 4);

        entity.Property(x => x.OutstandingQuantity)
            .HasPrecision(18, 4);

        entity.Property(x => x.QuantityReceived)
            .HasPrecision(18, 4);

        entity.Property(x => x.QuantityInvoiced)
            .HasPrecision(18, 4);

        entity.Property(x => x.UnitCost)
            .HasPrecision(18, 2);

        entity.Property(x => x.LineAmount)
            .HasPrecision(18, 2);

        entity.HasIndex(x => new
        {
            x.PurchaseOrderNo,
            x.LineNo
        }).IsUnique();

        entity.HasIndex(x => x.VendorNo);
        entity.HasIndex(x => x.ExpectedReceiptDate);
        entity.HasIndex(x => x.LastModifiedDateTime);
    });


    // ========================================================
    // BC Purchase Receipt
    // ========================================================

    modelBuilder.Entity<BCPurchaseReceipt>(entity =>
    {
        entity.HasKey(x => x.BCPurchaseReceiptID);

        entity.HasIndex(x => x.ReceiptNo)
            .IsUnique();

        entity.HasIndex(x => x.VendorNo);
        entity.HasIndex(x => x.PurchaseOrderNo);
        entity.HasIndex(x => x.PostingDate);
        entity.HasIndex(x => x.LastModifiedDateTime);
    });


    // ========================================================
    // BC Purchase Receipt Line
    // ========================================================

    modelBuilder.Entity<BCPurchaseReceiptLine>(entity =>
    {
        entity.HasKey(x => x.BCPurchaseReceiptLineID);

        entity.Property(x => x.Quantity)
            .HasPrecision(18, 4);

        entity.Property(x => x.QuantityBase)
            .HasPrecision(18, 4);

        entity.Property(x => x.DirectUnitCost)
            .HasPrecision(18, 2);

        entity.Property(x => x.UnitCost)
            .HasPrecision(18, 2);

        entity.Property(x => x.UnitCostLCY)
            .HasPrecision(18, 2);

        entity.Property(x => x.QuantityReceivedNotInvoiced)
            .HasPrecision(18, 4);

        entity.Property(x => x.QuantityInvoiced)
            .HasPrecision(18, 4);

        entity.HasIndex(x => new
        {
            x.ReceiptNo,
            x.LineNo
        }).IsUnique();

        entity.HasIndex(x => x.BuyFromVendorNo);
        entity.HasIndex(x => x.PurchaseOrderNo);
        entity.HasIndex(x => x.PostingDate);
        entity.HasIndex(x => x.LastModifiedDateTime);
    });


    // ========================================================
    // BC Purchase Invoice
    // ========================================================

    modelBuilder.Entity<BCPurchaseInvoice>(entity =>
    {
        entity.HasKey(x => x.BCPurchaseInvoiceID);

        entity.Property(x => x.Amount)
            .HasPrecision(18, 2);

        entity.Property(x => x.AmountIncludingVAT)
            .HasPrecision(18, 2);

        entity.HasIndex(x => x.InvoiceNo)
            .IsUnique();

        entity.HasIndex(x => x.VendorNo);
        entity.HasIndex(x => x.PurchaseOrderNo);
        entity.HasIndex(x => x.PostingDate);
        entity.HasIndex(x => x.DueDate);
        entity.HasIndex(x => x.LastModifiedDateTime);
    });


    // ========================================================
    // BC Purchase Invoice Line
    // ========================================================

    modelBuilder.Entity<BCPurchaseInvoiceLine>(entity =>
    {
        entity.HasKey(x => x.BCPurchaseInvoiceLineID);

        entity.Property(x => x.Quantity)
            .HasPrecision(18, 4);

        entity.Property(x => x.UnitCost)
            .HasPrecision(18, 2);

        entity.Property(x => x.LineAmount)
            .HasPrecision(18, 2);

        entity.Property(x => x.Amount)
            .HasPrecision(18, 2);

        entity.HasIndex(x => new
        {
            x.InvoiceNo,
            x.LineNo
        }).IsUnique();

        entity.HasIndex(x => x.VendorNo);
        entity.HasIndex(x => x.PostingDate);
        entity.HasIndex(x => x.LastModifiedDateTime);
    });


    // ========================================================
    // BC Vendor Ledger Entry
    // ========================================================

    modelBuilder.Entity<BCVendorLedgerEntry>(entity =>
    {
        entity.HasKey(x => x.BCVendorLedgerEntryID);

        entity.Property(x => x.Amount)
            .HasPrecision(18, 2);

        entity.Property(x => x.DebitAmount)
            .HasPrecision(18, 2);

        entity.Property(x => x.CreditAmount)
            .HasPrecision(18, 2);

        entity.Property(x => x.AmountLCY)
            .HasPrecision(18, 2);

        entity.Property(x => x.DebitAmountLCY)
            .HasPrecision(18, 2);

        entity.Property(x => x.CreditAmountLCY)
            .HasPrecision(18, 2);

        entity.Property(x => x.OriginalAmount)
            .HasPrecision(18, 2);

        entity.Property(x => x.OriginalAmountLCY)
            .HasPrecision(18, 2);

        entity.Property(x => x.RemainingAmount)
            .HasPrecision(18, 2);

        entity.Property(x => x.RemainingAmountLCY)
            .HasPrecision(18, 2);

        entity.HasIndex(x => x.EntryNo)
            .IsUnique();

        entity.HasIndex(x => x.VendorNo);
        entity.HasIndex(x => x.PostingDate);
        entity.HasIndex(x => x.DocumentNo);
        entity.HasIndex(x => x.DueDate);
        entity.HasIndex(x => x.Open);
        entity.HasIndex(x => x.LastModifiedDateTime);
    });
}

}