<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_featureditem_info" Codebehind="cms_featureditem_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Featured Item</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-6">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary ID="vsItemSum" runat="server" DisplayMode="BulletList" ValidationGroup="vgEditItem" CssClass="alert alert-warning" />
                <table class="table table-striped">
                    <tr>
                        <th>Product<strong>*</strong></th>
                        <td>
                            <asp:MultiView ID="mvProducts" runat="server" ActiveViewIndex="0">
                                <asp:View runat="server">
                                    <asp:Literal ID="ltlProductName" runat="server"></asp:Literal>&nbsp;
                                    <span style="display:none;"><asp:TextBox ID="txtProductId" runat="server"></asp:TextBox></span>
                                    <asp:LinkButton ID="lbEditProductId" OnClick="lbEditProductId_Click" runat="server" CssClass="btn btn-outline btn-warning" Text="Edit"></asp:LinkButton>
                                    <asp:RequiredFieldValidator ValidationGroup="vgNewItem" runat="server"
                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Product is required.</span>" Display="Dynamic" ErrorMessage="Product is required."
                                        ControlToValidate="txtProductId"></asp:RequiredFieldValidator>
                                </asp:View>
                                <asp:View runat="server">
                                    <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server"
                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Product is required.</span>" Display="Dynamic" ErrorMessage="Product is required."
                                        ControlToValidate="txtProductEditId"></asp:RequiredFieldValidator>
                                    <div class="table-responsive">
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                            <div class="html5buttons">
                                                <div class="dt-buttons btn-group">
                                                    <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbCancel" runat="server" Text="Cancel" OnClick="lbCancel_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ID="gvProducts" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvProducts_PageIndexChanging" 
                                            AutoGenerateColumns="false" OnSorting="gvProducts_Sorting" OnPreRender="gvProducts_PreRender" ShowHeader="true" OnRowDataBound="gvProducts_RowDataBound" 
                                            OnRowCommand="gvProducts_OnRowCommand"
                                            DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">                    
                                            <PagerSettings Visible="true" Position="TopAndBottom" Mode="NextPreviousFirstLast"/>                
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
                                                <asp:TemplateField HeaderStyle-Width="80px" HeaderStyle-CssClass="header" HeaderText="Id" ItemStyle-HorizontalAlign="Center" SortExpression="Id">
                                                    <ItemTemplate><%# Eval("Id") %></ItemTemplate>                                                        
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterId" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Name" SortExpression="Name">
                                                    <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterName" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </HeaderTemplate>
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
                                    <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server"
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
                        <td><asp:TextBox ID="txtPriority" CssClass="form-control" runat="server" Text="0"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server"
                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Priority is required.</span>" Display="Dynamic" ErrorMessage="Priority is required."
                                ControlToValidate="txtPriority"></asp:RequiredFieldValidator>
                            <asp:RangeValidator Type="Integer" ControlToValidate="txtPriority" ValidationGroup="vgEditItem" runat="server" MaximumValue="99999999" MinimumValue="-99999999"
                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Priority is invalid.</span>" Display="Dynamic" ErrorMessage="Priority is invalid."></asp:RangeValidator>
                        </td>
                    </tr>
                </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/marketing/cms_featureditem_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <asp:LinkButton ID="lbSaveContinue" runat="server" ValidationGroup="vgEditItem" Text="Update" OnClick="lbSaveContinue_Click" CssClass="btn btn-sm btn-info"></asp:LinkButton>
                    <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure to delete this item?');" CssClass="btn btn-sm btn-danger"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>