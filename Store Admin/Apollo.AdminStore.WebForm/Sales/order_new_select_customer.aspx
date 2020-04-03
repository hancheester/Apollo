<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_new_select_customer" Codebehind="order_new_select_customer.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">    
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Create New Order</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <div class="alert alert-info">
                                Please select a customer below or ignore and continue. 
                            </div>
                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                <div class="html5buttons">
                                    <div class="dt-buttons btn-group">
                                        <a class="btn btn-default buttons-copy buttons-html5" href="/sales/order_default.aspx">Back</a>
                                        <asp:LinkButton ID="lbResetFilter" CssClass="btn btn-default buttons-copy buttons-html5" runat="server" Text="Reset" OnClick="lbResetFilter_Click"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" CssClass="btn btn-default buttons-copy buttons-html5" runat="server" Text="Search" OnClick="lbSearch_Click"></asp:LinkButton>
                                        <a href="/sales/order_new.aspx" class="btn btn-default buttons-copy buttons-html5">Ignore and continue</a>
                                    </div>
                                </div>
                            </div>
                            
                            <Apollo:CustomGrid ID="cgUsers" runat="server" PageSize="10" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable"
                                AutoGenerateColumns="false" AllowPaging="true" AllowSorting="true"
                                OnPreRender="cgUsers_PreRender"
                                OnSorting="cgUsers_Sorting"
                                OnRowCreated="cgUsers_RowCreated"
                                OnPageIndexChanging="cgUsers_PageIndexChanging">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>    
                                    <asp:Panel runat="server" DefaultButton="btnGoPage">
                                        Page 
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# cgUsers.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                        <asp:ImageButton Visible='<%# (cgUsers.CustomPageCount > (cgUsers.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                        of <%= cgUsers.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= cgUsers.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>            
                                </PagerTemplate>
                                <Columns>            
                                    <asp:TemplateField HeaderText="Account ID" SortExpression="Id">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Account ID</asp:LinkButton><br />                    
                                            <asp:TextBox ID="txtFilterUserId" runat="server" CssClass="form-control"></asp:TextBox>
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
                                            <asp:TextBox ID="txtFilterContactNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("ContactNumber")%></ItemTemplate>
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