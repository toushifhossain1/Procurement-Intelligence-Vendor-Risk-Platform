page 50107 "PI Vendor Ledger Entry API"
{
    PageType = API;

    APIPublisher = 'BTCL_DEV';
    APIGroup = 'procurement';
    APIVersion = 'v1.0';

    EntityName = 'vendorLedgerEntry';
    EntitySetName = 'vendorLedgerEntries';

    SourceTable = "Vendor Ledger Entry";

    DelayedInsert = true;
    ODataKeyFields = SystemId;

    layout
    {
        area(Content)
        {
            repeater(Group)
            {
                field(id; Rec.SystemId)
                {
                    Caption = 'Id';
                    Editable = false;
                }

                field(entryNo; Rec."Entry No.")
                {
                    Caption = 'Entry No.';
                    Editable = false;
                }

                field(vendorNo; Rec."Vendor No.")
                {
                    Caption = 'Vendor No.';
                }

                field(postingDate; Rec."Posting Date")
                {
                    Caption = 'Posting Date';
                }

                field(documentType; Rec."Document Type")
                {
                    Caption = 'Document Type';
                }

                field(documentNo; Rec."Document No.")
                {
                    Caption = 'Document No.';
                }

                field(description; Rec.Description)
                {
                    Caption = 'Description';
                }

                field(currencyCode; Rec."Currency Code")
                {
                    Caption = 'Currency Code';
                }

                field(amount; Rec.Amount)
                {
                    Caption = 'Amount';
                }

                field(debitAmount; Rec."Debit Amount")
                {
                    Caption = 'Debit Amount';
                }

                field(creditAmount; Rec."Credit Amount")
                {
                    Caption = 'Credit Amount';
                }

                field(amountLCY; Rec."Amount (LCY)")
                {
                    Caption = 'Amount (LCY)';
                }

                field(debitAmountLCY; Rec."Debit Amount (LCY)")
                {
                    Caption = 'Debit Amount (LCY)';
                }

                field(creditAmountLCY; Rec."Credit Amount (LCY)")
                {
                    Caption = 'Credit Amount (LCY)';
                }

                field(dueDate; Rec."Due Date")
                {
                    Caption = 'Due Date';
                }

                field(pmtDiscountDate; Rec."Pmt. Discount Date")
                {
                    Caption = 'Payment Discount Date';
                }

                field(originalAmount; Rec."Original Amount")
                {
                    Caption = 'Original Amount';
                }

                field(originalAmtLCY; Rec."Original Amt. (LCY)")
                {
                    Caption = 'Original Amount (LCY)';
                }

                field(appliesToDocType; Rec."Applies-to Doc. Type")
                {
                    Caption = 'Applies-to Document Type';
                }

                field(appliesToDocNo; Rec."Applies-to Doc. No.")
                {
                    Caption = 'Applies-to Document No.';
                }

                field(open; Rec.Open)
                {
                    Caption = 'Open';
                }

                field(remainingAmount; Rec."Remaining Amount")
                {
                    Caption = 'Remaining Amount';
                }

                field(remainingAmtLCY; Rec."Remaining Amt. (LCY)")
                {
                    Caption = 'Remaining Amount (LCY)';
                }

                field(globalDimension1Code; Rec."Global Dimension 1 Code")
                {
                    Caption = 'Global Dimension 1 Code';
                }

                field(globalDimension2Code; Rec."Global Dimension 2 Code")
                {
                    Caption = 'Global Dimension 2 Code';
                }

                field(paymentMethodCode; Rec."Payment Method Code")
                {
                    Caption = 'Payment Method Code';
                }

                field(vendorName; Rec."Vendor Name")
                {
                    Caption = 'Vendor Name';
                }

                field(lastModifiedDateTime; Rec.SystemModifiedAt)
                {
                    Caption = 'Last Modified DateTime';
                    Editable = false;
                }
            }
        }
    }
}