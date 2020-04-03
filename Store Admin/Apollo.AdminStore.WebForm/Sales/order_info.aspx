<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_info" Codebehind="order_info.aspx.cs" ValidateRequest="false" %>
<%@ Register TagPrefix="Apollo" TagName="OrderInfo" Src="~/UserControls/OrderInfoControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/order.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.order-status-list').on('change', function () {
                var selectedCode = this.value;
                console.log(this.value);
                $('#s-message').hide({ duration: 600, easing: 'swing' });

                if (selectedCode == 'S') {
                    $('#s-message').show({ duration: 600, easing: 'swing' });
                }
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

        function getStockLevel(branchId, barcode, id) {
            <%= ClientScript.GetCallbackEventReference(this, "'stock_' + branchId + '_' + barcode", "load_stock_level", "id", true) %>;
        }

        function load_stock_level(msg, context) {
            $('#' + context).html(msg);
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
        
    <Apollo:OrderNav runat="server" ID="eonNav" Type="Order" OnActionOccurred="eonNav_ActionOccurred" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:OrderSideMenu runat="server" Type="Information"></Apollo:OrderSideMenu>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <Apollo:OrderInfo ID="eoiView" runat="server" OnActionOccurred="eoiView_ActionOccurred" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>                
            </div>
        </div>
    </div>
</asp:Content>