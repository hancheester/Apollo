<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderAddressSelectorControl" Codebehind="OrderAddressSelectorControl.ascx.cs" %>
<asp:PlaceHolder ID="phEdit" runat="server">    
    <table class="table <%= (AddressType == AddressType.Billing) ? "billing" : "shipping" %>">
        <asp:PlaceHolder runat="server" ID="phExistingAddresses" Visible="false">
            <tr>
                <th style="width: 150px;">Select from existing</th>
                <td><asp:DropDownList ID="ddlExistingAddresses" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlExistingAddresses_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <th>Name<strong>*</strong></th>
            <td>                
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfName" runat="server" ControlToValidate="txtName" Display="Dynamic" ValidationGroup="vgOrderDetails" ErrorMessage="A name is required." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Name is required.</span>"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <th>Address line 1<strong>*</strong></th>
            <td>
                <asp:TextBox ID="txtAddrLine1" runat="server" CssClass="form-control line1"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfAddressLine1" runat="server" ControlToValidate="txtAddrLine1" Display="Dynamic" ValidationGroup="vgOrderDetails" ErrorMessage="Address line 1 is required." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Address line 1 is required.</span>"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <th>Address line 2</th>
            <td><asp:TextBox ID="txtAddrLine2" runat="server" CssClass="form-control line2"></asp:TextBox></td>
        </tr>
        <tr>
            <th>County</th>
            <td><asp:TextBox ID="txtCounty" runat="server" CssClass="form-control county"></asp:TextBox></td>
        </tr>
        <tr>
            <th>City<strong>*</strong></th>
            <td>
                <asp:TextBox ID="txtCity" runat="server" CssClass="form-control city"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfCity" runat="server" ControlToValidate="txtCity" Display="Dynamic" ValidationGroup="vgOrderDetails" ErrorMessage="City is required." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> City is required.</span>"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <th>Post code<strong>*</strong></th>
            <td><asp:TextBox ID="txtPostCode" runat="server" CssClass="form-control postcode"></asp:TextBox></td>
        </tr>
        <asp:PlaceHolder ID="phState" runat="server" Visible="false">
            <tr>
                <th>State</th>
                <td>
                    <asp:DropDownList ID="ddlState" OnInit="ddlState_Init" runat="server" DataTextField="State" DataValueField="Code" CssClass="form-control state">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <th>Country</th>
            <td><asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control country" OnInit="ddlCountry_Init" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" DataTextField="Name" DataValueField="Id" AutoPostBack="true"></asp:DropDownList></td>
        </tr>
        <asp:PlaceHolder ID="phSaveAddress" runat="server">
            <tr>
                <th>Save address?</th>
                <td>
                    <asp:CheckBox ID="chkSaveAddress" runat="server" CssClass="saveAddress" />&nbsp;<i class="info <%= (AddressType == AddressType.Billing) ? "billing" : "shipping" %>"></i>
                    <asp:Literal ID="ltAddressId" runat="server"></asp:Literal>
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</asp:PlaceHolder>

<script type="text/javascript">
    run<%= (AddressType == AddressType.Billing) ? "Billing" : "Shipping" %>Script();

    function run<%= (AddressType == AddressType.Billing) ? "Billing" : "Shipping" %>Script() {
        if (window.$)
        {
            $(function () {
                
                var type = <%= (AddressType == AddressType.Billing) ? "'.billing'" : "'.shipping'" %>;
                var $table = $('table' + type);
                var retriever = retrieve<%= (AddressType == AddressType.Billing) ? "Billing" : "Shipping" %>Value;

                $('table' + type + ' .saveAddress input[type=checkbox]').change(function () {                    
                    if (this.checked) {
                        $('.info' + type).addClass('fa fa-spinner fa-spin');

                        var id = retriever($table, '.addressId');
                        var name = retriever($table, '.name');
                        var line1 = retriever($table, '.line1');
                        var line2 = retriever($table, '.line2');
                        var county = retriever($table, '.county');
                        var city = retriever($table, '.city');
                        var postcode = retriever($table, '.postcode');
                        var state = retriever($table, '.state');
                        var country = retriever($table, '.country');

                        <%= Page.ClientScript.GetCallbackEventReference(this, "id + '_' + name + '_' + line1 + '_' + line2 + '_' + county + '_' + city + '_' + postcode + '_' + state + '_' + country", (AddressType == AddressType.Billing) ? "loadBillingStatus" : "loadShippingStatus", "'.info' + type", true) %>;
                    }
                });
            });
        }
        else {
            window.setTimeout(run<%= (AddressType == AddressType.Billing) ? "Billing" : "Shipping" %>Script, 50);
        }
    }

    function load<%= (AddressType == AddressType.Billing) ? "Billing" : "Shipping" %>Status(msg, context) {
        $(context).removeClass('fa fa-spinner fa-spin');
        $(context).addClass('label label-success');
        $(context).text(msg);
    }

    function retrieve<%= (AddressType == AddressType.Billing) ? "Billing" : "Shipping" %>Value($table, className) {
        var result = ' ';
        if ($table.find(className).val() && $table.find(className).val() !== '')
            result = $table.find(className).val();

        return result;
    }

</script>

