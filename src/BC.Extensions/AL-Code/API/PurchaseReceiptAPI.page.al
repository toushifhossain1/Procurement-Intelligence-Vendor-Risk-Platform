page 50102 "PI Purchase Receipt API"
{
    PageType = API;

    APIPublisher = 'BTCL_DEV';
    APIGroup = 'procurement';
    APIVersion = 'v1.0';

    EntityName = 'purchaseReceipt';
    EntitySetName = 'purchaseReceipts';

    SourceTable = "Purch. Rcpt. Header";

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
                    Caption = 'Receipt No.';
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

                field(postingDate; Rec."Posting Date")
                {
                    Caption = 'Posting Date';
                }

                field(documentDate; Rec."Document Date")
                {
                    Caption = 'Document Date';
                }

                field(locationCode; Rec."Location Code")
                {
                    Caption = 'Location Code';
                }

                field(currencyCode; Rec."Currency Code")
                {
                    Caption = 'Currency Code';
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