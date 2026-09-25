page 50104 "PI Purchase Order Line API"
{
    PageType = API;

    APIPublisher = 'BTCL_DEV';
    APIGroup = 'procurement';
    APIVersion = 'v1.0';

    EntityName = 'purchaseOrderLine';
    EntitySetName = 'purchaseOrderLines';

    SourceTable = "Purchase Line";

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

                field(documentNo; Rec."Document No.")
                {
                    Caption = 'Purchase Order No.';
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

                field(unitOfMeasureCode; Rec."Unit of Measure Code")
                {
                    Caption = 'Unit of Measure';
                }

                field(quantity; Rec.Quantity)
                {
                    Caption = 'Quantity';
                }

                field(quantityBase; Rec."Quantity (Base)")
                {
                    Caption = 'Quantity Base';
                }

                field(outstandingQuantity; Rec."Outstanding Quantity")
                {
                    Caption = 'Outstanding Quantity';
                }

                field(quantityReceived; Rec."Quantity Received")
                {
                    Caption = 'Quantity Received';
                }

                field(quantityInvoiced; Rec."Quantity Invoiced")
                {
                    Caption = 'Quantity Invoiced';
                }

                field(unitCost; Rec."Direct Unit Cost")
                {
                    Caption = 'Direct Unit Cost';
                }

                field(lineAmount; Rec."Line Amount")
                {
                    Caption = 'Line Amount';
                }

                field(expectedReceiptDate; Rec."Expected Receipt Date")
                {
                    Caption = 'Expected Receipt Date';
                }

                field(plannedReceiptDate; Rec."Planned Receipt Date")
                {
                    Caption = 'Planned Receipt Date';
                }

                field(promisedReceiptDate; Rec."Promised Receipt Date")
                {
                    Caption = 'Promised Receipt Date';
                }

                field(requestedReceiptDate; Rec."Requested Receipt Date")
                {
                    Caption = 'Requested Receipt Date';
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