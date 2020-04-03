<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="True" CodeBehind="brand_default.aspx.cs" ValidateRequest="false" Inherits="Apollo.AdminStore.WebForm.Catalog.brand_default" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
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
        });
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <script type="text/javascript" src="/js/wz_tooltip.js"></script>
    <asp:HiddenField ID="hfCurrentPanel" runat="server" />
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Brand</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-4">
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                <div class="html5buttons">
                                    <asp:CheckBox ID="cbHideDisabled" runat="server" Text="Hide Enabled?" CssClass="input-group" />
                                    <div class="dt-buttons btn-group">                                        
                                        <asp:LinkButton ID="lbResetBrandFilter" runat="server" Text="Reset" OnClick="lbResetBrandFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <a href="/catalog/brand_default.aspx" class="btn btn-default buttons-copy buttons-html5">Create</a>
                                        <asp:LinkButton ID="lbSearchBrand" runat="server" Text="Search" OnClick="lbSearchBrand_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>                                        
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvBrands" runat="server" PageSize="20" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvBrands_PageIndexChanging" 
                                OnInit="gvBrands_Init" ShowHeaderWhenEmpty="true"
                                AutoGenerateColumns="false" OnSorting="gvBrands_Sorting" OnPreRender="gvBrands_PreRender" ShowHeader="true"
                                OnRowCommand="gvBrands_RowCommand" CssClass="table table-striped">                    
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>                
                                    <asp:Panel runat="server" DefaultButton="btnBrandGoPage">
                                        Page 
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" runat="server" ID="btnBrandGoPage" OnClick="btnBrandGoPage_Click" CssClass="hidden" />
                                        <asp:TextBox ID="txtPageIndexBrand" Width="25" Text='<%# gvBrands.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                        <asp:ImageButton Visible='<%# (gvBrands.CustomPageCount > (gvBrands.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                        of <%= gvBrands.PageCount %> pages | <asp:PlaceHolder runat="server">Total <%= gvBrands.RecordCount %> records found</asp:PlaceHolder>
                                                                             <asp:PlaceHolder runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>
                                </PagerTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderText="Brand ID" SortExpression="BrandId" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate><%# Eval("Id") %></ItemTemplate>                                                        
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Brand ID</asp:LinkButton><br />
                                            <asp:TextBox ID="txtBrandFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" SortExpression="Name">
                                        <ItemTemplate><%# Eval("Name")%><%# Convert.ToBoolean(Eval("Enabled")) ? null : " <i class='fa fa-eye-slash' title='hidden' aria-hidden='true'></i>" %></ItemTemplate>                                                        
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                            <asp:TextBox ID="txtBrandFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                    </asp:TemplateField>                        
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="150px">
                                        <ItemTemplate>
                                            <asp:LinkButton CommandName="EditInfo" CommandArgument='<%# Eval("Id") %>' runat="server" Text="Edit"></asp:LinkButton> | <a href='brand_category_info.aspx?brandid=<%# Eval("Id") %>'>View Categories</a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>                    
                            </Apollo:CustomGrid>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-8">
                <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgSave" CssClass="valSummary alert alert-warning" />
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgMeta" CssClass="valSummary alert alert-warning" />
                <div class="tabs-container">
                    <ul class="nav nav-tabs">
                        <li class="active"><a href="#general" data-toggle="tab">General information</a></li>
                        <asp:PlaceHolder runat="server" ID="phTabs">
                            <li><a href="#products" data-toggle="tab">Products</a></li>
                            <li><a href="#media" data-toggle="tab">Media</a></li>
                            <li><a href="#meta" data-toggle="tab">Metadata</a></li>
                            <li><a href="#featured" data-toggle="tab">Featured products</a></li>
                        </asp:PlaceHolder>
                    </ul>
                    <div class="tab-content">
                        <div id="general" class="tab-pane active">
                            <div class="panel-body">
                                <table class="table table-striped">
                                    <tr>
                                        <th>Name<strong>*</strong></th>
                                        <td>
                                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator ControlToValidate="txtName" runat="server" Display="Dynamic" ValidationGroup="vgSave" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>"
                                                ErrorMessage="Name is required."></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>URL key <small class="text-navy clearfix">(auto generated if left blank)</small></th>
                                        <td><asp:TextBox ID="txtUrlKey" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <th>Logo<small class="text-navy clearfix">(image URL 140x50px)</small></th>
                                        <td>
                                            <img id="imgLogo" runat="server" visible="false" src="" alt="" /><br />
                                            <asp:FileUpload ID="fuLogo" runat="server" CssClass="form-control" /><asp:CheckBox ID="cbRemoveLogo" runat="server" Text="Remove" Visible="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Default delivery</th>
                                        <td><asp:DropDownList ID="ddlDelivery" runat="server" OnInit="ddlDelivery_Init" CssClass="form-control"></asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <th>Enforce stock count? <small class="text-navy clearfix">(shows out of stock if stock reaches zero)</small></th>
                                        <td><asp:CheckBox ID="cbEnforceStockCount" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <th>Enabled? <small class="text-navy clearfix">(to be displayed in shop by brand list)</small></th>
                                        <td><asp:CheckBox ID="cbEnabled" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <th>Has microsite? <small class="text-navy clearfix">(requires brand categories)</small></th>
                                        <td><asp:CheckBox ID="cbMicrosite" runat="server" /></td>
                                    </tr>                
                                    <tr>
                                        <th>Microsite description <small class="text-navy clearfix">(introductory text/html)</small></th>
                                        <td class="ftb">
                                            <%--<asp:TextBox ID="txtDesc" runat="server" Width="300" Rows="8" TextMode="MultiLine" />--%>
                                            <FTB:FreeTextBox id="txtDesc" runat="Server" BreakMode="LineBreak" FormatHtmlTagsToXhtml="true" PasteMode="Text" Height="300" />
                                        </td>
                                    </tr>                                
                                </table>

                                <div class="hr-line-dashed"></div>
                                <div class="col-lg-12">
                                    <asp:LinkButton ID="lbSaveBrand" runat="server" Text="Save" OnClick="lbSaveBrand_Click" ValidationGroup="vgSave" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                    <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClick="lbPublish_Click" OnClientClick="return confirm('This action will refresh all brands and products related data on store front and performance could be affected.\nAre you sure to publish?');" CssClass="btn btn-sm btn-success"></asp:LinkButton>
                                    <asp:LinkButton ID="lbReset" runat="server" Text="Reset" OnClick="lbReset_Click" CssClass="btn btn-sm btn-danger"></asp:LinkButton>                            
                                    <asp:LinkButton ID="lbDeleteBrand" runat="server" Text="Delete" OnClick="lbDeleteBrand_Click" OnClientClick="javascript:return confirm('Are you sure to delete this brand? Subcategory will be deleted and related products will be removed from this brand and its subcategory.');" CssClass="btn btn-sm btn-warning"></asp:LinkButton>
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
                                                    <asp:LinkButton ID="lbSearchProduct" runat="server" Text="Search" OnClick="lbSearchProduct_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbResetProductFilter" runat="server" Text="Reset" OnClick="lbResetProductFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ID="gvProducts" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvProducts_PageIndexChanging" 
                                            AutoGenerateColumns="false" OnSorting="gvProducts_Sorting" OnPreRender="gvProducts_PreRender" ShowHeader="true" OnRowDataBound="gvProducts_RowDataBound" 
                                            CssClass="table table-striped table-bordered table-hover dataTable" DataKeyNames="Id">                    
                                        <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                        <PagerTemplate>                                
                                            <asp:Panel runat="server" DefaultButton="btnProductGoPage">
                                                Page 
                                                <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                <asp:Button Width="0" runat="server" ID="btnProductGoPage" OnClick="btnProductGoPage_Click" CssClass="hidden" />
                                                <asp:TextBox ID="txtPageIndexProduct" Width="25" Text='<%# gvProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                <asp:ImageButton Visible='<%# (gvProducts.CustomPageCount > (gvProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                of <%= gvProducts.PageCount %> pages | <asp:PlaceHolder runat="server">Total <%= gvProducts.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                            </asp:Panel>
                                        </PagerTemplate>
                                        <Columns>
                                            <asp:TemplateField>
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
                                                    <asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" Checked='<%# ((Convert.ToInt32(Eval("BrandId")) == GetIntState(BasePage.BRAND_ID)) && (Convert.ToInt32(Eval("BrandId")) != AppConstant.DEFAULT_BRAND)) ? true : false %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Product ID" SortExpression="Id">                    
                                                <HeaderTemplate>
                                                    <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                    <asp:TextBox ID="txtProductFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                                </HeaderTemplate>
                                                <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name" SortExpression="Name">                    
                                                <HeaderTemplate>                                
                                                    <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                    <asp:TextBox ID="txtProductFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                                </HeaderTemplate>
                                                <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                            </asp:TemplateField>                        
                                        </Columns>
                                    </Apollo:CustomGrid>
                                    </div>
                                </div>
                            </div>                            
                        </div>        
                        <div id="media" class="tab-pane">
                            <div class="panel-body">
                                <Apollo:CustomGrid ID="gvBanners" runat="server" AllowPaging="false" AllowSorting="false" AutoGenerateColumns="false" ShowHeader="true" 
                                    OnRowCommand="gvBanners_RowCommand" CssClass="table table-striped table-bordered table-hover dataTable">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                    <Columns>
                                        <asp:TemplateField HeaderText="Brand Banner Id" HeaderStyle-Width="120">
                                            <HeaderTemplate>Brand Banner ID</HeaderTemplate>
                                            <ItemTemplate><a href="#" onmouseover="Tip('<img src=/get_image_handler.aspx?type=brand&img=<%# Eval("MediaFilename") %> />', WIDTH, 980)" onmouseout="UnTip()"><%# Eval("Id")%></a></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Title">
                                            <HeaderTemplate>Title</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtBannerTitle" runat="server" CssClass="form-control" Text='<%# Eval("Title") %>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date Start" HeaderStyle-Width="120">
                                            <HeaderTemplate>From Date</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtBannerStartDate" runat="server" CssClass="form-control date" Text='<%# Eval("StartDate") %>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date Expire" SortExpression="EndDate" HeaderStyle-Width="120">
                                            <HeaderTemplate>To Date</HeaderTemplate>
                                            <ItemTemplate>                                                
                                                <asp:TextBox ID="txtBannerEndDate" runat="server" CssClass="form-control date" Text='<%# Eval("EndDate") %>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status" HeaderStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>Status</HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbBannerEnabled" runat="server" Checked='<%# Convert.ToBoolean(Eval("Enabled").ToString()) %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Display Order" HeaderStyle-Width="80">
                                            <HeaderTemplate>Display Order</HeaderTemplate>
                                            <ItemTemplate><asp:TextBox ID="txtBannerDisplayOrder" CssClass="form-control" Width="50" runat="server" Text='<%# Eval("Priority")%>'></asp:TextBox></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action" HeaderStyle-Width="160">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" CommandName="remove" CommandArgument='<%# Eval("Id") %>' Text="Delete"></asp:LinkButton> |
                                                <asp:LinkButton runat="server" CommandName="save" CommandArgument='<%# Eval("Id") %>' Text="Update"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </Apollo:CustomGrid>

                                <div class="col-lg-12"><p></p></div>
                                <h4><asp:Literal ID="ltlImageTitle" runat="server"></asp:Literal></h4>
                                <table class="table table-striped">
                                    <tr>
                                        <th>Image file</th>
                                        <td><asp:FileUpload ID="fuImage" runat="server" /></td>
                                    </tr>
                                </table>
                                <div class="hr-line-dashed"></div>
                                <div class="col-lg-12">
                                    <asp:LinkButton ID="lbSaveImage" runat="server" OnClick="lbUploadImage_Click" Text="Upload" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <div id="meta" class="tab-pane">
                            <div class="panel-body">
                                <table class="table table-striped">
                                     <tr>
                                        <th>Meta description</th>
                                        <td><asp:TextBox ID="txtMetaDesc" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <th>Meta title</th>
                                        <td><asp:TextBox ID="txtMetaTitle" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <th>Meta keywords</th>
                                        <td><asp:TextBox ID="txtMetaKeywords" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <th>Seconday keywords</th>
                                        <td><asp:TextBox ID="txtSecondaryKeywords" runat="server" CssClass="form-control"></asp:TextBox></td>
                                    </tr>                                    
                                 </table>
                                <div class="hr-line-dashed"></div>
                                <div class="col-lg-12">
                                    <asp:LinkButton ID="lbSaveMetaBrand" runat="server" Text="Save" ValidationGroup="vgMeta" CssClass="btn btn-sm btn-primary" OnClick="lbSaveMetaBrand_Click"></asp:LinkButton>
                                </div>
                            </div>
                         </div>
                        <div id="featured" class="tab-pane">
                            <div class="panel-body">
                                <h4>Auto Generate Items</h4>
                                <p>By selecting type and quantity below, items will be auto-populated for this brand.</p>
                                <table class="table table-striped">
                                    <tr>
                                        <th>Type</th>
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
                                                        <asp:TextBox ID="txtFeaturedProductPriority" CssClass="form-control" Width="50" runat="server" Text='<%# FindFeaturedItemPriority(Convert.ToInt32(Eval("Id")), GetIntState(BasePage.BRAND_ID)) %>' Visible='<%# ProductIsFeaturedInBrand(Convert.ToInt32(Eval("Id")), GetIntState(BasePage.BRAND_ID)) %>'></asp:TextBox>
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
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
