﻿<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_settings_store.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_settings_store" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Settings</h2>
            <h3>Store</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Company name</th>
                        <td><asp:TextBox ID="txtCompanyName" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Store link</th>
                        <td><asp:TextBox ID="txtStoreFrontLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Store secured link (https)</th>
                        <td><asp:TextBox ID="txtStoreFrontSecuredLink" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Payment terminal link</th>
                        <td><asp:TextBox ID="txtTermURL" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Display EU cookie law warning</th>
                        <td><asp:CheckBox ID="cbDisplayEuCookieLawWarning" runat="server" /></td>
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