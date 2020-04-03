<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_settings_email.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_settings_email" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Settings</h2>
            <h3>Email</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Email host</th>
                        <td><asp:TextBox ID="txtEmailHost" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Email username</th>
                        <td><asp:TextBox ID="txtEmailUsername" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Email password</th>
                        <td><asp:TextBox ID="txtEmailPassword" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Email from</th>
                        <td><asp:TextBox ID="txtEmailFrom" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Email display from</th>
                        <td><asp:TextBox ID="txtEmailDisplayFrom" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Email bcc</th>
                        <td><asp:TextBox ID="txtEmailBCC" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Contact us email</th>
                        <td><asp:TextBox ID="txtContactUsEmail" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Account register email template local path</th>
                        <td><asp:TextBox ID="txtAccountRegisterEmailTemplateLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Account register with password email template local path</th>
                        <td><asp:TextBox ID="txtAccountRegisterWithPasswordEmailTemplateLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Password retrieval email template local path</th>
                        <td><asp:TextBox ID="txtPasswordRetrievalEmailTemplateLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>New username email template local path</th>
                        <td><asp:TextBox ID="txtNewUsernameEmailTemplateLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Despatch confirmation email template local path</th>
                        <td><asp:TextBox ID="txtDespatchConfirmationEmailTemplateLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Order confirmation email template local path</th>
                        <td><asp:TextBox ID="txtOrderConfirmationEmailTemplateLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Payment invoice email template local path</th>
                        <td><asp:TextBox ID="txtPaymentInvoiceEmailTemplateLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Payment invoice confirmation email template local path</th>
                        <td><asp:TextBox ID="txtPaymentInvoiceConfirmationEmailTemplateLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
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