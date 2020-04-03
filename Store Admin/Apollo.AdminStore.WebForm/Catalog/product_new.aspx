<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.product_new" Codebehind="product_new.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CategoryTree" Src="~/UserControls/CategoryTreeControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="BrandCategoryTree" Src="~/UserControls/BrandCategoryTreeControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
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
        });
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Product</h2>
        </div>        
    </div>

    <div class="row wrapper">
        <div class="col-lg-12">
            <div class="row"><p></p></div>
            <div class="row">
                <div class="pull-right">
                    <asp:LinkButton runat="server" ValidationGroup="vgNewProduct" Text="Save and continue" OnClick="lbSaveContinue_Click" CssClass="btn btn-sm btn-success"></asp:LinkButton>                    
                </div>
            </div> 
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <asp:ValidationSummary ID="vsProductSum" runat="server" DisplayMode="BulletList" ValidationGroup="vgNewProduct" CssClass="alert alert-warning" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#general" data-toggle="tab">General</a></li>
                            <li><a href="#desc">Description <small class="text-navy">(please save to continue)</small></a></li>
                            <li><a href="#category">Category <small class="text-navy">(please save to continue)</small></a></li>
                            <li><a href="#meta">Metadata <small class="text-navy">(please save to continue)</small></a></li>
                            <li><a href="#prices">Prices <small class="text-navy">(please save to continue)</small></a></li>
                            <li><a href="#images">Images <small class="text-navy">(please save to continue)</small></a></li>
                            <li><a href="#tags">Tags <small class="text-navy">(please save to continue)</small></a></li>
                            <li><a href="#ratings">Rank rating <small class="text-navy">(please save to continue)</small></a></li>
                            <li><a href="#restriction">Product restrictions <small class="text-navy">(please save to continue)</small></a></li>
                            <li><a href="#google" data-toggle="tab">Google <small class="text-navy">(please save to continue)</small></a></li>
                        </ul>
                        <div class="tab-content ">
                            <div id="general" class="tab-pane active">
                                <div class="panel-body">
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Name<strong>*</strong><small class="text-navy clearfix">(100 characters maximum)</small></th>
                                            <td class="wide">
                                                <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" ValidationGroup="vgNewProduct" ErrorMessage="Name is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Name is required.</span>"></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtName" ValidationExpression="^(?=.+).{1,100}$" Display="Dynamic" ErrorMessage="Name has too many characters." 
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Name has too many characters.</span>" ValidationGroup="vgNewProduct"></asp:RegularExpressionValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>URL key <small class="text-navy clearfix">(auto generated if left blank)</small></th>
                                            <td><asp:TextBox ID="txtUrlKey" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Brand</th>
                                            <td><asp:DropDownList ID="ddlBrand" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Product code</th>
                                            <td><asp:TextBox ID="txtProductCode" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>H1 title <small class="text-navy clearfix">(it is for title on product page)</small><small class="text-navy clearfix">(generally, it could be used for adding<br />promotional text on product name<br />such as 'Avene Spring Water - Exclusively from us')</small></th>
                                            <td><asp:TextBox ID="txtH1Title" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>        
                                        <tr>
                                            <th>Status <small class="text-navy clearfix">(visibility)</small><small class="text-navy clearfix">(disabled by default as you need to fill in other information)</small></th>
                                            <td>
                                                <asp:DropDownList ID="ddlStatus" Enabled="false" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="Online" Value="enabled"></asp:ListItem>
                                                    <asp:ListItem Text="Offline" Value="disabled" Selected="true"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Discontinued</th>
                                            <td><asp:CheckBox ID="cbDiscontinued" runat="server" /></td>
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
                                            <th>Option type <small class="text-navy clearfix">(to determine price option type)</small></th>
                                            <td><asp:DropDownList ID="ddlOptionType" CssClass="form-control" runat="server"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Product mark</th>
                                            <td><asp:TextBox ID="txtProductMark" runat="server" CssClass="form-control"></asp:TextBox></td>
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
                                            <td><asp:CheckBox ID="cbFreeWrap" runat="server" /></td>
                                        </tr> 
                                        <tr>
                                            <th>
                                                Is pharmaceutical
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
                                            <td><asp:CheckBox ID="cbEnforceStockCount" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Step quantity <small class="text-navy clearfix">(positive integer only)</small><small class="text-navy clearfix">(maximum is 10)</small></th>
                                            <td><asp:TextBox ID="txtStepQuantity" Text="1" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Visible individually <small class="text-navy clearfix">(it allows product to display in a single product page)</small><small class="text-navy clearfix">(most products should have this ticked, free item should have this unticked)</small></th>
                                            <td><asp:CheckBox ID="cbVisibleIndividually" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Category</th>
                                            <td>
                                                <asp:HiddenField ID="hfCategory" runat="server" Value="-1" />                          
                                                <asp:MultiView ID="mvCategory" runat="server" ActiveViewIndex="0">
                                                    <asp:View ID="View1" runat="server"><asp:Literal ID="ltlCategory" runat="server"></asp:Literal>&nbsp;<asp:LinkButton ID="lbEditParent" CssClass="btn btn-outline btn-sm btn-warning" OnClick="lbEditCategory_Click" runat="server" Text="Edit"></asp:LinkButton></asp:View>                            
                                                    <asp:View ID="View2" runat="server">
                                                        <Apollo:CategoryTree ID="ectCategory" runat="server" OnTreeNodeSelected="ectCategory_TreeNodeSelected" />                                    
                                                    </asp:View>
                                                </asp:MultiView>
                                            </td>
                                        </tr>
                                    </table>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton runat="server" ValidationGroup="vgNewProduct" Text="Save and continue" OnClick="lbSaveContinue_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
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