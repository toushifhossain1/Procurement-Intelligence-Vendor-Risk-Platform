page 50101 "PI Purchase Order API"
{
    PageType = API;

    APIPublisher = 'BTCL_DEV';
    APIGroup = 'procurement';
    APIVersion = 'v1.0';

    EntityName = 'purchaseOrder';
    EntitySetName = 'purchaseOrders';

    SourceTable = "Purchase Header";

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

                field(documentType; Rec."Document Type")
                {
                    Caption = 'Document Type';
                }

                field(no; Rec."No.")
                {
                    Caption = 'Purchase Order No.';
                }

                field(buyFromVendorNo; Rec."Buy-from Vendor No.")
                {
                    Caption = 'Vendor No.';
                }

                field(buyFromVendorName; Rec."Buy-from Vendor Name")
                {
                    Caption = 'Vendor Name';
                }

                field(orderDate; Rec."Order Date")
                {
                    Caption = 'Order Date';
                }

                field(postingDate; Rec."Posting Date")
                {
                    Caption = 'Posting Date';
                }

                field(expectedReceiptDate; Rec."Expected Receipt Date")
                {
                    Caption = 'Expected Receipt Date';
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

                field(status; Rec.Status)
                {
                    Caption = 'Status';
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