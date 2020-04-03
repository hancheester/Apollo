<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="brand_category_info.aspx.cs" ValidateRequest="false" Inherits="Apollo.AdminStore.WebForm.Catalog.brand_category_info" %>
<%@ Register TagPrefix="Apollo" TagName="BrandCategoryTree" Src="~/UserControls/BrandCategoryTreeControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            var panel = $('#<%= hfCurrentPanel.ClientID %>').val();

            if (panel) {
                $('.nav-tabs a[href=#' + panel + ']').tab('show');
            }
        });
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <asp:HiddenField ID="hfCurrentPanel" runat="server" />
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Brand Category</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-3">
                <Apollo:BrandCategoryTree ID="ectBrandCategory" runat="server" CssClass="tree" OnTreeChanged="ectBrandCategory_TreeChanged" />
            </div>
            <div class="col-lg-9">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <ul class="nav nav-tabs">
                        <li class="active"><a href="#general" data-toggle="tab">General information</a></li>
                        <li><a href="#products" data-toggle="tab">Category products</a></li>
                    </ul>
                    <div class="tab-content">
                        <div id="general" class="tab-pane active">
                            <div class="panel-body">
                                <table class="table table-striped">
                                    <tr>
                                        <th>Name<strong>*</strong></th>
                                        <td>
                                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ControlToValidate="txtName" runat="server" Display="Dynamic" ValidationGroup="vgSave" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>"></asp:RequiredFieldValidator>
                                        </td>                       
                                    </tr>
                                    <tr>
                                        <th>Url key<small class="text-navy clearfix">(auto generated if leave blank)</small></th>    
                                        <td><asp:TextBox ID="txtUrlKey" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <th>Description</th>
                                        <td class="ftb">
                                            <%--<asp:TextBox ID="txtDesc" runat="server"></asp:TextBox>--%>
                                            <FTB:FreeTextBox id="txtDesc" runat="server" BreakMode="LineBreak" FormatHtmlTagsToXhtml="true" PasteMode="Text" Height="300" />
                                        </td>                       
                                    </tr>
                                    <tr>
                                        <th>Thumbnail<small class="text-navy clearfix">(size should be 420x250 for microsite)</small></th>
                                        <td>
                                            <img id="imgThumbnail" runat="server" visible="false" src="" alt="" /><br />
                                            <asp:FileUpload ID="fuThumbnail" runat="server" CssClass="form-control"/><asp:CheckBox ID="cbRemoveThumb" runat="server" Text="Remove" Visible="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Visible</th>
                                        <td><asp:CheckBox ID="cbVisible" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <th>Parent</th>
                                        <td>
                                            <asp:HiddenField ID="hfParent" runat="server" />
                                            <asp:MultiView ID="mvParent" runat="server" ActiveViewIndex="0">
                                                <asp:View ID="View1" runat="server">
                                                    <asp:Literal ID="ltlParent" runat="server"></asp:Literal>&nbsp;
                                                    <asp:LinkButton ID="lbEditParent" runat="server" OnClick="lbEditParent_Click" Text="Edit" CssClass="btn btn-outline btn-sm btn-warning"></asp:LinkButton>
                                                </asp:View>
                                                <asp:View ID="View2" runat="server">
                                                    <Apollo:BrandCategoryTree ID="ectParent" runat="server" OnTreeNodeSelected="ectParent_TreeNodeSelected" />
                                                </asp:View>
                                            </asp:MultiView>
                                        </td>
                                    </tr>
                                </table>
                                <div class="hr-line-dashed"></div>
                                <div class="col-lg-12">
                                    <a href="/catalog/brand_default.aspx" class="btn btn-sm btn-default">Back</a>                                    
                                    <asp:LinkButton ID="lbDeleteBrandCategory" runat="server" Text="Delete" OnClick="lbDeleteBrandCategory_Click" OnClientClick="javascript:return confirm('Are you sure to delete this category? Subcategory will be deleted and related products will be removed from this category and its subcategory.');" CssClass="btn btn-sm btn-warning"></asp:LinkButton>
                                    <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClick="lbPublish_Click" OnClientClick="return confirm('This action will refresh all brands and products related data on store front and performance could be affected.\nAre you sure to publish?');" CssClass="btn btn-sm btn-danger"></asp:LinkButton>
                                    <asp:LinkButton ID="lbReset" runat="server" Text="Reset / New" OnClick="lbReset_Click" CssClass="btn btn-sm btn-success"></asp:LinkButton>                            
                                    <asp:LinkButton ID="lbSaveBrandCategory" runat="server" Text="Save" ValidationGroup="vgSave" OnClick="lbSaveBrandCategory_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <div id="products" class="tab-pane">
                            <div class="ibox float-e-margins">
                                <div class="ibox-content">
                                    <div class="table-responsive">
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                            <div class="html5buttons">
                                                <div class="dt-buttons btn-group">
                                                    <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" CssClass="btn btn-default buttons-copy buttons-html5" OnClick="lbResetFilter_Click"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbSave" runat="server" Text="Save" OnClick="lbSave_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>                                                    
                                                    <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ID="gvProducts" runat="server" ShowHeaderWhenEmpty="true" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvProducts_PageIndexChanging" 
                                            AutoGenerateColumns="false" OnSorting="gvProducts_Sorting" OnPreRender="gvProducts_PreRender" ShowHeader="true" OnRowDataBound="gvProducts_RowDataBound" 
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
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <input type="checkbox" onclick="toggleChosen(this);" />
                                                        <br />
                                                        <asp:DropDownList ID="ddlFilterChosen" runat="server" OnPreRender="ddlFilterChosen_PreRender" CssClass="form-control">
                                                            <asp:ListItem Text="Any" Value="any"></asp:ListItem>
                                                            <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                                                            <asp:ListItem Text="No" Value="no"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" Checked='<%# (Convert.ToInt32(Eval("BrandCategoryId")) == GetIntState(BRAND_CATEGORY_ID) && Convert.ToInt32(Eval("BrandCategoryId")) != AppConstant.DEFAULT_BRAND_CATEGORY ? true : false) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ProductId" SortExpression="Id">
                                                    <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                                    <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </Apollo:CustomGrid>
                                    </div>
                                </div>
                            </div>                            
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
