<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderAddressViewControl" Codebehind="OrderAddressViewControl.ascx.cs" %>
<div class="<%= PrintHideFlag %>">
<div class="panel panel-default">
    <div class="panel-heading">
        <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        <asp:PlaceHolder ID="phEditItem" runat="server">
            <span class="pull-right label label-plain">
                <asp:LinkButton ID="lbEditItem" runat="server" Text="edit" OnClick="lbEditItem_Click"></asp:LinkButton>            
            </span>
        </asp:PlaceHolder>
    </div>

    <asp:PlaceHolder ID="phAddress" runat="server" Visible="true">
        <div id="divAddress<%= phAddress.ClientID %>" style="visibility: <%= Visibility %>; display: <%= Display %>">
            <table class="table">
                <tr style='<%= (GetNameCheckStatus() ? string.Empty : "background-color: #ed5565; border-color: #ed5565; color: #fff;") %>'>
                    <th>Name<span class="printShowInline" style="display:none;">: </span></th>
                    <td><asp:Literal ID="ltlName" runat="server" />
                        <asp:PlaceHolder runat="server" ID="phName" Visible="false" >
                            <span style="float: right;"><asp:LinkButton ID="lbVerifyName" runat="server" Text="Verify" OnClick="lbVerifyName_Click" OnClientClick="javascript:return confirm('Are you sure to verify this?');"></asp:LinkButton></span>
                        </asp:PlaceHolder>
                    </td>
                </tr>
                <asp:Literal ID="ltlEmail" runat="server"></asp:Literal>
                <asp:Literal ID="ltlPhone" runat="server"></asp:Literal>
                <asp:Literal ID="ltlDisplayPhone" runat="server"></asp:Literal>
                <tr style='<%= (GetAddrCheckStatus() ? string.Empty : "background-color: #ed5565; border-color: #ed5565; color: #fff;") %>'>
                    <th>Address Line 1<span class="printShowInline" style="display:none;">: </span></th>
                    <td><asp:Literal ID="ltlAddr1" runat="server" /></td>
                </tr>
                <tr style='<%= (GetAddrCheckStatus() ? string.Empty : "background-color: #ed5565; border-color: #ed5565; color: #fff;") %>'>
                    <th>Address Line 2<span class="printShowInline" style="display:none;">: </span></th>
                    <td><asp:Literal ID="ltlAddr2" runat="server" />
                        <asp:PlaceHolder runat="server" ID="phAddr" Visible="false" >
                            <span style="float: right;"><asp:LinkButton ID="lbVerifyAddr" runat="server" Text="Verify" OnClick="lbVerifyAddr_Click" OnClientClick="javascript:return confirm('Are you sure to verify this?');"></asp:LinkButton></span>
                        </asp:PlaceHolder>
                    </td>
                </tr>
                <asp:PlaceHolder ID="phCountryField" runat="server">
                <tr class="printHide">
                    <th>County<span class="printShowInline" style="display:none;">: </span></th>
                    <td><asp:Literal ID="ltlCounty" runat="server" /></td>
                </tr>
                </asp:PlaceHolder>
                <tr>
                    <th>City<span class="printShowInline" style="display:none;">: </span></th>
                    <td><asp:Literal ID="ltlCity" runat="server" /></td>
                </tr>
                <tr style='<%= (GetPostCodeCheckStatus() ? string.Empty : "background-color: #ed5565; border-color: #ed5565; color: #fff;") %>'>
                    <th>Post Code<span class="printShowInline" style="display:none;">: </span></th>
                    <td><asp:Literal ID="ltlPostCode" runat="server" />
                        <asp:PlaceHolder runat="server" ID="phPostCode" Visible="false" >
                            <span style="float: right;"><asp:LinkButton ID="lbVefiryPostCode" runat="server" Text="Verify" OnClick="lbVerifyPostCode_Click" OnClientClick="javascript:return confirm('Are you sure to verify this?');"></asp:LinkButton></span>
                        </asp:PlaceHolder>
                    </td>
                </tr>
                <asp:PlaceHolder ID="phStateField" runat="server">
                <tr class='<%= HasUSState() ? string.Empty : "printHide" %>'>
                    <th>State<span class="printShowInline" style="display:none;">: </span></th>
                    <td><asp:Literal ID="ltlState" runat="server" /> (<asp:Literal ID="ltlStateCode" runat="server"></asp:Literal>)</td>
                </tr>
                </asp:PlaceHolder>
                <tr>
                    <th>Country<span class="printShowInline" style="display:none;">: </span></th>
                    <td><asp:Literal ID="ltlCountry" runat="server" /></td>
                </tr>
            </table>
        </div>
        <asp:Literal ID="ltlAddress" runat="server" Visible="false" />
    </asp:PlaceHolder>

<asp:PlaceHolder ID="phEdit" runat="server" Visible="false">
    <table class="table">
        <tr>
            <th>Name</th>
            <td><asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox></td>
        </tr>
        <tr>
            <th>Address Line 1</th>
            <td><asp:TextBox ID="txtAddrLine1" runat="server" CssClass="form-control"></asp:TextBox></td>
        </tr>
        <tr>
            <th>Address Line 2</th>
            <td><asp:TextBox ID="txtAddrLine2" runat="server" CssClass="form-control"></asp:TextBox></td>
        </tr>
        <tr>
            <th>County</th>
            <td><asp:TextBox ID="txtCounty" runat="server" CssClass="form-control"></asp:TextBox></td>
        </tr>
        <tr>
            <th>City</th>
            <td><asp:TextBox ID="txtCity" runat="server" CssClass="form-control"></asp:TextBox></td>
        </tr>
        <tr>
            <th>Post Code</th>
            <td><asp:TextBox ID="txtPostCode" runat="server" CssClass="form-control"></asp:TextBox></td>
        </tr>
        <asp:PlaceHolder ID="phState" runat="server" Visible="false">
        <tr>
            <th>State (US only)</th>
            <td><asp:DropDownList ID="ddlState" runat="server" OnInit="ddlState_Init" DataTextField="State" DataValueField="Code" CssClass="form-control"></asp:DropDownList></td>
        </tr>
        </asp:PlaceHolder>
        <tr>
            <th>Country</th>
            <td><asp:DropDownList ID="ddlCountry" runat="server" OnInit="ddlCountry_Init" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" DataTextField="Name" DataValueField="Id" CssClass="form-control"></asp:DropDownList></td>
        </tr>
        <tr>
            <td colspan="2">
                <span class="pull-right">
                    <asp:LinkButton ID="lbSave" runat="server" CssClass="btn btn-outline btn-warning" Text="Save" OnClick="lbSave_Click"></asp:LinkButton>&nbsp;<asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" CssClass="btn btn-outline btn-success" OnClick="lbCancel_Click"></asp:LinkButton>
                </span>
            </td>
        </tr>
    </table>        
</asp:PlaceHolder>
</div>
</div>