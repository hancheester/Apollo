<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_email_payment_default" Codebehind="order_email_payment_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderInfo" Src="~/UserControls/OrderInfoControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
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
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Order</h2>
            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        </div>
        <Apollo:OrderPrevNext runat="server"></Apollo:OrderPrevNext>
    </div>

    <Apollo:OrderNav runat="server" ID="eonNav" OnActionOccurred="eonNav_ActionOccurred" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:OrderSideMenu runat="server" Type="EmailPayment"></Apollo:OrderSideMenu>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <table class="table table-striped">
                                        <tr>
                                            <th>Recipient's name<strong>*</strong></th>
                                            <td><asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Recipient's email<strong>*</strong></th>
                                            <td><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Recipient's contact number<strong>*</strong></th>
                                            <td><asp:TextBox ID="txtContactNumber" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Currency<strong>*</strong></th>
                                            <td>
                                                <asp:DropDownList runat="server" CssClass="form-control" ID="ddlCurrency" DataTextField="CurrencyCode" DataValueField="CurrencyCode" OnInit="ddlCurrency_OnInit">
                                                </asp:DropDownList>
                                            </td>
                                         </tr>
                                        <tr>
                                            <th>Amount<strong>*</strong></th>
                                            <td><asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator ValidationGroup="vgNewInvoice" runat="server"
                                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Amount is required."
                                                ControlToValidate="txtAmount"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr> 
                                        <tr>
                                            <th>Messsage<strong>*</strong></th>
                                            <td><asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Height="100" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>End date<strong>*</strong><small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox ID="txtEndDate" runat="server" CssClass="date form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator ValidationGroup="vgNewInvoice" runat="server"
                                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Date is required."
                                                ControlToValidate="txtEndDate"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbSend" runat="server" ValidationGroup="vgNewInvoice" Text="Send email payment" 
                                            OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to send customer the email payment?');" OnClick="lbSend_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
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