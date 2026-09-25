page 50106 "PI Purchase Invoice Line API"
{
    PageType = API;

    APIPublisher = 'BTCL_DEV';
    APIGroup = 'procurement';
    APIVersion = 'v1.0';

    EntityName = 'purchaseInvoiceLine';
    EntitySetName = 'purchaseInvoiceLines';

    SourceTable = "Purch. Inv. Line";

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

                field(documentNo; Rec."Document No.")
                {
                    Caption = 'Invoice No.';
                }

                field(lineNo; Rec."Line No.")
                {
                    Caption = 'Line No.';
                }

                field(buyFromVendorNo; Rec."Buy-from Vendor No.")
                {
                    Caption = 'Vendor No.';
                }

                field(type; Rec.Type)
                {
                    Caption = 'Type';
                }

                field(no; Rec."No.")
                {
                    Caption = 'Item / Account No.';
                }

                field(description; Rec.Description)
                {
                    Caption = 'Description';
                }

                field(locationCode; Rec."Location Code")
                {
                    Caption = 'Location Code';
                }

                field(unitOfMeasure; Rec."Unit of Measure")
                {
                    Caption = 'Unit of Measure';
                }

                field(quantity; Rec.Quantity)
                {
                    Caption = 'Quantity';
                }

                field(unitCost; Rec."Direct Unit Cost")
                {
                    Caption = 'Direct Unit Cost';
                }

                field(lineAmount; Rec."Line Amount")
                {
                    Caption = 'Line Amount';
                }

                field(amount; Rec.Amount)
                {
                    Caption = 'Amount';
                }

                field(expectedReceiptDate; Rec."Expected Receipt Date")
                {
                    Caption = 'Expected Receipt Date';
                }

                field(postingDate; Rec."Posting Date")
                {
                    Caption = 'Posting Date';
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