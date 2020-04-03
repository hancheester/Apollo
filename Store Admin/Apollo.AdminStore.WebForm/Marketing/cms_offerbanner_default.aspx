<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_offerbanner_default" Codebehind="cms_offerbanner_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Offer Banners</h2>
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
                                        <a href="/marketing/cms_offerbanner_new.aspx" class="btn btn-default buttons-copy buttons-html5">Create</a>
                                        <a href="/marketing/cms_offerbanner_default.aspx" class="btn btn-default buttons-copy buttons-html5">Refresh</a>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvBanners" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvBanners_PageIndexChanging" 
                                AutoGenerateColumns="false" OnSorting="gvBanners_Sorting" OnPreRender="gvBanners_PreRender" ShowHeader="true" CssClass="table table-striped table-bordered table-hover dataTable">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>                                       
                                    <asp:Panel runat="server" DefaultButton="btnGoPage">
                                        Page 
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" CssClass="hidden" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvBanners.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                        <asp:ImageButton Visible='<%# (gvBanners.CustomPageCount > (gvBanners.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                        of <%= gvBanners.PageCount%> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvBanners.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>            
                                </PagerTemplate>
                                <Columns>            
                                    <asp:TemplateField HeaderText="Offer Banner ID" SortExpression="Id" HeaderStyle-Width="100">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Offer Banner ID</asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Title" SortExpression="Title">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Title" runat="server" CommandName="Sort">Title</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterTitle" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Title")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date Start" SortExpression="StartDate">                
                                        <HeaderTemplate>                                
                                            From Date<br />
                                            <asp:TextBox CssClass="date form-control" ID="txtFromDate" runat="server"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Convert.ToDateTime(Eval("StartDate")) == DateTime.MinValue ? string.Empty : Eval("StartDate")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date Expire" SortExpression="EndDate">                
                                        <HeaderTemplate>                                
                                            To Date<br />
                                            <asp:TextBox ID="txtToDate" CssClass="date form-control" runat="server"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Convert.ToDateTime(Eval("EndDate")) == DateTime.MinValue ? string.Empty : Eval("EndDate")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" SortExpression="Enabled" HeaderStyle-Width="140" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lbEnabled" CommandArgument="Enabled" runat="server" CommandName="Sort">Status</asp:LinkButton><br />
                                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Active" Value="true"></asp:ListItem>
                                                <asp:ListItem Text="Inactive" Value="false"></asp:ListItem>
                                            </asp:DropDownList>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Enabled").ToString()%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Priority" SortExpression="Priority" HeaderStyle-Width="80">
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lbPriority" CommandArgument="Priority" runat="server" CommandName="Sort">Priority</asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Priority")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="80">
                                        <ItemTemplate>
                                            <a href="/marketing/cms_offerbanner_info.aspx?id=<%# Eval("Id") %>">Edit</a>
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