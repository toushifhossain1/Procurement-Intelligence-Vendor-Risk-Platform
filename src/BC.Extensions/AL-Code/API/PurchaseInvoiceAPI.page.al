page 50103 "PI Purchase Invoice API"
{
    PageType = API;

    APIPublisher = 'BTCL_DEV';
    APIGroup = 'procurement';
    APIVersion = 'v1.0';

    EntityName = 'purchaseInvoice';
    EntitySetName = 'purchaseInvoices';

    SourceTable = "Purch. Inv. Header";

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

                field(no; Rec."No.")
                {
                    Caption = 'Invoice No.';
                }

                field(buyFromVendorNo; Rec."Buy-from Vendor No.")
                {
                    Caption = 'Vendor No.';
                }

                field(buyFromVendorName; Rec."Buy-from Vendor Name")
                {
                    Caption = 'Vendor Name';
                }

                field(orderNo; Rec."Order No.")
                {
                    Caption = 'Purchase Order No.';
                }

                field(VendorInvoiceNo; Rec."Vendor Invoice No.")
                {
                    Caption = 'Vendor Invoice No.';
                }

                field(postingDate; Rec."Posting Date")
                {
                    Caption = 'Posting Date';
                }

                field(documentDate; Rec."Document Date")
                {
                    Caption = 'Document Date';
                }

                field(dueDate; Rec."Due Date")
                {
                    Caption = 'Due Date';
                }

                field(currencyCode; Rec."Currency Code")
                {
                    Caption = 'Currency Code';
                }

                field(paymentTermsCode; Rec."Payment Terms Code")
                {
                    Caption = 'Payment Terms Code';
                }

                field(locationCode; Rec."Location Code")
                {
                    Caption = 'Location Code';
                }

                field(amount; Rec."Amount")
                {
                    Caption = 'Amount';
                }

                field(amountIncludingVAT; Rec."Amount Including VAT")
                {
                    Caption = 'Amount Including VAT';
                }

                field(lastModifiedDateTime; Rec.SystemModifiedAt)
                {
                    Caption = 'Last Modified Date Time';
                    Editable = false;
                }
            }
        }
    }
}