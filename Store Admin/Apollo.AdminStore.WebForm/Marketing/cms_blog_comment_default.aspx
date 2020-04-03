<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="cms_blog_comment_default.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_blog_comment_default" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Blog Comments</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <Apollo:CustomGrid ID="gvBlogComments" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvBlogComments_PageIndexChanging" 
                                AutoGenerateColumns="false" ShowHeader="true" OnRowCommand="gvBlogComments_RowCommand"
                                CssClass="table table-striped table-bordered table-hover dataTable">                    
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>            
                                    <asp:Panel runat="server" DefaultButton="btnGoPage">
                                        Page 
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" CssClass="hidden" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvBlogComments.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                        <asp:ImageButton Visible='<%# (gvBlogComments.CustomPageCount > (gvBlogComments.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                        of <%= gvBlogComments.PageCount%> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvBlogComments.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>            
                                </PagerTemplate>
                                <Columns>            
                                    <asp:TemplateField HeaderText="Blog Comment ID" HeaderStyle-Width="120">
                                        <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Blog Post" HeaderStyle-Width="200">                                        
                                        <ItemTemplate><%# GetBlogPostTitle(Convert.ToInt32(Eval("BlogPostId"))) %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comment">
                                        <ItemTemplate><%# Eval("CommentText")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Profile Name" HeaderStyle-Width="120">                
                                        <ItemTemplate><%# Eval("ProfileName") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Created On" HeaderStyle-Width="120">                
                                        <ItemTemplate><%# Eval("CreatedOnDate") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="80">
                                        <ItemTemplate>
                                            <asp:LinkButton CommandName="remove" runat="server" CommandArgument='<%# Eval("Id") %>' Text="Delete"></asp:LinkButton>
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