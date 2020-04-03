<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.product_info" ValidateRequest="false" Codebehind="product_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CategoryTree" Src="~/UserControls/CategoryTreeControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="GoogleTaxonomy" Src="~/UserControls/GoogleTaxonomyControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <%--<link href="/css/inspinia/plugins/summernote/summernote.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/summernote/summernote-bs3.css" rel="stylesheet">--%>
    <link href="/css/inspinia/plugins/icheck/custom.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/product.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">

    <%--<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/normalize/4.1.1/normalize.min.css" />--%>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ion-rangeslider/2.1.4/css/ion.rangeSlider.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/ion-rangeslider/2.1.4/css/ion.rangeSlider.skinFlat.min.css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/ion-rangeslider/2.1.4/js/ion.rangeSlider.min.js"></script>
    <%--<script src="/js/inspinia/plugins/summernote/summernote.min.js"></script>--%>
    <script type="text/javascript" src="/js/product.js"></script>
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            var panel = $('#<%= hfCurrentPanel.ClientID %>').val();
            if (panel) {
                $('.nav-tabs a[href=#' + panel + ']').tab('show');
            }

            $('.date').datepicker({
                format: 'dd/mm/yyyy',
                keyboardNavigation: false,
                forceParse: false,
                autoclose: true,
                todayHighlight: true
            }); 

            //$('.summernote').summernote({
            //    height: 200
            //});

            var ApolloRating = $('#<%= hfApolloRating.ClientID %>').val();
            $('#ApolloRating').ionRangeSlider({
                min: 0,
                type: 'single',
                from: ApolloRating,
                onFinish: function (obj) {
                    $('.ApolloRating').val(obj.from);
                }
            });

            var customerRating = $('#<%= hfCustomerRating.ClientID %>').val();
            $('#customerRating').ionRangeSlider({
                from_fixed: true,
                min: 0,
                type: 'single',
                from: customerRating
            });

            var popularity = $('#<%= hfPopularity.ClientID %>').val();
            $('#popularity').ionRangeSlider({
                from_fixed: true,
                min: 0,
                type: 'single',
                from: popularity                
            });

            <% 
            var branches = OrderService.GetAllBranches();
            foreach (var branch in branches)
            {
            %>
                $('.<%= branch.Name.ToLower() %>').each(
                    function (index, domEle) {
                        $(domEle).find('.info').click(function () {
                            $(domEle).find('.info').attr('class', 'fa fa-spinner fa-spin');
                            getStockLevel($(domEle).find('.branchId').val(), $(domEle).find('.barcode').val(), $(domEle).find('.elementId').val());
                        });
                    });
            <%
            }
            %>
        });
    
        function selectTag(value, context) {
            $('#divTag').html('');
            $('#tagContent').css('visibility', 'visible');
            
            FTB_API['<%= ftbTagContent.ClientID %>'].SetHtml(value);
        }
        
        function getStockLevel(branchId, barcode, id) {
            <%= ClientScript.GetCallbackEventReference(this, "'stock_' + branchId + '_' + barcode", "load_stock_level", "id", true) %>;
        }
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <asp:HiddenField ID="hfCurrentPanel" runat="server" />
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Product</h2>            
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
        <div class="col-lg-4">
            <div class="row"><p></p></div>
            <div class="pull-right">
                <span class="form-inline">
                    Product ID: <asp:TextBox ID="txtGoProductId" CssClass="form-control input-sm" runat="server"></asp:TextBox>
                    <asp:LinkButton ID="lbGo" runat="server" Text="Go" OnClick="lbGo_Click" CssClass="btn btn-outline btn-sm btn-primary"></asp:LinkButton>
                </span>
                <asp:HyperLink ID="hlNext" runat="server" Text="Next" CssClass="btn btn-outline btn-sm btn-warning"></asp:HyperLink>
                <asp:HyperLink ID="hlPrev" runat="server" Text="Prev" CssClass="btn btn-outline btn-sm btn-danger"></asp:HyperLink>
            </div>
        </div>
    </div>
    
    <div class="row wrapper">
        <div class="col-lg-12">
            <div class="row"><p></p></div>
            <div class="row">
                <div class="pull-right">
                    <a href="/catalog/product_default.aspx" class="btn btn-sm btn-default">Back</a>    
                    <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClientClick="return confirm('This action will refresh all products related data on store front and performance could be affected.\nAre you sure to publish?');" OnClick="lbPublish_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                    <asp:LinkButton ID="lbReset" runat="server" Text="Reset" OnClick="lbReset_Click" CssClass="btn btn-sm btn-success"></asp:LinkButton>
                    <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure to delete this product?');" CssClass="btn btn-sm btn-info"></asp:LinkButton>                
                </div>
            </div> 
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary ID="vsProductSum" runat="server" DisplayMode="BulletList" ValidationGroup="vgProduct" CssClass="valSummary alert alert-warning" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#general" data-toggle="tab">General<asp:PlaceHolder ID="phGeneralAlert" runat="server" Visible="false"> <i class="fa fa-exclamation-circle"></i></asp:PlaceHolder></a></li>
                            <li><a href="#desc" data-toggle="tab">Description<asp:PlaceHolder ID="phDescriptionAlert" runat="server" Visible="false"> <i class="fa fa-exclamation-circle"></i></asp:PlaceHolder></a></li>
                            <li><a href="#category" data-toggle="tab">Category<asp:PlaceHolder ID="phCategoryAlert" runat="server" Visible="false"> <i class="fa fa-exclamation-circle"></i></asp:PlaceHolder></a></li>
                            <li><a href="#meta" data-toggle="tab">Metadata</a></li>
                            <li><a href="#prices" data-toggle="tab">Prices<asp:PlaceHolder ID="phPricesAlert" runat="server" Visible="false"> <i class="fa fa-exclamation-circle"></i></asp:PlaceHolder></a></li>
                            <li><a href="#images" data-toggle="tab">Images<asp:PlaceHolder ID="phImagesAlert" runat="server" Visible="false"> <i class="fa fa-exclamation-circle"></i></asp:PlaceHolder></a></li>
                            <li><a href="#tags" data-toggle="tab">Tags</a></li>
                            <li><a href="#ratings" data-toggle="tab">Rank rating</a></li>
                            <li><a href="#restriction" data-toggle="tab">Product restrictions</a></li>
                            <li><a href="#google" data-toggle="tab">Google<asp:PlaceHolder ID="phGoogleAlert" runat="server" Visible="false"> <i class="fa fa-exclamation-circle"></i></asp:PlaceHolder></a></li>
                        </ul>
                        <div class="tab-content">
                            <div id="general" class="tab-pane active">
                                <div class="panel-body">
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Name<strong>*</strong><small class="text-navy clearfix">(100 characters maximum)</small></th>
                                            <td>
                                                <asp:TextBox ID="txtName" CssClass="form-control" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" ErrorMessage="Name is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Name is required.</span>"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtName" ValidationExpression="^(?=.+).{1,100}$" Display="Dynamic" ErrorMessage="Name has too many characters." 
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Name has too many characters.</span>"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>URL key <small class="text-navy clearfix">(auto generated if left blank)</small></th>
                                            <td><asp:TextBox ID="txtUrlKey" CssClass="form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Brand</th>
                                            <td><asp:DropDownList ID="ddlBrand" CssClass="form-control" runat="server"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Product code</th>
                                            <td><asp:TextBox ID="txtProductCode" CssClass="form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>H1 title <small class="text-navy clearfix">(it is for title on product page)</small><small class="text-navy clearfix">(generally, it could be used for adding<br />promotional text on product name<br />such as 'Avene Spring Water - Exclusively from us')</small></th>
                                            <td><asp:TextBox ID="txtH1Title" CssClass="form-control" runat="server"></asp:TextBox></td>
                                        </tr>                               
                                        <tr>
                                            <th>Status <small class="text-navy clearfix">(visibility)</small></th>
                                            <td>
                                                <asp:DropDownList ID="ddlStatus" CssClass="form-control" runat="server">
                                                    <asp:ListItem Text="Online" Value="enabled"></asp:ListItem>
                                                    <asp:ListItem Text="Offline" Value="disabled"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Discontinued</th>
                                            <td><asp:CheckBox ID="cbDiscontinued" runat="server"/></td>
                                        </tr>
                                        <tr>
                                            <th>Display pre-order button</th>
                                            <td><asp:CheckBox ID="cbDisplayPreOrder" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Open for offer</th>
                                            <td><asp:CheckBox ID="cbOpenForOffer" runat="server" /></td>
                                        </tr>                                        
                                        <tr>
                                            <th>Tax category</th>
                                            <td><asp:DropDownList ID="ddlTaxCategory" CssClass="form-control" runat="server"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Option type<small class="text-navy clearfix">(to determine price option type)</small></th>
                                            <td><asp:DropDownList ID="ddlOptionType" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Product mark</th>
                                            <td><asp:TextBox ID="txtProductMark" CssClass="form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Product mark colour
                                                <small class="text-navy clearfix">(to set colour on product ribbon)</small>
                                                <small class="text-navy clearfix">(Yellow - NEW)</small>
                                                <small class="text-navy clearfix">(Red - OFFER)</small>
                                                <small class="text-navy clearfix">(Orange - Gift with Purchase)</small>
                                            </th>
                                            <td><asp:DropDownList ID="ddlProductMarks" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Product mark expiry date<small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox ID="txtProductMarkExpiryDate" runat="server" CssClass="form-control date"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Delivery information</th>
                                            <td><asp:DropDownList ID="ddlDelivery" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Allow free gift wrapped</th>
                                            <td><asp:CheckBox ID="cbFreeWrapped" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Is pharmaceutical?
                                                <small class="text-navy clearfix">(required to fill in a form during checkout)</small>
                                                <small class="text-navy clearfix">(step quantity will always be set to 1)</small>
                                            </th>
                                            <td><asp:CheckBox ID="cbIsPharm" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Is Google Product Search disabled? <small class="text-navy clearfix">(it will not be available in Google Shopping)</small></th>
                                            <td><asp:CheckBox ID="cbGoogleProductSearchDisabled" runat="server" /></td>
                                        </tr> 
                                        <tr>
                                            <th>Is phone order?
                                                <small class="text-navy clearfix">(buy button will be hidden)</small>
                                                <small class="text-navy clearfix">(a message will appear to inform customer to order via email or phone)</small>
                                            </th>
                                            <td><asp:CheckBox ID="cbIsPhoneOrder" runat="server" /></td>
                                        </tr>    
                                        <tr>
                                            <th>Enforce stock count? <small class="text-navy clearfix">(shows out of stock if stock reaches zero)</small></th>
                                            <td>
                                                <asp:CheckBox ID="cbEnforceStockCount" runat="server" />
                                                <asp:Literal ID="ltlBrandStock" runat="server"></asp:Literal>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Step quantity<small class="text-navy clearfix">(positive integer only)</small><small class="text-navy clearfix">(maximum is 10)</small></th>
                                            <td><asp:TextBox ID="txtStepQuantity" CssClass="form-control" Text="1" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Visible individually <small class="text-navy clearfix">(it allows product to display individually in a list or as itself)</small><small class="text-navy clearfix">(most products should have this ticked, free item should have this unticked)</small></th>
                                            <td><asp:CheckBox ID="cbVisibleIndividually" runat="server" /></td>
                                        </tr>                                        
                                    </table>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbSave" runat="server" Text="Update" OnClick="lbUpdate_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>                                        
                                    </div>
                                </div>
                            </div>
                            <div id="desc" class="tab-pane">
                                <div class="panel-body">
                                    <%--<style type="text/css">
                                        .btn-toolbar {
                                            height: 50px;
                                        }
                                    </style>
                                    <div class="summernote">
                                        <asp:Literal ID="ltDescription" runat="server"></asp:Literal>
                                    </div>--%>
                                    <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Height="200px" Width="400px"></asp:TextBox>
                                    <FTB:FreeTextBox id="ftbDesc"
                                                ToolbarLayout="bold,italic,underline;bulletedlist,numberedlist" 
                                                runat="Server" BreakMode="LineBreak" FormatHtmlTagsToXhtml="true" PasteMode="Text" />
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbSaveDescription" runat="server" Text="Save" CssClass="btn btn-sm btn-primary" OnClick="lbSaveDescription_Click"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div id="category" class="tab-pane">
                                <div class="panel-body">                                    
                                    <asp:HiddenField ID="hfCategory" runat="server" />
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Category selection</th>
                                            <td>
                                                <asp:DropDownList ID="ddlCategorySelection" runat="server" CssClass="form-control"></asp:DropDownList>
                                                <asp:LinkButton runat="server" CssClass="btn btn-outline btn-sm btn-warning" Text="Remove" ID="lbRemoveCategory" OnClick="lbRemoveCategory_Click"></asp:LinkButton>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>New category</th>
                                            <td>
                                                <asp:Literal ID="ltlCategory" runat="server"></asp:Literal>
                                                <asp:LinkButton ID="lbSearchNewCategory" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Search" OnClick="lbSearchNewCategory_Click"></asp:LinkButton>
                                                <asp:LinkButton ID="lbAddNewCategory" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Add" OnClick="lbAddNewCategory_Click" Visible="false"></asp:LinkButton>
                                                <asp:LinkButton ID="lbCancelCategory" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Cancel" OnClick="lbCancelCategory_Click" Visible="false"></asp:LinkButton>
                                                <Apollo:CategoryTree ID="ectCategory" runat="server" Visible="false" OnTreeChanged="ectCategory_TreeChanged" OnTreeNodeSelected="ectCategory_TreeNodeSelected" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div id="google" class="tab-pane">
                                <div class="panel-body">                                                                        
                                    <div class="col-lg-6">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Google Shopping Custom Label
                                            </div>
                                            <table class="table table-striped">
                                                <tr>
                                                    <th>Custom Label 1</th>
                                                    <td><asp:TextBox ID="txtCustomLabel1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Value 1</th>
                                                    <td><asp:TextBox ID="txtValue1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Custom Label 2</th>
                                                    <td><asp:TextBox ID="txtCustomLabel2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Value 2</th>
                                                    <td><asp:TextBox ID="txtValue2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Custom Label 3</th>
                                                    <td><asp:TextBox ID="txtCustomLabel3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Value 3</th>
                                                    <td><asp:TextBox ID="txtValue3" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Custom Label 4</th>
                                                    <td><asp:TextBox ID="txtCustomLabel4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Value 4</th>
                                                    <td><asp:TextBox ID="txtValue4" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Custom Label 5</th>
                                                    <td><asp:TextBox ID="txtCustomLabel5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Value 5</th>
                                                    <td><asp:TextBox ID="txtValue5" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                            </table>
                                            <div class="hr-line-dashed"></div>
                                            <div class="col-lg-12">
                                                <asp:LinkButton ID="lbUpdateGoogleShoppingLabels" runat="server" Text="Update" CssClass="btn btn-sm btn-primary" OnClick="lbUpdateGoogleShoppingLabels_Click"></asp:LinkButton>
                                            </div>
                                            <div class="col-lg-12"><p></p></div>
                                            <div class="clearfix"></div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                Google Taxonomy
                                            </div>
                                            <asp:HiddenField ID="hfGoogleTaxonomyId" runat="server" />
                                            <table class="table table-striped">
                                                <tr>
                                                    <th>Google Taxonomy</th>
                                                    <td>
                                                        <asp:Literal ID="ltlGoogleTaxonomy" runat="server"></asp:Literal>
                                                        <asp:LinkButton ID="lbRemoveGoogleTaxonomy" CssClass="btn btn-outline btn-sm btn-warning" runat="server" Text="Remove" OnClick="lbRemoveGoogleTaxonomy_Click"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>New Google Taxonomy</th>
                                                    <td>
                                                        <asp:HiddenField ID="hfNewGoogleTaxonomyId" runat="server" />
                                                        <asp:Literal ID="ltlNewGoogleTaxonomy" runat="server"></asp:Literal>
                                                        <asp:LinkButton ID="lbSearchNewGoogleTaxonomy" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Search" OnClick="lbSearchNewGoogleTaxonomy_Click"></asp:LinkButton>
                                                        <asp:LinkButton ID="lbUpdateNewGoogleTaxonomy" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Update" Visible="false" OnClick="lbUpdateNewGoogleTaxonomy_Click"></asp:LinkButton>
                                                        <asp:LinkButton ID="lbCancelGoogleTaxonomy" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Cancel" Visible="false" OnClick="lbCancelGoogleTaxonomy_Click"></asp:LinkButton>                                                
                                                        <Apollo:GoogleTaxonomy ID="egtGoogle" runat="server" Visible="false" OnTreeChanged="egtGoogle_TreeChanged" OnTreeNodeSelected="egtGoogle_TreeNodeSelected" />                                                
                                                    </td>
                                                </tr>
                                            </table>
                                            <div class="col-lg-12"><p></p></div>
                                            <div class="clearfix"></div>
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
                                                        <small class="text-navy clearfix">(auto generated from description if left blank)</small>
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
                                                    <td><asp:TextBox ID="txtMetaKeywords" runat="server" CssClass="form-control" TextMode="MultiLine" Height="100px"></asp:TextBox>
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
                                                <asp:LinkButton ID="lbAddMeta" runat="server" Text="Update" ValidationGroup="metadata" CssClass="btn btn-sm btn-primary" OnClick="lbAddMeta_Click"></asp:LinkButton>
                                            </div>
                                            <div class="col-lg-12"><p></p></div>
                                            <div class="clearfix"></div>
                                        </div>
                                    </div>                                    
                                </div>
                            </div>
                            <div id="prices" class="tab-pane">
                                <div class="panel-body">                                    
                                    <asp:Repeater ID="rptPrices" runat="server" OnItemCommand="rptPrices_ItemCommand">
                                        <HeaderTemplate>
                                            <table class="table table-striped">
                                                <tr>
                                                    <th>Product Price ID</th>
                                                    <th>Status</th>
                                                    <th>Priority</th>
                                                    <th>Info</th>
                                                    <th>Option</th>
                                                    <th>Barcode</th>
                                                    <th>Stock</th>
                                                    <th>Branch Stock</th>
                                                    <th style="width:100px;">Actions</th>
                                                </tr>
                                        </HeaderTemplate>                
                                        <ItemTemplate>                    
                                                <tr>
                                                    <td><%# Eval("Id") %></td>
                                                    <td><%# Eval("Enabled") %></td>
                                                    <td><%# Eval("Priority") %></td>
                                                    <td>
                                                        <table class="table">
                                                            <tr>
                                                                <th>Price</th>
                                                                <td><%= CurrencySettings.PrimaryStoreCurrencyCode %> <%# Eval("Price", "{0:f2}") %></td>
                                                            </tr>
                                                            <tr>
                                                                <th>Price code</th>
                                                                <td><%# Eval("PriceCode") %></td>
                                                            </tr>
                                                            <tr>
                                                                <th>Weight</th>
                                                                <td><%# Eval("Weight") %> grams</td>
                                                            </tr>
                                                            <tr>
                                                                <th>Additional Shipping Cost</th>
                                                                <td><%= CurrencySettings.PrimaryStoreCurrencyCode %> <%# Eval("AdditionalShippingCost", "{0:f2}") %></td>
                                                            </tr>
                                                            <tr>
                                                                <th>Maximum Allowed Purchase Quantity</th>
                                                                <td><%# Eval("MaximumAllowedPurchaseQuantity") %></td>
                                                            </tr>
                                                            <tr>
                                                                <th>Disable Stock Sync</th>
                                                                <td><%# Eval("DisableStockSync") %></td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                    <td><%# GeneratePriceOption(Convert.ToInt32(Eval("Id"))) %></td>
                                                    <td><%# Eval("Barcode") %></td>
                                                    <td><%# Eval("Stock") %></td>
                                                    <td>
                                                        <table class="table">
                                                            <%
                                                                var branches = OrderService.GetAllBranches();
                                                                foreach (var branch in branches)
                                                                {
                                                            %>
                                                                <tr>
                                                                    <td><%= branch.Name.ToUpper() %></td>
                                                                    <td>
                                                                        <div id='<%= branch.Name.ToLower() %>_<%# Eval("Id") %>' class="<%= branch.Name.ToLower() %>">
                                                                            <i class="info fa fa-question-circle" style="cursor:pointer;"></i>
                                                                            <input type="hidden" value='<%= branch.Id %>' class="branchId" />
                                                                            <input type="hidden" value='<%# ProductService.GetProductPrice(Convert.ToInt32(Eval("Id"))).Barcode %>' class="barcode" />                                    
                                                                            <input type="hidden" value='<%= branch.Name.ToLower() %>_<%# Eval("Id").ToString() %>' class="elementId" />                        
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            <%
                                                                }
                                                            %>
                                                        </table>
                                                    </td>
                                                    <td>
                                                        <asp:LinkButton ID="lbEdit" runat="server" CommandArgument='<%# Eval("Id") %>' CommandName="edit" Text="Edit"></asp:LinkButton> | 
                                                        <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" CommandArgument='<%# Eval("Id") %>' CommandName="delete"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    <div class="col-md-6">
                                        <h4><asp:Literal ID="ltlPriceTitle" runat="server"></asp:Literal></h4>
                                        <asp:HiddenField ID="hfProductPriceId" runat="server" />
                                        <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgPrice" CssClass="alert alert-warning" />
                                        <table class="table table-striped">
                                            <tr>
                                                <th>Price code</th>
                                                <td><asp:TextBox ID="txtPriceCode" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            </tr>                
                                            <asp:MultiView ID="mvOption" runat="server" ActiveViewIndex="0">
                                                <asp:View runat="server"></asp:View>
                                                <asp:View runat="server">
                                                    <tr>
                                                        <th>Size<small class="text-navy clearfix">(because selected option type is size)</small></th>
                                                        <td><asp:TextBox ID="txtSize" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                    </tr>             
                                                </asp:View>
                                                <asp:View runat="server">
                                                    <tr>
                                                        <th>Colour<strong>*</strong><small class="text-navy clearfix">(because selected option type is colour)</small></th>
                                                        <td>
                                                            <asp:Image runat="server" ID="imgColourOption" />
                                                            <asp:Literal runat="server" ID="ltlColourOptionName"></asp:Literal>
                                                            <span style="display:none;"><asp:TextBox ID="txtColourId" runat="server"></asp:TextBox></span>
                                                            <asp:RequiredFieldValidator ValidationGroup="vgPrice" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>" Display="Dynamic" ErrorMessage="Colour is required."
                                                                ControlToValidate="txtColourId"></asp:RequiredFieldValidator>
                                                            <asp:LinkButton ID="lbEditColourId" OnClick="lbEditColourId_Click" runat="server" Text="Edit"></asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </asp:View>
                                                <asp:View runat="server">
                                                    <tr>
                                                        <th>Colour<strong>*</strong> <small class="text-navy clearfix">(because selected option type is colour)</small></th>
                                                        <td>           
                                                            <asp:PlaceHolder ID="phEditColourPanel" Visible="false" runat="server">
                                                                <div class="editColourPanel">
                                                                    <h4>Edit colour</h4>                                
                                                                    <table class="table table-striped">
                                                                        <tr>
                                                                            <th>Colour image</th>
                                                                            <td><asp:Image runat="server" ID="imgEditColourImg" /></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <th>New image file<small class="text-navy clearfix">(will be resized to 50x50)</small><small class="text-navy clearfix">(accepted file types: *.jpg, *.jpeg)</small></th>
                                                                            <td><asp:FileUpload ID="fuEditColour" runat="server" /></td>
                                                                        </tr>
                                                                        <tr>
                                                                            <th>Name</th>
                                                                            <td><asp:TextBox ID="txtEditColourName" runat="server" /></td>
                                                                        </tr>                                                                              
                                                                        <tr>
                                                                            <td></td>
                                                                            <td><asp:LinkButton ID="lbEditUpdateColour" runat="server" Text="Update Colour" OnClick="lbEditUpdateColour_Click" CssClass="ABtn"></asp:LinkButton></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </asp:PlaceHolder>
                                                            <asp:RequiredFieldValidator ValidationGroup="vgPrice" runat="server" Text="Required." Display="Dynamic" ControlToValidate="txtColourOptionEditId"></asp:RequiredFieldValidator>
                                
                                                            <asp:PlaceHolder ID="phNewColour" runat="server">
                                                                <div>
                                                                    <h4>Add new colour</h4>
                                                                    <table class="table table-striped">
                                                                        <tr>
                                                                            <th>Name</th>
                                                                            <td><asp:TextBox ID="txtColourName" runat="server" CssClass="form-control" /></td>
                                                                        </tr> 
                                                                        <tr>
                                                                            <th>Image file<small class="text-navy clearfix">(will be resized to 50x50)</small><small class="text-navy clearfix">(accepted file types: *.jpg, *.jpeg)</small></th>
                                                                            <td><asp:FileUpload ID="fuColourImage" runat="server" /></td>
                                                                        </tr>
                                                                    </table>
                                                                    <div class="btnBox">
                                                                        <asp:LinkButton ID="lbUploadColour" runat="server" Text="Add colour" OnClick="lbUploadColour_Click" CssClass="btn btn-sm btn-success"></asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                                <br />
                                                                <br />
                                                                <br />
                                                                <asp:RequiredFieldValidator ValidationGroup="vgPrice" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>" Display="Dynamic" ControlToValidate="txtColourOptionEditId"></asp:RequiredFieldValidator>
                                                            
                                                                <div class="table-responsive">
                                                                    <h4>Choose colour</h4>
                                                                    <div class="dataTables_wrapper form-inline dt-bootstrap" style="padding-bottom: 0;">
                                                                        <div class="html5buttons">
                                                                            <div class="dt-buttons btn-group">
                                                                                <asp:LinkButton ID="lbResetFilterColour" runat="server" Text="Reset filter" OnClick="lbResetFilterColour_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                                                <asp:LinkButton ID="lbSearchColour" runat="server" Text="Search" OnClick="lbSearchColour_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                    <Apollo:CustomGrid ID="gvBrandColours" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" 
                                                                        OnPageIndexChanging="gvBrandColours_PageIndexChanging" OnRowCommand="gvBrandColours_RowCommand"
                                                                        AutoGenerateColumns="false" OnSorting="gvBrandColours_Sorting" OnPreRender="gvBrandColours_PreRender" ShowHeader="true"
                                                                        DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">                    
                                                                        <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                                                        <PagerTemplate>                                                           
                                                                            <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                                                Page 
                                                                                <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                                                <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnColourGoPage_Click" CssClass="hidden" />
                                                                                <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvBrandColours.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                                                <asp:ImageButton Visible='<%# (gvBrandColours.CustomPageCount > (gvBrandColours.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                                                of <%= gvBrandColours.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvBrandColours.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                                                            </asp:Panel>
                                                                        </PagerTemplate>
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderStyle-Width="20px" HeaderText="Colour" ItemStyle-HorizontalAlign="Center">
                                                                                <ItemTemplate>
                                                                                    <img alt='<%# Eval("ColourFileName") %>' src="/get_image_handler.aspx?type=colour&img=<%# Eval("ColourFileName") %>" />
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderStyle-Width="20px" SortExpression="Id">
                                                                                <HeaderTemplate>                                
                                                                                    <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Colour ID</asp:LinkButton><br />
                                                                                    <asp:TextBox ID="txtFilterColourId" runat="server" CssClass="form-control"></asp:TextBox>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderStyle-Width="20px" SortExpression="Value">
                                                                                <HeaderTemplate>                                
                                                                                    Name<br />
                                                                                    <asp:TextBox ID="txtFilterColourName" runat="server" CssClass="form-control"></asp:TextBox>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate><%# Eval("Value")%></ItemTemplate>
                                                                            </asp:TemplateField>                        
                                                                            <asp:TemplateField HeaderStyle-Width="20px" HeaderText="Brand" SortExpression="BrandName">
                                                                                <HeaderTemplate>
                                                                                    Brand<br />
                                                                                    <asp:TextBox ID="txtFilterColourBrand" runat="server" CssClass="form-control"></asp:TextBox>
                                                                                </HeaderTemplate>
                                                                                <ItemTemplate>
                                                                                    <%# Eval("BrandName") %>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Action" HeaderStyle-Width="50px">
                                                                                <ItemTemplate>
                                                                                    <asp:LinkButton runat="server" Text="Select" CommandArgument='<%# Eval("Id") %>' CommandName="select"></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                        </Columns>
                                                                    </Apollo:CustomGrid>
                                                                </div> 
                                                                <span style="display: none;"><asp:TextBox ID="txtColourOptionEditId" runat="server"></asp:TextBox></span>
                                                            </asp:PlaceHolder>
                                                        </td>
                                                    </tr>        
                                                </asp:View>
                                            </asp:MultiView>
                                            <tr>
                                                <th>Price<strong>*</strong></th>
                                                <td><asp:TextBox ID="txtPrice" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtPrice" runat="server" ValidationGroup="vgPrice" ErrorMessage="Price is required."
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPrice" ErrorMessage="Price can only be decimals." ValidationExpression="^[.\?\d*]*\.?\d*" ValidationGroup="vgPrice" Display="Dynamic">Price can only be decimals.</asp:RegularExpressionValidator>
                                                </td>                     
                                            </tr>
                                            <tr>
                                                <th>Weight<strong>*</strong><small class="text-navy clearfix">(in gram, integer only)</small></th>
                                                <td><asp:TextBox ID="txtWeight" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtWeight" runat="server" ValidationGroup="vgPrice" ErrorMessage="Weight is required."
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtWeight" Display="Dynamic" ErrorMessage="Only Numbers" ValidationExpression="^\d*" ValidationGroup="vgPrice">Only Numbers</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Additional shipping cost <small class="text-navy clearfix">(for international orders only)</small></th>
                                                <td><asp:TextBox ID="txtAdditionalShippingCost" Text="0" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtAdditionalShippingCost" ErrorMessage="Only Numbers" ValidationExpression="((\d+)((\.\d{1,2})?))$" ValidationGroup="vgPrice" Display="Dynamic">Only Decimal</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Stock<strong>*</strong></th>
                                                <td><asp:TextBox ID="txtStock" runat="server" MaxLength="8" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtStock" runat="server" ValidationGroup="vgPrice" ErrorMessage="Stock is required."
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtStock" ErrorMessage="Only Numbers" ValidationExpression="^\d*" ValidationGroup="vgPrice" Display="Dynamic">Only Numbers</asp:RegularExpressionValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Status<small class="text-navy clearfix">(visibility)</small></th>
                                                <td><asp:CheckBox ID="cbPriceStatus" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>Barcode</th>
                                                <td><asp:TextBox ID="txtBarcode" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <th>Priority<small class="text-navy clearfix">(sorting order)</small></th>
                                                <td><asp:TextBox ID="txtPriority" runat="server" Text="0" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtPriority" runat="server" ValidationGroup="vgPrice" ErrorMessage="Priority is required."
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Maximum allowed purchase quantity</th>
                                                <td><asp:TextBox ID="txtMaximumAllowedPurchaseQuantity" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <th>Disable stock sync <small class="text-navy clearfix">(for vector stock sync)</small></th>
                                                <td><asp:CheckBox ID="cbDisableStockSync" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>Associated image</th>
                                                <td>
                                                    <asp:MultiView ID="mvAssociatedImage" runat="server" ActiveViewIndex="0">
                                                        <asp:View runat="server">
                                                            <asp:Image runat="server" ID="imgAssociatedPicture" />
                                                            <asp:LinkButton ID="lbRemoveAssociatedImage" CssClass="btn btn-outline btn-sm btn-warning" OnClick="lbRemoveAssociatedImage_Click" runat="server" Text="Remove"></asp:LinkButton>
                                                            <asp:LinkButton ID="lbChangeAssociatedImage" CssClass="btn btn-outline btn-sm btn-primary" OnClick="lbChangeAssociatedImage_Click" runat="server" Text="Change"></asp:LinkButton>
                                                            <span style="display:none;"><asp:TextBox ID="txtProductMediaIdForOption" runat="server"></asp:TextBox></span>
                                                        </asp:View>
                                                        <asp:View runat="server">
                                                            <asp:Repeater ID="rptRelatedImages" runat="server" OnItemCommand="rptRelatedImages_ItemCommand">
                                                                <HeaderTemplate>
                                                                    <div class="product-images col-lg-12">
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <div class="image_box image_box_<%# Eval("Id") %> pull-left col-lg-3">
                                                                        <img src="/get_image_handler.aspx?type=media&img=<%# Eval("ThumbnailFileName") %>" alt='<%# Eval("ThumbnailFileName") %>' class='<%# Convert.ToBoolean(Eval("Enabled")) ? "" : "img-disabled" %>' />
                                                                        <asp:LinkButton ID="lbChoose" runat="server" Text="Choose" CssClass="btn btn-danger btn-xs" CommandName="choose" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    </div>
                                                                </FooterTemplate>
                                                            </asp:Repeater>
                                                            <div class="col-lg-12 space-15">
                                                                <asp:LinkButton ID="lbCancelAssociatedImage" CssClass="btn btn-outline btn-sm btn-primary" OnClick="lbCancelAssociatedImage_Click" runat="server" Text="Cancel"></asp:LinkButton>
                                                            </div>
                                                        </asp:View>
                                                    </asp:MultiView>
                                                </td>
                                            </tr>
                                        </table>
                                        <div class="hr-line-dashed"></div>
                                        <div class="col-lg-12">
                                            <asp:LinkButton ID="lbAddNewPrice" runat="server" Text="Create" OnClick="lbAddNewPrice_Click" CssClass="btn btn-sm btn-primary" ValidationGroup="vgPrice"></asp:LinkButton>
                                            <asp:LinkButton ID="lbSavePrice" runat="server" Text="Update" OnClick="lbSavePrice_Click" CssClass="btn btn-sm btn-success" ValidationGroup="vgPrice"></asp:LinkButton>
                                            <asp:LinkButton ID="lbDeletePrice" runat="server" Text="Delete" OnClick="lbDeletePrice_Click" CssClass="btn btn-sm btn-warning"></asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="images" class="tab-pane">
                                <div class="panel-body">                                    
                                    <asp:Repeater ID="rptImages" runat="server" OnItemDataBound="rptImages_ItemDataBound" OnItemCommand="rptImages_ItemCommand">
                                        <HeaderTemplate>
                                            <div class="product-images col-lg-12">
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                                <div class="image_box image_box_<%# Eval("Id") %> pull-left col-lg-3">
                                                    <img src="/get_image_handler.aspx?type=media&img=<%# Eval("ThumbnailFileName") %>" alt='<%# Eval("ThumbnailFileName") %>' class='<%# Convert.ToBoolean(Eval("Enabled")) ? "" : "img-disabled" %>' />
                                                    <asp:RadioButton ID="rbPrimary" runat="server" CssClass="radio" Checked='<%# Convert.ToBoolean(Eval("PrimaryImage")) %>' Text="Primary image"/>
                                                    <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-xs" CommandName="delete" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                    <asp:LinkButton ID="lbToggle" runat="server" Text='<%# Convert.ToBoolean(Eval("Enabled")) ? "Disable" : "Enable" %>' CssClass="btn btn-warning btn-xs" CommandName="toggle" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                </div>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </div>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    <div class="col-lg-12"><p></p></div>
                                    <h4><asp:Literal ID="ltlImageTitle" runat="server"></asp:Literal></h4>            
                                    <asp:HiddenField ID="hfProductMediaId" runat="server" />
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Image file<small class="text-navy clearfix">(size should be 1200x1200)</small><small class="text-navy clearfix">(accepted file types: *.jpg, *.jpeg)</small></th>
                                            <td><asp:FileUpload ID="fuImage" runat="server" CssClass="form-control"/></td>
                                        </tr>                                      
                                        <tr>
                                            <td></td>
                                            <td><asp:LinkButton ID="lbSaveImage" runat="server" Text="Upload" OnClick="lbSaveImage_Click" CssClass="btn btn-sm btn-outline btn-primary"></asp:LinkButton></td>
                                        </tr>
                                    </table>
                                    <div class="text-center">
                                        <h2>Or</h2>
                                    </div>                                    
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Main image file<small class="text-navy clearfix">(size should be 300x300)</small><small class="text-navy clearfix">(accepted file types: *.jpg, *.jpeg)</small></th>
                                            <td><asp:FileUpload ID="fuMainImage" runat="server" CssClass="form-control"/></td>
                                        </tr>     
                                        <tr>
                                            <th>Thumb image file<small class="text-navy clearfix">(size should be 100x100)</small><small class="text-navy clearfix">(accepted file types: *.jpg, *.jpeg)</small></th>
                                            <td><asp:FileUpload ID="fuThumbImage" runat="server" CssClass="form-control"/></td>
                                        </tr>                                      
                                        <tr>
                                            <td></td>
                                            <td>
                                                <div class="btnBox">
                                                    <asp:LinkButton ID="lbSave300Image" runat="server" Text="Upload" OnClick="lbSave300Image_Click" CssClass="btn btn-sm btn-outline btn-warning"></asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div id="tags" class="tab-pane">
                                <div class="panel-body">
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Tag</th>
                                            <td><asp:DropDownList ID="ddlTag" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                            <td><div id="divTag"></div></td>
                                        </tr>      
                                    </table>
                                    <div id="tagContent">
                                        <asp:TextBox ID="txtTagContent" runat="server" TextMode="MultiLine" Height="200px" Width="400px"></asp:TextBox>              
                                        <FTB:FreeTextBox id="ftbTagContent"
                                                ToolbarLayout="bold,italic,underline;bulletedlist,numberedlist"
                                                runat="Server" BreakMode="LineBreak" FormatHtmlTagsToXhtml="true" PasteMode="Text" />
                                    </div>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbSaveTag" runat="server" Text="Save" OnClick="lbSaveTag_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div id="ratings" class="tab-pane">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <table class="table table-striped">
                                            <tr>
                                                <th style="width: 200px;">Apollo rating<small class="text-navy clearfix">(used to influence ranking in search result)</small></th>
                                                <td>
                                                    <input id="ApolloRating" type="text" />
                                                    <asp:HiddenField ID="hfApolloRating" runat="server" />
                                                    <asp:TextBox ID="txtApolloRating" runat="server" Text="0" CssClass="ApolloRating" BorderWidth="0" Width="0" Height="0"/>                        
                                                </td>
                                            </tr>                                            
                                            <tr>
                                                <th>Customer rating<small class="text-navy clearfix">(not adjustable)</small><small class="text-navy clearfix">(based on product reviews)</small></th>
                                                <td>
                                                    <input id="customerRating" type="text" />
                                                    <asp:HiddenField ID="hfCustomerRating" runat="server" />
                                                </td>
                                            </tr>                
                                            <tr>
                                                <th>Product popularity<small class="text-navy clearfix">(not adjustable)</small><small class="text-navy clearfix">(items ordered in the last 60 days)</small></th>
                                                <td>
                                                    <input id="popularity" type="text" />
                                                    <asp:HiddenField ID="hfPopularity" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div class="hr-line-dashed"></div>
                                        <div class="col-lg-12">
                                            <asp:LinkButton ID="lbSaveRating" runat="server" Text="Save" CssClass="btn btn-sm btn-primary" OnClick="lbSaveRating_Click"></asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="restriction" class="tab-pane">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <table class="table table-striped">
                                            <tr>
                                                <th>Restricted Groups</th>
                                                <td><asp:DropDownList ID="ddlRestrictedGroups" CssClass="form-control" runat="server" DataTextField="Name" DataValueField="Id"></asp:DropDownList></td>
                                            </tr>
                                        </table>
                                        <div class="hr-line-dashed"></div>
                                        <div class="col-lg-12">
                                            <asp:LinkButton runat="server" Text="Assign" ID="lbAssignRestrictedGroup" CssClass="btn btn-sm btn-primary" OnClick="lbAssignRestrictedGroup_Click"></asp:LinkButton>
                                        </div>
                                        <div class="col-lg-12"><p></p></div>
                                        <h4>Assigned restricted groups</h4>
                                        <asp:Repeater ID="rptAssignedRestrictedGroups" runat="server" OnItemCommand="rptAssignedRestrictedGroups_ItemCommand">
                                            <HeaderTemplate>
                                                <table class="table table-striped">
                                                    <tr>
                                                        <th>Group</th>
                                                        <th>Action</th>
                                                    </tr>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                    <tr>
                                                        <td><%# Eval("Name") %></td>
                                                        <td><asp:LinkButton runat="server" Text="Remove" CommandName="remove" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton></td>
                                                    </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
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