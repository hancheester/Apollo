<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_payment_new" Codebehind="order_payment_new.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Email Payment</h2>
            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        </div>
        <Apollo:OrderPrevNext runat="server"></Apollo:OrderPrevNext>
    </div>

    <Apollo:OrderNav runat="server" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgEditTestimonial" CssClass="alert alert-warning" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:OrderSideMenu runat="server"></Apollo:OrderSideMenu>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <table class="table table-striped">
                                            <tr>
                                                <th>Payment Reference<strong>*</strong></th>
                                                <td><asp:TextBox ID="txtPaymentRef" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ValidationGroup="vgNewPayment" runat="server"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is required.</span>" Display="Dynamic" ErrorMessage="Payment reference is required."
                                                    ControlToValidate="txtPaymentRef"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Currency<strong>*</strong></th>
                                                <td>
                                                    <asp:DropDownList CssClass="form-control" runat="server" ID="ddlCurrency" DataTextField="CurrencyCode" DataValueField="CurrencyCode" OnInit="ddlCurrency_OnInit">
                                                    </asp:DropDownList>
                                                </td>
                                                </tr>
                                            <tr>
                                                <th>Amount<strong>*</strong></th>
                                                <td><asp:TextBox ID="txtAmount" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ValidationGroup="vgNewPayment" runat="server"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is required.</span>" Display="Dynamic" ErrorMessage="Amount is required."
                                                    ControlToValidate="txtAmount"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>

                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewBanner" Text="Save Payment" 
                                            OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to save the payment?');" OnClick="lbSave_Click" CssClass="btn btn-primary"></asp:LinkButton>
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