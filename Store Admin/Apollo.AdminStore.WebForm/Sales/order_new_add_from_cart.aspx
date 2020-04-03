<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="order_new_add_from_cart.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Sales.order_new_add_from_cart" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerTopRightNav" Src="~/UserControls/CustomerTopRightNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Order</h2>
            <h3>Add Items</h3>
        </div>
    </div>
    <Apollo:CustomerTopRightNav ID="ectTogRightNav" runat="server" OnActionOccurred="ectTogRightNav_ActionOccurred"/>
    <div class="order-new wrapper wrapper-content animated fadeInRight">
        <div class="row">            
            <div class="order-new">
                <div class="cart col-lg-12">
                    <Apollo:NoticeBox ID="enbNotice" runat="server" />
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            Cart contents
                            <span class="pull-right label label-plain">
                                <asp:LinkButton ID="lbHideAddItemView" runat="server" OnClick="lbHideAddItemView_Click" CausesValidation="false">finish adding items</asp:LinkButton>
                            </span>
                        </div>
                        <div class="panel-body">
                            <asp:GridView ID="gvTempCart" runat="server" AutoGenerateColumns="false" 
                                CssClass="table table-striped table-bordered table-hover dataTable" GridLines="None" OnRowCommand="gvTempCart_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Product ID" SortExpression="ProductId">
                                        <ItemTemplate><%# Eval("ProductId")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Product name" SortExpression="Name">
                                        <ItemTemplate>
                                            <%# Eval("Product.Name") %> <%# Eval("ProductPrice.Option")%>
                                            <%# Eval("ProductPrice.Note") != null ? "<span class='text-danger clearfix'><strong>" + Eval("ProductPrice.Note") + "</strong></span>" : null %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity" HeaderStyle-Width="50px">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQty" runat="server" Text='<%# Eval("Quantity")%>' CssClass="form-control"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Price (including tax)" SortExpression="Price">
                                        <ItemTemplate>
                                            <%# AdminStoreUtility.GetFormattedPrice(Eval("ProductPrice.OfferPriceInclTax"), CustomerCurrency.CurrencyCode, CurrencyType.HtmlEntity, CustomerCurrency.ExchangeRate)%>                                            
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Line total">
                                        <ItemTemplate>
                                            <%# AdminStoreUtility.GetFormattedPrice(Convert.ToInt32(Eval("Quantity")) * Convert.ToDecimal(Eval("ProductPrice.OfferPriceInclTax")), CustomerCurrency.CurrencyCode, CurrencyType.HtmlEntity, CustomerCurrency.ExchangeRate)%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" CommandArgument='<%# Eval("ProductPriceId")%>' CommandName="updateItem">Update</asp:LinkButton>
                                            |
                                            <asp:LinkButton runat="server" CommandArgument='<%# Eval("ProductPriceId")%>' CommandName="removeItem">Remove</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-lg-12">
                            <asp:LinkButton ID="lbAddFromProductSearch" runat="server" CssClass="btn btn-primary pull-right" OnClick="lbAddFromProductSearch_Click">Add from product search</asp:LinkButton>
                        </div>
                    </div>
                    <p></p>
                    <asp:PlaceHolder ID="phCustomerCart" runat="server" Visible="false">
                        <div class="panel panel-info">
                            <div class="panel-heading">Customer's current cart</div>
                            <div class="panel-body">
                                <asp:GridView ID="gvCustCart" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-bordered table-hover dataTable" 
                                    GridLines="None" 
                                    OnRowDataBound="gvCustCart_RowDataBound" OnRowCommand="gvCustCart_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Product ID">
                                            <ItemTemplate><%# Eval("ProductId")%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Product name">
                                            <ItemTemplate><%# Eval("Product.Name")%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="(Price) Option">
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlOptions" Width="300px"></asp:DropDownList>
                                                <asp:Literal ID="litSingleOption" runat="server"></asp:Literal>
                                                <asp:HiddenField ID="hdnSingleOptionId" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Quantity">
                                            <ItemTemplate><%# Eval("Quantity") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Quantity to add">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQty" runat="server" Text="1" Width="50px"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Discontinued">
                                            <ItemTemplate>
                                                <%# IsDiscontinued(Convert.ToInt32(Eval("ProductId"))).ToString()%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" CommandArgument='<%# Eval("ProductId")%>' CommandName="addProduct">Add to basket</asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
