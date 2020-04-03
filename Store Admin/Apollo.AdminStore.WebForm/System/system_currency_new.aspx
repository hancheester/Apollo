<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" ValidateRequest="false" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.System.system_currency_new" Codebehind="system_currency_new.aspx.cs" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Currency</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-6">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgNewItem" CssClass="alert alert-warning" />
                <table class="table table-striped">
                    <tr>
                        <th>Currency code<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtCurrencyCode" CssClass="form-control" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Currency code is required." ControlToValidate="txtCurrencyCode"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rev3" ValidationGroup="vgEditItem" Text="*Enter Alphabets" runat="server" Display="Dynamic" ErrorMessage="*Enter Alphabets" ControlToValidate="txtCurrencyCode" ValidationExpression="^[a-zA-Z ]+$"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Html entity<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtHtmlEntity" CssClass="form-control" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="HTML entity is required." ControlToValidate="txtHtmlEntity"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Symbol<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtSymbol" CssClass="form-control" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Symbol is required." ControlToValidate="txtSymbol"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Exchange rate<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtExchangeRate" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Exchange rate is required." ControlToValidate="txtExchangeRate"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/system/system_currency_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewItem" Text="Create" OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to save the currency?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                </div>  
            </div>
        </div>
    </div>
</asp:Content>
