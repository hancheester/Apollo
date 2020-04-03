<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.FulFillment.order_line_items" Codebehind="order_line_items.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript">
        $(function() {
	        // Set interval for alert
            setInterval('checkAlert()', 2500);

            if (typeof loading !== 'undefined' && $.isFunction(loading)) {
                loading();
            }

            $('.generate').click(function () {
                setTimeout('delayedHide()', 1);
            });
        });

        function delayedHide() {
            var $lbPrint = $('#<%= lbPrintNote.ClientID %>');
            $lbPrint.replaceWith("<a class='btn btn-sm btn-danger'>Please wait...</a>");
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
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Ordered Line Items</h2>
        </div>
    </div>

    <div class="row wrapper hidden-print">
        <div class="col-lg-12">
            <div class="row"><p></p></div>
            <div class="row">
                <div class="pull-right">
                    <a href="/fulfillment/order_line_items.aspx" class="btn btn-sm btn-default">Refresh</a>
                    <asp:LinkButton runat="server" ID="lbPrintNote" Text="Generate note" CssClass="btn btn-sm btn-danger generate" OnClick="lbPrintNote_Click"></asp:LinkButton>                    
                </div>
            </div>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight hidden-print">
        <div class="row">
            <div class="col-lg-12">
                <div class="alertBox alert alert-warning" style="display: none;"></div>
            </div>
            <div class="col-lg-3">                        
                <Apollo:NoticeBox ID="enbInfo" runat="server"/>
                    <asp:Repeater ID="rptList" runat="server">
                    <HeaderTemplate>
                        <table class="table table-striped table-bordered table-hover dataTable">
                        <tr>
                            <th>Dates</th>
                            <th>Number of Orders</th>
                            <th>Select All<br /></bt><input type="checkbox" onclick="toggle_chosen(this);"/></th>
                        </tr>
                    </HeaderTemplate>        
                    <ItemTemplate>
                        <tr>
                            <td><asp:Literal ID="ltlDate" runat="server" Text='<%# GetProperDateString(Eval("Date")) %>'></asp:Literal></td>
                            <td><%# Eval("NumOrder")%></td>
                            <td><asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" /></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>