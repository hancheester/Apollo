<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Customer.customer_order_info" Codebehind="customer_order_info.aspx.cs" %>
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
                        <Apollo:CustomerNav runat="server" DisabledItem="Order" />
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
                                        <Apollo:CustomGrid ID="gvOrders" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvOrders_PageIndexChanging" 
                                            CssClass="table table-striped table-bordered table-hover dataTable" 
                                            AutoGenerateColumns="false" OnSorting="gvOrders_Sorting" OnPreRender="gvOrders_PreRender" ShowHeader="true">                    
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>                
                                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                    Page 
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvOrders.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                    <asp:ImageButton Visible='<%# (gvOrders.CustomPageCount > (gvOrders.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                    of <%= gvOrders.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvOrders.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                                </asp:Panel>                        
                                            </PagerTemplate>
                                            <Columns>                    
                                                <asp:TemplateField HeaderText="Order ID" SortExpression="OrderId">                
                                                    <HeaderTemplate>                                
                                                        <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Order ID</asp:LinkButton><br />                    
                                                        <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <a href='<%# "/sales/order_info.aspx?orderid=" + Eval("Id") %>'><%# Eval("Id") %></a>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Purchased On" SortExpression="OrderPlaced">                
                                                    <HeaderTemplate> 
                                                        <asp:LinkButton CommandArgument="OrderPlaced" runat="server" CommandName="Sort">Order Placed (From - To)</asp:LinkButton><br />                          
                                                       <asp:TextBox ID="txtFromDate" CssClass="date form-control" runat="server" /> - <asp:TextBox ID="txtToDate" CssClass="date form-control" runat="server" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("OrderPlaced")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill to">
                                                    <ItemTemplate><%# Eval("BillTo")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ship to">
                                                    <ItemTemplate><%# Eval("ShipTo")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Used Points">
                                                    <ItemTemplate><%# Eval("AllocatedPoint")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Earned Points">
                                                    <ItemTemplate><%# Eval("EarnedPoint")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Awarded Points">
                                                    <ItemTemplate><%# Eval("AwardedPoint")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total">
                                                    <ItemTemplate><%# AdminStoreUtility.GetFormattedPrice(Convert.ToDecimal(Eval("GrandTotal")), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity) %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Shipping">
                                                    <ItemTemplate>
                                                        <%# Eval("ShippingOptionId") != null ? AdminStoreUtility.GetShippingImage((int)Eval("ShippingOptionId")) : null %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Status" SortExpression="StatusCode">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="StatusCode" runat="server" CommandName="Sort">Status</asp:LinkButton><br />
                                                        <asp:DropDownList ID="ddlStatus" CssClass="form-control" runat="server" OnInit="ddlStatus_Init" DataTextField="Status" DataValueField="StatusCode"></asp:DropDownList>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%# Eval("OrderStatus") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>                        
                                            </Columns>          
                                        </Apollo:CustomGrid>
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