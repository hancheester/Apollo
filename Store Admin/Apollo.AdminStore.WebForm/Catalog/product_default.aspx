<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.product_default" Codebehind="product_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript" src="/js/product.js"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Products</h2>
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
                            <div class="dataTables_wrapper form-inline dt-bootstrap" style="padding-bottom: 0;">
                                <div class="html5buttons">
                                    <div class="dt-buttons btn-group">
                                        <asp:LinkButton ID="lbSubmit" runat="server" CssClass="btn btn-default buttons-copy buttons-html5" Text="Update" OnClick="lbSubmit_Click"></asp:LinkButton>
                                        <asp:LinkButton ID="lbAddProduct" runat="server" Text="Create" PostBackUrl="/catalog/product_new.aspx" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClientClick="return confirm('This action will refresh all product related data on store front and performance could be affected.\nAre you sure to publish?');" OnClick="lbPublish_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbResetProductFilter" runat="server" Text="Reset" OnClick="lbResetProductFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearchProduct" runat="server" Text="Search" OnClick="lbSearchProduct_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>                                        
                                    </div>
                                </div>
                                <div class="dataTables_filter">
                                    <label>
                                        Actions: 
                                        <asp:DropDownList ID="ddlActions" runat="server" CssClass="form-control input-sm" OnPreRender="ddlActions_PreRender">
                                            <asp:ListItem Text="- Select -" Value=""></asp:ListItem> 
                                            <%--<asp:ListItem Text="Delete" Value="delete"></asp:ListItem>--%>
                                            <asp:ListItem Text="Change status" Value="changestatus"></asp:ListItem>
                                            <asp:ListItem Text="Change discontinued" Value="changediscontinued"></asp:ListItem>
                                        </asp:DropDownList>
                                        <span class="actions" style="display: none;">
                                            Status: 
                                            <asp:DropDownList ID="ddlActionStatus" CssClass="form-control input-sm" runat="server">
                                                <asp:ListItem Text="Online" Value="enabled"></asp:ListItem>
                                                <asp:ListItem Text="Offline" Value="disabled"></asp:ListItem>
                                            </asp:DropDownList>
                                        </span>
                                        <span class="discontinued" style="display: none;">
                                            Discontinued: 
                                            <asp:DropDownList ID="ddlActionDiscontinued" CssClass="form-control input-sm" runat="server">
                                                <asp:ListItem Text="Yes" Value="enabled"></asp:ListItem>
                                                <asp:ListItem Text="No" Value="disabled"></asp:ListItem>
                                            </asp:DropDownList>
                                        </span>
                                    </label>
                                </div>
                            </div>
                            <asp:Panel runat="server" DefaultButton="lbSearchProduct">
                                <Apollo:CustomGrid ID="gvProducts" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvProducts_PageIndexChanging" 
                                    AutoGenerateColumns="false" OnSorting="gvProducts_Sorting" OnPreRender="gvProducts_PreRender" ShowHeader="true" DataKeyNames="Id" ShowHeaderWhenEmpty="true"
                                    CssClass="table table-striped table-bordered table-hover dataTable">
                                    <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                    <PagerTemplate>                        
                                        <div style="float: left; width: 50%;">
                                            <asp:Panel runat="server" DefaultButton="btnProductGoPage">
                                                Page 
                                                <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                <asp:Button Width="0" runat="server" ID="btnProductGoPage" OnClick="btnProductGoPage_Click" CssClass="hidden" />
                                                <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                <asp:ImageButton Visible='<%# (gvProducts.CustomPageCount > (gvProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                of <%= gvProducts.PageCount%> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvProducts.RecordCount%> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                            </asp:Panel>
                                        </div>
                                    </PagerTemplate>
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <input type="checkbox" onclick="toggle_chosen(this);" />
                                                <br />
                                                <asp:DropDownList ID="ddlFilterChosen" CssClass="form-control" runat="server" OnPreRender="ddlFilterChosen_PreRender">
                                                    <asp:ListItem Text="Any" Value="Any"></asp:ListItem>
                                                    <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                                    <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                                </asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Id" SortExpression="Id" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control" />
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
                                        <asp:TemplateField HeaderText="Status" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                Status<br />
                                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="lbSearchProduct_Click">
                                                    <asp:ListItem Text="- Select -" Value="any"></asp:ListItem>
                                                    <asp:ListItem Text="Online" Value="enabled"></asp:ListItem>
                                                    <asp:ListItem Text="Offline" Value="disabled"></asp:ListItem>
                                                </asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate><i class="fa fa-eye<%# (bool)Eval("Enabled") ? null : "-slash" %>" aria-hidden="true"></i></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Discontinued" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                Discontinued<br />
                                                <asp:DropDownList ID="ddlDiscontinued" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="lbSearchProduct_Click">
                                                    <asp:ListItem Text="- Select -" Value="any"></asp:ListItem>
                                                    <asp:ListItem Text="Yes" Value="enabled"></asp:ListItem>
                                                    <asp:ListItem Text="No" Value="disabled"></asp:ListItem>
                                                </asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate><i class="fa fa-<%# (bool)Eval("Discontinued") ? "times-circle" : "check-circle"%>" aria-hidden="true"></i></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action" HeaderStyle-Width="100px">
                                            <ItemTemplate>
                                                <a href="/catalog/product_info.aspx?productid=<%# Eval("Id") %>">Edit</a> | 
                                                <a href="<%# AdminStoreUtility.GetProductUrl(Eval("UrlKey").ToString()) %>" target="_blank">View</a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </Apollo:CustomGrid>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>