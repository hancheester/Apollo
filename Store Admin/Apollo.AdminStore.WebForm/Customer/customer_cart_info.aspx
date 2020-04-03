<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" ValidateRequest="false" Inherits="Apollo.AdminStore.WebForm.Customer.customer_cart_info" Codebehind="customer_cart_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerNav" Src="~/UserControls/CustomerNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerTopRightNav" Src="~/UserControls/CustomerTopRightNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.date').datepicker({
                format: 'dd/mm/yyyy',
                keyboardNavigation: false,
                forceParse: false,
                autoclose: true,
                todayHighlight: true
            });
        });

        function loadHTML(html){
            var generator = window.open('','name','height=750,width=700');  
            generator.document.write('<html><head><title>Email preview</title></head><body>' + html + '</body></html>');
            generator.document.close();
            generator.moveTo(50,10);
        };
    </script>    
</asp:Content>
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
                        <Apollo:CustomerNav runat="server" DisabledItem="ShoppingCart" />
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                            <div class="html5buttons">
                                                <div class="dt-buttons btn-group">                                                    
                                                    <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ID="gvCart" runat="server" PageSize="10" AllowPaging="true"
                                            AllowSorting="true" OnPageIndexChanging="gvCart_PageIndexChanging" AutoGenerateColumns="false"
                                            OnSorting="gvCart_Sorting" OnPreRender="gvCart_PreRender" ShowHeader="true" ShowHeaderWhenEmpty="true"
                                            OnRowDataBound="gvCart_RowDataBound" CssClass="table table-striped table-bordered table-hover dataTable" OnRowCommand="gvCart_RowCommand">
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>                
                                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                    Page
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvCart.CustomPageIndex + 1 %>'
                                                        runat="server"></asp:TextBox>
                                                    <asp:ImageButton Visible='<%# (gvCart.CustomPageCount > (gvCart.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                    of
                                                    <%= gvCart.PageCount %>
                                                    pages |
                                                    <asp:PlaceHolder ID="phRecordFound" runat="server">Total
                                                        <%= gvCart.RecordCount %>
                                                        records found</asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                </asp:Panel>
                                            </PagerTemplate>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Product ID" SortExpression="ProductId">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="ProductId" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%# Eval("ProductId")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Product Name" SortExpression="Name">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Product Name</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%# Eval("Name") %> <%# Eval("Option") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity">
                                                    <ItemTemplate><%# Eval("Quantity")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Price" SortExpression="Price">
                                                    <ItemTemplate>
                                                        <%# AdminStoreUtility.GetFormattedPrice(Convert.ToDecimal(Eval("Price")), CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Line Total">
                                                    <ItemTemplate>
                                                        <%# AdminStoreUtility.GetFormattedPrice(Convert.ToInt32(Eval("Quantity")) * Convert.ToDecimal(Eval("Price")), CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <HeaderTemplate>Action</HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:LinkButton CommandName="remove" runat="server" CommandArgument='<%# Eval("ProductPriceId")%>' Text="Delete"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </Apollo:CustomGrid>
                                        <asp:PlaceHolder ID="phSendEmailAbdnCart" runat="server">
                                            <div class="col-lg-6">
                                                <table class="table">
                                                    <tr>
                                                        <th>New offer rule name</th>
                                                        <td><asp:TextBox ID="txtOfferRuleName" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <th>New offer rule alias</th>
                                                        <td><asp:TextBox ID="txtOfferRuleAlias" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <th>Promotion value (%)</th>
                                                        <td><asp:TextBox ID="txtPromoValue" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <th>Expiry date</th>
                                                        <td>
                                                            <asp:RadioButton ID="radExp24hrs" runat="server" GroupName="expiry" Checked="true" />24
                                                            hours from time of send<br />
                                                            <asp:RadioButton ID="radExpSpecifyDate" runat="server" GroupName="expiry" />Specify
                                                            date (Expires at 12:00am on selected date):<br />
                                                            <asp:TextBox CssClass="date form-control" ID="txtExpiry" runat="server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>Additional HTML</th>
                                                        <td><asp:TextBox ID="txtHTML" runat="server" TextMode="MultiLine" Rows="8" CssClass="form-control"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <%--<asp:LinkButton ID="lbPreviewEmail" runat="server" Text="Preview email" CssClass="ABtn"
                                                                OnClick="lbPreviewEmail_Click"></asp:LinkButton>&nbsp;
                                                            <asp:LinkButton ID="lbSendEmail" runat="server" Text="Send email" CssClass="ABtn"
                                                                OnClick="lbSendEmail_Click"></asp:LinkButton>--%>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </asp:PlaceHolder>
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
