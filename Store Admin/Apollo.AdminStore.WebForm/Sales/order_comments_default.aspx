<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_comments_default" Codebehind="order_comments_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
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
            <h2>Order</h2>
            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        </div>
        <Apollo:OrderPrevNext runat="server"></Apollo:OrderPrevNext>
    </div>

    <Apollo:OrderNav runat="server" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:OrderSideMenu runat="server" Type="Comments"></Apollo:OrderSideMenu>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">                                    
                                    <div class="table-responsive">
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                            <div class="html5buttons">
                                                <div class="dt-buttons btn-group">
                                                    <asp:LinkButton ID="lbResetFilter" runat="server" CssClass="btn btn-default buttons-copy buttons-html5" Text="Reset" OnClick="lbResetFilter_Click"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbSearch" runat="server" Text="Search" CssClass="btn btn-default buttons-copy buttons-html5" OnClick="lbSearch_Click"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ID="gvComments" CssClass="table table-striped table-bordered table-hover dataTable" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvComments_PageIndexChanging" 
                                            AutoGenerateColumns="false" OnSorting="gvComments_Sorting" OnPreRender="gvComments_PreRender" ShowHeader="true">
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>                        
                                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                    Page 
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvComments.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                    <asp:ImageButton Visible='<%# (gvComments.CustomPageCount > (gvComments.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                    of <%= gvComments.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvComments.RecordCount %> records found</asp:PlaceHolder>
                                                                                            <asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                                </asp:Panel>
                                            </PagerTemplate>
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">                    
                                                    <HeaderTemplate>                                
                                                        Comment ID
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="200px" HeaderText="Date" SortExpression="DateStamp">
                                                    <ItemTemplate><%# Eval("DateStamp")%></ItemTemplate>                
                                                    <HeaderTemplate>
                                                            Date:        
                                                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="date form-control" /> to <asp:TextBox ID="txtToDate" CssClass="date form-control" runat="server"/>
                                                    </HeaderTemplate>                    
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Comment Text" SortExpression="CommentText">                    
                                                    <HeaderTemplate>                                
                                                        <asp:LinkButton CommandArgument="CommentText" runat="server" CommandName="Sort">Comment</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterComment" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("CommentText")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="FullName" SortExpression="FullName">                    
                                                    <HeaderTemplate>                                
                                                        <asp:LinkButton CommandArgument="FullName" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterFullName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("FullName")%></ItemTemplate>
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