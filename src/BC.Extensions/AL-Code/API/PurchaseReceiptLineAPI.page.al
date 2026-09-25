page 50105 "PI Purchase Receipt Line API"
{
    PageType = API;

    APIPublisher = 'BTCL_DEV';
    APIGroup = 'procurement';
    APIVersion = 'v1.0';

    EntityName = 'purchaseReceiptLine';
    EntitySetName = 'purchaseReceiptLines';

    SourceTable = "Purch. Rcpt. Line";

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
                    Caption = 'Receipt No.';
                }

                field(lineNo; Rec."Line No.")
                {
                    Caption = 'Line No.';
                }

                field(buyFromVendorNo; Rec."Buy-from Vendor No.")
                {
                    Caption = 'Buy-from Vendor No.';
                }

                field(payToVendorNo; Rec."Pay-to Vendor No.")
                {
                    Caption = 'Pay-to Vendor No.';
                }

                field(orderNo; Rec."Order No.")
                {
                    Caption = 'Order No.';
                }

                field(orderLineNo; Rec."Order Line No.")
                {
                    Caption = 'Order Line No.';
                }

                field(type; Rec.Type)
                {
                    Caption = 'Type';
                }

                field(no; Rec."No.")
                {
                    Caption = 'No.';
                }

                field(description; Rec.Description)
                {
                    Caption = 'Description';
                }

                field(quantity; Rec.Quantity)
                {
                    Caption = 'Quantity';
                }

                field(quantityBase; Rec."Quantity (Base)")
                {
                    Caption = 'Quantity (Base)';
                }

                field(unitOfMeasureCode; Rec."Unit of Measure Code")
                {
                    Caption = 'Unit of Measure Code';
                }

                field(directUnitCost; Rec."Direct Unit Cost")
                {
                    Caption = 'Direct Unit Cost';
                }

                field(unitCost; Rec."Unit Cost")
                {
                    Caption = 'Unit Cost';
                }

                field(unitCostLCY; Rec."Unit Cost (LCY)")
                {
                    Caption = 'Unit Cost (LCY)';
                }

                field(quantityReceivedNotInvoiced; Rec."Qty. Rcd. Not Invoiced")
                {
                    Caption = 'Qty. Received Not Invoiced';
                }

                field(quantityInvoiced; Rec."Quantity Invoiced")
                {
                    Caption = 'Quantity Invoiced';
                }

                field(postingDate; Rec."Posting Date")
                {
                    Caption = 'Posting Date';
                }

                field(locationCode; Rec."Location Code")
                {
                    Caption = 'Location Code';
                }

                field(expectedReceiptDate; Rec."Expected Receipt Date")
                {
                    Caption = 'Expected Receipt Date';
                }

                field(promisedReceiptDate; Rec."Promised Receipt Date")
                {
                    Caption = 'Promised Receipt Date';
                }

                field(plannedReceiptDate; Rec."Planned Receipt Date")
                {
                    Caption = 'Planned Receipt Date';
                }

                field(orderDate; Rec."Order Date")
                {
                    Caption = 'Order Date';
                }

                field(vendorOrderNo; Rec."Vendor Order No.")
                {
                    Caption = 'Vendor Order No.';
                }

                field(vendorShipmentNo; Rec."Vendor Shipment No.")
                {
                    Caption = 'Vendor Shipment No.';
                }

                field(currencyCode; Rec."Currency Code")
                {
                    Caption = 'Currency Code';
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