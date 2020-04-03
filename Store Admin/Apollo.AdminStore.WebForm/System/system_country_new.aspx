<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.System.system_country_new" Codebehind="system_country_new.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Country</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-6">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary ID="vsItemSum" runat="server" DisplayMode="BulletList" ValidationGroup="vgEditItem" CssClass="alert alert-warning" />
                <table class="table table-striped">
                    <tr>
                        <th>Name<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfName" ValidationGroup="vgEditItem" runat="server" Text="<br/><i class='fa fa-exclamation-circle'></i> Required." Display="Dynamic" ErrorMessage="Name is required." ControlToValidate="txtName"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>ISO3166Code<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtISO3166Code" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="ISO3166Code" ValidationGroup="vgEditItem" runat="server" Text="<br/><i class='fa fa-exclamation-circle'></i> Required." Display="Dynamic" ErrorMessage="ISO3166Code is required." ControlToValidate="txtISO3166Code"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Is EU country</th>
                        <td><asp:CheckBox ID="cbIsEC" runat="server"/></td>
                    </tr>
                    <tr>
                        <th>Enabled</th>
                        <td><asp:CheckBox ID="cbEnabled" runat="server" /></td>
                    </tr>                    
                </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/system/system_country_default.aspx" class="btn btn-sm btn-default">Back</a>                    
                    <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewItem" Text="Create" OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to create the Country?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>                    
                </div>
            </div>
        </div>
    </div>
</asp:Content>

