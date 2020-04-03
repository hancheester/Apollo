<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.reviews_all" Codebehind="reviews_all.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>All Reviews</h2>
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
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvReviews" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvReviews_PageIndexChanging" 
                                AutoGenerateColumns="false" OnSorting="gvReviews_Sorting" OnPreRender="gvReviews_PreRender" ShowHeader="true" DataKeyNames="Id"
                                CssClass="table table-striped table-bordered table-hover dataTable">                    
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>            
                                    <asp:Panel runat="server" DefaultButton="btnGoPage">
                                        Page 
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvReviews.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                        <asp:ImageButton Visible='<%# (gvReviews.CustomPageCount > (gvReviews.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                        of <%= gvReviews.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvReviews.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>            
                                </PagerTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderText="Product Review ID" SortExpression="Id" HeaderStyle-Width="100" ItemStyle-HorizontalAlign="Center">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product Review ID</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Created On" SortExpression="TimeStamp" HeaderStyle-Width="120">
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="TimeStamp" runat="server" CommandName="Sort">Created On</asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("TimeStamp")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Alias">                
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="Alias" runat="server" CommandName="Sort">Alias</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterAlias" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Alias")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comment">     
                                        <HeaderTemplate>
                                            Comment<br />                                
                                            <asp:TextBox ID="txtFilterComment" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Comment") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Product" SortExpression="ProductId" HeaderStyle-Width="180">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Product" runat="server" CommandName="Sort">Product</asp:LinkButton><br />                    
                                        </HeaderTemplate>
                                        <ItemTemplate><a href='<%# "/catalog/product_info.aspx?productid=" + Eval("ProductId") %>'><%# Eval("ProductName") %></a></ItemTemplate>
                                    </asp:TemplateField>            
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="40">
                                        <ItemTemplate>
                                            <a href='<%# "/catalog/review_info.aspx?productreviewid=" + Eval("Id") %>'>Edit</a>
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