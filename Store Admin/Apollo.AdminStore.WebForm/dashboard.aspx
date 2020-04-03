<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="Apollo.AdminStore.WebForm.dashboard" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderScript" runat="server">
    <script type="text/javascript">
        function showCounter(type) {
            $('#' + type).find('.info').attr('class', 'fa fa-spinner fa-spin');
            <%= ClientScript.GetCallbackEventReference(this, "type", "loadCounter", "type", true) %>;
        }

        function loadCounter(msg, context) {
            $('#' + context).html(msg);
        }

    </script>   
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-10">
            <h2>Dashboard</h2>
        </div>
        <div class="col-lg-2"></div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row animated fadeInRight">            
            <div class="col-lg-6">
                <div class="ibox float-e-margins">
                    <div class="ibox-title">
                        <h5>Incomplete Orders</h5>
                        <div class="ibox-tools">
                            <a class="collapse-link"><i class="fa fa-chevron-up"></i></a>
                            <a class="close-link"><i class="fa fa-times"></i></a>
                        </div>
                    </div>
                    <div class="ibox-content">
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Status</th>
                                    <th>Count</th>
                                </tr>
                            </thead>
                            <tbody>
                            <asp:Repeater ID="rpIncompleteOrders" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("Item")%>  </td>                                        
                                        <td><%# Eval("Count")%> </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>                            
                        </table>                        
                    </div>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="ibox float-e-margins">
                    <div class="ibox-title">
                        <h5>Registration Activity</h5>
                        <div class="ibox-tools">
                            <a class="collapse-link"><i class="fa fa-chevron-up"></i></a>
                            <a class="close-link"><i class="fa fa-times"></i></a>
                        </div>
                    </div>
                    <div class="ibox-content">                    
                        <table class="table table-striped">
                            <thead>
                            <tr>
                                <th>Period</th>                                
                                <th>Count</th>                                
                            </tr>
                            </thead>
                            <tbody>
                            <asp:Repeater ID="rpRegistrationActivity" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("Period")%></td>                                        
                                        <td><%# Eval("Count")%> </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>                            
                        </table>                        
                    </div>
                </div>
            </div>
        </div>
        <div class="row animated fadeInRight">
            <div class="col-lg-12">
                <div class="ibox float-e-margins">
                    <div class="ibox-content" id="ibox-content">
                        <div class="text-center float-e-margins p-md">
                            <h4>Order Process Workflow</h4>
                        </div>
                        <div id="vertical-timeline" class="vertical-container dark-timeline center-orientation">
                            <div class="vertical-timeline-block">
                                <div class="vertical-timeline-icon navy-bg">
                                    <i class="fa fa-exclamation-circle"></i>
                                </div>

                                <div class="vertical-timeline-content">
                                    <h2>Charge Failed</h2>
                                    <p>Order which is failed to complete payment.</p>
                                    <a href="/sales/order_default.aspx?orderissuecode=ic13" class="btn btn-sm btn-primary">Go to orders</a>
                                    <span class="vertical-date">
                                        Charge Failed<br>
                                        <small>no. of orders: <span id="charge_failed" onclick="showCounter('charge_failed')"><i class="info fa fa-question-circle"></i></span></small>
                                    </span>
                                </div>
                            </div>

                            <div class="vertical-timeline-block">
                                <div class="vertical-timeline-icon blue-bg">
                                    <i class="fa fa-file-text"></i>
                                </div>

                                <div class="vertical-timeline-content">
                                    <h2>OTC Order</h2>
                                    <p>Order which contains pharmaceutical item and form.</p>
                                    <a href="/sales/order_default.aspx?orderissuecode=ic39&orderstatuscode=oh" class="btn btn-sm btn-success">Go to orders</a>

                                    <div class="clearfix"></div>
                                    
                                    <h2>OTC Order with System Check</h2>
                                    <p>Order which contains pharmaceutical item and form. However, the order has failed system check.</p>
                                    <a href="/sales/order_default.aspx?orderissuecode=ic42" class="btn btn-sm btn-info">Go to orders</a>

                                    <span class="vertical-date">OTC Order
                                        <br>
                                        <small>no. of orders: <span id="otc_order" onclick="showCounter('otc_order')"><i class="info fa fa-question-circle"></i></span></small>
                                    </span>
                                </div>
                            </div>

                            <div class="vertical-timeline-block">
                                <div class="vertical-timeline-icon yellow-bg">
                                    <i class="fa fa-bullhorn"></i>
                                </div>

                                <div class="vertical-timeline-content">
                                    <h2>System Check</h2>
                                    <p>Order which failed system check. For instance, an order might have suspicious address.</p>
                                    <a href="/sales/order_default.aspx?orderissuecode=ic3" class="btn btn-sm btn-info">Go to orders</a>
                                    <span class="vertical-date">System Check
                                        <br>
                                        <small>no. of orders: <span id="system_check" onclick="showCounter('system_check')"><i class="info fa fa-question-circle"></i></span></small>
                                    </span>
                                </div>
                            </div>

                            <div class="vertical-timeline-block">
                                <div class="vertical-timeline-icon lazur-bg">
                                    <i class="fa fa-gift"></i>
                                </div>

                                <div class="vertical-timeline-content">
                                    <h2>Order Placed</h2>
                                    <p>Order which has completed payment and passed system check.</p>
                                    <a href="/sales/order_default.aspx?orderstatuscode=op" class="btn btn-sm btn-info">Go to orders</a>

                                    <div class="clearfix"></div>

                                    <h2>Refunds</h2>
                                    <p>Order which has scheduled for refund.</p>
                                    <a href="/sales/refund_default.aspx" class="btn btn-sm btn-success">Go to orders</a>

                                    <div class="clearfix"></div>

                                    <h2>Cancellation</h2>
                                    <p>Order which has scheduled for cancellation.</p>
                                    <a href="/sales/cancellation_default.aspx" class="btn btn-sm btn-warning">Go to orders</a>

                                    <span class="vertical-date">Order Placed
                                        <br>
                                        <small>no. of orders: <span id="order_placed" onclick="showCounter('order_placed')"><i class="info fa fa-question-circle"></i></span></small>
                                    </span>
                                </div>
                            </div>

                            <div class="vertical-timeline-block">
                                <div class="vertical-timeline-icon navy-bg">
                                    <i class="fa fa-comments"></i>
                                </div>

                                <div class="vertical-timeline-content">
                                    <h2>Awaiting Reply</h2>
                                    <p>Order which is awaiting reply from client.</p>
                                    <a href="/sales/order_default.aspx?orderstatuscode=ar" class="btn btn-sm btn-success">Go to orders</a>
                                    <span class="vertical-date">Awaiting Reply
                                        <br>
                                        <small>no. of orders: <span id="awaiting_reply" onclick="showCounter('awaiting_reply')"><i class="info fa fa-question-circle"></i></span></small>
                                    </span>
                                </div>
                            </div>

                            <div class="vertical-timeline-block">
                                <div class="vertical-timeline-icon lazur-bg">
                                    <i class="fa fa-arrows"></i>
                                </div>

                                <div class="vertical-timeline-content">
                                    <h2>Distribution</h2>
                                    <p>Place where you decide which branch to supply items.</p>
                                    <a href="/fulfillment/order_line_item_distribution_async.aspx" class="btn btn-sm btn-info">Go to distribution</a>

                                    <div class="clearfix"></div>

                                    <h2>Ordered Line Items</h2>
                                    <p>Order which is ready for distribution.</p>
                                    <a href="/fulfillment/order_line_items.aspx" class="btn btn-sm btn-success">Go to lines</a>

                                    <span class="vertical-date">Distribution</span>
                                </div>
                            </div>

                            <div class="vertical-timeline-block">
                                <div class="vertical-timeline-icon navy-bg">
                                    <i class="fa fa-truck"></i>
                                </div>

                                <div class="vertical-timeline-content">
                                    <h2>Packing</h2>
                                    <p>Order which is ready for despatch.</p>
                                    <a href="/fulfillment/order_fulfillment.aspx" class="btn btn-sm btn-success">Go to orders</a>
                                    <span class="vertical-date">Packing
                                        <br>
                                        <small>no. of order: <span id="packing" onclick="showCounter('packing')"><i class="info fa fa-question-circle"></i></span></small>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
