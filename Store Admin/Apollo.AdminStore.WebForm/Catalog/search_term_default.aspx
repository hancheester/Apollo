<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.search_term_default" Codebehind="search_term_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Search Terms</h2>
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
                                        <a href="/catalog/search_term_new.aspx" class="btn btn-default buttons-copy buttons-html5">Create</a> 
                                        <a href="/catalog/search_term_default.aspx" class="btn btn-default buttons-copy buttons-html5">Refresh</a>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvItems" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvItems_PageIndexChanging" 
                                AutoGenerateColumns="false" OnSorting="gvItems_Sorting" OnPreRender="gvItems_PreRender" ShowHeader="true" CssClass="table table-striped table-bordered table-hover dataTable">
                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                            <PagerTemplate>            
                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                    Page
                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                    <asp:Button Width="0" CssClass="hidden" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" />
                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvItems.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                    <asp:ImageButton Visible='<%# (gvItems.CustomPageCount > (gvItems.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                    of
                                    <%= gvItems.PageCount%>
                                    pages |
                                    <asp:PlaceHolder ID="phRecordFound" runat="server">Total
                                        <%= gvItems.RecordCount %>
                                        records found</asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false">No record found</asp:PlaceHolder>
                                </asp:Panel>
                            </PagerTemplate>
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="120" ItemStyle-HorizontalAlign="Center">                
                                    <HeaderTemplate>
                                        <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Search Term ID</asp:LinkButton><br />
                                        <asp:TextBox ID="txtFilterSearchTermId" runat="server" CssClass="form-control"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Query" HeaderStyle-Width="120">
                                    <HeaderTemplate>
                                        <asp:LinkButton CommandArgument="Query" runat="server" CommandName="Sort">Query</asp:LinkButton><br />
                                        <asp:TextBox ID="txtFilterQuery" runat="server" CssClass="form-control"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Query")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="RedirectUrl">
                                    <HeaderTemplate>
                                        <asp:LinkButton CommandArgument="RedirectUrl" runat="server" CommandName="Sort">Redirect URL</asp:LinkButton><br />
                                        <asp:TextBox ID="txtFilterRedirectUrl" runat="server" CssClass="form-control"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("RedirectUrl")%></ItemTemplate>
                                </asp:TemplateField>            
                                <asp:TemplateField HeaderText="Action" HeaderStyle-Width="60">
                                    <ItemTemplate>
                                        <a href="/catalog/search_term_info.aspx?id=<%# Eval("Id") %>">Edit</a>
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