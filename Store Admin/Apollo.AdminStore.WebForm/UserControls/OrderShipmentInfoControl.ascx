<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderShipmentInfoControl" Codebehind="OrderShipmentInfoControl.ascx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderHeader" Src="~/UserControls/OrderHeaderControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="AccountView" Src="~/UserControls/OrderAccountViewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="AddressView" Src="~/UserControls/OrderAddressViewControl.ascx" %>

<div class="col-lg-6">
    <Apollo:OrderHeader ID="eohHeader" runat="server" />
</div>

<div class="col-lg-6">
    <Apollo:AccountView ID="eavAccount" runat="server"/>
</div>

<div class="col-lg-6">
    <Apollo:AddressView ID="eavShipping" AddressType="Shipping" runat="server" Title="Shipping address" EditDisabled="true"/>
</div>

<div class="col-lg-6">
    <div class="panel panel-default">
        <div class="panel-heading">
            Shipping &amp; handling information
            <span class="pull-right label label-plain">
                <asp:LinkButton ID="lbEditItem" runat="server" OnClick="lbEditItem_Click" Text="edit" CssClass="printHide"></asp:LinkButton>
            </span>    
        </div>
        <asp:PlaceHolder ID="phShipping" runat="server" Visible="true">
            <table class="table">
                <tr>
                    <th>Carrier</th>
                    <td><asp:Literal ID="ltlCarrier" runat="server"></asp:Literal></td>
                </tr>
                <tr>
                    <th>Tracking Reference</th>
                    <td><asp:Literal ID="ltlTrackingRef" runat="server"></asp:Literal></td>
                </tr>
            </table>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phEditShipping" runat="server" Visible="false">
            <table class="table">
                <tr>
                    <th>Carrier</th>
                    <td>
                        <asp:DropDownList ID="ddlCarrier" runat="server" CssClass="form-control">   
                            <asp:ListItem Text="APC" Value="APC"></asp:ListItem>             
                            <asp:ListItem Text="Royal Mail (1st class)" Value="Royal Mail (1st class)"></asp:ListItem>
                            <asp:ListItem Text="Royal Mail (2nd class)" Value="Royal Mail (2nd class)"></asp:ListItem>
                            <asp:ListItem Text="Royal Mail (Next Day)" Value="Royal Mail (Next Day)"></asp:ListItem>
                            <asp:ListItem Text="Royal Mail (International)" Value="Royal Mail (International)"></asp:ListItem>
                            <asp:ListItem Text="Royal Mail (International Airsure)" Value="Royal Mail (International Airsure)"></asp:ListItem>
                            <asp:ListItem Text="Skynet Mail" Value="Skynet Mail"></asp:ListItem>
                            <asp:ListItem Text="Skynet Courier Express" Value="Skynet Courier Express"></asp:ListItem>                            
                            <asp:ListItem Text="DPD" Value="DPD"></asp:ListItem>
                            <asp:ListItem Text="Citylink" Value="Citylink"></asp:ListItem>
                            <asp:ListItem Text="DHL" Value="DHL"></asp:ListItem>                
                            <asp:ListItem Text="Top Flight Couriers" Value="Top Flight Couriers"></asp:ListItem>
                            <asp:ListItem Text="TNT" Value="TNT"></asp:ListItem>
                            <asp:ListItem Text="UPS" Value="UPS"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <th>Tracking Reference</th>
                    <td><asp:TextBox ID="txtTrackingReference" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="pull-right">
                            <asp:LinkButton ID="lbSave" runat="server" CssClass="btn btn-outline btn-warning" Text="Save" OnClick="lbSave_Click"></asp:LinkButton>&nbsp;<asp:LinkButton ID="lbCancel" runat="server" CssClass="btn btn-outline btn-danger" Text="Cancel" OnClick="lbCancel_Click"></asp:LinkButton>
                        </div>
                    </td>
                </tr>
            </table>      
        </asp:PlaceHolder>
    </div>
</div>

<div class="col-lg-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            Sent items
        </div>
        <table class="table table-striped">
            <tr>
                <th>Product</th>
                <th>Quantity</th>
            </tr>
            <asp:Repeater ID="rptItems" runat="server">                               
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("LineItem.Name")%></td>
                        <td><%# Eval("Quantity")%></td>
                    </tr>
                </ItemTemplate>                
            </asp:Repeater>
        </table>
    </div>
</div>