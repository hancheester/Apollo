﻿<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_widgets_google_analytics_default.aspx.cs" ValidateRequest="false" Inherits="Apollo.AdminStore.WebForm.System.system_widgets_google_analytics_default" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Widgets</h2>
            <h3>Google Analytics</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Google Analytics ID</th>
                        <td><asp:TextBox ID="txtGoogleId" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>
                            Tracking code with {ECOMMERCE} line
                            <small class="text-navy clearfix">(paste the tracking code generated by<br />Google Analytics here. {GOOGLEID} and {ECOMMERCE} <br />will be dynamically replaced.)</small>
                        </th>
                        <td><asp:TextBox ID="txtTrackingScript" TextMode="MultiLine" Height="200px" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>
                            Tracking code for {ECOMMERCE} part, with {DETAILS} line
                            <small class="text-navy clearfix">(paste the tracking code generated by<br />Google analytics here. {ORDERID}, {SITE}, {TOTAL},<br />{TAX}, {SHIP}, {CITY}, {STATEPROVINCE},<br />{COUNTRY}, {DETAILS} will be dynamically replaced.)</small>
                        </th>
                        <td><asp:TextBox ID="txtEcommerceScript" TextMode="MultiLine" Height="100px" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>
                            Tracking code for {DETAILS} part
                            <small class="text-navy clearfix">(paste the tracking code generated by<br />Google analytics here. {ORDERID}, {PRODUCTSKU},<br />{PRODUCTNAME}, {CATEGORYNAME}, {UNITPRICE},<br />{QUANTITY} will be dynamically replaced.)</small>
                        </th>
                        <td><asp:TextBox ID="txtEcommerceDetailScript" TextMode="MultiLine" Height="100px" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>
                            Include tax
                            <small class="text-navy clearfix">(check to include tax when generating tracking code for {ECOMMERCE} part.)</small>
                        </th>
                        <td><asp:CheckBox ID="cbIncludingTax" runat="server" /></td>
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