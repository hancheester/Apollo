<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" ViewStateEncryptionMode="Never" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Report.report_orders" Codebehind="report_orders.aspx.cs" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="phHeaderScript" runat="server">
    <script type="text/javascript">
                
        function showNoOrders(hour, day, week, month, quarter, year, noneu, type, tick) {    
            <%= ClientScript.GetCallbackEventReference(this, "'noOrders_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function showPromoDiscount(hour, day, week, month, quarter, year, noneu, type, tick) {
            <%= ClientScript.GetCallbackEventReference(this, "'promoDiscount_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function showLoyaltyDiscount(hour, day, week, month, quarter, year, noneu, type, tick) {
            <%= ClientScript.GetCallbackEventReference(this, "'loyaltyDiscount_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function showLineCostPrice(hour, day, week, month, quarter, year, noneu, type, tick) {
            <%= ClientScript.GetCallbackEventReference(this, "'lineCostPrice_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function showMargin(hour, day, week, month, quarter, year, noneu, type, tick) {
            <%= ClientScript.GetCallbackEventReference(this, "'margin_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function showShipping(hour, day, week, month, quarter, year, noneu, type, tick) {
            <%= ClientScript.GetCallbackEventReference(this, "'shipping_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function showOrderAverageValue(hour, day, week, month, quarter, year, noneu, type, tick) {
            <%= ClientScript.GetCallbackEventReference(this, "'orderAverageValue_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function showOrderValue(hour, day, week, month, quarter, year, noneu, type, tick) {
            <%= ClientScript.GetCallbackEventReference(this, "'orderValue_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function showProductDiscountValue(hour, day, week, month, quarter, year, noneu, type, tick) {
            <%= ClientScript.GetCallbackEventReference(this, "'productDiscount_' + hour + '_' + day + '_' + week + '_' + month + '_' + quarter + '_' + year + '_' + noneu + '_' + type", "loadValue", "tick", true) %>;
        }
        
        function loadValue(msg, context)
        {
             var contextCode = context.split("_");
             
             $("#" + context).html(msg);
             
             if(contextCode[0] == "no")
             {
                sumTotalOrder();
                sumOrderAverageValue();
             }
             else if (contextCode[0] == "pd")
             {                
                sumPromoDiscount();
             }
             else if (contextCode[0] == "ld")
             {                
                sumLoyaltyDiscount();
             }
             else if (contextCode[0] == "dv")
             {                
                sumProductDiscountValue();
             }
             else if (contextCode[0] == "lcp")
             {                
                sumLineCostPriceValue();
             }
             else if (contextCode[0] == "m")
             {                
                sumMarginValue();
             }
             else if (contextCode[0] == "s")
             {                
                sumShippingValue();
             }
             else if (contextCode[0] == "ov")
             {                
                sumOrderValue();
                sumOrderAverageValue();
             }
        }       
       
        var currencySymbol = "";
        function loadNewYearData(msg, context) {
        var contextCode = context.split("_");
        var variableToUpdate ="temp_" + context;
             
        if(window[variableToUpdate] != undefined)
        {
             if(contextCode[0] == "no")
             {  
                window[variableToUpdate] = window[variableToUpdate] + parseInt(msg);
             }
             else
             {                               
                if(window[variableToUpdate] == 0)
                {             
                    var newDiscount = msg.split(";");
                    currencySymbol = newDiscount[0] + ";";               
                    window[variableToUpdate] = parseFloat(newDiscount[1]);
                }
                else
                {             
                    var newDiscount = msg.split(";");
                    window[variableToUpdate] = parseFloat(window[variableToUpdate]) + parseFloat(newDiscount[1]);                
                }
                window[variableToUpdate] = Math.round(window[variableToUpdate] * 100) / 100;
                
                if(window[variableToUpdate] == 0)
                {
                    window[variableToUpdate] = "0.00";
                }
            }
        }
        
             if(contextCode[0] != "no")
             {
                $('#' + context).html(currencySymbol + eval(variableToUpdate));
             }
             else
             {
                $('#' + context).html(eval(variableToUpdate));
             }
             
             if(contextCode[0] == "no")
             {
                sumTotalOrder();
                sumOrderAverageValue();
             }
             else if (contextCode[0] == "pd")
             {                
                sumPromoDiscount();
             }
             else if (contextCode[0] == "ld")
             {                
                sumLoyaltyDiscount();
             }
             else if (contextCode[0] == "dv")
             {                
                sumProductDiscountValue();
             }
             else if (contextCode[0] == "lcp")
             {                
                sumLineCostPriceValue();
             }
             else if (contextCode[0] == "m")
             {                
                sumMarginValue();
             }
             else if (contextCode[0] == "s")
             {                
                sumShippingValue();
             }
             else if (contextCode[0] == "ov")
             {                
                sumOrderValue();
                sumOrderAverageValue();
             }             
        }
        /**/
        function sumTotalOrder()
        {
            var tot = 0;
            $('.area').each (
                function() {
                    var val = parseInt($(this).html());
                    
                    if (!isNaN(val))
                        tot += val;
            });
           
	        $('.totalOrders').html(tot);
        }
        
        function sumOrderAverageValue()
        {
            var totVal = 0;
            $('.areaOv').each (
                function() {
                    var val = parseFloat($(this).text().replace('£',''));
                    
                    if (!isNaN(val))
                        totVal += val;
            });
                      
            var tot = 0;
            $('.area').each (
                function() {
                    var val = parseInt($(this).html());
                    
                    if (!isNaN(val))
                        tot += val;
            });
            
            if (totVal != 0 && tot != 0)
	            $('.totalAverageValue').html('£' + (totVal / tot).toFixed(2));
	        else 
	            $('.totalAverageValue').html('£0.00');
        }
        
        function sumOrderValue()
        {
            var tot = 0;
            $('.areaOv').each (
                function() {
                    var val = parseFloat($(this).text().replace('£',''));
                    
                    if (!isNaN(val))
                        tot += val;
            });
           
	        $('.totalValue').html('£' + tot.toFixed(2));
        }
        
        function sumPromoDiscount()
        {
            var tot = 0;
            $('.areaPd').each (
                function() {
                    var val = parseFloat($(this).text().replace('£',''));
                    
                    if (!isNaN(val))
                        tot += val;
            });
           
	        $('.promoDiscount').html('£' + tot.toFixed(2));
        }
        
        function sumLoyaltyDiscount()
        {
            var tot = 0;
            $('.areaLd').each (
                function() {
                    var val = parseFloat($(this).text().replace('£',''));
                    
                    if (!isNaN(val))
                        tot += val;
            });
           
	        $('.loyaltyDiscount').html('£' + tot.toFixed(2));
        }
        
        function sumProductDiscountValue()
        {
            var tot = 0;
            $('.areaPdv').each (
                function() {
                    var val = parseFloat($(this).text().replace('£',''));
                    
                    if (!isNaN(val))
                        tot += val;
            });
           
	        $('.productDiscount').html('£' + tot.toFixed(2));
        }
        
        function sumMarginValue()
        {
            var tot = 0;
            var counter = 0
            $('.areaM').each (
                function() {
                    var val = parseFloat($(this).text().replace('%',''));
                    
                    if (!isNaN(val))
                    {
                        tot += val;
                        counter += 1;    
                    }
            });
           
	        $('.totalMargin').html((tot.toFixed(2) / counter).toFixed(2) + '%');
        }
        
        function sumShippingValue()
        {
            var tot = 0;
            $('.areaS').each (
                function() {
                    var val = parseFloat($(this).text().replace('£',''));
                    
                    if (!isNaN(val))
                        tot += val;
            });
           
	        $('.totalShipping').html('£' + tot.toFixed(2));
        }
        
        function sumLineCostPriceValue()
        {
            var tot = 0;
            $('.areaLcp').each (
                function() {
                    var val = parseFloat($(this).text().replace('£',''));
                    
                    if (!isNaN(val))
                        tot += val;
            });
           
	        $('.totalLineCostPrice').html('£' + tot.toFixed(2));
        }
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>

    <script type="text/javascript">
        function buildVariableDeclaration()
        {
            var ids = "test";

            //Generate dynamic variables for week 1s of years 2005 to 2030.
            for(var i = 2005;i <= 2030;i++)
            {
                ids += ",temp_no_1_" + i ;
                ids += ",temp_pd_1_" + i ;
                ids += ",temp_ld_1_" + i ;
                ids += ",temp_dv_1_" + i;
                ids += ",temp_lcp_1_" + i;
                ids += ",temp_m_1_" + i;
                ids += ",temp_s_1_" + i;
                ids += ",temp_oav_1_" + i;
                ids += ",temp_ov_1_" + i;            
            }
        
            //The ideal way... but function returns before DOM is fully loaded     
            /*$(document).ready(function() 
                {
                    $('.firstWeek .area').each
                    (
                        function(index, domEle) 
                        {                   
                            ids+=",temp_no_"+$(domEle).find('.week').val()+"_"+ $(domEle).find('.year').val();   
                        }
                    )
                }
            );*/
            return ids;
        }
    </script>

    <script type="text/javascript">

    var ids=buildVariableDeclaration();        
    var idsArray = ids.split(",");
    var variableDeclaration ="";

    for (var i = 0;i<idsArray.length;i++)
    {
        variableDeclaration+="var "+idsArray[i]+"=0; ";
    }

    eval(variableDeclaration);

    function getValue($element, target) {
        return $element.find(target).length ? $element.find(target).val() : 0;
    }

    $(function() {
        $('#<%= txtDateFrom.ClientID %>').datepicker({
            format: 'dd/mm/yyyy'
        });

        $('#<%= txtDateTo.ClientID %>').datepicker({
            format: 'dd/mm/yyyy'
        });

        $('.area').each(
	            function(index, domEle) {
	                showNoOrders(getValue($(domEle), '.hour'),
	                             getValue($(domEle), '.day'),
	                             getValue($(domEle), '.week'),
	                             getValue($(domEle), '.month'),
	                             getValue($(domEle), '.quarter'),
	                             getValue($(domEle), '.year'),
	                             new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
	                             $(domEle).find('.type').val(),
	                             $(domEle).find('.tick').val());
	            }
	        );

        $('.areaPd').each(
	            function(index, domEle) {
                    showPromoDiscount(getValue($(domEle), '.hour'),
	                                  getValue($(domEle), '.day'),
	                                  getValue($(domEle), '.week'),
	                                  getValue($(domEle), '.month'),
	                                  getValue($(domEle), '.quarter'),
	                                  getValue($(domEle), '.year'),
	                                  new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
	                                  $(domEle).find('.type').val(), 
	                                  $(domEle).find('.tick').val());
	            }
	        );

        $('.areaLd').each(
	            function(index, domEle) {
                  showLoyaltyDiscount(getValue($(domEle), '.hour'),
	                                  getValue($(domEle), '.day'),
	                                  getValue($(domEle), '.week'),
	                                  getValue($(domEle), '.month'),
	                                  getValue($(domEle), '.quarter'),
	                                  getValue($(domEle), '.year'),
	                                  new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
	                                  $(domEle).find('.type').val(),
	                                  $(domEle).find('.tick').val());
	            }
	        );

        $('.areaPdv').each(
	            function(index, domEle) {
                    showProductDiscountValue(getValue($(domEle), '.hour'),
	                                         getValue($(domEle), '.day'),
	                                         getValue($(domEle), '.week'),
	                                         getValue($(domEle), '.month'),
	                                         getValue($(domEle), '.quarter'),
	                                         getValue($(domEle), '.year'),
	                                         new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
	                                         $(domEle).find('.type').val(),
	                                         $(domEle).find('.tick').val());
	            }
	        );

        $('.areaLcp').each(
	            function(index, domEle) {
                    showLineCostPrice(getValue($(domEle), '.hour'),
                                      getValue($(domEle), '.day'),
                                      getValue($(domEle), '.week'),
                                      getValue($(domEle), '.month'),
                                      getValue($(domEle), '.quarter'),
                                      getValue($(domEle), '.year'),
                                      new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
                                      $(domEle).find('.type').val(),
                                      $(domEle).find('.tick').val());
	            }
	        );

        $('.areaM').each(
	            function(index, domEle) {
                    showMargin(getValue($(domEle), '.hour'),
                               getValue($(domEle), '.day'),
                               getValue($(domEle), '.week'),
                               getValue($(domEle), '.month'),
                               getValue($(domEle), '.quarter'),
                               getValue($(domEle), '.year'),
                               new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
                               $(domEle).find('.type').val(),
                               $(domEle).find('.tick').val());
	            }
	        );

        $('.areaS').each(
	            function(index, domEle) {
                    showShipping(getValue($(domEle), '.hour'),
                                 getValue($(domEle), '.day'),
                                 getValue($(domEle), '.week'),
                                 getValue($(domEle), '.month'),
                                 getValue($(domEle), '.quarter'),
                                 getValue($(domEle), '.year'),
                                 new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
                                 $(domEle).find('.type').val(),
                                 $(domEle).find('.tick').val());
	            }
	        );

        $('.areaOav').each(
                function(index, domEle) {
                    showOrderAverageValue(getValue($(domEle), '.hour'),
                                          getValue($(domEle), '.day'),
                                          getValue($(domEle), '.week'),
                                          getValue($(domEle), '.month'),
                                          getValue($(domEle), '.quarter'),
                                          getValue($(domEle), '.year'),
                                          new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
                                          $(domEle).find('.type').val(),
                                          $(domEle).find('.tick').val());
                }
	        );

        $('.areaOv').each(
	            function(index, domEle) {
                    showOrderValue(getValue($(domEle), '.hour'),
                                   getValue($(domEle), '.day'),
                                   getValue($(domEle), '.week'),
                                   getValue($(domEle), '.month'),
                                   getValue($(domEle), '.quarter'),
                                   getValue($(domEle), '.year'),
                                   new Boolean($(document).find('.nonEuFlag > input[type=checkbox]:checked').val()),
                                   $(domEle).find('.type').val(),
                                   $(domEle).find('.tick').val());
	            }
	        );
    });
    </script>    
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Report</h2>
            <h3>Orders</h3>
        </div>
    </div>
    
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="reportFilter form-inline">
                            <asp:Label runat="server" ID="lblDateFrom" Text="From:" AssociatedControlID="txtDateFrom" />
                            <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control" />
                            <asp:Label runat="server" ID="lblDateTo" Text="To:" AssociatedControlID="txtDateTo" />
                            <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control" />
                            <asp:Label runat="server" Text="non-EU only" AssociatedControlID="cbNonEU" />
                            <asp:CheckBox ID="cbNonEU" CssClass="nonEuFlag" runat="server" />
                            <asp:Label runat="server" Text="Show By:" AssociatedControlID="ddlShowBy" />
                            <asp:DropDownList ID="ddlShowBy" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Hour" Value="hour"></asp:ListItem>
                                <asp:ListItem Text="Day" Value="day" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Week" Value="week"></asp:ListItem>
                                <asp:ListItem Text="Month" Value="month"></asp:ListItem>
                                <asp:ListItem Text="Quarter" Value="quarter"></asp:ListItem>
                                <asp:ListItem Text="Year" Value="year"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:LinkButton ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn btn-primary" OnClick="btnRefresh_Click" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-3">
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <table class="table table-striped">
                            <tr>
                                <th class="text-danger">Total Orders:</th>
                                <td class="text-danger"><div class="totalOrders"></div></td>
                            </tr>
                            <tr>
                                <th class="text-danger">Total Value:</th>
                                <td class="text-danger"><div class="totalValue"></div></td>
                            </tr>
                            <tr>
                                <th class="text-danger">Total Average Value:</th>
                                <td class="text-danger"><div class="totalAverageValue"></div></td>
                            </tr>
                            <tr>
                                <th>Total Promo Discount:</th>
                                <td><div class="promoDiscount"></div></td>
                            </tr>
                            <tr>
                                <th>Total Loyalty Discount:</th>
                                <td><div class="loyaltyDiscount"></div></td>
                            </tr>
                            <tr>
                                <th>Total Product Discount:</th>
                                <td><div class="productDiscount"></div></td>
                            </tr>
                            <tr>
                                <th>Total Line Cost Price:</th>
                                <td><div class="totalLineCostPrice"></div></td>
                            </tr>
                            <tr>
                                <th>Average Margin:</th>
                                <td><div class="totalMargin"></div></td>
                            </tr>
                            <tr>
                                <th>Total Shipping:</th>
                                <td><div class="totalShipping"></div></td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div class="col-lg-9">
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <asp:GridView ID="gvOrdersByHour" Visible="false" runat="server" CssClass="table table-striped table-bordered table-hover dataTable" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Hour">
                                    <ItemTemplate>
                                        <%# Eval("Hour") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <%# new DateTime(Convert.ToInt32(Eval("Year")), Convert.ToInt32(Eval("Month")), Convert.ToInt32(Eval("Day"))).ToString("dd/MM/yyyy") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No. Orders">
                                    <ItemTemplate>
                                        <div id='no_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="area">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='no_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Promo Discount">
                                    <ItemTemplate>
                                        <div id='pd_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaPd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='pd_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Loyalty Discount">
                                    <ItemTemplate>
                                        <div id='ld_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaLd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='ld_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                         </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product Discount">
                                    <ItemTemplate>
                                        <div id='dv_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaPdv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='dv_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Cost Price">
                                    <ItemTemplate>
                                        <div id='lcp_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaLcp">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='lcp_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Margin">
                                    <ItemTemplate>
                                        <div id='m_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaM">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='m_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipping">
                                    <ItemTemplate>
                                        <div id='s_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaS">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='s_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Average Value">
                                    <ItemTemplate>
                                        <div id='oav_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaOav">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='oav_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Value">
                                    <ItemTemplate>
                                        <div id='ov_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaOv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Hour") %>' class="hour" />
                                            <input type="hidden" value='<%# Eval("Day") %>' class="day" />
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="hour" class="type" />
                                            <input type="hidden" value='ov_<%# Eval("Hour") %>_<%# Eval("Day") %>_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:GridView ID="gvOrdersByDay" Visible="false" runat="server" CssClass="table table-striped table-bordered table-hover dataTable" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Day">
                                    <ItemTemplate>
                                        <%# Convert.ToDateTime(Eval("Date")).DayOfWeek %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <%# Convert.ToDateTime(Eval("Date")).ToString("dd/MM/yyyy") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No. Orders">
                                    <ItemTemplate>
                                        <div id='no_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="area">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='no_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Promo Discount">
                                    <ItemTemplate>
                                        <div id='pd_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="areaPd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='pd_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Loyalty Discount">
                                    <ItemTemplate>
                                        <div id='ld_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="areaLd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='ld_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product Discount">
                                    <ItemTemplate>
                                        <div id='dv_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="areaPdv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='dv_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Cost Price">
                                    <ItemTemplate>
                                        <div id='lcp_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="areaLcp">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='lcp_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Margin">
                                    <ItemTemplate>
                                        <div id='m_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="areaM">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='m_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipping">
                                    <ItemTemplate>
                                        <div id='s_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="areaS">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='s_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Average Value">
                                    <ItemTemplate>
                                        <div id='oav_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="areaOav">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='oav_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Value">
                                    <ItemTemplate>
                                        <div id='ov_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="areaOv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Day %>' class="day" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Month %>' class="month" />
                                            <input type="hidden" value='<%# Convert.ToDateTime(Eval("Date")).Year %>' class="year" />
                                            <input type="hidden" value="day" class="type" />
                                            <input type="hidden" value='ov_<%# Convert.ToDateTime(Eval("Date")).Ticks %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:GridView ID="gvOrdersByWeek" Visible="false" runat="server" CssClass="table table-striped table-bordered table-hover dataTable" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Year">
                                    <ItemTemplate>
                                        <%# Eval("Year")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Week">
                                    <ItemTemplate>
                                        <%# Eval("Week") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No. Orders">
                                    <ItemTemplate>
                                        <div id='no_<%# Eval("Week") %>_<%# Eval("Year") %>' class="area" name="area">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" name="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" name="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='no_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Promo Discount">
                                    <ItemTemplate>
                                        <div id='pd_<%# Eval("Week") %>_<%# Eval("Year") %>' class="areaPd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='pd_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Loyalty Discount">
                                    <ItemTemplate>
                                        <div id='ld_<%# Eval("Week") %>_<%# Eval("Year") %>' class="areaLd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='ld_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product Discount">
                                    <ItemTemplate>
                                        <div id='dv_<%# Eval("Week") %>_<%# Eval("Year") %>' class="areaPdv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='dv_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Cost Price">
                                    <ItemTemplate>
                                        <div id='lcp_<%# Eval("Week") %>_<%# Eval("Year") %>' class="areaLcp">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='lcp_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Margin">
                                    <ItemTemplate>
                                        <div id='m_<%# Eval("Week") %>_<%# Eval("Year") %>' class="areaM">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='m_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipping">
                                    <ItemTemplate>
                                        <div id='s_<%# Eval("Week") %>_<%# Eval("Year") %>' class="areaS">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='s_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Average Value">
                                    <ItemTemplate>
                                        <div id='oav_<%# Eval("Week") %>_<%# Eval("Year") %>' class="areaOav">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='oav_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Value">
                                    <ItemTemplate>
                                        <div id='ov_<%# Eval("Week") %>_<%# Eval("Year") %>' class="areaOv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value='<%# Eval("Week") %>' class="week" />
                                            <input type="hidden" value="week" class="type" />
                                            <input type="hidden" value='ov_<%# Eval("Week") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:GridView ID="gvOrdersByMonth" Visible="false" runat="server" CssClass="table table-striped table-bordered table-hover dataTable" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Year">
                                    <ItemTemplate>
                                        <%# Eval("Year")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Month">
                                    <ItemTemplate>
                                        <%# Eval("Month") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No. Orders">
                                    <ItemTemplate>
                                        <div id='no_<%# Eval("Month") %>_<%# Eval("Year") %>' class="area">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='no_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Promo Discount">
                                    <ItemTemplate>
                                        <div id='pd_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaPd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='pd_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Loyalty Discount">
                                    <ItemTemplate>
                                        <div id='ld_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaLd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='ld_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product Discount">
                                    <ItemTemplate>
                                        <div id='dv_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaPdv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='dv_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Cost Price">
                                    <ItemTemplate>
                                        <div id='lcp_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaLcp">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='lcp_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Margin">
                                    <ItemTemplate>
                                        <div id='m_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaM">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='m_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipping">
                                    <ItemTemplate>
                                        <div id='s_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaS">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='s_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Average Value">
                                    <ItemTemplate>
                                        <div id='oav_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaOav">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='oav_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Value">
                                    <ItemTemplate>
                                        <div id='ov_<%# Eval("Month") %>_<%# Eval("Year") %>' class="areaOv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Month") %>' class="month" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="month" class="type" />
                                            <input type="hidden" value='ov_<%# Eval("Month") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:GridView ID="gvOrdersByQuarter" Visible="false" runat="server" CssClass="table table-striped table-bordered table-hover dataTable" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Year">
                                    <ItemTemplate>
                                        <%# Eval("Year")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Quarter">
                                    <ItemTemplate>
                                        <%# Eval("Quarter")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No. Orders">
                                    <ItemTemplate>
                                        <div id='no_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="area">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='no_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Promo Discount">
                                    <ItemTemplate>
                                        <div id='pd_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="areaPd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='pd_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Loyalty Discount">
                                    <ItemTemplate>
                                        <div id='ld_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="areaLd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='ld_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product Discount">
                                    <ItemTemplate>
                                        <div id='dv_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="areaPdv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='dv_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Cost Price">
                                    <ItemTemplate>
                                        <div id='lcp_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="areaLcp">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='lcp_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Margin">
                                    <ItemTemplate>
                                        <div id='m_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="areaM">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='m_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipping">
                                    <ItemTemplate>
                                        <div id='s_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="areaS">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='s_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Average Value">
                                    <ItemTemplate>
                                        <div id='oav_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="areaOav">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='oav_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Value">
                                    <ItemTemplate>
                                        <div id='ov_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="areaOv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Quarter") %>' class="quarter" />
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="quarter" class="type" />
                                            <input type="hidden" value='ov_<%# Eval("Quarter") %>_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:GridView ID="gvOrdersByYear" Visible="false" runat="server" CssClass="table table-striped table-bordered table-hover dataTable" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Year">
                                    <ItemTemplate>
                                        <%# Eval("Year")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No. Orders">
                                    <ItemTemplate>
                                        <div id='no_<%# Eval("Year") %>' class="area">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='no_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Promo Discount">
                                    <ItemTemplate>
                                        <div id='pd_<%# Eval("Year") %>' class="areaPd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='pd_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Loyalty Discount">
                                    <ItemTemplate>
                                        <div id='ld_<%# Eval("Year") %>' class="areaLd">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='ld_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product Discount">
                                    <ItemTemplate>
                                        <div id='dv_<%# Eval("Year") %>' class="areaPdv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='dv_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line Cost Price">
                                    <ItemTemplate>
                                        <div id='lcp_<%# Eval("Year") %>' class="areaLcp">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='lcp_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Margin">
                                    <ItemTemplate>
                                        <div id='m_<%# Eval("Year") %>' class="areaM">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='m_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipping">
                                    <ItemTemplate>
                                        <div id='s_<%# Eval("Year") %>' class="areaS">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='s_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>                
                                <asp:TemplateField HeaderText="Order Average Value">
                                    <ItemTemplate>
                                        <div id='oav_<%# Eval("Year") %>' class="areaOav">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='oav_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Order Value">
                                    <ItemTemplate>
                                        <div id='ov_<%# Eval("Year") %>' class="areaOv">
                                            <i class="fa fa-spinner fa-spin"></i>
                                            <input type="hidden" value='<%# Eval("Year") %>' class="year" />
                                            <input type="hidden" value="year" class="type" />
                                            <input type="hidden" value='ov_<%# Eval("Year") %>' class="tick" />
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <asp:GridView ID="gvOrdersSum" runat="server" CssClass="table table-striped table-bordered table-hover dataTable" AutoGenerateColumns="false">
                            <Columns>
                                <asp:TemplateField HeaderText="Day">
                                    <ItemTemplate>
                                        <%# Convert.ToDateTime(Eval("TheDay")).DayOfWeek %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <%# Convert.ToDateTime(Eval("TheDay")).ToLongDateString() %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="TotalOrders" HeaderText="No. Orders" />
                                <asp:TemplateField HeaderText="PromoUsed(%)">
                                    <ItemTemplate>
                                        <%# Eval("TotalPromoUsed") %>
                                        (<%# Convert.ToInt32(((Convert.ToDouble(Eval("TotalPromoUsed")) / Convert.ToDouble(Eval("TotalOrders"))) * 100)).ToString() %>%)
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="TotalPromoDiscount" HeaderText="Promo Discount" DataFormatString="£{0:0.00}" />
                                <asp:BoundField DataField="TotalLoyaltyDiscount" HeaderText="Loyalty Discount" DataFormatString="£{0:0.00}" />
                                <asp:BoundField DataField="TotalShipping" HeaderText="Shipping" DataFormatString="£{0:0.00}" />
                                <asp:BoundField DataField="GrandTotal" HeaderText="Order Value" DataFormatString="£{0:0.00}" ItemStyle-Font-Bold="true" />
                            </Columns>
                            <AlternatingRowStyle BackColor="#FAFAFA" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
