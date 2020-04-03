<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Customer.customer_default" Codebehind="customer_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">    
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Accounts</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgEdit" CssClass="alert alert-warning" />
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                <div class="html5buttons">
                                    <div class="dt-buttons btn-group">
                                        <a href="/customer/customer_new.aspx" class="btn btn-default buttons-copy buttons-html5">Create</a>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvUsers" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvUsers_PageIndexChanging" 
                                AutoGenerateColumns="false" OnSorting="gvUsers_Sorting" OnPreRender="gvUsers_PreRender" ShowHeader="true" DataKeyNames="Id" ShowHeaderWhenEmpty="true"
                                CssClass="table table-striped table-bordered table-hover dataTable">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>
                                    <asp:Panel runat="server" DefaultButton="btnGoPage">
                                        Page
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvUsers.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                        <asp:ImageButton Visible='<%# (gvUsers.CustomPageCount > (gvUsers.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                        of <%= gvUsers.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvUsers.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>            
                                </PagerTemplate>
                                <Columns>            
                                    <asp:TemplateField HeaderText="UserId" SortExpression="Id">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Account ID</asp:LinkButton><br />                    
                                            <asp:TextBox ID="txtFilterUserId" CssClass="form-control" runat="server"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" SortExpression="Name">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />                    
                                            <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Email" SortExpression="Email">
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="Email" runat="server" CommandName="Sort">Email</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Email")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contact Number" SortExpression="ContactNumber">
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="ContactNumber" runat="server" CommandName="Sort">Contact Number</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterContactNumber" runat="server" CssClass="form-control" ></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("ContactNumber")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="DOB">
                                        <HeaderTemplate>
                                            DOB
                                            <asp:TextBox ID="txtDob" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("DOB")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LastActivityDate">
                                        <HeaderTemplate>
                                            Last Activity Date
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("LastActivityDate") %></ItemTemplate>
                                    </asp:TemplateField>            
                                    <asp:TemplateField HeaderText="Action">                    
                                            <ItemTemplate>
                                                <a href='<%# "/customer/customer_info.aspx?userid=" + Eval("ProfileId") %>'>View</a>
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