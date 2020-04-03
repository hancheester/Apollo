<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="order_new_add_from_product_search.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Sales.order_new_add_from_product_search" %>
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
    <Apollo:CustomerTopRightNav ID="ectTogRightNav" runat="server" OnActionOccurred="ectTogRightNav_ActionOccurred" />
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
                                        <ItemTemplate>
                                            <%# Eval("ProductId")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Product name">
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
                            <asp:LinkButton ID="lbAddFromCustomerCart" runat="server" CssClass="btn btn-primary pull-right" OnClick="lbAddFromCustomerCart_Click">Add from customer's cart</asp:LinkButton>
                        </div>
                    </div>
                    <p></p>
                    <div style="margin: 0 auto; width: 500px;">
                        <div class="panel panel-info">
                            <div class="panel-heading">
                                Product search
                            </div>            
                            <table class="table">
                                <tr>
                                    <th>Product ID</th>
                                    <td><asp:TextBox ID="txtProductId" runat="server" CssClass="form-control"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>Product name</th>
                                    <td><asp:TextBox ID="txtProductName" runat="server" CssClass="form-control"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>Brand ID</th>
                                    <td><asp:TextBox ID="txtBrandId" runat="server" CssClass="form-control"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <th>Category ID</th>
                                    <td><asp:TextBox ID="txtCategpryId" runat="server" CssClass="form-control"></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                        <div class="col-lg-12">
                            <div class="pull-right">
                                <asp:LinkButton ID="lbBulkAddFromSearch" runat="server" Text="Add selected" CssClass="btn btn-outline btn-primary" OnClick="lbBulkAddFromSearch_Click"></asp:LinkButton>
                                <asp:LinkButton ID="lbSearch" runat="server" Text="Search" CssClass="btn btn-outline btn-info" OnClick="lbSearch_Click"></asp:LinkButton>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-12"><p></p></div>
                    <Apollo:NoticeBox ID="enbProducts" runat="server" />
                    <Apollo:CustomGrid ID="gvProducts" runat="server" PageSize="10" 
                        AllowPaging="true" AllowSorting="false" OnPreRender="gvProducts_PreRender" 
                        AutoGenerateColumns="false" ShowHeader="true" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable" 
                        OnRowDataBound="gvProducts_RowDataBound" OnPageIndexChanging="gvProducts_PageIndexChanging"
                        OnRowCommand="gvProducts_RowCommand">
                        <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                        <PagerTemplate>
                            <div style="float: left; width: 50%;">
                                <asp:Panel runat="server" DefaultButton="btnProductGoPage">
                                    Page
                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                    <asp:Button Width="0" runat="server" ID="btnProductGoPage" CssClass="hidden" />
                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                    <asp:ImageButton Visible='<%# (gvProducts.CustomPageCount > (gvProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                    of
                                    <%= gvProducts.PageCount %>
                                    pages |
                                    <asp:PlaceHolder ID="phRecordFound" runat="server">Total
                                        <%= gvProducts.RecordCount %>
                                        records found</asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false">No record found</asp:PlaceHolder>
                                </asp:Panel>
                            </div>
                        </PagerTemplate>
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <input type="checkbox" onclick="toggleChosen(this);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product ID">
                                <ItemTemplate>
                                    <%# Eval("Id")%>
                                    <asp:HiddenField ID="hfProductId" runat="server" Value='<%# Eval("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product name">
                                <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="(Price) Option">
                                <ItemTemplate>
                                    <asp:DropDownList runat="server" ID="ddlOptions" CssClass="form-control">
                                    </asp:DropDownList>
                                    <asp:Literal ID="litSingleOption" runat="server"></asp:Literal>
                                    <asp:HiddenField ID="hdnSingleOptionId" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity to add">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtQty" runat="server" Text="1" CssClass="form-control"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Discontinued">
                                <ItemTemplate><%# Eval("Discontinued")%></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbAddToBasket" runat="server" CommandArgument='<%# Eval("Id")%>' CommandName="addProduct">Add to basket</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </Apollo:CustomGrid>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
