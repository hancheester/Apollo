<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderShippingNewControl" Codebehind="OrderShippingNewControl.ascx.cs" %>
<div class="panel panel-default">
    <div class="panel-heading">
        Shipping &amp; Handling Information
    </div>
    <table class="table">
        <tr>
            <th>Total order value</th>
            <td><asp:Literal ID="ltlOrderValue" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Shipping</th>
            <td><asp:Literal ID="ltlShippingInfo" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Note</th>
            <td style="font-size: 20px; color: red;"><asp:Literal ID="ltlPackingInfo" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Carrier</th>
            <td>
                <asp:DropDownList ID="ddlCarrier" runat="server" CssClass="form-control">
                    <asp:ListItem Text="APC" Value="APC"></asp:ListItem>
                    <%--<asp:ListItem Text="Royal Mail (1st class)" Value="Royal Mail (1st class)"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="Royal Mail (2nd class)" Value="Royal Mail (2nd class)"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="Royal Mail (Next Day)" Value="Royal Mail (Next Day)"></asp:ListItem>--%>
                    <asp:ListItem Text="Royal Mail (International)" Value="Royal Mail (International)"></asp:ListItem>
                    <%--<asp:ListItem Text="Royal Mail (International Tracked)" Value="Royal Mail (International Tracked)"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="Royal Mail (International Airsure)" Value="Royal Mail (International Airsure)"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="Skynet Mail" Value="Skynet Mail"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="Skynet Courier Express" Value="Skynet Courier Express"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="DPD" Value="DPD"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="Citylink" Value="Citylink"></asp:ListItem>--%>
                    <asp:ListItem Text="UK 24" Value="UK 24"></asp:ListItem>
                    <asp:ListItem Text="UK 48" Value="UK 48"></asp:ListItem>
                    <asp:ListItem Text="UK 24 Tracked" Value="UK 24 Tracked"></asp:ListItem>
                    <asp:ListItem Text="UK 48 Tracked" Value="UK 48 Tracked"></asp:ListItem>
                    <%--<asp:ListItem Text="DHL (International Standard)" Value="DHL (International Standard)"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="DHL (UK 24)" Value="DHL (UK 24)"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="DHL (UK 48)" Value="DHL (UK 48)"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="DHL (UK 24 Tracked)" Value="DHL (UK 24 Tracked)"></asp:ListItem>--%>
                    <%--<asp:ListItem Text="DHL (UK 48 Tracked)" Value="DHL (UK 48 Tracked)"></asp:ListItem>--%>
                    <asp:ListItem Text="DHL (Express)" Value="DHL (Express)"></asp:ListItem>
                    <asp:ListItem Text="DHL (International Standard)" Value="DHL (International Standard)"></asp:ListItem>
                    <asp:ListItem Text="DHL (International Tracked)" Value="DHL (International Tracked)"></asp:ListItem>
                    <%--<asp:ListItem Text="TNT" Value="TNT"></asp:ListItem>--%>
                    <asp:ListItem Text="Top Flight Couriers" Value="Top Flight Couriers"></asp:ListItem>
                    <%--<asp:ListItem Text="UPS" Value="UPS"></asp:ListItem>--%>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <th>Tracking number</th>
            <td>
                <asp:TextBox ID="txtTrackingNumber" runat="server" CssClass="form-control"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <th>Notification email</th>
            <td><asp:CheckBox ID="cbNotificationEmail" runat="server" Text="Send email to customer" Checked="true" /></td>
        </tr>
    </table>
</div>