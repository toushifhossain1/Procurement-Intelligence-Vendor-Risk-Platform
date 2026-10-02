page 50100 "PI Vendor API"
{
    PageType = API;

    APIPublisher = 'BTCL_DEV';
    APIGroup = 'procurement';
    APIVersion = 'v1.0';

    EntityName = 'vendor';
    EntitySetName = 'vendors';

    SourceTable = Vendor;

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
                    Caption = 'Vendor No';
                }

                field(name; Rec.Name)
                {
                }

                field(address; Rec.Address)
                {
                }

                field(city; Rec.City)
                {
                }

                field(countryRegionCode; Rec."Country/Region Code")
                {
                }

                field(phoneNo; Rec."Phone No.")
                {
                }

                field(email; Rec."E-Mail")
                {
                }

                field(currencyCode; Rec."Currency Code")
                {
                }

                field(blocked; Rec.Blocked)
                {
                }

                field(balanceLCY; Rec."Balance (LCY)")
                {
                }
                field(paymentTermsCode; Rec."Payment Terms Code")
                {
                }

                field(vendorPostingGroup; Rec."Vendor Posting Group")
                {
                }
                field(lastModifiedDateTime; Rec.SystemModifiedAt)
                {
                    Editable = false;
                }
            }
        }
    }
}