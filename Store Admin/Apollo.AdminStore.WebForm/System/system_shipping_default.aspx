﻿<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.System.system_shipping_default" Codebehind="system_shipping_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/flag-icon-css/2.1.0/css/flag-icon.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Shipping Option</h2>
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
                                        <a href="/system/system_shipping_new.aspx" class="btn btn-default buttons-copy buttons-html5">Create</a> 
                                        <a href="/system/system_shipping_default.aspx" class="btn btn-default buttons-copy buttons-html5">Refresh</a>
                                        <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClientClick="return confirm('This action will refresh all shipping options related data on store front and performance could be affected.\nAre you sure to publish?');" OnClick="lbPublish_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
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
                                    <asp:TemplateField HeaderText="Shipping Option ID">                
                                        <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Country">
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lnkCountryName" CommandArgument="Name" runat="server" CommandName="Sort">Country</asp:LinkButton><br />
                                            <asp:TextBox ID="txtShippingCountry" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <%# GetCountryImage(Convert.ToInt32(Eval("CountryId")))%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" SortExpression="Name">                
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lnkName" CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                            <asp:TextBox ID="txtShippingName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Description" SortExpression="Description">                
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lnkDescription" CommandArgument="Description" runat="server" CommandName="Sort">Description</asp:LinkButton><br />
                                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Description")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Enabled">                
                                        <ItemTemplate><%# Convert.ToBoolean(Eval("Enabled")) ? "Yes" : "No"%></ItemTemplate>                
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Priority" SortExpression="Priority">                
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lnkPriority" CommandArgument="Priority" runat="server" CommandName="Sort">Priority</asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Priority")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <a href="/system/system_shipping_info.aspx?id=<%# Eval("Id") %>">Edit</a>                    
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

