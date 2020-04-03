<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.cancellation_default" Codebehind="cancellation_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/flag-icon-css/2.1.0/css/flag-icon.min.css" rel="stylesheet">
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
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Cancellation</h2>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                <div class="html5buttons">
                                    <div class="dt-buttons btn-group">
                                        <asp:LinkButton ID="lbResetRefundFilter" runat="server" Text="Reset" OnClick="lbResetRefundFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearchRefund" runat="server" Text="Search" OnClick="lbSearchRefund_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>                            
                            <Apollo:CustomGrid ID="gvRefunds" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" ShowHeaderWhenEmpty="true"
                                OnPageIndexChanging="gvRefunds_PageIndexChanging" AutoGenerateColumns="false" OnSorting="gvRefunds_Sorting" OnPreRender="gvRefunds_PreRender" 
                                ShowHeader="true" CssClass="table table-striped table-bordered table-hover dataTable">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>            
                                    <asp:Panel runat="server" DefaultButton="btnRefundGoPage">
                                        Page 
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" CssClass="hidden" runat="server" ID="btnRefundGoPage" OnClick="btnRefundGoPage_Click" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvRefunds.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                        <asp:ImageButton Visible='<%# (gvRefunds.CustomPageCount > (gvRefunds.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                        of <%= gvRefunds.PageCount%> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvRefunds.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>            
                                </PagerTemplate>
                                <Columns>            
                                    <asp:TemplateField HeaderText="OrderId" SortExpression="OrderId" HeaderStyle-Width="45" ItemStyle-HorizontalAlign="Center">                
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="OrderId" runat="server" CommandName="Sort">Order ID</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterOrderId" runat="server" CssClass="form-control" />
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("OrderId")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Country" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="45">
                                        <HeaderTemplate>
                                            Country
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <%# GetShippingCountryImage(Convert.ToInt32(Eval("OrderId"))) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Reason" HeaderStyle-Width="400">
                                        <HeaderTemplate>Reason</HeaderTemplate>
                                        <ItemTemplate><%# Eval("Reason")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Point To Refund" HeaderStyle-Width="30" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate><%# Eval("PointToRefund")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount To Refund" HeaderStyle-Width="30" ItemStyle-HorizontalAlign="Center">                
                                        <ItemTemplate><%# AdminStoreUtility.GetFormattedPrice((decimal)Eval("ValueToRefund"), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity, (decimal)Eval("ExchangeRate")) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date" SortExpression="DateStamp" HeaderStyle-Width="100">
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="DateStamp" runat="server" CommandName="Sort">Date</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="date form-control" /> to <asp:TextBox ID="txtToDate" CssClass="date form-control" runat="server" />
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("DateStamp")%></ItemTemplate>
                                    </asp:TemplateField>            
                                    <asp:TemplateField HeaderText="Status" SortExpression="IsCompleted" HeaderStyle-Width="100">
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="IsCompleted" runat="server" CommandName="Sort">Status</asp:LinkButton><br />
                                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Completed" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Pending" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <%# Convert.ToBoolean(Eval("IsCompleted")) ? "Completed" : "Pending" %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="100">
                                        <ItemTemplate>
                                            <a href='<%# "/sales/cancellation_info.aspx?refundinfoid=" + Eval("Id") + "&orderid=" + Eval("OrderId") %>'>View</a>
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
</asp:Content>