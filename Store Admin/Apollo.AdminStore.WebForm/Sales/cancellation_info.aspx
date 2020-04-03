<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.cancellation_info" Codebehind="cancellation_info.aspx.cs" ValidateRequest="false" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderInfo" Src="~/UserControls/OrderInfoControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Cancellation</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>
    
    <Apollo:OrderNav runat="server" Type="Cancellation" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgRefund" CssClass="alert alert-warning" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <ul class="nav nav-tabs hidden-print">
                            <li class="active"><a href="#">Information</a></li>
                            <li><asp:HyperLink runat="server" ID="hlOrderView" Text="Order Information"></asp:HyperLink></li>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <Apollo:OrderInfo ID="eoiView" NewCommentAreaVisible="false" CommentAreaVisible="true" SummaryAreaVisible="false" runat="server" OnActionOccurred="eoiView_ActionOccurred" />
                                    <div class="col-lg-12">
                                        <div class="panel panel-info">
                                            <div class="panel-heading">
                                                Order totals
                                            </div>
                                             <table class="table">
                                                <tr>
                                                    <th>Paid Amount</th>
                                                    <th>Shipping Amount</th>
                                                    <th>Order Grand Total</th>
                                                    <th>Refund Amount</th>
                                                </tr>
                                                <tr>
                                                    <td><asp:Literal ID="ltlPaidAmount" runat="server"></asp:Literal></td>
                                                    <td><asp:Literal ID="ltlShippingAmount" runat="server"></asp:Literal></td>
                                                    <td><asp:Literal ID="ltlOrderGrandTotal" runat="server"></asp:Literal></td>
                                                    <td><asp:Literal ID="ltlRefundedAmount" runat="server"></asp:Literal></td>
                                                </tr>
                                            </table>
                                         </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="panel panel-info">
                                            <div class="panel-heading">
                                                Refund information
                                            </div>
                                            <table class="table">
                                                <tr class="questions">
                                                    <td>
                                                        <span class="q clearfix">How much do you wish to refund?</span>
                                                        <div class="label label-success">Please check if you need to include VAT in the value that you wish to refund.</div>
                                                        <div class="input-group">
                                                            <span class="input-group-addon"><asp:Literal ID="ltlCurrencyCode" runat="server"></asp:Literal></span>
                                                            <asp:TextBox ID="txtRefund" CssClass="form-control" runat="server" Text="0"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <span class="q clearfix">How many points do you wish to refund?</span>
                                                        <div class="label label-success">Please check if you need to include VAT in the points that you wish to refund.</div>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtPoint" runat="server" CssClass="form-control" Text="0"></asp:TextBox>
                                                            <span class="input-group-addon">points</span>
                                                        </div>                                                        
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <div class="panel panel-info">
                                            <div class="panel-heading">
                                                Refund description
                                            </div>
                                            <table class="table">
                                                <tr><td>Comment</td></tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Height="90px" Width="100%"></asp:TextBox>
                                                        <asp:RequiredFieldValidator Display="Dynamic" ControlToValidate="txtComment" runat="server" ValidationGroup="vgRefund" ErrorMessage="Comment is required."
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <asp:LinkButton ID="lbConfirm" ValidationGroup="vgRefund" runat="server" CssClass="btn btn-primary" Text="Confirm Cancellation" OnClick="lbConfirm_Click"></asp:LinkButton>
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