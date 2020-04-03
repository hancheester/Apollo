<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Report.report_pending_line_inventory" Codebehind="report_pending_line_inventory.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Pending Line Inventory</h2>
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
                                    <div class="dt-buttons btn-group">
                                        <asp:LinkButton ID="lbDownloadPendingItems" runat="server" Text="Download all pending" OnClick="lbDownloadPendingItems_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvResults" runat="server" PageSize="100" AllowPaging="true" AllowSorting="false" OnPageIndexChanging="gvResults_PageIndexChanging" 
                                AutoGenerateColumns="false" OnPreRender="gvResults_PreRender" ShowHeader="true" DataKeyNames="ProductId"
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
                                    <asp:TemplateField HeaderText="Name" SortExpression="Name" HeaderStyle-Width="400">
                                        <HeaderTemplate>
                                            Name<br />
                                            <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>                                            
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Option" HeaderStyle-Width="160">                
                                        <HeaderTemplate>
                                            Option
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Option")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pending Quantity" HeaderStyle-Width="80">
                                        <HeaderTemplate>
                                            Pending Quantity
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Quantity") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Stock" HeaderStyle-Width="80">     
                                        <HeaderTemplate>
                                            Stock
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Stock") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ordered Since" HeaderStyle-Width="160">
                                        <HeaderTemplate>
                                            Ordered Since
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("OrderPlaced") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Related Order ID" HeaderStyle-Width="160">
                                        <HeaderTemplate>
                                            Related Order ID
                                        </HeaderTemplate>
                                        <ItemTemplate><%# GetOrderList(Eval("RelatedOrderIds")) %></ItemTemplate>
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