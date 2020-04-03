<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.category_default" ValidateRequest="false" Codebehind="category_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CategoryTree" Src="~/UserControls/CategoryTreeControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.date').datepicker({
                format: 'dd/mm/yyyy',
                keyboardNavigation: false,
                forceParse: false,
                autoclose: true,
                todayHighlight: true
            });

            var panel = $('#<%= hfCurrentPanel.ClientID %>').val();

            if (panel) {
                $('.nav-tabs a[href=#' + panel + ']').tab('show');
            }

            $('[data-toggle="popover"]').popover('show');
        
        });
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <script type="text/javascript" src="/js/wz_tooltip.js"></script>
    <asp:HiddenField ID="hfCurrentPanel" runat="server" />
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Category</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-3">
                <Apollo:CategoryTree ID="ectCategory" runat="server" OnTreeChanged="ectCategory_TreeChanged" CssClass="tree" />
            </div>
            <div class="col-lg-9">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <ul class="nav nav-tabs">
                        <li class="active"><a href="#general" data-toggle="tab">General information</a></li>
                        <asp:PlaceHolder runat="server" ID="phTabs">
                            <asp:PlaceHolder ID="phCategoryProducts" runat="server">
                                <li id="productsTab"><a href="#products" data-toggle="tab">Category products</a></li>
                            </asp:PlaceHolder>
                            <li><a href="#meta" data-toggle="tab">Metadata</a></li>
                            <li><a href="#media" data-toggle="tab">Category media</a></li>
                            <asp:PlaceHolder ID="phFilters" runat="server">
                                <li><a href="#filters" data-toggle="tab">Category filters</a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phFeatureProducts" runat="server">
                                <li><a href="#featProds" data-toggle="tab">Featured products</a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phFeatureBrands" runat="server">
                                <li><a href="#featBrands" data-toggle="tab">Featured brands</a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phWhatsNew" runat="server">
                                <li><a href="#whatsnew" data-toggle="tab">What's New</a></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phBanners" runat="server">
                                <li><a href="#banners" data-toggle="tab">Banners</a></li>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                    </ul>
                    <div class="tab-content">
                        <div id="general" class="tab-pane active">
                            <div class="panel-body">
                                <asp:ValidationSummary runat="server" DisplayMode="BulletList" CssClass="valSummary alert alert-warning" ValidationGroup="vgSave" />
                                <table class="table table-striped">
                                    <tr>
                                        <th>Name<strong>*</strong></th>
                                        <td>
                                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ControlToValidate="txtName" runat="server" Display="Dynamic" ValidationGroup="vgSave" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>" ErrorMessage="Name is required."></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>H1 title</th>
                                        <td><asp:TextBox ID="txtH1Title" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <th>
                                            Description                                            
                                            <small class="text-navy clearfix">(it will be used in category heading)</small>
                                            <img class="img-thumbnail" src="/img/category-description-1.png" style="width: 400px"/>
                                            <p></p>
                                            <img class="img-thumbnail" src="/img/category-description-2.png" style="width: 400px"/>
                                        </th>
                                        <td class="ftb">
                                           <FTB:FreeTextBox id="txtDesc" runat="Server" BreakMode="LineBreak" FormatHtmlTagsToXhtml="true" PasteMode="Text" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>
                                            Short description
                                            <small class="text-navy clearfix">(only applicable for level 2 category)</small>
                                            <small class="text-navy clearfix">(it will be used in 'category in grid' template)</small>
                                            <img class="img-thumbnail" src="/img/category-short-description.png" style="width: 400px"/>
                                        </th>
                                        <td class="ftb">                                           
                                           <FTB:FreeTextBox id="txtShortDesc" runat="Server" BreakMode="LineBreak" FormatHtmlTagsToXhtml="true" PasteMode="Text" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Thumbnail<small class="text-navy clearfix">(size should be 160x200)</small><small class="text-navy clearfix">(accepted file types: *.jpg, *.jpeg)</small></th>
                                        <td>
                                            <img id="imgThumbnail" runat="server" visible="false" alt="" src="" /><br />
                                            <asp:FileUpload ID="fuThumbnail" runat="server" CssClass="form-control"/><asp:CheckBox ID="cbRemoveThumb" runat="server" Text="Remove" Visible="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Visible</th>
                                        <td><asp:CheckBox ID="cbVisible" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <th>URL key<small class="text-navy clearfix">(auto generated if leave blank)</small></th>
                                        <td><asp:TextBox ID="txtUrlKey" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </tr>
                                    <asp:PlaceHolder ID="phCategoryTemplate" runat="server">
                                    <tr>
                                        <th>Category template<small class="text-navy clearfix">(for 'Category with featured products',<br /> please make sure WhatsNew and TopRated items are generated.)</small></th>
                                        <td>
                                            <asp:DropDownList ID="ddlCategoryTemplate" runat="server" CssClass="form-control"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    </asp:PlaceHolder>
                                    <tr>
                                        <th>Colour scheme<small class="text-navy clearfix">(used to set colour, background, etc on menu)</small><small class="text-navy clearfix">(at the moment, it's only for parent category)</small></th>
                                        <td>
                                            <asp:DropDownList ID="ddlColourScheme" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                                <asp:ListItem Text="xmas" Value="xmas"></asp:ListItem>
                                                <asp:ListItem Text="motherday" Value="motherday"></asp:ListItem>
                                                <asp:ListItem Text="valentine" Value="valentine"></asp:ListItem>                            
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Parent</th>
                                        <td>
                                            <asp:HiddenField ID="hfParent" runat="server" />
                                            <asp:MultiView ID="mvParent" runat="server" ActiveViewIndex="0">
                                                <asp:View runat="server">
                                                    <asp:Literal ID="ltlParent" runat="server"></asp:Literal>&nbsp;<asp:LinkButton ID="lbEditParent" OnClick="lbEditParent_Click" runat="server" Text="Edit" CssClass="btn btn-outline btn-sm btn-warning"></asp:LinkButton></asp:View>
                                                <asp:View runat="server">
                                                    <Apollo:CategoryTree ID="ectParent" runat="server" OnTreeNodeSelected="ectParent_TreeNodeSelected" />
                                                </asp:View>
                                            </asp:MultiView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Priority</th>
                                        <td><asp:TextBox ID="txtPriority" runat="server" CssClass="form-control" MaxLength="4"></asp:TextBox></td>
                                    </tr>
                                </table>
                                <div class="hr-line-dashed"></div>
                                <div class="col-lg-12">
                                    <asp:LinkButton ID="lbSaveCategory" runat="server" Text="Save" OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to save this category?');" OnClick="lbSaveCategory_Click" ValidationGroup="vgSave" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                    <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClientClick="return confirm('This action will refresh all categories and products related data on store front and performance could be affected.\nAre you sure to publish?');" OnClick="lbPublish_Click" CssClass="btn btn-sm btn-danger"></asp:LinkButton>
                                    <asp:LinkButton ID="lbDeleteCategory" runat="server" Text="Delete" OnClick="lbDeleteCategory_Click" OnClientClick="return confirm('Are you sure to delete this category? Subcategory will be deleted and related products will be removed from this category and its subcategory.');" CssClass="btn btn-sm btn-warning"></asp:LinkButton>
                                    <asp:LinkButton ID="lbReset" runat="server" Text="Reset" OnClick="Reset" CssClass="btn btn-sm btn-success"></asp:LinkButton>
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
                                                    <asp:LinkButton ID="lbSaveProducts" runat="server" Text="Save" OnClientClick="return confirm('Are you sure to save?');" OnClick="lbSaveProducts_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbResetFilterProducts" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ID="gvProducts" ShowHeaderWhenEmpty="true" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" 
                                            OnPageIndexChanging="gvProducts_PageIndexChanging" AutoGenerateColumns="false" OnSorting="gvProducts_Sorting" 
                                            OnPreRender="gvProducts_PreRender" ShowHeader="true" OnRowDataBound="gvProducts_RowDataBound" DataKeyNames="Id" 
                                            CssClass="table table-striped table-bordered table-hover dataTable">
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>
                                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                    Page
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                                    <asp:ImageButton Visible='<%# (gvProducts.CustomPageCount > (gvProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                    of
                                                    <%= gvProducts.PageCount %>
                                                    pages |
                                                    <asp:PlaceHolder runat="server">Total
                                                        <%= gvProducts.RecordCount %>
                                                        records found</asp:PlaceHolder>
                                                    <asp:PlaceHolder runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                </asp:Panel>
                                                <div class="btnBox">
                                            
                                                </div>
                                            </PagerTemplate>
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-CssClass="header" ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>
                                                        <input type="checkbox" onclick="toggle_chosen(this);" />
                                                        <br />
                                                        <asp:DropDownList ID="ddlFilterChosen" runat="server" OnPreRender="ddlFilterChosen_PreRender" CssClass="form-control">
                                                            <asp:ListItem Text="Any" Value="any"></asp:ListItem>
                                                            <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                                                            <asp:ListItem Text="No" Value="no"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" Checked='<%# ProductHasThisCategory(Convert.ToInt32(Eval("Id")), GetIntState(CATEGORY_ID), GetIntState(CATEGORY_ID_FILTER)) %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" SortExpression="Id">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" SortExpression="Name">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Status">
                                                    <ItemTemplate><%# Convert.ToBoolean(Eval("Enabled")) ? "Online" : "Offline" %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Discontinued">
                                                    <ItemTemplate><%# Convert.ToBoolean(Eval("Discontinued")) ? "Yes" : "No" %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Assigned Category">
                                                    <ItemTemplate><%# GetAssignedCategory(Convert.ToInt32(Eval("Id"))) %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action" HeaderStyle-Width="60px">
                                                <ItemTemplate>
                                                    <a href="/catalog/product_info.aspx?productid=<%# Eval("Id") %>">Edit</a> | 
                                                    <a href="<%# AdminStoreUtility.GetProductUrl(Eval("UrlKey").ToString()) %>" target="_blank">View</a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            </Columns>
                                        </Apollo:CustomGrid>
                                    </div>
                                </div>
                            </div>
                        </div>                        
                        <div id="media" class="tab-pane">
                            <div class="panel-body">
                                <asp:Repeater ID="rptImages" runat="server" OnItemCommand="rptImages_ItemCommand">
                                    <HeaderTemplate>
                                        <div class="categoryMedia">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="imgBox">
                                            <asp:LinkButton CssClass="btnDelete" runat="server" CommandName="delete" CommandArgument='<%# Eval("Id") %>' OnClientClick="return confirm('Are you sure to delete this image?');" ToolTip="delete"><i class="fa fa-trash"></i></asp:LinkButton>                        
                                            <img src="/get_image_handler.aspx?type=category&img=<%# Eval("MediaFilename") %>" alt="<%# Eval("MediaFilename") %>" />
                                            <p>path: <%# "/media/category/" + Eval("MediaFilename").ToString() %></p>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </div>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <br />
                                <h4><asp:Literal ID="ltlImageTitle" runat="server"></asp:Literal></h4>
                                <table class="table table-striped">
                                    <tr>
                                        <th>Image file</th>
                                        <td><asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                                <div class="hr-line-dashed"></div>
                                <div class="col-lg-12">
                                    <asp:LinkButton ID="lbSaveImage" runat="server" OnClick="lbUploadImage_Click" Text="Upload" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <div id="featProds" class="tab-pane">
                            <div class="panel-body">
                                <h4>Auto Generate Items</h4>
                                <p>By selecting type and quantity below, items will be auto-populated for this category.</p>
                                <table class="table table-striped">
                                    <tr>
                                        <th>
                                            Type
                                            <small class="text-navy clearfix">(Position is used for menu)</small>
                                            <small class="text-navy clearfix">(WhatsNew &amp; TopRated are used for template 'Category with featured products', for parent category only)</small>
                                        </th>
                                        <td><asp:DropDownList ID="ddlFeaturedItemType" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <th>Quantity</th>
                                        <td>
                                            <asp:DropDownList ID="ddlFeaturedQuantity" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                                <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                                <div class="hr-line-dashed"></div>
                                <div class="col-lg-12">
                                    <asp:LinkButton ID="lbGenerateFeaturedItems" runat="server" Text="Generate" CssClass="btn btn-sm btn-primary" OnClick="lbGenerateFeaturedItems_Click"></asp:LinkButton>
                                </div>
                                
                                <div class="col-lg-12"><p><br /><br /></p></div>
                                
                                <div class="col-lg-6">
                                    <h4>Assigned Items - <small><asp:Literal ID="ltByFeaturedItemType" runat="server"></asp:Literal></small></h4>
                                    <div class="table-responsive">
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                                <div class="html5buttons">
                                                <div class="dt-buttons btn-group">                                                    
                                                    <asp:LinkButton ID="lbSearchFeaturedProducts" runat="server" Text="Load" OnClick="gvFeaturedProducts_lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    &nbsp;<span>Type <asp:DropDownList ID="ddlFeaturedItemTypeFilter" runat="server" CssClass="form-control"></asp:DropDownList></span>                                                    
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ShowHeaderWhenEmpty="true" OnRowCommand="gvFeaturedProducts_RowCommand" ID="gvFeaturedProducts" runat="server" 
                                            PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvFeaturedProducts_PageIndexChanging" 
                                            AutoGenerateColumns="false" OnSorting="gvFeaturedProducts_Sorting" OnPreRender="gvFeaturedProducts_PreRender" 
                                            ShowHeader="true" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>                        
                                                <asp:Panel runat="server" DefaultButton="btnGoPageFeaturedProducts">
                                                    Page
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPageFeaturedProducts" OnClick="gvFeaturedProducts_btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtFeaturedProductPageIndex" Width="25" Text='<%# gvFeaturedProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                                    <asp:ImageButton Visible='<%# (gvFeaturedProducts.CustomPageCount > (gvFeaturedProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                    of
                                                    <%= gvFeaturedProducts.PageCount %>
                                                    pages |
                                                    <asp:PlaceHolder runat="server">Total
                                                        <%= gvFeaturedProducts.RecordCount %>
                                                        records found</asp:PlaceHolder>
                                                    <asp:PlaceHolder runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                </asp:Panel>
                                            </PagerTemplate>
                                            <Columns>
                                                <asp:TemplateField HeaderStyle-CssClass="header" ItemStyle-HorizontalAlign="Center" HeaderText="Product ID"  HeaderStyle-Width="80px">
                                                    <ItemTemplate><%# Eval("Id") %></ItemTemplate>                        
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Name">                        
                                                    <ItemTemplate><%# Eval("Name") %><%# Convert.ToBoolean(Eval("Enabled")) ? null : " <i class='fa fa-eye-slash' title='hidden' aria-hidden='true'></i>" %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Priority"  HeaderStyle-Width="50px">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtFeaturedProductPriority" CssClass="form-control" Width="50" runat="server" Text='<%# FindFeaturedItemPriority(Convert.ToInt32(Eval("Id")), GetIntState(BasePage.CATEGORY_ID)) %>' Visible='<%# ProductIsFeaturedInCategory(Convert.ToInt32(Eval("Id")), GetIntState(BasePage.CATEGORY_ID)) %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" CommandName="savePriority" CommandArgument='<%# Eval("Id") %>' Text="Update"></asp:LinkButton> |
                                                        <asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# Eval("Id") %>' Text="Remove"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </Apollo:CustomGrid>
                                    </div>
                                </div>

                                <div class="col-lg-6">
                                    <h4>Search &amp; Assign</h4>
                                    <div class="table-responsive">
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                                <div class="html5buttons">
                                                <div class="dt-buttons btn-group">                                                    
                                                    <asp:LinkButton ID="lbSearchNewFeaturedProducts" OnClick="lbSearchNewFeaturedProducts_Click" runat="server" Text="Search" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>                                                    
                                                    <asp:LinkButton ID="lbResetNewFeaturedProducts" OnClick="lbResetNewFeaturedProducts_Click" runat="server" Text="Reset" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ShowHeaderWhenEmpty="true" ID="gvNewFeaturedProducts" runat="server" OnRowCommand="gvNewFeaturedProducts_RowCommand"
                                            PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvNewFeaturedProducts_PageIndexChanging" 
                                            AutoGenerateColumns="false" OnSorting="gvNewFeaturedProducts_Sorting" OnPreRender="gvNewFeaturedProducts_PreRender" 
                                            ShowHeader="true" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>                        
                                                <asp:Panel runat="server" DefaultButton="btnGoPageNewFeaturedProducts">
                                                    Page
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPageNewFeaturedProducts" OnClick="gvNewFeaturedProducts_btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtNewFeaturedProductPageIndex" Width="25" Text='<%# gvNewFeaturedProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                                    <asp:ImageButton Visible='<%# (gvNewFeaturedProducts.CustomPageCount > (gvNewFeaturedProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                    of
                                                    <%= gvNewFeaturedProducts.PageCount %>
                                                    pages |
                                                    <asp:PlaceHolder runat="server">Total <%= gvNewFeaturedProducts.RecordCount %> records found</asp:PlaceHolder>
                                                    <asp:PlaceHolder runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                </asp:Panel>
                                            </PagerTemplate>
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Product ID">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product ID</asp:LinkButton>                                                        
                                                        <asp:TextBox ID="txtNewFeaturedProductId" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Name">                        
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton>
                                                        <asp:TextBox ID="txtNewFeaturedProductName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Name") %><%# Convert.ToBoolean(Eval("Enabled")) ? null : " <i class='fa fa-eye-slash' title='hidden' aria-hidden='true'></i>" %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" CommandName="assign" CommandArgument='<%# Eval("Id") %>' Text="Assign"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </Apollo:CustomGrid>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="featBrands" class="tab-pane">
                            <div class="panel-body">
                                <p>Featured brands are to be displayed on store front's main menu dropdown.</p>
                                <div class="ibox float-e-margins">
                                    <div class="ibox-content">
                                        <div class="table-responsive">
                                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                                    <div class="html5buttons">
                                                    <div class="dt-buttons btn-group">
                                                        <asp:LinkButton ID="lbSaveProductsFeaturedBrands" runat="server" Text="Save" OnClientClick="return confirm('Are you sure to save?');" OnClick="gvFeaturedBrands_lbSaveProducts_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                        <asp:LinkButton ID="lbSearchFeaturedBrands" runat="server" Text="Search" OnClick="gvFeaturedBrands_lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                        <asp:LinkButton ID="lbResetFilterBrands" runat="server" Text="Reset" OnClick="gvFeaturedBrands_lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                            <Apollo:CustomGrid ShowHeaderWhenEmpty="true" OnRowCommand="gvFeaturedBrands_RowCommand" ID="gvFeaturedBrands" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvFeaturedBrands_PageIndexChanging" AutoGenerateColumns="false" OnSorting="gvFeaturedBrands_Sorting" OnPreRender="gvFeaturedBrands_PreRender" ShowHeader="true" OnRowDataBound="gvFeaturedBrands_RowDataBound" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">
                                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                                <PagerTemplate>
                                                    <asp:Panel runat="server" DefaultButton="btnGoPageFeaturedBrands">
                                                        Page
                                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                        <asp:Button Width="0" runat="server" ID="btnGoPageFeaturedBrands" OnClick="gvFeaturedBrands_btnGoPage_Click" CssClass="hidden" />
                                                        <asp:TextBox ID="txtFeaturedBrandPageIndex" Width="25" Text='<%# gvFeaturedBrands.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                                        <asp:ImageButton Visible='<%# (gvFeaturedBrands.CustomPageCount > (gvFeaturedBrands.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                        of
                                                        <%= gvFeaturedBrands.PageCount %>
                                                        pages |
                                                        <asp:PlaceHolder runat="server">Total
                                                            <%= gvFeaturedBrands.RecordCount %>
                                                            records found</asp:PlaceHolder>
                                                        <asp:PlaceHolder runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                    </asp:Panel>
                                                </PagerTemplate>
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="75px">
                                                        <HeaderTemplate>
                                                            <input type="checkbox" onclick="toggle_chosen(this);" />
                                                            <br />
                                                            <asp:DropDownList ID="ddlFeaturedBrandFilterChosen" runat="server" CssClass="form-control" OnPreRender="gvFeaturedBrands_ddlFilterChosen_PreRender">
                                                                <asp:ListItem Text="Any" Value="any"></asp:ListItem>
                                                                <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                                                                <asp:ListItem Text="No" Value="no"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox runat="server" ID="cbFeaturedBrandChosen" CssClass="chosen" Checked='<%# ((IList<int>)Eval("AssignedCategoryIdForFeaturedBrand")).Contains(GetIntState(CATEGORY_ID)) %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" ItemStyle-HorizontalAlign="Center" SortExpression="Id" HeaderStyle-Width="100px">
                                                        <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                        <HeaderTemplate>
                                                            <asp:LinkButton CommandArgument="BrandId" runat="server" CommandName="Sort">Brand ID</asp:LinkButton><br />
                                                            <asp:TextBox ID="txtFeaturedBrandFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                                        </HeaderTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Name" SortExpression="Name">
                                                        <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                                        <HeaderTemplate>
                                                            <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                            <asp:TextBox ID="txtFeaturedBrandFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                                        </HeaderTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Priority" HeaderStyle-Width="50px">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtFeaturedBrandPriority" CssClass="form-control" runat="server" Text='<%# FindFeaturedBrandPriority(Convert.ToInt32(Eval("Id")), GetIntState(CATEGORY_ID)) %>' Visible='<%# ((IList<int>)Eval("AssignedCategoryIdForFeaturedBrand")).Contains(GetIntState(CATEGORY_ID)) %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Action" HeaderStyle-Width="100px">
                                                        <ItemTemplate>
                                                            <asp:LinkButton runat="server" CommandName="savePriority" CommandArgument='<%# Eval("Id") %>' Visible='<%# ((IList<int>)Eval("AssignedCategoryIdForFeaturedBrand")).Contains(GetIntState(CATEGORY_ID)) %>'>Update priority</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </Apollo:CustomGrid>
                                        </div>
                                    </div>
                                </div>
                            </div>                        
                        </div>
                        <div id="meta" class="tab-pane">
                            <div class="panel-body">
                                <div class="row col-lg-6">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            Metadata
                                        </div>
                                        <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgSave" CssClass="alert alert-warning" />
                                        <table class="table table-striped">
                                            <tr>
                                                <th>
                                                    Meta title
                                                    <small class="text-navy clearfix">(60 characters maximum)</small>
                                                    <small class="text-navy clearfix">(used in Open Graph tag og:title)</small>
                                                    <small class="text-navy clearfix">(used in Twitter Card tag twitter:title)</small>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtMetaTitle" runat="server" CssClass="form-control" TextMode="MultiLine" Height="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMetaTitle" ValidationExpression="^(?=.+).{0,60}$" Display="Dynamic" ErrorMessage="Meta title has too many characters." 
                                                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Meta title has too many characters.</span>" ValidationGroup="metadata"></asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Meta description
                                                    <small class="text-navy clearfix">(160 characters maximum)</small>
                                                    <small class="text-navy clearfix">(used in Open Graph tag og:description)</small>
                                                    <small class="text-navy clearfix">(used in Twitter Card tag twitter:description)</small>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtMetaDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Height="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMetaDescription" ValidationExpression="^(?=.+).{0,160}$" Display="Dynamic" ErrorMessage="Meta description has too many characters." 
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Meta description has too many characters.</span>" ValidationGroup="metadata"></asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Meta keywords
                                                    <small class="text-navy clearfix">(160 characters maximum)</small>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtMetaKeywords" runat="server" CssClass="form-control" TextMode="MultiLine" Height="100px"></asp:TextBox>
                                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMetaKeywords" ValidationExpression="^(?=.+).{0,160}$" Display="Dynamic" ErrorMessage="Meta keywords has too many characters." 
                                                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Meta keywords has too many characters.</span>" ValidationGroup="metadata"></asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Secondary keywords
                                                    <small class="text-navy clearfix">(used in search analysis)</small>
                                                </th>
                                                <td><asp:TextBox ID="txtSecondaryKeywords" runat="server" CssClass="form-control" TextMode="MultiLine" Height="100px"></asp:TextBox></td>
                                            </tr>
                                        </table>
                                        <div class="hr-line-dashed"></div>
                                        <div class="col-lg-12">
                                            <asp:LinkButton ID="lbAddMeta" runat="server" Text="Update" CssClass="btn btn-sm btn-primary" OnClick="lbAddMeta_Click"></asp:LinkButton>
                                        </div>
                                        <div class="col-lg-12"><p></p></div>
                                        <div class="clearfix"></div>
                                    </div>
                                </div>
                                <div class="col-lg-12"><p></p></div>
                            </div>
                        </div>
                        <div id="filters" class="tab-pane">
                            <div class="panel-body">
                                <asp:PlaceHolder ID="phNewCategoryFilter" runat="server">
                                    <h4>New Category Filter</h4>
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Type</th>
                                            <td><asp:TextBox ID="txtCategoryFilterType" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbSaveCategoryFilter" Text="Save" runat="server" CssClass="btn btn-sm btn-primary" OnClick="lbSaveCategoryFilter_Click"></asp:LinkButton>
                                    </div>
                                    <div class="col-lg-12"><p></p></div>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="phEditCategoryFilter" runat="server" Visible="false">
                                    <h4><asp:Literal ID="ltEditCategoryFilterTitle" runat="server"></asp:Literal></h4>
                                    <asp:HiddenField ID="hfCategoryFilterId" runat="server" />
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Type</th>
                                            <td><asp:TextBox ID="txtEditCategoryFilterType" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbUpdateCategoryFilter" Text="Update" runat="server" CssClass="btn btn-sm btn-primary" OnClick="lbUpdateCategoryFilter_Click"></asp:LinkButton>
                                        <asp:LinkButton ID="lbDeleteCategoryFilter" Text="Delete" runat="server" CssClass="btn btn-sm btn-primary" OnClick="lbDeleteCategoryFilter_Click"></asp:LinkButton>
                                    </div>
                                </asp:PlaceHolder>

                                <asp:PlaceHolder ID="phCategoryFilterProducts" runat="server">
                                    <h4>Category Filters</h4>
                                    <asp:Repeater ID="rptFilters" runat="server" OnItemCommand="rptFilters_ItemCommand">
                                        <HeaderTemplate>
                                            <table class="table table-striped">
                                                <tr>
                                                    <th>Category Filter ID</th>
                                                    <th>Type</th>
                                                    <th>Action</th>
                                                </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("Id") %></td>
                                                    <td><%# Eval("Type") %></td>
                                                    <td>
                                                        <asp:LinkButton ID="lbEdit" runat="server" CommandArgument='<%# Eval("Id") %>' CommandName="edit" Text="Edit"></asp:LinkButton> | 
                                                        <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" CommandArgument='<%# Eval("Id") %>' CommandName="remove"></asp:LinkButton> |
                                                        <asp:LinkButton ID="lbList" runat="server" Text="List" CommandArgument='<%# Eval("Id") %>' CommandName="list"></asp:LinkButton> 
                                                    </td>
                                                </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>

                                    <asp:PlaceHolder ID="phFilterProducts" runat="server" Visible="false">
                                        <div class="col-lg-4">
                                            <h4>Products <asp:Literal ID="ltProductFilterTitle" runat="server"></asp:Literal></h4>
                                            <asp:HiddenField ID="hfCategoryFilterIdForProducts" runat="server" />
                                            <Apollo:CustomGrid ShowHeaderWhenEmpty="true" OnRowCommand="gvFiltersProducts_RowCommand" ID="gvFiltersProducts" runat="server" 
                                                PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvFiltersProducts_PageIndexChanging" 
                                                AutoGenerateColumns="false" OnSorting="gvFiltersProducts_Sorting" OnPreRender="gvFiltersProducts_PreRender" 
                                                ShowHeader="true" DataKeyNames="Id" CssClass="grid">
                                                <PagerSettings Visible="true" Position="TopAndBottom" Mode="NextPreviousFirstLast" />
                                                <PagerTemplate>
                                                    <asp:Panel runat="server" DefaultButton="btnGoPageFiltersProducts">
                                                        Page
                                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                        <asp:Button Width="0" runat="server" ID="btnGoPageFiltersProducts" OnClick="gvFiltersProducts_btnGoPage_Click" CssClass="hiddenBtn" />
                                                        <asp:TextBox ID="txtFiltersProductPageIndex" Width="25" Text='<%# gvFiltersProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                                        <asp:ImageButton Visible='<%# (gvFiltersProducts.CustomPageCount > (gvFiltersProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                        of
                                                        <%= gvFiltersProducts.PageCount %>
                                                        pages |
                                                        <asp:PlaceHolder runat="server">Total
                                                            <%= gvFiltersProducts.RecordCount%>
                                                            records found</asp:PlaceHolder>
                                                        <asp:PlaceHolder runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                    </asp:Panel>
                                                    <div class="btnBox">
                                                        <asp:LinkButton runat="server" Text="Search" OnClick="gvFiltersProducts_lbSearch_Click" CssClass="ABtn"></asp:LinkButton>
                                                        <asp:LinkButton runat="server" Text="Reset" OnClick="gvFiltersProducts_lbResetFilter_Click" CssClass="GBtn"></asp:LinkButton>
                                                    </div>
                                                </PagerTemplate>
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" ItemStyle-HorizontalAlign="Center" SortExpression="ProductId"  HeaderStyle-Width="100px">
                                                        <HeaderTemplate>
                                                            <asp:LinkButton CommandArgument="ProductId" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                            <asp:TextBox ID="txtFiltersProductFilterId" runat="server" Width="100px"></asp:TextBox>
                                                        </HeaderTemplate>
                                                        <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" SortExpression="Name">
                                                        <HeaderTemplate>
                                                            <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                            <asp:TextBox ID="txtFiltersProductFilterName" runat="server"></asp:TextBox>
                                                        </HeaderTemplate>
                                                        <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Action" HeaderStyle-Width="100px">
                                                        <ItemTemplate>
                                                            <asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# Eval("Id") %>'>Delete</asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </Apollo:CustomGrid>
                                        </div>
                                        <div class="col-lg-6">
                                            <h4>Assign Product To This Category Filter <asp:Literal ID="ltAssignCatagoryFilterTitle" runat="server"></asp:Literal></h4>
                                            <Apollo:CustomGrid ID="gvNotFilterProducts" ShowHeaderWhenEmpty="true" runat="server" PageSize="10" 
                                                AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvNotFilterProducts_PageIndexChanging" 
                                                AutoGenerateColumns="false" OnSorting="gvNotFilterProducts_Sorting" OnPreRender="gvNotFilterProducts_PreRender" 
                                                ShowHeader="true" DataKeyNames="Id" CssClass="grid" OnRowCommand="gvNotFilterProducts_RowCommand">
                                                <PagerSettings Visible="true" Position="TopAndBottom" Mode="NextPreviousFirstLast" />
                                                <PagerTemplate>
                                                    <asp:Panel runat="server" DefaultButton="gvNotFilterProducts_btnGoPage">
                                                        Page
                                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                        <asp:Button Width="0" runat="server" ID="gvNotFilterProducts_btnGoPage" OnClick="gvNotFilterProducts_btnGoPage_Click" CssClass="hiddenBtn" />
                                                        <asp:TextBox ID="gvNotFilterProducts_txtPageIndex" Width="25" Text='<%# gvNotFilterProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                                        <asp:ImageButton Visible='<%# (gvNotFilterProducts.CustomPageCount > (gvNotFilterProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                        of
                                                        <%= gvNotFilterProducts.PageCount %>
                                                        pages |
                                                        <asp:PlaceHolder runat="server">Total
                                                            <%= gvNotFilterProducts.RecordCount %>
                                                            records found</asp:PlaceHolder>
                                                        <asp:PlaceHolder runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                    </asp:Panel>
                                                    <div class="btnBox">
                                                        <asp:LinkButton runat="server" Text="Search" OnClick="gvNotFilterProducts_lbSearch_Click" CssClass="ABtn"></asp:LinkButton>
                                                        <asp:LinkButton runat="server" Text="Reset" OnClick="gvNotFilterProducts_lbResetFilter_Click" CssClass="GBtn"></asp:LinkButton>
                                                    </div>
                                                </PagerTemplate>
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" ItemStyle-HorizontalAlign="Center" SortExpression="Id">                        
                                                        <HeaderTemplate>
                                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                            <asp:TextBox ID="txtNotFilterProductId" runat="server"></asp:TextBox>
                                                        </HeaderTemplate>
                                                        <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-CssClass="header" SortExpression="Name">
                                                        <HeaderTemplate>
                                                            <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                            <asp:TextBox ID="txtNotFilterName" runat="server"></asp:TextBox>
                                                        </HeaderTemplate>
                                                        <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderStyle-CssClass="header">
                                                        <HeaderTemplate>
                                                            Action
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lbAssign" runat="server" CommandArgument='<%# Eval("Id") %>' CommandName="assign" Text="Assign"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </Apollo:CustomGrid>
                                        </div>
                                    </asp:PlaceHolder>
                
                                </asp:PlaceHolder>
                            </div>
                        </div>
                        <div id="whatsnew" class="tab-pane">
                            <div class="panel-body">
                                <p>Only the first active item will be chosen to display on website.</p>
                                <asp:PlaceHolder ID="phEditWhatsNew" runat="server">
                                    <h4><asp:Literal ID="ltWhatsNewTitle" runat="server" Text="New Item"></asp:Literal></h4>
                                    <asp:HiddenField ID="hfWhatsNewId" runat="server" />
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Content</th>
                                            <td><asp:TextBox ID="txtWhatsNewContent" TextMode="MultiLine" CssClass="form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>From date <small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox ID="txtWhatsNewDateFrom" runat="server" CssClass="date form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>To date <small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox ID="txtWhatsNewDateTo" runat="server" CssClass="date form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Enabled</th>
                                            <td><asp:CheckBox ID="cbWhatsNewEnabled" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Priority</th>
                                            <td><asp:TextBox ID="txtWhatsNewPriority" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbSaveWhatsNew" Text="Create" OnClick="lbSaveWhatsNew_Click" runat="server" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                        <asp:LinkButton ID="lbUpdateWhatsNew" Text="Update" OnClick="lbUpdateWhatsNew_Click" Visible="false" runat="server" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                    </div>
                                    <div class="col-lg-12"><p></p></div>
                                </asp:PlaceHolder>
                                <h4>What's New Items</h4>
                                <asp:Repeater ID="rptWhatsNewItems" runat="server" OnItemCommand="rptWhatsNewItems_ItemCommand">
                                    <HeaderTemplate>
                                        <table class="table table-striped">
                                            <tr>
                                                <th>ID</th>
                                                <th>Content</th>
                                                <th>Start</th>
                                                <th>End</th>
                                                <th>Status</th>
                                                <th>Priority</th>
                                                <th>Action</th>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                            <tr>
                                                <td><%# Eval("Id") %></td>
                                                <td><%# Server.HtmlEncode(Eval("HtmlContent").ToString()) %></td>
                                                <td><%# Eval("StartDate") %></td>
                                                <td><%# Eval("EndDate") %></td>
                                                <td><i class="fa fa-eye<%# (bool)Eval("Enabled") ? null : "-slash" %>" aria-hidden="true"></i></td>
                                                <td><%# Eval("Priority") %></td>
                                                <td><asp:LinkButton runat="server" Text="Delete" CommandArgument='<%# Eval("Id") %>' CommandName="remove"></asp:LinkButton> | <asp:LinkButton runat="server" Text="Edit" CommandArgument='<%# Eval("Id") %>' CommandName="edit"></asp:LinkButton></td>
                                            </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                        <div id="banners" class="tab-pane">
                            <div class="panel-body">
                                <Apollo:CustomGrid ID="gvBanners" runat="server" AllowPaging="false" AllowSorting="false" AutoGenerateColumns="false" ShowHeader="true" 
                                    OnRowCommand="gvBanners_RowCommand"
                                    CssClass="table table-striped table-bordered table-hover dataTable">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <Columns>            
                                    <asp:TemplateField HeaderText="LargeBannerId" HeaderStyle-Width="120">
                                        <HeaderTemplate>Large Banner ID</HeaderTemplate>
                                        <ItemTemplate><%# Eval("LargeBanner.Id")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Title">
                                        <HeaderTemplate>
                                            Title
                                        </HeaderTemplate>
                                        <ItemTemplate><a href="#" onmouseover="Tip('<img src=/get_image_handler.aspx?type=largebanner&id=<%# Eval("LargeBanner.Id") %> />', WIDTH, 1170)" onmouseout="UnTip()"><%# Eval("LargeBanner.Title")%></a></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date Start" HeaderStyle-Width="120">
                                        <HeaderTemplate>
                                            From Date
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Convert.ToDateTime(Eval("LargeBanner.StartDate")) == DateTime.MinValue ? string.Empty : Eval("LargeBanner.StartDate") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date Expire" SortExpression="EndDate" HeaderStyle-Width="120">
                                        <HeaderTemplate>
                                            To Date
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Convert.ToDateTime(Eval("LargeBanner.EndDate")) == DateTime.MinValue ? string.Empty : Eval("LargeBanner.EndDate")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" HeaderStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Status
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <%# Eval("LargeBanner.Enabled").ToString()%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Display Order" HeaderStyle-Width="80">
                                        <HeaderTemplate>
                                            Display Order
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtBannerDisplayOrder" CssClass="form-control" Width="50" runat="server" Text='<%# Eval("DisplayOrder")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Display On Homepage" HeaderStyle-Width="80">
                                        <HeaderTemplate>
                                            Display On Homepage
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("LargeBanner.DisplayOnHomePage")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="160">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# Eval("LargeBanner.Id") %>' Text="Delete"></asp:LinkButton> |
                                            <asp:LinkButton runat="server" CommandName="save" CommandArgument='<%# Eval("Id") %>' Text="Update"></asp:LinkButton> |
                                            <a target="_blank" href="/marketing/cms_largebanner_info.aspx?id=<%# Eval("LargeBanner.Id") %>">Edit <i class="fa fa-external-link" aria-hidden="true"></i></a>
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
    </div>
</asp:Content>
