<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.FulFillment.order_fulfillment" Codebehind="order_fulfillment.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/flag-icon-css/2.1.0/css/flag-icon.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phHeaderScript" runat="server">
    <script type="text/javascript">
    //<[CDATA[
	    function checkAlert() {
            <%= ClientScript.GetCallbackEventReference(this, "", "receiveAlert", "", true) %>;
	    }
	    
	    function receiveAlert(arg, context) {
	        if (arg != "") {
	            $(".alertBox").html(arg);
	            $(".alertBox").css("display", "block");
	        }
	        else {
	            $(".alertBox").html('');
	            $(".alertBox").css("display", "none");
	        }
	    }
	    
	    function togglePrintPreview(classSender) {
             var $printCSS = $('.printCSS');
             if ($printCSS.attr('media') == 'print') {
                $('.printCSS').attr('media', 'all');
                $('#header').css('display', 'none');
                $('#nav').css('display', 'none');
                $('.' + classSender).html('Close');
             }
             else {
                $('.printCSS').attr('media', 'print');
                $('#header').css('display', 'block');
                $('#nav').css('display', 'block');
                $('.' + classSender).html('Print Preview');
             }
        }
    //]]>
	</script>
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <!-- Data picker -->
   <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            // Set interval for alert
            setInterval(checkAlert, 20);

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
     <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Ship Orders</h2>            
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <div class="alertBox alert alert-warning" style="display: none;"></div>
                <Apollo:NoticeBox ID="enbMsg" runat="server"/>
                <Apollo:NoticeBox ID="enbInfo" runat="server"/>

                <asp:MultiView ID="mvScreen" runat="server" ActiveViewIndex="0">
                    <asp:View runat="server">
                        <div class="ibox float-e-margins">
                            <div class="ibox-content">
                                <div class="table-responsive">
                                    <div class="dataTables_wrapper form-inline dt-bootstrap">
                                        <div class="html5buttons">
                                            <div class="dt-buttons btn-group">
                                                <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                <asp:LinkButton ID="lbPrint" Visible="false" runat="server" Text="Print allocated note" CssClass="btn btn-default buttons-copy buttons-html5" OnClick="lbPrint_Click"></asp:LinkButton>
                                                <asp:LinkButton ID="lbExportToCsv" runat="server" Text="Export to CSV (DMO)" CssClass="btn btn-default buttons-copy buttons-html5" OnClick="lbExportToCsv_Click"></asp:LinkButton>
                                            </div>
                                        </div>
                                        <div class="dataTables_filter">
                                            <label>
                                                Order ID: 
                                                <asp:TextBox ID="txtOrderId" runat="server" CssClass="form-control" ValidationGroup="orderId"></asp:TextBox>
                                                <asp:LinkButton ID="lbGo" CssClass="btn btn-default btn-sm" runat="server" Text="Find" OnClick="lbGo_Click" ValidationGroup="orderId"></asp:LinkButton>                                        
                                            </label>
                                        </div>
                                    </div>
                                    <Apollo:CustomGrid ID="gvOrders" runat="server" PageSize="50" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvOrders_PageIndexChanging" ShowHeaderWhenEmpty="true"
                                        AutoGenerateColumns="false" OnSorting="gvOrders_Sorting" OnPreRender="gvOrders_PreRender" ShowHeader="true" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">                    
                                        <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                        <PagerTemplate>                        
                                            <div style="float: left; width: 50%;">
                                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                    Page 
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvOrders.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                    <asp:ImageButton Visible='<%# (gvOrders.CustomPageCount > (gvOrders.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                    of <%= gvOrders.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvOrders.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                                </asp:Panel>
                                            </div>
                                        </PagerTemplate>
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-Width="80px">
                                                <HeaderTemplate>
                                                    <input type="checkbox" onclick="toggle_chosen(this);" />
                                                    <br />
                                                    <asp:DropDownList ID="ddlFilterChosen" runat="server" OnPreRender="ddlFilterChosen_PreRender" CssClass="form-control">
                                                        <asp:ListItem Text="Any" Value="Any"></asp:ListItem>
                                                        <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                                        <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </HeaderTemplate>
                                                <ItemTemplate>                    
                                                    <asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" />
                                                </ItemTemplate>
                                            </asp:TemplateField>  
                                            <asp:TemplateField HeaderStyle-Width="80px" SortExpression="Id">
                                                <HeaderTemplate>
                                                    <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Order ID</asp:LinkButton><br />
                                                    <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                                </HeaderTemplate>
                                                <ItemTemplate><a href='<%# "/fulfillment/order_packing_info.aspx?orderid=" + Eval("Id") %>'><%# Eval("Id")%></a></ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Name">
                                                <HeaderTemplate>
                                                    Ship To
                                                </HeaderTemplate>
                                                <ItemTemplate><%# Eval("ShipTo") %></a></ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Delivery" SortExpression="ShippingName">
                                                <HeaderTemplate>
                                                    Delivery
                                                    <asp:DropDownList ID="ddlDelivery" runat="server" OnInit="ddlDelivery_Init" CssClass="form-control"></asp:DropDownList>
                                                </HeaderTemplate>
                                                <ItemTemplate><%# Eval("ShippingOptionName") %></ItemTemplate>
                                            </asp:TemplateField>                        
                                            <asp:TemplateField HeaderText="Country" ItemStyle-HorizontalAlign="Center">
                                                <HeaderTemplate>
                                                    Country
                                                    <asp:DropDownList ID="ddlCountries" CssClass="form-control" runat="server" OnInit="ddlCountries_Init" DataTextField="Name" DataValueField="CountryId"></asp:DropDownList>
                                                </HeaderTemplate>
                                                <ItemTemplate>                                
                                                    <%# AdminStoreUtility.GetShippingCountryImage(Convert.ToInt32(Eval("ShippingCountryId"))) %>                                
                                                </ItemTemplate>
                                            </asp:TemplateField>                        
                                            <asp:TemplateField HeaderText="Placed On" SortExpression="OrderPlaced">                
                                                <HeaderTemplate>                                
                                                    Placed On Date<asp:TextBox ID="txtFromDate" runat="server" CssClass="date form-control"></asp:TextBox>
                                                    to <asp:TextBox ID="txtToDate" runat="server" CssClass="date form-control"></asp:TextBox>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div style="text-align: center;"><%# Convert.ToDateTime(Eval("OrderPlaced")).ToString("dd/MM/yyyy")%></div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Last Activity" SortExpression="LastActivityDate">                
                                                <HeaderTemplate>
                                                    <asp:LinkButton CommandArgument="LastActivityDate" runat="server" CommandName="Sort">Last Activity Date</asp:LinkButton>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div style="text-align: center;"><%# Eval("LastActivityDate") %></div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action">
                                                <HeaderTemplate>
                                                    Action
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div style="text-align: center;"><a href='<%# "/fulfillment/order_packing_info.aspx?orderid=" + Eval("Id") %>'>Ship</a></div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>        
                                    </Apollo:CustomGrid>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-6">
                            <table class="table table-striped">
                                <tr>
                                    <th>Service Code</th>
                                    <td></td>                    
                                </tr>
                                <tr>
                                    <td>CRL24</td>
                                    <td>ROYAL MAIL 24 PARCELS</td>
                                </tr>
                                <tr>
                                    <td>STL1</td>
                                    <td>1ST CLASS ACCOUNT MAIL LETTERS</td>
                                </tr>
                                <tr>
                                    <td>CRL48</td>
                                    <td>ROYAL MAIL 48 PARCELS</td>
                                </tr>
                                <tr>
                                    <td>STL2</td>
                                    <td>2ND CLASS ACCOUNT MAIL LETTERS</td>
                                </tr>
                                <tr>
                                    <td>SD1</td>
                                    <td>NEXT DAY DELIVERY</td>
                                </tr>
                            </table>

                            <table class="table table-striped">
                                <tr>
                                    <th>Service Format</th>
                                    <td></td>                    
                                </tr>
                                 <tr>
                                    <td>PARCEL</td>
                                    <td></td>
                                </tr>
                                 <tr>
                                    <td>LARGE LETTER</td>
                                    <td></td>
                                </tr>
                                 <tr>
                                    <td>LETTER</td>
                                    <td></td>
                                </tr>
                            </table>
                        </div>
                    </asp:View>
                    <asp:View runat="server">
                        <div style="width: 60%;" class="allocNoteWidth">
                            <h3 style="width: 70%; float: left; border: 0px;">Allocated Note</h3>
                
                            <div class="btnBox">
                                <a href="#" onclick="window.print();" class="ABtn">Print</a>
                                <a href="#" class="ABtn PrintPreview" onclick="javascript:togglePrintPreview('PrintPreview');">Print Preview</a>
                                <a href="/fulfillment/order_fulfillment.aspx" class="ABtn">Back</a>
                            </div>
                
                            <asp:Repeater ID="rptOrders" runat="server" OnItemDataBound="rptOrders_ItemDataBound">
                                <HeaderTemplate>
                                    <table cellspacing="0" class="customGrid">
                                        <tr>
                                            <th align="left">Order #</th>
                                            <th align="left">Shipping Option</th>
                                        </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                        <tr>
                                            <td style="font-size: 1em;" class="printRowText"><asp:Literal ID="ltlOrderId" runat="server" Text='<%# Container.DataItem.ToString() %>'></asp:Literal></td>
                                            <td style="font-size: 1em;" class="printRowText"><%# GetShippingOptionNameByOrderId(Convert.ToInt32(Container.DataItem)) %></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <asp:Repeater ID="rptItems" runat="server">
                                                    <HeaderTemplate>
                                                        <table style="border-collapse: collapse;" cellspacing="0">
                                                            <tr>
                                                                <th>Quantity</th>
                                                                <th>Item</th>
                                                                <th>Option</th>
                                                            </tr>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                            <tr visible='<%# Eval("StatusCode").ToString() != LineStatusCode.GOODS_ALLOCATED ? "false": "true" %>'>
                                                                <td class="printRowText" style="width: 50px; text-align: center; font-size: 2em;">
                                                                    <asp:HiddenField ID="hfPendingQuantity" runat="server" Value='<%# Eval("PendingQuantity") %>' />
                                                                    <%# Eval("PendingQuantity") %>
                                                                </td>
                                                                <td class="printRowText" style="font-size: 2em;" valign="top"><%# Eval("Name") %></td>       
                                                                <td class="printRowText" style="font-size: 2em;" valign="top"><%# Eval("Option") %></td>                                                    
                                                            </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </td>
                                        </tr>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                        <tr style="background-color: #fff1c2;" class="printAltRow">
                                            <td class="printRowText" style="font-size: 1em;"><asp:Literal ID="ltlOrderId" runat="server" Text='<%# Eval("OrderId") %>'></asp:Literal></td>
                                            <td class="printRowText" style="font-size: 1em;"><%# Eval("ShippingOption.Name") %></td>
                                        </tr>
                                        <tr style="background-color: #fff1c2;" class="printAltRow">
                                            <td colspan="2">
                                                <asp:Repeater ID="rptItems" runat="server">
                                                    <HeaderTemplate>
                                                        <table style="border-collapse: collapse;" cellspacing="0">
                                                            <tr>
                                                                <th>Quantity</th>
                                                                <th>Item</th>
                                                                <th>Option</th>
                                                            </tr>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                            <tr visible='<%# Eval("StatusCode").ToString() != LineStatusCode.GOODS_ALLOCATED ? "false": "true" %>'>
                                                                <td  class="printRowText" style="width: 50px; text-align: center; font-size: 2em;">
                                                                    <asp:HiddenField ID="hfPendingQuantity" runat="server" Value='<%# Eval("PendingQuantity") %>' />
                                                                    <%# Eval("PendingQuantity") %>
                                                                </td>
                                                                <td class="printRowText" style="font-size: 2em;" valign="top"><%# Eval("Name") %></td>       
                                                                <td class="printRowText" style="font-size: 2em;" valign="top"><%# Eval("Option") %></td>                                                    
                                                            </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </td>
                                        </tr>
                                </AlternatingItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>
                
                            <div class="btnBox">
                                <a href="#" onclick="window.print();" class="ABtn">Print</a>
                                <a href="#" class="ABtn PrintPreview" onclick="javascript:togglePrintPreview('PrintPreview');">Print Preview</a>
                                <a href="/fulfillment/order_fulfillment.aspx" class="ABtn">Back</a>
                            </div>
                
                        </div>    
                    </asp:View>
                </asp:MultiView>

                <div class="alertBox alert alert-info alert-dismissable hidden"></div>
            </div>
        </div>
    </div>

    
</asp:Content>