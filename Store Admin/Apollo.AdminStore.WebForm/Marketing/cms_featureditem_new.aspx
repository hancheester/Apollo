<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_featureditem_new" Codebehind="cms_featureditem_new.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Item</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-6">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgNewItem" CssClass="alert alert-warning" />
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Product<strong>*</strong></th>
                        <td>
                            <asp:MultiView ID="mvProducts" runat="server" ActiveViewIndex="0">
                                <asp:View runat="server">
                                    <asp:Literal ID="ltlProductId" runat="server"></asp:Literal>&nbsp;
                                    <span style="display:none;"><asp:TextBox ID="txtProductId" runat="server"></asp:TextBox></span>
                                    <asp:LinkButton ID="lbEditProductId" OnClick="lbEditProductId_Click" runat="server" Text="Edit" CssClass="btn btn-outline btn-warning"></asp:LinkButton>
                                    <asp:RequiredFieldValidator ValidationGroup="vgNewItem" runat="server"
                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Product is required.</span>" Display="Dynamic" ErrorMessage="Product is required."
                                        ControlToValidate="txtProductId"></asp:RequiredFieldValidator>
                                </asp:View>
                                <asp:View runat="server">
                                    <asp:RequiredFieldValidator ValidationGroup="vgNewItem" runat="server"
                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Product is required.</span>" Display="Dynamic" ErrorMessage="Product is required."
                                        ControlToValidate="txtProductEditId"></asp:RequiredFieldValidator>
                                    <div class="table-responsive">
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                            <div class="html5buttons">
                                                <div class="dt-buttons btn-group">
                                                    <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ID="gvProducts" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvProducts_PageIndexChanging" 
                                            AutoGenerateColumns="false" OnSorting="gvProducts_Sorting" OnPreRender="gvProducts_PreRender" ShowHeader="true" OnRowDataBound="gvProducts_RowDataBound" 
                                            OnRowCommand="gvProducts_OnRowCommand"
                                            DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">                    
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>                
                                            <PagerTemplate>  
                                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                    Page 
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                    <asp:ImageButton Visible='<%# (gvProducts.CustomPageCount > (gvProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                    of <%= gvProducts.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvProducts.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                                </asp:Panel>
                                            </PagerTemplate>         
                                            <Columns>                                    
                                                <asp:TemplateField HeaderStyle-Width="80px" HeaderStyle-CssClass="header" HeaderText="ProductId" ItemStyle-HorizontalAlign="Center" SortExpression="Id">                                        
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="ProductId" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterId" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Name" SortExpression="Name">                                        
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterName" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" Text="Select" CommandArgument='<%# Eval("Id") %>' CommandName="select"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </Apollo:CustomGrid>
                                    </div>                                    
                                    <span style="display:none;"><asp:TextBox ID="txtProductEditId" runat="server"></asp:TextBox></span>                            
                                    <asp:RequiredFieldValidator ValidationGroup="vgNewItem" runat="server"
                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Product is required.</span>" Display="Dynamic" ControlToValidate="txtProductEditId"></asp:RequiredFieldValidator>
                                </asp:View>
                            </asp:MultiView>
                        </td>
                    </tr>
                    <tr>
                        <th>Group</th>
                        <td><asp:DropDownList ID="ddlGroups" runat="server" CssClass="form-control"></asp:DropDownList></td>
                    </tr>
                    <tr>
                        <th>Priority <small class="text-navy clearfix">(integer only)</small></th>
                        <td><asp:TextBox ID="txtPriority" runat="server" Text="0" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgNewItem" runat="server"
                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Priority is required.</span>" Display="Dynamic" ErrorMessage="Priority is required."
                                ControlToValidate="txtPriority"></asp:RequiredFieldValidator>
                            <asp:RangeValidator Type="Integer" ControlToValidate="txtPriority" ValidationGroup="vgNewItem" runat="server" MaximumValue="99999999" MinimumValue="-99999999"
                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Priority is invalid.</span>" Display="Dynamic" ErrorMessage="Priority is invalid."></asp:RangeValidator>
                        </td>
                    </tr>                   
                </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/marketing/cms_featureditem_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewItem" Text="Create" 
                        OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to save the item?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>