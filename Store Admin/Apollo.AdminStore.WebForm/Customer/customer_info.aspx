<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Customer.customer_info" Codebehind="customer_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerNav" Src="~/UserControls/CustomerNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerTopRightNav" Src="~/UserControls/CustomerTopRightNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">    
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Account</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>

    <Apollo:CustomerTopRightNav ID="ectTogRightNav" runat="server" OnActionOccurred="ectTogRightNav_ActionOccurred" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:CustomerNav runat="server" DisabledItem="CustomerView" />
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <div class="panel panel-danger">
                                            <div class="panel-heading">
                                                User information
                                            </div>
                                            <table class="table">
                                                <tr>
                                                    <th>
                                                        Last logged in
                                                    </th>
                                                    <td>
                                                        <asp:Literal ID="ltlLastLoggedIn" runat="server"></asp:Literal>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        Last activity in
                                                    </th>
                                                    <td>
                                                        <asp:Literal ID="ltlLastActivityIn" runat="server"></asp:Literal>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        Account created on
                                                    </th>
                                                    <td>
                                                        <asp:Literal ID="ltlAccCreatedOn" runat="server"></asp:Literal>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        User group
                                                    </th>
                                                    <td>
                                                        <asp:Literal ID="ltlCustGroup" runat="server"></asp:Literal>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>
                                                        Primary billing address
                                                    </th>
                                                    <td>
                                                        <asp:Literal ID="ltlBillingAddr" runat="server"></asp:Literal>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>                                    
                                    <div class="col-lg-6">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Points remaining
                                            </div>
                                            <table class="table">
                                                <tr>
                                                    <th>
                                                        <asp:Literal ID="ltlLoyaltyPoints" runat="server"></asp:Literal>
                                                    </th>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="panel panel-info">
                                            <div class="panel-heading">
                                                Note
                                            </div>
                                            <table class="table">
                                                <tr>
                                                    <th>
                                                        <asp:Literal ID="ltlNote" runat="server"></asp:Literal>
                                                    </th>
                                                </tr>
                                            </table>
                                            <p></p>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="panel panel-warning">
                                            <div class="panel-heading">
                                                Recent orders
                                            </div>
                                            <asp:Repeater ID="rptOrders" runat="server">
                                                <HeaderTemplate>
                                                    <table class="table">
                                                        <tr class="heading">
                                                            <th>
                                                                Order ID
                                                            </th>
                                                            <th>
                                                                Purchased at
                                                            </th>
                                                            <th>
                                                                Ship to
                                                            </th>
                                                            <th>
                                                                Grand Total
                                                            </th>
                                                            <th>
                                                                Status
                                                            </th>
                                                        </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <a href='<%# "/sales/order_info.aspx?orderid=" + Eval("Id") %>'>
                                                                <%# Eval("Id") %></a>
                                                        </td>
                                                        <td>
                                                            <%# Eval("OrderPlaced") %>
                                                        </td>
                                                        <td>
                                                            <%# Eval("ShipTo")%>
                                                        </td>
                                                        <td>
                                                            <%# AdminStoreUtility.GetFormattedPrice(OrderService.CalculateOrderTotalByOrderId(Convert.ToInt32(Eval("Id"))), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity) %>
                                                        </td>
                                                        <td>
                                                            <%# OrderService.GetOrderStatusByCode(Eval("StatusCode").ToString()) %>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>            
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <asp:Literal ID="ltlShoppingCart" runat="server"></asp:Literal>
                                            </div>
                                            <asp:Repeater ID="rptShoppingCart" runat="server">
                                                <HeaderTemplate>
                                                    <div>
                                                        <table class="table">
                                                            <tr class="heading">
                                                                <th>Product ID</th>
                                                                <th>Product Name</th>
                                                                <th>Quantity</th>
                                                                <th>Price (incl tax)</th>
                                                                <th>Line Total</th>
                                                                <th>Action</th>
                                                            </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td><%# Eval("ProductId") %></td>
                                                        <td>
                                                            <%# Eval("Product.Name") %> <%# Eval("ProductPrice.Option") %>
                                                        </td>
                                                        <td><%# Eval("Quantity") %></td>
                                                        <td><%# AdminStoreUtility.GetFormattedPrice(Eval("ProductPrice.OfferPriceInclTax"), CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity)%></td>
                                                        <td><%# AdminStoreUtility.GetFormattedPrice(Convert.ToInt32(Eval("Quantity")) * Convert.ToDecimal(Eval("ProductPrice.OfferPriceInclTax")), CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity) %></td>
                                                        <td><a href="/catalog/product_info.aspx?productid=<%# Eval("ProductId") %>">Edit</a> | <a href="<%# AdminStoreUtility.GetProductUrl(Eval("Product.UrlRewrite").ToString()) %>" target="_blank">View</a></td>
                                                    </tr>
                                                </ItemTemplate>            
                                                <FooterTemplate>
                                                    </table>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>                
                </div>
            </div>
        </div>
    </div>
</asp:Content>
