<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_new" Codebehind="order_new.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerNav" Src="~/UserControls/CustomerNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerTopRightNav" Src="~/UserControls/CustomerTopRightNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="AddressView" Src="~/UserControls/OrderAddressSelectorControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script src="/js/order.js" type="text/javascript"></script>
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script type="text/javascript">        
        $(document).ready(function () {
            var panel = $('#<%= hfCurrentTab.ClientID %>').val();
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
        });        
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">    
    <asp:HiddenField ID="hfCurrentTab" runat="server" />
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <% 
                if (QueryOrderId != 0)
                {
                    loadFromOrder.Text = "<span style='font-size: 15px;'>(loaded from order ID " + QueryOrderId + ")</span>";
                }
            %>
            <h2>New Order <asp:Literal ID="loadFromOrder" runat="server"></asp:Literal></h2>            
        </div>
    </div>
    <Apollo:CustomerTopRightNav ID="ectTogRightNav" runat="server" OnActionOccurred="ectTogRightNav_ActionOccurred" SetBackUrl="/sales/order_new_select_customer.aspx" />                
    <div class="order-new wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
            </div>
            <div class="cart col-lg-12">
                <asp:ValidationSummary ID="vsOrder" CssClass="alert alert-danger" runat="server" DisplayMode="BulletList" ValidationGroup="vgOrderDetails" HeaderText="There were some items invalidating the order." />
                <asp:ValidationSummary ID="vsPayment" CssClass="alert alert-danger" runat="server" DisplayMode="BulletList" ValidationGroup="vgPayment" HeaderText="There were some items invalidating the payment." />
                <asp:ValidationSummary ID="vsPoint" CssClass="alert alert-danger" runat="server" DisplayMode="BulletList" ValidationGroup="vgPoints" HeaderText="There was an error with loyalty points." />                            
                <div class="panel panel-default">
                    <div class="panel-heading">
                        Cart contents
                        <span class="pull-right label label-plain">
                            <asp:LinkButton ID="lbAddItems" runat="server" OnClick="lbAddItems_Click" Text="add items"></asp:LinkButton>
                        </span>
                    </div>
                    <div class="panel-body">            
                        <div class="table-responsive">
                            <asp:GridView ID="gvTempCart" runat="server" AutoGenerateColumns="false" 
                                CssClass="table table-striped table-bordered table-hover dataTable" GridLines="None" OnRowCommand="gvTempCart_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Product ID" SortExpression="ProductId">
                                        <ItemTemplate>
                                            <%# Eval("ProductId")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Product name" SortExpression="Name">
                                        <ItemTemplate><%# Eval("Product.Name") %> <%# Eval("ProductPrice.Option")%>
                                            <%# Eval("ProductPrice.Note") != null ? "<span class='text-danger clearfix'><strong>" + Eval("ProductPrice.Note") + "</strong></span>" : null %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity" HeaderStyle-Width="50px">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQty" runat="server" CssClass="form-control" Text='<%# Eval("Quantity")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Price (including tax)" SortExpression="Price">
                                        <ItemTemplate>
                                            <%# AdminStoreUtility.GetFormattedPrice(Eval("ProductPrice.OfferPriceInclTax"), CustomerCurrency.CurrencyCode, CurrencyType.HtmlEntity, CustomerCurrency.ExchangeRate)%>                                            
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Line total">
                                        <ItemTemplate>
                                            <%# AdminStoreUtility.GetFormattedPrice(Convert.ToInt32(Eval("Quantity")) * Convert.ToDecimal(Eval("ProductPrice.OfferPriceInclTax")), CustomerCurrency.CurrencyCode, CurrencyType.HtmlEntity, CustomerCurrency.ExchangeRate)%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" CommandArgument='<%# Eval("ProductPriceId")%>' CommandName="updateItem">Update</asp:LinkButton>
                                            |
                                            <asp:LinkButton runat="server" CommandArgument='<%# Eval("ProductPriceId")%>' CommandName="removeItem">Remove</asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <div class="info col-lg-9">
                <div class="tabs-container">
                    <ul class="nav nav-tabs">
                        <li class="active"><a href="#existing" data-toggle="tab">Existing incomplete orders</a></li>
                        <li><a href="#addresses"data-toggle="tab">Address information</a></li>
                        <li><a href="#contact" data-toggle="tab">Contact information</a></li>
                        <li><a href="#promo" data-toggle="tab">Promo / Loyalty points</a></li>
                        <li><a href="#notes" data-toggle="tab">Notes</a></li>
                        <asp:PlaceHolder ID="phPharmFormTab" runat="server">
                            <li><a href="#pharm" data-toggle="tab">Pharmaceutical form</a></li>
                        </asp:PlaceHolder>
                        <li><a href="#payment" data-toggle="tab">Shipping &amp; Payment</a></li>
                    </ul>
                    <div class="tab-content">
                        <div id="existing" class="tab-pane active">
                            <div class="panel-body">
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        Existing incomplete orders (last 10 orders)
                                    </div>
                                    <table class="table">
                                        <tr>
                                            <th>Select order</th>
                                            <td><asp:DropDownList ID="ddlExistingOrders" runat="server" OnSelectedIndexChanged="ddlExistingOrders_SelectedIndexChanged" AutoPostBack="true" Visible="false" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                    </table>
                                </div>
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        Existing discarded orders (last 10 orders)
                                    </div>
                                    <table class="table">
                                        <tr>
                                            <th>Select order</th>
                                            <td><asp:DropDownList ID="ddlDiscardedOrders" runat="server" OnSelectedIndexChanged="ddlDiscardedOrders_SelectedIndexChanged" AutoPostBack="true" Visible="false" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div id="addresses" class="tab-pane">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-5">
                                        <div class="panel panel-info">
                                            <div class="panel-heading">Billing address</div>
                                            <Apollo:AddressView AddressType="Billing" runat="server" ID="adrBilling" OnAddressCountryChanged="adrBilling_AddressCountryChanged" OnAddressChanged="adrBilling_AddressChanged" />                    
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <br /><br /><br /><br />
                                        <div class="text-center">
                                            <asp:LinkButton ID="lbCopyToBilling" OnClick="lbCopyToBilling_Click" runat="server" ToolTip="copy to billing address" CssClass="fa fa-arrow-left fa-4x center-block"></asp:LinkButton>
                                        </div>
                                        <div class="text-center">
                                            <asp:LinkButton ID="lbCopyShipping" OnClick="lbCopyShipping_Click" runat="server" ToolTip="copy to shipping address" CssClass="fa fa-arrow-right fa-4x center-block"></asp:LinkButton>                                            
                                        </div>
                                    </div>
                                    <div class="col-lg-5">
                                        <div class="panel panel-danger">
                                            <div class="panel-heading">Shipping address</div>
                                            <Apollo:AddressView AddressType="Shipping" runat="server" ID="adrShipping" OnAddressCountryChanged="adrShipping_AddressCountryChanged" OnAddressChanged="adrShipping_AddressChanged" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="contact" class="tab-pane">                            
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">Customer</div>
                                            <table class="table">
                                                <tr>
                                                    <th>Name</th>
                                                    <td>
                                                        <asp:Literal ID="litCIName" runat="server"></asp:Literal>
                                                        <asp:TextBox ID="txtCIName" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvCIName" runat="server" ControlToValidate="txtCIName" Display="Dynamic" ValidationGroup="vgOrderDetails" ErrorMessage="First name is required." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Name is required.</span>"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>Email</th>
                                                    <td>
                                                        <asp:Literal ID="litCIEmail" runat="server"></asp:Literal>
                                                        <asp:TextBox ID="txtCIEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvCIEmail" runat="server" ControlToValidate="txtCIEmail" Display="Dynamic" ValidationGroup="vgOrderDetails" ErrorMessage="Email is required." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Email is required.</span>"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>Contact number</th>
                                                    <td>
                                                        <asp:Literal ID="litCIContactNumber" runat="server"></asp:Literal>
                                                        <asp:TextBox ID="txtCIContactNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>Date of birth</th>
                                                    <td>
                                                        <asp:Literal ID="litCIDOB" runat="server"></asp:Literal>
                                                        <asp:TextBox ID="txtCIDOB" runat="server" CssClass="form-control date"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <asp:PlaceHolder ID="phCreateAccount" runat="server">
                                                    <tr>
                                                        <th>Create customer account?<small class="text-navy clearfix">(it will automatically save address)</small></th>
                                                        <td><asp:CheckBox ID="chkCreateAccount" runat="server" /></td>
                                                    </tr>
                                                </asp:PlaceHolder>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>                
                        </div>
                        <div id="promo" class="tab-pane">                            
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <table class="table table-bordered">
                                            <tr>
                                                <th>Promo code</th>
                                                <td>
                                                    <asp:TextBox ID="txtPromoCode" runat="server" CssClass="form-control"></asp:TextBox><br />
                                                    <asp:LinkButton ID="lbApplyPromo" runat="server" Text="Apply promo" CssClass="btn btn-outline btn-danger" OnClick="lbApplyPromo_Click"></asp:LinkButton>&nbsp;
                                                    <asp:LinkButton ID="lbRemovePromo" runat="server" Text="Remove promo" CssClass="btn btn-outline btn-info" OnClick="lbRemovePromo_Click"></asp:LinkButton>
                                                </td>
                                            </tr>
                                            <asp:PlaceHolder ID="phLoyalty" runat="server">
                                                <tr>
                                                    <th>Loyalty points available</th>
                                                    <td><asp:Literal ID="litLoyaltyPointsAvail" runat="server"></asp:Literal></td>
                                                </tr>
                                                <tr>
                                                    <th>Loyalty points to use</th>
                                                    <td>
                                                        <asp:TextBox ID="txtLoyaltyPointsToUse" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RangeValidator Type="Integer" runat="server" ControlToValidate="txtLoyaltyPointsToUse" ID="valLoyaltyPoints" ValidationGroup="vgPoints" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>"></asp:RangeValidator>
                                                        <br />
                                                        <asp:LinkButton ID="lbApplyPoints" runat="server" Text="Apply points" CssClass="btn btn-outline btn-danger" ValidationGroup="vgPoints" CausesValidation="true" OnClick="lbApplyPoints_Click"></asp:LinkButton>&nbsp;
                                                        <asp:LinkButton ID="lbClearPoints" runat="server" Text="Clear points" CssClass="btn btn-outline btn-info" OnClick="lbClearPoints_Click"></asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </asp:PlaceHolder>
                                        </table>
                                    </div>
                                </div>                                
                            </div>
                        </div>
                        <div id="pharm" class="tab-pane">
                            <div class="panel-body">
                            <asp:PlaceHolder ID="phPharm" runat="server">
                                <p class="text-danger">
                                    Pharmacy medicines are sold at the professional discretion of our pharmacist.                           
                                    Please answer the following questions accurately to allow our pharmacist to check that
                                    the medicines in your basket are suitable for the intended user.
                                    All information provided will be treated confidentially and securely.
                                </p>
                                <table class="table">
                                    <tr>
                                        <th>Will all the medication in your basket be taken by you?</th>
                                        <td><asp:CheckBox ID="cbTakenByOwner" runat="server" CssClass="takenOwner" /></td>
                                    </tr>
                                    <tr>
                                        <th>Please provide information on any known allergies.</th>
                                        <td><asp:TextBox ID="txtAllergy" TextMode="MultiLine" CssClass="form-control" Height="100px" runat="server"></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <th>How old are you?</th>
                                        <td>
                                            <asp:DropDownList ID="ddlOwnerAge" runat="server" CssClass="form-control"></asp:DropDownList>
                                            <asp:RequiredFieldValidator CssClass="rfvAge" runat="server" InitialValue="" ControlToValidate="ddlOwnerAge" ValidationGroup="vgOrderDetails" ErrorMessage="Please enter age of patient." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Do you have any existing conditions or are you taking other medication?</th>
                                        <td><asp:CheckBox ID="cbOtherCondOwner" Checked="false" runat="server" CssClass="otherCond" /></td>
                                    </tr>
                                    <tr id="qOwnerCondition" style="display: none;">
                                        <th>Please provide information about your conditions or other medication.</th>
                                        <td><asp:TextBox ID="txtOwnerOtherCond" TextMode="MultiLine" CssClass="form-control" Height="100px" runat="server"></asp:TextBox></td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="rptPharmItems" runat="server">
                                    <ItemTemplate>
                                        <asp:PlaceHolder ID="phItemArea" runat="server">
                                            <div class="panel panel-info">
                                                <div class="panel-heading">
                                                    <asp:HiddenField ID="hfProductId" runat="server" Value='<%# Eval("ProductId") %>' />
                                                    <asp:HiddenField ID="hfProductPriceId" runat="server" Value='<%# Eval("ProductPriceId") %>' />
                                                    <b><%# Eval("Product.Name") %></b>
                                                </div>                                                
                                                <table class="table">
                                                    <tr><td colspan="2"><%# CheckForCodeineMessage(Convert.ToInt32(Eval("ProductId"))) %></td></tr>
                                                    <tr>
                                                        <th>What symptoms are going to be treated with this medication?</th>
                                                        <td>
                                                            <asp:TextBox ID="txtSymptom" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ValidationGroup="vgOrderDetails" ControlToValidate="txtSymptom" ErrorMessage='<%# "Please enter symptoms for " + Eval("Product.Name") + "." %>' Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>What other medicines has the intended user tried for the symptom?</th>
                                                        <td>
                                                            <asp:TextBox ID="txtMedForSymptom" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ValidationGroup="vgOrderDetails" ControlToValidate="txtMedForSymptom" ErrorMessage='<%# "Please enter other medicines for the symptoms for " + Eval("Product.Name") + "." %>' Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>How long have the symptoms persisted for?</th>
                                                        <td>
                                                            <asp:DropDownList ID="ddlPersistedDays" OnInit="ddlPersistedDays_Init" runat="server" CssClass="form-control"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator CssClass="rfvAge" runat="server" ControlToValidate="ddlPersistedDays" ValidationGroup="vgOrderDetails" InitialValue="" ErrorMessage='<%# "Please enter persisted in days for " + Eval("Product.Name") + "." %>' Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>What action has been taken to treat this condition?</th>
                                                        <td>
                                                            <asp:TextBox ID="txtActionTaken" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtActionTaken" ValidationGroup="vgOrderDetails" ErrorMessage='<%# "Please enter action that has been taken to treat this condition for " + Eval("Product.Name") + "." %>' Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>                                   
                                                    <tr class="age">
                                                        <th>What is the age of the person who will be using this product?</th>
                                                        <td>
                                                            <asp:DropDownList ID="ddlAge" OnInit="ddlAge_Init" runat="server" CssClass="form-control"></asp:DropDownList>
                                                            <asp:RequiredFieldValidator CssClass="rfvAge" runat="server" ControlToValidate="ddlAge" ValidationGroup="vgOrderDetails" InitialValue="" ErrorMessage='<%# "Please enter age for " + Eval("Product.Name") + "." %>' Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="hasOtherCondition">
                                                        <th>Does the intended user have any existing conditions or taking other medication?</th>
                                                        <td><asp:CheckBox ID="cbOtherCond" Checked="false" runat="server" /></td>
                                                    </tr>
                                                    <tr class="otherCondition" style="display: none;">
                                                        <th>Please provide information about conditions or other medication:</th>
                                                        <td>
                                                            <asp:TextBox ID="txtOtherCond" TextMode="MultiLine" CssClass="form-control" Height="100px" runat="server"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" CssClass="rfvOtherCond" ValidationGroup="vgOrderDetails" ControlToValidate="txtOtherCond" ErrorMessage='<%# "Please enter conditions or other medication for " + Eval("Product.Name") + "." %>' Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>"></asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="hasTaken">
                                                        <th>Has the intended user taken this medication before?</th>
                                                        <td><asp:CheckBox ID="cbHasTaken" runat="server" /></td>
                                                    </tr>
                                                    <tr class="takenQuantity forHasTaken" style="display: none;">
                                                        <th>On how many different occasions has the intended user taken this medication?</th>
                                                        <td>
                                                            <asp:DropDownList ID="ddlTakenQuantity" runat="server" CssClass="form-control">
                                                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                                                <asp:ListItem Text="9" Value="9"></asp:ListItem>
                                                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                                <asp:ListItem Text="> 10" Value="> 10"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <tr class="lastTimeTaken forHasTaken" style="display: none;">
                                                        <th>When was the last time the intended user took this medication?</th>
                                                        <td><asp:TextBox runat="server" ID="txtLastTimeTaken" CssClass="form-control date"></asp:TextBox></td>
                                                    </tr>
                                                </table>                                    
                                            </div>
                                        </asp:PlaceHolder>
                                    </ItemTemplate>
                                </asp:Repeater>               
                            </asp:PlaceHolder>
                            </div>
                        </div>
                        <div id="notes" class="tab-pane">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtNotes" TextMode="MultiLine" Rows="6" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="payment" class="tab-pane">
                            <div class="panel-body">
                                <div class="row">
                                    <div class="col-lg-6">
                                        <div class="panel panel-info">
                                            <div class="panel-heading">Shipping methods</div>
                                            <div class="panel-body">
                                                <table class="table">
                                                    <tr class="text-danger">
                                                        <th>Estimated weight</th>
                                                        <td><asp:Literal ID="ltEstimatedWeight" runat="server"></asp:Literal>g</td>
                                                    </tr>
                                                </table>
                                                <asp:RadioButtonList ID="rblDeliveryRate" runat="server" DataTextField="Description" CssClass="radDelMethod table" DataValueField="Id" AutoPostBack="true" OnSelectedIndexChanged="rblDeliveryRate_SelectedIndexChanged"></asp:RadioButtonList>
                                            </div>
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="phPaymentMethod" runat="server">
                                        <div class="col-lg-6">
                                            <div class="panel panel-danger">
                                                <div class="panel-heading">Card payment</div>
                                                <table class="table">
                                                    <tr>
                                                        <th>Total amount</th>
                                                        <td class="text-primary h3"><asp:Literal ID="ltlTotalValue" runat="server"></asp:Literal></td>
                                                    </tr>
                                                    <tr>
                                                        <th>Card type<strong>*</strong></th>
                                                        <td><asp:DropDownList ID="ddlPaymentType" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <th>Card number<strong>*</strong></th>
                                                        <td>
                                                            <asp:TextBox ID="txtCardNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ValidationGroup="vgPayment" ErrorMessage="Card number is required." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>" ControlToValidate="txtCardNumber" Display="Dynamic" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>
                                                            Cardholder's name<strong>*</strong><br />
                                                            <small class="text-navy">(as shown on card)</small>
                                                        </th>
                                                        <td>
                                                            <asp:TextBox ID="txtCardHolderName" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ValidationGroup="vgPayment" ErrorMessage="Card holder name is required." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>" ControlToValidate="txtCardHolderName" Display="Dynamic" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>Security code<strong>*</strong></th>
                                                        <td>
                                                            <asp:TextBox ID="txtVerificationNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                                            <asp:RequiredFieldValidator runat="server" ValidationGroup="vgPayment" ErrorMessage="Security code is required." Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i></span>" ControlToValidate="txtVerificationNumber" Display="Dynamic" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>Expiry date<strong>*</strong></th>
                                                        <td>
                                                            <asp:DropDownList ID="ddlExpiryDateMM" runat="server" CssClass="form-control pull-left" Width="70px">
                                                                <asp:ListItem Text="01" Value="01"></asp:ListItem>
                                                                <asp:ListItem Text="02" Value="02"></asp:ListItem>
                                                                <asp:ListItem Text="03" Value="03"></asp:ListItem>
                                                                <asp:ListItem Text="04" Value="04"></asp:ListItem>
                                                                <asp:ListItem Text="05" Value="05"></asp:ListItem>
                                                                <asp:ListItem Text="06" Value="06"></asp:ListItem>
                                                                <asp:ListItem Text="07" Value="07"></asp:ListItem>
                                                                <asp:ListItem Text="08" Value="08"></asp:ListItem>
                                                                <asp:ListItem Text="09" Value="09"></asp:ListItem>
                                                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                                            </asp:DropDownList> 
                                                            <asp:DropDownList ID="ddlExpiryDateYY" runat="server" OnInit="ddlExpiryDateYY_Init" CssClass="form-control pull-left" Width="100px"/>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>Start date</th>
                                                        <td>
                                                            <asp:DropDownList ID="ddlStartDateMM" runat="server" CssClass="form-control pull-left" Width="70px">
                                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                                <asp:ListItem Text="01" Value="01"></asp:ListItem>
                                                                <asp:ListItem Text="02" Value="02"></asp:ListItem>
                                                                <asp:ListItem Text="03" Value="03"></asp:ListItem>
                                                                <asp:ListItem Text="04" Value="04"></asp:ListItem>
                                                                <asp:ListItem Text="05" Value="05"></asp:ListItem>
                                                                <asp:ListItem Text="06" Value="06"></asp:ListItem>
                                                                <asp:ListItem Text="07" Value="07"></asp:ListItem>
                                                                <asp:ListItem Text="08" Value="08"></asp:ListItem>
                                                                <asp:ListItem Text="09" Value="09"></asp:ListItem>
                                                                <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                                                <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                                                <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                                            </asp:DropDownList>
                                                            &nbsp;
                                                            <asp:DropDownList ID="ddlStartDateYY" runat="server" OnInit="ddlStartDateYY_Init" CssClass="form-control pull-left"  Width="100px"/>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th>
                                                            Issue number<br />
                                                            <small class="text-navy">(leave blank if it's not available)</small>
                                                        </th>
                                                        <td><asp:TextBox ID="txtIssueNumber" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="summary col-lg-3">
                <div class="panel panel-primary">
                    <div class="panel-heading">Order currency</div>                
                    <table class="table">
                        <tr>
                            <th>Currency code</th>
                            <td>
                                <asp:DropDownList CssClass="form-control" ID="ddlCurrency" OnInit="ddlCurrency_Init" DataTextField="CurrencyCode" DataValueField="CurrencyCode" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCurrency_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="panel panel-warning">
                    <div class="panel-heading">Order totals</div>
                    <table class="table">
                        <tr>
                            <th>Subtotal (excluding tax)</th>
                            <td><asp:Literal ID="litTotalsSubTot" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>Discount Offer</th>
                            <td><asp:Literal ID="litTotalsDiscount" runat="server"></asp:Literal></td>
                        </tr>
                        <asp:Literal ID="litDiscounts" runat="server"></asp:Literal>
                        <tr>
                            <th>Loyalty Point</th>
                            <td><asp:Literal ID="litTotalsPoints" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>Tax</th>
                            <td><asp:Literal ID="litTotalsVAT" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>Delivery</th>
                            <td><asp:Literal ID="litTotalsDelivery" runat="server"></asp:Literal></td>
                        </tr>
                        <tr>
                            <th>Grand Total</th>
                            <td><asp:Literal ID="litTotalsTotal" runat="server"></asp:Literal></td>
                        </tr>                        
                    </table>
                </div>
                <div class="col-lg-12">
                    <div class="row">
                        <div class="pull-right">
                            <asp:LinkButton CssClass="btn btn-default" ID="lbRefreshOrder" runat="server" OnClick="lbRefreshOrder_Click">Refresh</asp:LinkButton>
                            <asp:LinkButton CssClass="btn btn-primary" ID="lbPlaceOrder" OnClientClick="validate('vgOrderDetails', 'vgPayment');" runat="server" OnClick="lbPlaceOrder_Click">Place order</asp:LinkButton>                                
                            <asp:LinkButton CssClass="btn btn-danger" ID="lbSaveOrder" OnClientClick="validate('vgOrderDetails');" runat="server" OnClick="lbSaveOrder_Click" CausesValidation="true" ValidationGroup="vgOrderDetails">Save order</asp:LinkButton>                            
                        </div>                        
                        <div class="pull-right space-15">
                            <asp:LinkButton CssClass="btn btn-warning" ID="lbSaveOrderAndDeleteBasket" OnClientClick="validate('vgOrderDetails');" runat="server" OnClick="lbSaveOrderAndDeleteBasket_Click" CausesValidation="true" ValidationGroup="vgOrderDetails">Save order and delete basket</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
