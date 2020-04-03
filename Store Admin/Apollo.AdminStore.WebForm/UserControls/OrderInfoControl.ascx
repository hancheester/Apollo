<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderInfoControl" Codebehind="OrderInfoControl.ascx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="AddressView" Src="~/UserControls/OrderAddressViewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderHeader" Src="~/UserControls/OrderHeaderControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="AccountView" Src="~/UserControls/OrderAccountViewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="PaymentView" Src="~/UserControls/OrderPaymentViewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="ShippingView" Src="~/UserControls/OrderShippingViewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OfferView" Src="~/UserControls/OrderOfferViewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="LoyaltyView" Src="~/UserControls/OrderLoyaltyViewControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="SystemCheckView" Src="~/UserControls/OrderSystemCheckViewControl.ascx" %>
<script type="text/javascript" src="/js/wz_tooltip.js"></script>

<div class="col-lg-6">
    <Apollo:OrderHeader ID="eohHeader" runat="server" />
</div>

<div class="col-lg-6">
    <Apollo:AccountView ID="eavAccount" runat="server" OnVerified="eavAccount_Verified"/>
</div>

<div class="clearfix"></div>

<div class="col-lg-6">
    <Apollo:AddressView ID="eavBiling" runat="server" AddressType="Billing" Title="Billing address" OnVerified="eavBilling_Verified" OnAddressChanged="eavBilling_Changed" />
</div>

<div class="col-lg-6">
    <Apollo:AddressView ID="eavShipping" runat="server" AddressType="Shipping" Title="Shipping address" OnVerified="eavShipping_Verified" OnAddressChanged="eavShipping_Changed" />
</div>

<div class="clearfix"></div>

<div class="col-lg-6">
    <Apollo:OfferView ID="eovOffer" runat="server" OnOfferChanged="eovOffer_OfferChanged" />
</div>

<div class="col-lg-6">
    <Apollo:LoyaltyView ID="elvLoyalty" runat="server" OnLoyaltyChanged="elvLoyalty_LoyaltyChanged" />
</div>

<div class="clearfix"></div>

<div class="col-lg-6">
    <Apollo:PaymentView ID="epvPayment" runat="server" OnVerified="epvPayment_Verified" OnPaidByPhone="epvPayment_PaidByPhone" />
</div>

<div class="col-lg-6">
    <Apollo:ShippingView ID="esvShipping" runat="server" OnShippingChanged="esvShipping_ShippingChanged" />
</div>

<div class="col-lg-6">
    <Apollo:SystemCheckView ID="escSystemCheck" runat="server" />
</div>

<div class="clearfix"></div>

<div class="col-lg-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            Ordered items
            <asp:PlaceHolder ID="phAddItem" runat="server">
                <span class="pull-right label label-plain"><asp:LinkButton ID="lbAddItem" runat="server" OnClientClick="javascript:return alert('Please make sure to authorise payment before proceed.');" OnClick="lbAddItem_Click" Text="add item"></asp:LinkButton></span>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phFinishAddingItem" runat="server">
                <span class="pull-right label label-plain"><asp:LinkButton ID="lbFinishAddingItem" runat="server" OnClick="lbFinishAddingItem_Click" Text="finish adding item"></asp:LinkButton></span>
            </asp:PlaceHolder>
        </div>
                
        <asp:PlaceHolder ID="phAddItemGrid" runat="server">                                    
            <div class="ibox float-e-margins">
                <div class="ibox-content">
                    <div class="table-responsive">
                        <div class="dataTables_wrapper form-inline dt-bootstrap" style="padding-bottom: 0;">
                            <div class="html5buttons">
                                <div class="dt-buttons btn-group">
                                    <asp:LinkButton ID="lbSearchProduct" runat="server" Text="Search" OnClick="lbSearchProduct_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        <Apollo:CustomGrid ID="gvProducts" runat="server" PageSize="10" AllowPaging="true" AllowSorting="false" 
                            OnPreRender="gvProducts_PreRender" AutoGenerateColumns="false" ShowHeader="true" DataKeyNames="Id" ShowHeaderWhenEmpty="true"
                            CssClass="table table-striped" OnRowDataBound="gvProducts_RowDataBound" OnPageIndexChanging="gvProducts_PageIndexChanging" OnRowCommand="gvProducts_RowCommand">
                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                            <PagerTemplate>
                                <div style="float: left; width: 50%;">
                                    <asp:Panel runat="server" DefaultButton="btnProductGoPage">
                                        Page
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" runat="server" ID="btnProductGoPage" CssClass="hidden" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                        <asp:ImageButton Visible='<%# (gvProducts.CustomPageCount > (gvProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                        of
                                        <%= gvProducts.PageCount %>
                                        pages |
                                        <asp:PlaceHolder ID="phRecordFound" runat="server">Total
                                            <%= gvProducts.RecordCount %>
                                            records found</asp:PlaceHolder>
                                        <asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false">No record found</asp:PlaceHolder>
                                    </asp:Panel>
                                </div>                    
                            </PagerTemplate>
                            <Columns>                        
                                <asp:TemplateField HeaderText="Product ID" HeaderStyle-Width="90px">                            
                                    <HeaderTemplate>                                
                                        <asp:LinkButton CommandArgument="ProductId" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                        <asp:TextBox ID="txtProductIdFilter" runat="server" CssClass="form-control"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product name">
                                    <HeaderTemplate>                                
                                        <asp:LinkButton CommandArgument="ProductName" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                        <asp:TextBox ID="txtProductNameFilter" runat="server" CssClass="form-control"></asp:TextBox>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%# Eval("Name")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="(Price) Option">
                                    <ItemTemplate>
                                        <asp:DropDownList runat="server" ID="ddlOptions" CssClass="form-control"></asp:DropDownList>
                                        <asp:Literal ID="litSingleOption" runat="server"></asp:Literal>
                                        <asp:HiddenField ID="hdnSingleOptionId" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Quantity to add" HeaderStyle-Width="90px">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtQty" runat="server" Text="1" CssClass="form-control"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Discontinued">
                                    <ItemTemplate>
                                        <%# Eval("Discontinued")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbAddToOrder" runat="server" CommandArgument='<%# Eval("Id") %>' CommandName="addProduct">Add to order</asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </Apollo:CustomGrid>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        
        <asp:Repeater ID="rptItems" runat="server" OnItemDataBound="rptItems_ItemDataBound" OnItemCommand="rptItems_ItemCommand">
            <HeaderTemplate>
                <table class="table">
                    <tr>
                        <th style="display:none;" class="printShow">Qty</th>
                        <th>Product</th>
                        <th>Status</th>
                        <th>Option</th>
                        <th>Retail</th>
                        <th>Giftwrap</th>
                        <th>Quantity</th>
                        <th>Line Total</th>
                        <th>Action</th>
                    </tr>                            
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td style="display:none;" class="printShow"><%# Eval("InvoicedQuantity") %></td>
                    <td>
                        <a target="_blank" href="<%# AdminStoreUtility.GetProductUrl(Eval("UrlRewrite").ToString()) %>" onmouseover="Tip('<img src=/get_image_handler.aspx?type=media&product_id=<%# Eval("ProductId") %> />')" onmouseout="UnTip()"><%# Eval("Name") %> <%# Eval("Option") %></a>
                        <div class="info_<%# Eval("Id") %> info">
                            <%# Eval("Note") != null ? "<span class='text-danger clearfix'><strong>" + Eval("Note") + "</strong></span><br/>" : null %>                            
                            <%# CheckIfDifferentFromRRP(Eval("CurrencyCode").ToString(), Convert.ToDecimal(Eval("ExchangeRate")), Convert.ToDecimal(Eval("InitialPriceInclTax")), Convert.ToDecimal(Eval("PriceInclTax"))) %>
                        </div>
                    </td>
                    <td>
                        <div class="info_<%# Eval("Id") %> info">
                            <%# OrderService.GetLineStatusByCode(Convert.ToString(Eval("StatusCode"))) %>
                        </div>
                    </td>
                    <td style="width: 35px;">
                        <div class="info_<%# Eval("Id") %> info">
                            <%# Eval("Option") %>
                        </div>
                    </td>
                    <td>
                        <div class="info_<%# Eval("Id") %> info">
                            <%# AdminStoreUtility.GetFormattedPrice(Eval("PriceInclTax"), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity, Convert.ToDecimal(Eval("ExchangeRate"))) %><br />
                            <%# AdminStoreUtility.GetFormattedPrice(Eval("PriceExclTax"), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity, Convert.ToDecimal(Eval("ExchangeRate"))) %> (excl tax)<br />
                            <%# Eval("CurrencyCode").ToString() != CurrencySettings.PrimaryStoreCurrencyCode ?
                                "<span class='printHide'><br/>(" + AdminStoreUtility.GetFormattedPrice(Eval("PriceInclTax"), CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity) + " incl tax)</span>" : string.Empty %>
                        </div>
                    </td>
                    <td>
                        <div class="info_<%# Eval("Id") %> info">
                            <%# Convert.ToBoolean(Eval("FreeWrapped")) ? "Yes" : "No" %>
                        </div>
                    </td>
                    <td>                                        
                        <asp:HiddenField ID="hfLineItemId" runat="server" Value='<%# Eval("Id") %>' />
                        <div class="info_<%# Eval("Id") %> info">
                            Ordered <%# Eval("Quantity") %><br />
                            Pending <%# Eval("PendingQuantity") %>
                            <%# Convert.ToInt32(Eval("InTransitQuantity")) > 0 ? "<br/>In Transit " + Eval("InTransitQuantity") : string.Empty %>
                            <%# Convert.ToInt32(Eval("AllocatedQuantity")) > 0 ? "<br/>Allocated " + Eval("AllocatedQuantity") : string.Empty %>
                            <%# Convert.ToInt32(Eval("ShippedQuantity")) > 0 ? "<br/>Shipped " + Eval("ShippedQuantity") : string.Empty %>
                            <%# Convert.ToInt32(Eval("InvoicedQuantity")) > 0 ? "<br/>Invoiced " + Eval("InvoicedQuantity") : string.Empty %>
                        </div>
                    </td>
                    <td>
                        <%# AdminStoreUtility.GetFormattedPrice(AdminStoreUtility.RoundPrice(Convert.ToInt32(Eval("Quantity")) * Convert.ToDecimal(Eval("PriceInclTax"))), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity, Convert.ToDecimal(Eval("ExchangeRate"))) %><br />
                        <%# AdminStoreUtility.GetFormattedPrice(AdminStoreUtility.RoundPrice(Convert.ToInt32(Eval("Quantity")) * Convert.ToDecimal(Eval("PriceExclTax"))), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity, Convert.ToDecimal(Eval("ExchangeRate"))) %> (excl tax)<br />
                        <%# Eval("CurrencyCode").ToString() != CurrencySettings.PrimaryStoreCurrencyCode ?
                            "<span class='printHide'><br/>(" + AdminStoreUtility.GetFormattedPrice(Convert.ToInt32(Eval("Quantity")) * Convert.ToDecimal(Eval("PriceInclTax")), CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity) + " incl tax)</span>" : string.Empty %>
                    </td>
                    <td>
                        <asp:PlaceHolder runat="server">
                            <a id='edit_<%# Eval("Id") %>' href="javascript:editMode('<%# Eval("Id") %>');"><i class="fa fa-pencil-square"></i></a>
                            <a id='cancel_<%# Eval("Id") %>' class="hide" href="javascript:cancelMode('<%# Eval("Id") %>');"><i class="fa fa-times-circle"></i></a>
                            <span id='update_<%# Eval("Id") %>' class="hide"><asp:LinkButton runat="server" CommandName="Update" CommandArgument='<%# Eval("Id") %>' Text="Update" OnClientClick="javascript:return confirm('Are you sure to update this item?');"><i class="fa fa-check-circle"></i></asp:LinkButton></span>
                            <span id='delete_<%# Eval("Id") %>' class="hide"><asp:LinkButton runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' Text="Delete" OnClientClick="javascript:return confirm('Are you sure to delete this item?');"><i class="fa fa-trash"></i></asp:LinkButton></span>
                        </asp:PlaceHolder>
                        <a href="javascript:void(0);" onclick="javascript:stockMode('<%# Eval("Id") %>');"><i class="fa fa-question-circle"></i></a>                                        
                    </td>
                </tr>
                <tr class="item_stock_<%# Eval("Id") %> stock hide">
                    <td colspan="10">
                        <h4 class="text-success">Stock <i class="fa fa-level-up" aria-hidden="true"></i></h4>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Option</th>
                                    <th>Apollo</th>
                                    <%
                                        var branches = OrderService.GetAllBranches();
                                        foreach (var branch in branches)
                                        {
                                    %>
                                        <th><%= branch.Name.ToUpper() %></th>
                                    <%
                                        }
                                    %>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="rptOptions" runat="server">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# Eval("Option") %></td>
                                            <td><%# Eval("Stock") %></td>
                                            <%
                                                var branches = OrderService.GetAllBranches();
                                                foreach (var branch in branches)
                                                {
                                            %>
                                                <td>
                                                    <div id='<%= branch.Name.ToLower() %>_<%# Eval("Id") + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") %>' class="<%= branch.Name.ToLower() %>">
                                                        <i class="info fa fa-question-circle" style="cursor: pointer;"></i>
                                                        <input type="hidden" value='<%= branch.Id %>' class="branchId" />
                                                        <input type="hidden" value='<%# Eval("Barcode") %>' class="barcode" />
                                                        <input type="hidden" value='<%= branch.Name.ToLower() %>_<%# Eval("Id") + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") %>' class="elementId" />
                                                    </div>
                                                </td>
                                            <%
                                                }
                                            %>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr class="item_<%# Eval("Id") %> edit hide">
                    <td colspan="10">                        
                        <h4 class="text-success">Edit <%# Eval("Name") %> <%# Eval("Option") %> <i class="fa fa-level-up" aria-hidden="true"></i></h4>                        
                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Note</label>
                                <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" Text='<%# Eval("Note") != null ? Eval("Note").ToString() : null %>'></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>Status</label>
                                <asp:DropDownList ID="ddlLineStatus" runat="server" CssClass="form-control" DataValueField="StatusCode" DataTextField="Status"></asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Option</label>
                                <asp:TextBox ID="txtOption" runat="server" CssClass="form-control" Text='<%# Eval("Option") %>'></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>Retail</label>                            
                                <div class="input-group">
                                    <span class="input-group-addon"><%# Eval("CurrencyCode") %></span>
                                    <asp:TextBox ID="txtRetail" runat="server" CssClass="form-control" Text='<%# AdminStoreUtility.GetFormattedPrice(Eval("PriceInclTax"), Eval("CurrencyCode").ToString(), CurrencyType.None, Convert.ToDecimal(Eval("ExchangeRate")), 2) %>'></asp:TextBox>
                                </div>           
                            </div>          
                        </div>
                        <div class="col-lg-3">
                            <div class="form-group">
                                <label>Giftwrap</label>
                                <asp:CheckBox ID="cbFreeWrapped" runat="server" Checked='<%# Convert.ToBoolean(Eval("FreeWrapped")) %>' />
                            </div>
                        </div>
                        <div class="col-lg-3">                            
                            <div class="form-group">
                                <label>Quantity</label>
                                <table class="table table-bordered">
                                    <tr>
                                        <td>Ordered</td>
                                        <td><asp:TextBox ID="txtOrdered" runat="server" CssClass="form-control" Text='<%# Eval("Quantity") %>'></asp:TextBox></td>
                                    </tr>
                                    <tr>
                                        <td>Pending</td>
                                        <td><asp:TextBox ID="txtPending" runat="server" CssClass="form-control" Text='<%# Eval("PendingQuantity") %>'></asp:TextBox></td>
                                    </tr>
                                    <%--<tr>
                                        <td>In Transit</td>
                                        <td><asp:TextBox ID="txtInTransit" runat="server"CssClass="form-control" Text='<%# Eval("InTransitQuantity") %>'></asp:TextBox></td>
                                    </tr>--%>
                                    <tr>
                                        <td>Allocated</td>
                                        <td><asp:TextBox ID="txtAllocated" runat="server" CssClass="form-control" Text='<%# Eval("AllocatedQuantity") %>'></asp:TextBox></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>
                </tr>
            </ItemTemplate>            
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>        
    </div>
</div>

<div class="col-lg-8">    
    <asp:PlaceHolder ID="phNewCommentSection" runat="server">
        <div class="panel panel-default">
            <div class="panel-heading">
                Action
            </div>
            <table class="table">
                <tr>
                    <th>Status</th>
                    <td>
                        <asp:DropDownList ID="ddlOrderStatus" runat="server" CssClass="form-control order-status-list">
                            <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                            <asp:ListItem Text="Order Placed" Value="OP"></asp:ListItem>
                            <asp:ListItem Text="Awaiting Reply" Value="AR"></asp:ListItem>
                            <asp:ListItem Text="Shipping" Value="S"></asp:ListItem>
                            <asp:ListItem Text="Shipping, Ignore Earned Points" Value="S_NO_POINT"></asp:ListItem>
                            <asp:ListItem Text="Stock Allocation" Value="SW"></asp:ListItem>
                            <asp:ListItem Text="On Hold" Value="OH"></asp:ListItem>
                            <asp:ListItem Text="Cancelled" Value="C"></asp:ListItem>
                        </asp:DropDownList>
                        <div id="s-message" class="alert alert-success order-status-change-message" style="display: none;">By selecting "Shipping" status, points earned from this order will be added to (registered) customer loyalty points. <strong>Used with caution, multiple assignments could add points repeatedly.</strong></div>
                    </td>
                </tr>
                <tr>
                    <th>Progress</th>
                    <td><asp:DropDownList ID="ddlIssue" runat="server" DataTextField="Issue" DataValueField="IssueCode" CssClass="form-control"/></td>
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
                    <th>Reset alert</th>
                    <td><asp:CheckBox ID="cbResetAlert" runat="server" Checked="true" /></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:LinkButton ID="lbSubmitAction" ValidationGroup="vgNewComment" runat="server" Text="Submit" CssClass="btn btn-outline btn-info pull-right" OnClick="lbSubmitAction_Click"></asp:LinkButton></td>
                </tr>
            </table>            
        </div>
    </asp:PlaceHolder>  

    <asp:PlaceHolder ID="phCommentSection" runat="server">
        <asp:Repeater ID="rptComments" runat="server">
            <HeaderTemplate>
                <div class="panel panel-default">
                    <div class="panel-heading">
                        Comments (Last 5 entries)
                        <span class="pull-right label label-plain">
                            <a data-toggle="collapse" href="#comment">view</a>
                        </span>
                    </div>
                    <div id="comment">
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
                        <td><a class="btn btn-outline btn-info pull-right" href="/sales/order_comments_default.aspx?orderid=<%= QueryOrderId %>">More</a></td>    
                    </tr>
                    </asp:PlaceHolder>
                </table>
                </div>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </asp:PlaceHolder>
</div>

<div class="col-lg-4">
    <asp:PlaceHolder ID="phSummarySection" runat="server">
    <div class="panel panel-default">
        <div class="panel-heading">
            Order totals
        </div>
        <table class="table table-striped">
            <tr>
                <td>Subtotal (excluding tax)</td>
                <td class="text-right"><asp:Literal ID="ltlSubtotal" runat="server"></asp:Literal></td>
            </tr>
            <tr <%= HasDiscountOffer() ? string.Empty : "printHide" %>'>
                <td>Discount Offer</td>
                <td class="text-right"><asp:Literal ID="ltlDiscount" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>Loyalty Point</td>
                <td class="text-right"><asp:Literal ID="ltlLoyaltyPoint" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>Tax</td>
                <td class="text-right"><asp:Literal ID="ltlTax" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>Delivery</td>
                <td class="text-right"><asp:Literal ID="ltlShipping" runat="server"></asp:Literal></td>
            </tr>                            
            <tr class="bg-warning">
                <td style="font-weight: bold;">Grand Total</td>
                <td class="text-right"><asp:Literal ID="ltlGrandTotal" runat="server"></asp:Literal></td>
            </tr>                       
            <tr class="bg-danger">
                <td><b>Total Paid</b></td>
                <td class="text-right"><asp:Literal ID="ltlTotalPaid" runat="server"></asp:Literal></td>
            </tr>
            <tr class="bg-success">
                <td><b>Total Refunded</b></td>
                <td class="text-right"><asp:Literal ID="ltlTotalRefund" runat="server"></asp:Literal></td>
            </tr>                     
        </table>
    </div>
    </asp:PlaceHolder>
</div>

<script type="text/javascript">
    function editMode(id) {
        targetId = '.item_' + id;
        infoId = '.info_' + id;
        
        $('.info').removeClass('hide');
        $('.edit').removeClass('show');
        
        $(targetId).removeClass('hide');
        $(infoId).removeClass('show');
        
        $('#edit_' + id).removeClass('show');        
        $('#cancel_' + id).removeClass('hide');        
        $('#update_' + id).removeClass('hide');        
        $('#delete_' + id).removeClass('hide');
    }

    function stockMode(id) {
        targetId = '.item_stock_' + id;
        $(targetId).removeClass('hide');        
    }
    
    function cancelMode(id) {
        targetId = '.item_' + id;
        infoId = '.info_' + id;
        
        $('.info').removeClass('hide');
        $('.info').addClass('show');
        $('.edit').removeClass('show');
        $('.edit').addClass('hide');
         
        $(targetId).removeClass('show');
        $(targetId).addClass('hide');
        $(infoId).removeClass('hide');
        $(infoId).addClass('show');
        
        $('#cancel_' + id).removeClass('show');
        $('#cancel_' + id).addClass('hide');            
        $('#update_' + id).removeClass('show');
        $('#update_' + id).addClass('hide');
        $('#delete_' + id).removeClass('show');
        $('#delete_' + id).addClass('hide');
        $('#edit_' + id).removeClass('hide');
        $('#edit_' + id).addClass('show');
    }
</script>