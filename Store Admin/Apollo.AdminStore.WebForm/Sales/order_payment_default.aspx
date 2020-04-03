<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_payment_default" Codebehind="order_payment_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderScript" runat="server">
    <script type="text/javascript" src="/js/order.js"></script>
    <script type="text/javascript">        
        function loadData(msg, context) {
            $("#" + context).html(msg);
        }
        
        function loadLog(msg, context) {
            $("#" + context + " > .log").html(msg);
        }
        
        function getTranLog(type, id, logId) {
            $("#" + id).css("display", "table-row");
            <%= ClientScript.GetCallbackEventReference(this, "type + '_' + logId", "loadLog", "id", true) %>;
        }      
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Order</h2>
            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        </div>
        <Apollo:OrderPrevNext runat="server"></Apollo:OrderPrevNext>
    </div>

    <Apollo:OrderNav runat="server" />
    
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:OrderSideMenu runat="server" Type="Transaction"></Apollo:OrderSideMenu>
                         <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <asp:Repeater ID="rptLogs" runat="server">
                                        <HeaderTemplate>
                                            <table class="table table-striped">
                                                <tr>
                                                    <th>Date Time</th>
                                                    <th>Log</th>
                                                    <th>Action</th>
                                                </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>  
                                            <tr>
                                                <td><%# Eval("TimeStamp") %></td>
                                                <td><%# Eval("Status") %></td>
                                                <td><a href="javascript:getTranLog('sp', 'sagePayLogId_<%# Eval("Id") %>', <%# Eval("Id") %>);">show</a></td>
                                            </tr>
                                            <tr id='sagePayLogId_<%# Eval("Id") %>' style="display:none;">
                                                <td>Log</td>
                                                <td colspan="2" class="log"><i class="fa fa-spinner fa-spin"></i></td>
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
</asp:Content>