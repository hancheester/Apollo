﻿<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_settings_currency.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_settings_currency" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Settings</h2>
            <h3>Currency</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Primary store currency ID</th>
                        <td><asp:TextBox ID="txtPrimaryStoreCurrencyId" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Primary store currency code</th>
                        <td><asp:TextBox ID="txtPrimaryStoreCurrencyCode" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Provider link for exchange rate</th>
                        <td><asp:TextBox ID="txtExchangeRateProviderLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Exchange rate factor</th>
                        <td><asp:TextBox ID="txtExchangeRateFactor" runat="server" CssClass="form-control"></asp:TextBox></td>
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