<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_plugins_sagepay_default.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_plugins_sagepay_default" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Settings</h2>
            <h3>SagePay</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Vendor</th>
                        <td><asp:TextBox ID="txtSagePayVendor" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>VPS Protocol</th>
                        <td><asp:TextBox ID="txtSagePayVPSProtocol" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Payment Gateway Link</th>
                        <td><asp:TextBox ID="txtSagePayPaymentGatewayLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>3D Secure Call Back Link</th>
                        <td><asp:TextBox ID="txtSagePay3DSecureCallbackLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Register Release Link</th>
                        <td><asp:TextBox ID="txtSagePayRegisterReleaseLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Register Repeat Link</th>
                        <td><asp:TextBox ID="txtSagePayRegisterRepeatLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Register Refund Link</th>
                        <td><asp:TextBox ID="txtSagePayRegisterRefundLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Register Abort Link</th>
                        <td><asp:TextBox ID="txtSagePayRegisterAbortLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Reporting Admin Username</th>
                        <td><asp:TextBox ID="txtSagePayWebUser" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Reporting Admin Password</th>
                        <td><asp:TextBox ID="txtSagePayWebPwd" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Reporting Admin API Link</th>
                        <td><asp:TextBox ID="txtSagePayReportingAdminAPILink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                 </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12 row">
                    <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClick="lbPublish_Click" CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('This action will refresh all setting related data on store front and performance could be affected.\nAre you sure to publish?');"></asp:LinkButton>
                    <asp:LinkButton ID="lbUpdate" runat="server" Text="Update" OnClick="lbUpdate_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>