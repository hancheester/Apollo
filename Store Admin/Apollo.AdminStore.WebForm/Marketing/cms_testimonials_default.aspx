<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_testimonials_default" Codebehind="cms_testimonials_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Testimonials</h2>
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
                                        <a href="/marketing/cms_testimonials_new.aspx" class="btn btn-default buttons-copy buttons-html5">Create</a>
                                        <a href="/marketing/cms_testimonials_default.aspx" class="btn btn-default buttons-copy buttons-html5">Refresh</a>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton> 
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvTestimonials" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvTestimonials_PageIndexChanging" 
                                AutoGenerateColumns="false" OnSorting="gvTestimonials_Sorting" OnPreRender="gvTestimonials_PreRender" ShowHeader="true" CssClass="table table-striped table-bordered table-hover dataTable">                    
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>
                                    <asp:Panel runat="server" DefaultButton="btnGoPage">
                                        Page 
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" CssClass="hidden" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvTestimonials.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                        <asp:ImageButton Visible='<%# (gvTestimonials.CustomPageCount > (gvTestimonials.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                        of <%= gvTestimonials.PageCount%> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvTestimonials.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>            
                                </PagerTemplate>
                                <Columns>            
                                    <asp:TemplateField HeaderText="Testimonial ID" SortExpression="Id">
                                        <ItemTemplate><%# Eval("Id")%></ItemTemplate>                                                        
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Testimonial ID</asp:LinkButton>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comment" SortExpression="Comment">
                                        <ItemTemplate><%# Eval("Comment")%></ItemTemplate>                                                        
                                        <HeaderTemplate>                                
                                            <span>Comment</span><br />
                                            <asp:TextBox ID="txtFilterComment" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                        <ItemTemplate><%# Eval("Name")%></ItemTemplate>                                                        
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Priority" SortExpression="Priority" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lbPriority" CommandArgument="Priority" runat="server" CommandName="Sort">Priority</asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Priority")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="80">
                                        <ItemTemplate>
                                            <a href="/marketing/cms_testimonials_info.aspx?id=<%# Eval("Id") %>">Edit</a>
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