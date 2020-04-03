<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.promo_catalog_offer_new" Codebehind="promo_catalog_offer_new.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet" />
    <link href="/css/inspinia/offer-rule-tree.css" rel="stylesheet" />
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script src="/js/offer.js" type="text/javascript"></script>
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
            <h2>Catalog Offer</h2>
            <h3>New Rule</h3>
        </div>
    </div>

    <div class="row wrapper">
        <div class="col-lg-12">
            <div class="row"><p></p></div>
            <div class="row">
                <div class="pull-right">
                    <asp:LinkButton ID="lbBack" runat="server" Text="Back" OnClick="lbBack_Click" CssClass="btn btn-sm btn-default"></asp:LinkButton>
                    <asp:LinkButton ID="lbSave" runat="server" Text="Save" OnClick="lbSave_Click" ValidationGroup="vgNew" CssClass="btn btn-sm btn-info"></asp:LinkButton>
                    <asp:LinkButton ID="lbReset" runat="server" Text="Reset" OnClick="lbReset_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>                    
                </div>
            </div> 
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgNew" CssClass="alert alert-warning" />
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#info" data-toggle="tab">General information</a></li>
                            <li><a href="#conditions" data-toggle="tab">Conditions</a></li>
                            <li><a href="#action" data-toggle="tab">Action</a></li>
                            <li><a href="#">Offer page management <small class="text-navy clearfix">(please save to continue)</small></a></li>                            
                            <li><a href="#">Related products <small class="text-navy clearfix">(please save to continue)</small></a></li>
                            <li><a href="#">Test offer <small class="text-navy clearfix">(please save to continue)</small></a></li>
                        </ul>
                        <div id="offer" class="tab-content">
                            <div id="info" class="tab-pane active">
                                <div class="panel-body">
                                    <table class="table">
                                        <tr>
                                            <th>Rule name<strong>*</strong></th>
                                            <td>
                                                <asp:TextBox ID="txtRuleName" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRuleName" ValidationGroup="vgNew" ErrorMessage="Rule name is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Rule name is required.</span>"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>  
                                        <tr>
                                            <th>Rule alias<strong>*</strong></th>
                                            <td>
                                                <asp:TextBox ID="txtRuleAlias" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRuleAlias" ValidationGroup="vgNew" ErrorMessage="Rule alias is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Rule alias is required.</span>"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>                
                                        <tr>
                                            <th>Status<strong>*</strong></th>
                                            <td><asp:RadioButtonList ID="rblStatus" runat="server">
                                                    <asp:ListItem Text="Active" Value="true"></asp:ListItem>
                                                    <asp:ListItem Text="Inactive" Value="false"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>From date <small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox ID="txtDateFrom" CssClass="date form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>To date <small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox ID="txtDateTo"  CssClass="date form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Priority</th>
                                            <td><asp:TextBox ID="txtPriority" runat="server" CssClass="form-control" Text="0"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>URL key<p class="hint">(auto generated if left blank)</p></th>
                                            <td><asp:TextBox ID="txtUrlKey" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Show offer tag
                                                <small class="text-navy clearfix">(only visible on product grid)</small>
                                                <small class="text-navy clearfix">(could be overwritten by product mark)</small>
                                            </th>
                                            <td><asp:CheckBox ID="chkShowOfferTag" runat="server" Checked="true" /></td>
                                        </tr>
                                        <tr>
                                            <th>Show RRP / price reduction</th>
                                            <td><asp:CheckBox ID="chkShowRRP" runat="server" Checked="true" /></td>
                                        </tr>
                                        <tr>
                                            <th>Point spendable</th>
                                            <td><asp:CheckBox ID="chkPointSpendable" runat="server" Checked="true" /></td>
                                        </tr>
                                        <tr>
                                            <th>Offer type</th>
                                            <td><asp:DropDownList ID="ddlOfferTypes" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Show count down timer
                                                <small class="text-navy clearfix">(only visible on product details page)</small>
                                                <small class="text-navy clearfix">(only works with valid 'to date')</small>
                                            </th>
                                            <td><asp:CheckBox ID="cbShowCountDownTimer" runat="server" Checked="true" /></td>
                                        </tr>
                                        <tr>
                                            <th>Display on product page</th>
                                            <td><asp:CheckBox ID="cbDisplayOnProductPage" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Related items</th>
                                            <td>
                                                <asp:DropDownList ID="ddlRelatedTypes" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="Products" Value="products"></asp:ListItem>
                                                    <asp:ListItem Text="Brands" Value="brands"></asp:ListItem>
                                                    <asp:ListItem Text="Category" Value="category"></asp:ListItem>
                                                </asp:DropDownList>
                                                <br />
                                                <asp:TextBox ID="txtRelatedItems" runat="server" CssClass="form-control"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div id="conditions" class="tab-pane">
                                <div class="panel-body">
                                    <h4>Conditions (leave blank for all products)</h4>
                                    <div class="rule-root" data-ruleid="<%= QueryTempOfferRuleId %>">
                                        <ul class="rule-tree">
                                            <asp:Literal ID="ltlConditions" runat="server"></asp:Literal>
                                        </ul>
                                    </div>
                                </div>
                            </div>                    
                            <div id="action" class="tab-pane">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <h4>Update prices using the following information</h4>
                                        <table class="table">
                                            <tr>
                                                <th>Apply</th>
                                                <td><asp:DropDownList ID="ddlAction" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                            </tr>
                                            <tr>
                                                <th>Discount amount<strong>*</strong></th>
                                                <td>
                                                    <asp:TextBox ID="txtDiscountAmount" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDiscountAmount" ValidationGroup="vgNew" ErrorMessage="Discount amount is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Discount amount is required.</span>"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Discount target option
                                                    <small class="text-navy clearfix">(size, colour)</small>
                                                </th>
                                                <td>
                                                    <div class="form-inline">
                                                        <asp:DropDownList ID="ddlOptionOperator" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <asp:TextBox ID="txtOption" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </td>
                                            </tr>                
                                            <tr>
                                                <th>Stop further rules processing</th>
                                                <td>
                                                    <asp:RadioButtonList ID="rblProceed" runat="server">
                                                        <asp:ListItem Text="Yes" Value="false"></asp:ListItem>
                                                        <asp:ListItem Text="No" Value="true" Selected="True"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
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