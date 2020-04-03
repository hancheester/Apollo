<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="report_product_analysis.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Report.report_product_analysis" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/flag-icon-css/2.1.0/css/flag-icon.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phHeaderScript" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
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
            <h2>Product Analysis</h2>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server"/>
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                <div class="html5buttons">
                                    <div class="input-group">
                                        <span class="input-group-addon">From</span>
                                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="date form-control"></asp:TextBox>
                                    </div>
                                    <div class="input-group">
                                        <span class="input-group-addon">To</span>
                                        <asp:TextBox ID="txtToDate" runat="server" CssClass="date form-control"></asp:TextBox>
                                    </div>
                                    <div class="dt-buttons btn-group">                                        
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" OnClick="lbResetFilter_Click" Text="Reset" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvResults" runat="server" PageSize="50" AllowPaging="true" AllowSorting="false" OnPageIndexChanging="gvResults_PageIndexChanging" 
                                AutoGenerateColumns="false" OnPreRender="gvResults_PreRender" ShowHeader="true" DataKeyNames="ProductId" ShowHeaderWhenEmpty="true"
                                CssClass="table table-striped table-bordered table-hover dataTable">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>
                                    <div style="float: left; width: 50%;">
                                        <asp:Panel runat="server" DefaultButton="btnGoPage">
                                            Page 
                                            <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                            <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                            <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvResults.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                            <asp:ImageButton Visible='<%# (gvResults.CustomPageCount > (gvResults.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                            of <%= gvResults.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvResults.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                        </asp:Panel>
                                    </div>                                    
                                </PagerTemplate>
                                <Columns>                                    
                                    <asp:TemplateField HeaderText="Product ID" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80">                
                                        <HeaderTemplate>                                
                                            Product ID<br />
                                            <asp:TextBox ID="txtFilterProductId" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("ProductId")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                        <HeaderTemplate>
                                            Name<br />
                                            <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>                                            
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Current Stock" HeaderStyle-Width="80">
                                        <ItemTemplate><%# Eval("CurrentStock") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sold" HeaderStyle-Width="80">
                                        <ItemTemplate><%# Eval("Sold") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cancelled" HeaderStyle-Width="80">     
                                        <ItemTemplate><%# Eval("Cancelled") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Top Countries">
                                        <ItemTemplate><%# BuildTopCountries(Eval("TopCountries")) %></ItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="40">
                                        <ItemTemplate>
                                            <a href='<%# "/catalog/product_info.aspx?productid=" + Eval("ProductId") %>'>Edit</a>
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