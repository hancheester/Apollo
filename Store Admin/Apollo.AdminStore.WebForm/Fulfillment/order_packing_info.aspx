<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" ValidateRequest="false" Inherits="Apollo.AdminStore.WebForm.FulFillment.order_packing_info" Codebehind="order_packing_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderHeader" Src="~/UserControls/OrderHeaderControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="AddressView" Src="~/UserControls/OrderAddressViewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="ShippingNew" Src="~/UserControls/OrderShippingNewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript">
        //<![CDATA[
        var disabled = true;

        $(function () {
            if ($('.submit').length > 0) {
                $('.submit').attr('href', $('.submit').attr('href').replace('javascript:', "javascript:alert('Please print out D-Note first.');//"));
                $('.submit').css('text-decoration', 'line-through');
            }

            $('.printNote').click(function () {
                window.print();
                $('.submit').attr('href', $('.submit').attr('href').replace("alert('Please print out D-Note first.');//", ""));
                $('.submit').css('text-decoration', 'none');
            });

            $('.submitPayment').click(function () {
                setTimeout('delayedHide()', 1);
            });

            $('#stampTypeList input[type=radio]').click(function () {                
                processStampSelection($('#stampTypeList input[type=radio]:checked').val());
            });

            var stamp = $('input:radio[name=stampType]:checked').val();
            processStampSelection(stamp);

            $('#cn22WeightValue').html($('#cn22Weight').val());
            $('#cn22OrderValue').html($('#cn22Value').val());

            // Load pre-selected shipping type
            var shippingType = getQueryVariable('type');
            var selectedCountryId = getQueryVariable('countryid');

            if (shippingType)
                $('#delivery-options').val(shippingType);

            if (shippingType == "intl_standard_delivery") {
                $('#delivery-options').val('intl_standard_delivery');
                $('.std-del-countries').css({ 'display': 'inline', 'visibility': 'visible' });
                $('.std-del-countries select').css({ 'float': 'left', 'margin-right': '5px', 'width': '130px' });
                $('.std-del-countries select').val(selectedCountryId);
            }
            else if (shippingType == "intl_tracked_delivery") {
                $('#delivery-options').val('intl_tracked_delivery');
                $('.std-del-countries').css({ 'display': 'inline', 'visibility': 'visible' });
                $('.std-del-countries select').css({ 'float': 'left', 'margin-right': '5px', 'width': '130px' });
                $('.std-del-countries select').val(selectedCountryId);
            }

            $('#cn22Desc').keyup(function () {
                $('#cn22Content').html($('#cn22Desc').val());
            });

            $("#cn22Desc").bind('paste', function (e) {
                var elem = $(this);

                setTimeout(function () {
                    // gets the copied text after a specified time (100 milliseconds)
                    $('#cn22Content').html(elem.val());                    
                }, 100);
            });
        });

        function processStampSelection(stamp) {
            $('.cn22').removeClass('invisible');
            $('.stamp').removeClass('invisible');
            $('.return').removeClass('invisible');
            $('.address').removeClass('invisible');
            $('.address').removeClass('hideBorder');
            $('#stampLogo').removeClass('invisible');
            $('#stampLogo').attr('src', '/img/1st_class_royal_mail.jpg');

            if (stamp == 'firstClass') {
                console.log('firstClass');
                $('.cn22').addClass('invisible');
                $('.address').addClass('hideBorder');
            }
            else if (stamp == 'secondClass') {
                console.log('secondClass');
                $('.cn22').addClass('invisible');
                $('.address').addClass('hideBorder');
                $('#stampLogo').attr('src', '/img/2nd_class_royal_mail.jpg');
            }
            else if (stamp == 'addressCn22') {
                console.log('addressCn22');
                $('#stampLogo').addClass('invisible');
            }
            else if (stamp == 'addressReturn') {
                console.log('addressReturn');
                $('.cn22').addClass('invisible');
                $('#stampLogo').addClass('invisible');
            }
            else if (stamp == 'none') {
                console.log('none');
                $('.stamp').addClass('invisible');
                $('.address').addClass('invisible');
                $('.cn22').addClass('invisible');
                $('.return').addClass('invisible');
            }
        }

        function receiveAlert(arg, context) {
            if (arg != '') {
                $('.alertBox').html(arg);
                $('.alertBox').css('display', 'block');
            }
            else {
                $('.alertBox').html('');
                $('.alertBox').css('display', 'none');
            }
        }

        function delayedHide() {
            var $btnTop = $('#<%= lbProcessPaymentTop.ClientID %>');	        
            $btnTop.replaceWith("<a class='btn btn-sm btn-success'>Please wait...</a>");

            var $btnBottom = $('#<%= lbProcessPaymentBottom.ClientID %>');	        
            $btnBottom.replaceWith("<a class='btn btn-sm btn-success'>Please wait...</a>");
	    }

	    function getWeightInput(event) {
	        $('#cn22WeightValue').html($('#cn22Weight').val());
	    }

	    function getValueInput(event) {
	        $('#cn22OrderValue').html($('#cn22Value').val());
	    }

	    function checkOption(trigger) {
	        $('.std-del-countries').css({ 'display': 'none', 'visibility': 'hidden' });
	        $('.std-del-countries select').removeAttr('style');

	        for (var i = 0; i < trigger.options.length; i++) {
	            if (trigger.options[i].selected == true && trigger.options[i].value == 'intl_standard_delivery') {
	                $('.std-del-countries').css({ 'display': 'inline', 'visibility': 'visible' });
	                $('.std-del-countries select').css({ 'float': 'left', 'margin-right': '5px', 'width': '130px' });
	            }
	            else if (trigger.options[i].selected == true && trigger.options[i].value == "intl_tracked_delivery") {
	                $('.std-del-countries').css({ 'display': 'inline', 'visibility': 'visible' });
	                $('.std-del-countries select').css({ 'float': 'left', 'margin-right': '5px', 'width': '130px' });
	            }
	        }
	    }

	    function getQueryVariable(variable) {
	        var query = window.location.search.substring(1);
	        var vars = query.split('&');
	        for (var i = 0; i < vars.length; i++) {
	            var pair = vars[i].split('=');
	            if (pair[0] == variable) { return pair[1]; }
	        }
	        return false;
	    }

	    //]]>
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading hidden-print">
        <div class="col-lg-8">
            <h2>Order Packing</h2>
            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        </div>
        <div class="col-lg-4 hidden-print">
            <div class="row"><p></p></div>
            <div class="pull-right">
                <span class="form-inline">
                    Order ID: <asp:TextBox ID="txtGoOrderId" CssClass="form-control input-sm" runat="server"></asp:TextBox>
                    <asp:LinkButton ID="lbGo" runat="server" Text="Go" OnClick="lbGo_Click" CssClass="btn btn-outline btn-sm btn-primary"></asp:LinkButton>
                </span>
            </div>
        </div>
    </div>

    <div class="row wrapper hidden-print">
        <div class="col-lg-12">
            <div class="row"><p></p></div>
            <div class="row">
                <div class="pull-right">
                    <a href="/fulfillment/order_fulfillment.aspx" class="btn btn-sm btn-default printHide">Back</a>
                    <asp:LinkButton ID="lbProcessPaymentTop" runat="server" CssClass="btn btn-sm btn-success submitPayment" Text="Process payment" OnClick="ProcessPayment"></asp:LinkButton>
                    <asp:PlaceHolder ID="phAfterPaymentTop" runat="server">
                        <asp:LinkButton ID="lbSubmitTop" runat="server" CssClass="btn btn-sm btn-info submit" ValidationGroup="vgNewShipment" Text="Submit" OnClick="lbSubmit_Click"></asp:LinkButton>
                        <a href="javascript:;" onclick="window.print(); $('.submit').attr('href', $('.submit').attr('href').replace('alert('Please print out D-Note first.');//', '')); $('.submit').css('text-decoration', 'none');" class="btn btn-sm btn-warning printNote">Print D-Note</a>  
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight hidden-print">
        <div class="row">
            <div class="col-lg-12">
                <div class="alertBox alert alert-warning" style="display: none;"></div>
                <Apollo:NoticeBox ID="enbInfo" runat="server"/>
                <asp:ValidationSummary runat="server" ValidationGroup="vgNewShipment" CssClass="alert alert-warning" />
                <div class="row">
                    <div class="col-lg-6">
                        <Apollo:OrderHeader ID="eohHeader" runat="server" ShowExtraInfo="false" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-6">
                        <Apollo:AddressView ID="eavShipping" AddressType="Shipping" HideSystemCheck="true" runat="server" Title="Shipping Address" EditDisabled="true" />
                    </div>

                    <div class="col-lg-6">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                Stamp
                            </div>
                            <table class="table">
                                <tr>
                                    <th>Type</th>
                                    <td>
                                        <div id="stampTypeList">
                                            <asp:Literal ID="ltlFirstClass" runat="server"></asp:Literal><br />
                                            <input type="radio" name="stampType" value="secondClass" /> 2nd class<br />
                                            <asp:Literal ID="ltlInternational" runat="server"></asp:Literal><br />
                                            <input type="radio" name="stampType" value="addressCn22" /> Address &amp; CN22 with return<br />
                                            <input type="radio" name="stampType" value="addressReturn" /> Address with return<br />                                            
                                            <input type="radio" name="stampType" value="none" /> None
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Weight in grams <small class="text-navy clearfix">(international only)</small></th>
                                    <td><asp:Literal ID="ltlWeightGrams" runat="server"></asp:Literal></td>
                                </tr>
                                <tr>
                                    <th>Total value <small class="text-navy clearfix">(excluding VAT, international only)</small></th>
                                    <td><asp:Literal ID="ltlNetValue" runat="server"></asp:Literal></td>
                                </tr>
                                <tr>
                                    <th>Deminimis Value <small class="text-navy clearfix">(minimum customs value for duties and taxes to apply)</small></th>
                                    <td><asp:Literal ID="ltlDeminimis" runat="server"></asp:Literal></td>
                                </tr>
                                <tr>
                                    <th>Description <small class="text-navy clearfix">(international only)</small></th>
                                    <td><textarea id="cn22Desc" rows="3" cols="30" class="form-control"></textarea></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
                            
                <div class="row">
                    <div class="col-lg-12">
                    <asp:PlaceHolder ID="phItems" runat="server">
                        <div class="panel panel-default">
                            <div class="panel-heading">Items to ship</div>
                            <asp:Repeater ID="rptItems" runat="server">
                                <HeaderTemplate>
                                    <table class="table table-striped">
                                        <tr>
                                            <th><span style="display:none;" class="printShow">Quantity</span><span class="printHide">Qty to Ship</span></th>                                                        
                                            <th>Name</th>
                                            <th>Giftwrap</th>
                                            <th>Option</th>                                            
                                            <th><span class="printHide">Image</span><span style="display:none;" class="printShow">Price</span></th>
                                            <th>Restricted Group</th>                                                                
                                        </tr>                            
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <asp:PlaceHolder ID="phItem" runat="server" Visible='<%# Eval("StatusCode").ToString() != LineStatusCode.GOODS_ALLOCATED ? false: true %>'>                          
                                        <tr>
                                            <td class="printRowText" style="width: 50px; text-align: center; font-size: 10em; color: #FF0000;">
                                                <asp:HiddenField ID="hfPendingQuantity" runat="server" Value='<%# Eval("PendingQuantity") %>' />
                                                <asp:HiddenField ID="hfAllocatedQuantity" runat="server" Value='<%# Eval("AllocatedQuantity") %>' />
                                                <span id='qtyToShip_<%# Eval("Barcode") %>'><%# Eval("AllocatedQuantity")%></span>
                                            </td>
                                            <td class="printHide" style="display:none; width: 50px; text-align: center; font-size: 10em; color: #0800FF;">
                                                <span id='qtyScanned_<%# Eval("Barcode") %>'>0</span>
                                            </td>
                                            <td class="printRowText" style="font-size: 2em;">
                                                <asp:HiddenField ID="hfName" runat="server" Value='<%# Eval("Name") %>' />
                                                <asp:HiddenField ID="hfLineItemId" runat="server" Value='<%# Eval("Id") %>' />
                                                <%# Eval("Name") %><br />
                                                <span class="printHide" style="color: red;"><%# CheckIfDifferentFromRRP( Eval("CurrencyCode").ToString(), Convert.ToDecimal(Eval("ExchangeRate")), Convert.ToDecimal(Eval("InitialPriceInclTax")), Convert.ToDecimal(Eval("PriceInclTax"))) %></span><br />
                                                <span class="printHide" style="color: red;"><%# Eval("Note") %></span><br />
                                                <span class="printHide">Barcode: <%# Eval("Barcode") %></span>                                            
                                            </td>
                                            <td class="printHide" style="font-size: 2em;"><%# Convert.ToBoolean(Eval("FreeWrapped")) ? "<span style='color: #FF0000;'>Yes</span>" : "No" %></td>
                                            <td class="printRowText" style="font-size: 2em;"><%# Eval("Option") %>&nbsp;</td>
                                            <td class="printRowText">
                                                <span class="printHide"><a target="_blank" href="<%# AdminStoreUtility.GetProductUrl(Eval("UrlRewrite").ToString()) %>"><img style="border:0;" alt='<%# Eval("Name") %>' src="/get_image_handler.aspx?type=media&product_id=<%# Eval("ProductId").ToString().Replace("~", string.Empty) %>" /></a></span>
                                                <div class="label label-danger center-block"><h4><%# AdminStoreUtility.GetFormattedPrice(Eval("PriceInclTax"), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity, Convert.ToDecimal(Eval("ExchangeRate"))) %></h4></div>
                                                <div class="label label-warning center-block"><h4><%# AdminStoreUtility.GetFormattedPrice(Eval("InitialPriceInclTax"), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity) %> (RRP)</h4></div>
                                                <div class="label label-info center-block"><h4><%# AdminStoreUtility.GetFormattedPrice(Eval("PriceExclTax"), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity, Convert.ToDecimal(Eval("ExchangeRate"))) %> (VAT excluded)</h4></div>
                                            </td>
                                            <td>
                                                <%# Eval("RestrictedGroup") %>
                                            </td>
                                        </tr>     
                                        </asp:PlaceHolder>                         
                                </ItemTemplate>                            
                                <FooterTemplate>                        
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:PlaceHolder>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-6">
                        <asp:Repeater ID="rptComments" runat="server">
                            <HeaderTemplate>
                                <div class="panel panel-default">
                                    <div class="panel-heading">
                                        Comments (Last 5 entries)
                                    </div>
                                    <table class="table">
                            </HeaderTemplate>
                            <ItemTemplate>
                                    <tr><td class="text-danger"><%# Eval("DateStamp") %> by <%# Eval("FullName") %></td></tr>
                                    <tr><td><%# Eval("CommentText") %></td></tr>
                                    <tr><td>&nbsp;</td></tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                    <tr><td class="text-danger"><%# Eval("DateStamp") %> by <%# Eval("FullName") %></td></tr>
                                    <tr><td><%# Eval("CommentText") %></td></tr>
                                    <tr><td>&nbsp;</td></tr>
                            </AlternatingItemTemplate>
                            <FooterTemplate>
                                    <asp:PlaceHolder runat="server" Visible='<%# HasMoreComments() %>'>
                                        <tr>
                                            <td><a class="btn btn-outline btn-info pull-right" target="_blank" href="/sales/order_comments_default.aspx?orderid=<%= QueryOrderId %>">More</a></td>    
                                        </tr>
                                    </asp:PlaceHolder>
                                </table>
                                </div>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>

                    <div class="col-lg-6">
                        <asp:PlaceHolder ID="phNewCommentSection" runat="server">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    Action
                                </div>                                    
                                <table class="table">
                                    <tr>
                                        <th>Status</th>
                                        <td>
                                            <asp:DropDownList ID="ddlOrderStatus" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Order Placed" Value="OP"></asp:ListItem>
                                                <asp:ListItem Text="Awaiting Reply" Value="AR"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Progress</th>
                                        <td><asp:DropDownList ID="ddlIssue" runat="server" OnInit="ddlIssue_Init" DataTextField="Issue" DataValueField="IssueCode" CssClass="form-control"/></td>
                                    </tr>
                                    <tr>
                                        <th>Comment</th>
                                        <td>
                                            <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Height="100" CssClass="form-control"></asp:TextBox>
                                            <asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtComment" runat="server" ValidationGroup="vgNewComment" ErrorMessage="Comment is required."
                                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <th>Notify Customer</th>
                                        <td><asp:CheckBox ID="cbNotifyCust" runat="server" Enabled="false" /></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td><asp:LinkButton ID="lbSubmitComment" ValidationGroup="vgNewComment" runat="server" Text="Submit" CssClass="btn btn-outline btn-info pull-right" OnClick="lbSubmitComment_Click"></asp:LinkButton></td>
                                    </tr>
                                </table>                                        
                            </div>
                        </asp:PlaceHolder>  
                    </div>

                    <div class="col-lg-6">
                        <Apollo:ShippingNew runat="server" ID="esnShipping" />
                    </div>

                    <div class="col-lg-6">
                        <div class="pull-right">
                            <a href="/fulfillment/order_fulfillment.aspx" class="btn btn-sm btn-default printHide">Back</a>
                            <asp:LinkButton ID="lbProcessPaymentBottom" runat="server" CssClass="btn btn-sm btn-success submitPayment" Text="Process payment" OnClick="ProcessPayment"></asp:LinkButton>
                            <asp:PlaceHolder ID="phAfterPaymentBottom" runat="server">
                                <asp:LinkButton ID="lbSubmitBottom" runat="server" CssClass="btn btn-sm btn-info submit" ValidationGroup="vgNewShipment" Text="Submit" OnClick="lbSubmit_Click"></asp:LinkButton>
                                <a href="javascript:;" class="btn btn-sm btn-warning printNote">Print D-Note</a>  
                            </asp:PlaceHolder>
                        </div>
                    </div>

                </div>                
            </div>
        </div>
    </div>

    <div class="row visible-print-inline-block">
        <table class="dnote">
            <tr>
                <td colspan="2">
                    <h4>Delivery Note</h4>
                </td>
            </tr>
            <tr>
                <td class="info">
                    <div class="orderNumber"><asp:Literal ID="ltOrderNumber" runat="server"></asp:Literal></div>
                    <asp:Literal runat="server" ID="ltlHeaderAddress"></asp:Literal>                        
                </td>
                <td>
                    <div class="stamp">
                        <div class="cn22">
                            <span class="title">CN22</span>
                            <span class="description">
                                MAY BE OPENED OFFICIALLY<br />                                
                                DETAILED DESCRIPTION OF CONTENTS:
                            </span>
                            <br />
                            <span id="cn22Content"></span><br />                                
                            Gross weight: <span id="cn22WeightValue"></span>g<br />
                            Value: <span id="cn22OrderValue"></span><br /><br />
                            Gift<span class="merchandise">Merchandise [ X ]</span><br />
                            <br />
                            I certify that this item does not contain any dangerous article or contents prohibited by postal regulations.<br />
                            <div class="box">
                                <asp:Literal runat="server" ID="ltlSignature"></asp:Literal><span class="pull-right"><asp:Literal ID="ltlCN22Date" runat="server"></asp:Literal></span>
                            </div>                                    
                        </div>
                        <div class="address">
                            <asp:Literal runat="server" ID="ltlDeliveryAddr"></asp:Literal>                                
                            <br />
                            <asp:Literal runat="server" ID="ltContactNumber"></asp:Literal>
                        </div>
                        <div>
                            <span class="stamp"><img id="stampLogo" src="/img/1st_class_royal_mail.jpg" alt="type" width="210" height="54" /></span>
                            <br />
                            <br />
                            <br />                            
                            <br />
                            <br />
                            <br />
                            <br />
                            <div class="return">
                                <asp:Literal ID="ltOrderNumberReturn" runat="server"></asp:Literal><br />
                                If undelivered return to:<br />
                                Apollo.co.uk, Unit 27, Orbital 25 Business Park<br />
                                Dwight Road, Watford, Herts WD18 9DA, United Kingdom
                            </div>
                        </div>
                    </div>                        
                </td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Repeater ID="rptDnoteItems" runat="server">
                        <HeaderTemplate>
                            <table class="table table-bordered">
                                <tr>
                                    <th>Quantity</th>
                                    <th>Item Description</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                                <asp:PlaceHolder runat="server" Visible='<%# Eval("StatusCode").ToString() != LineStatusCode.GOODS_ALLOCATED ? false: true %>'>
                                <tr>
                                    <td>
                                        <%# Eval("AllocatedQuantity") %>
                                    </td>
                                    <td>
                                        <%# Eval("Name") %> <%# Eval("Option") %> <%# Eval("CodeinMessage") == null ? null : string.Format("<br/><small>{0}</small>", Eval("CodeinMessage")) %>
                                    </td>
                                </tr>
                                </asp:PlaceHolder>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </table>
            
        <div>
            <br />
            Thank you for shopping at <%= StoreInformationSettings.CompanyName %>!<br />
            <asp:PlaceHolder ID="phEarnPoint" runat="server">You have earned <asp:Literal ID="ltlPointValue" runat="server"></asp:Literal> points from this purchase to spend on your next purchase with <%= StoreInformationSettings.CompanyName %>.</asp:PlaceHolder>
            <br />
            <br />
            <br />
            <div style="text-align: center;">
                Apollo.co.uk,<br />
                Unit 27, Orbital 25 Business Park,<br />
                Dwight Road,<br />
                Watford,<br />
                Herts WD18 9DA</div>
        </div>
    </div>
</asp:Content>