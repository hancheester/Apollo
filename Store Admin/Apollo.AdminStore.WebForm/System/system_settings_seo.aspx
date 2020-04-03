<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_settings_seo.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_settings_seo" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Settings</h2>
            <h3>SEO</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                   <tr>
                       <th>Page title separator</th>
                       <td><asp:TextBox ID="txtPageTitleSeparator" runat="server" CssClass="form-control"></asp:TextBox></td>
                   </tr>
                   <tr>
                       <th>Page title SEO adjustment</th>
                       <td><asp:DropDownList ID="ddlPageTitleSeoAdjustments" runat="server" CssClass="form-control"></asp:DropDownList></td>
                   </tr>
                   <tr>
                       <th>Default page title</th>
                       <td><asp:TextBox ID="txtDefaultTitle" runat="server" CssClass="form-control"></asp:TextBox></td>
                   </tr>
                   <tr>
                       <th>Default meta keywords</th>
                       <td><asp:TextBox ID="txtDefaultMetaKeywords" runat="server" CssClass="form-control"></asp:TextBox></td>
                   </tr>
                   <tr>
                       <th>Default meta description</th>
                       <td><asp:TextBox ID="txtDefaultMetaDescription" runat="server" CssClass="form-control"></asp:TextBox></td>
                   </tr>
                   <tr>
                       <th>Generate product meta description</th>
                       <td><asp:CheckBox ID="cbGenerateProductMetaDescription" runat="server" /></td>
                   </tr>
                   <tr>
                       <th>Convert non-western characters</th>
                       <td><asp:CheckBox ID="cbConvertNonWesternChars" runat="server" /></td>
                   </tr>
                   <tr>
                       <th>Enable canonical URLs</th>
                       <td><asp:CheckBox ID="cbCanonicalUrlsEnabled" runat="server" /></td>
                   </tr>
                   <tr>
                       <th>Allow unicode character in URLs</th>
                       <td><asp:CheckBox ID="cbAllowUnicodeCharsInUrls" runat="server" /></td>
                   </tr>
                   <tr>
                       <th>WWW prefix requirement</th>
                       <td><asp:DropDownList ID="ddlWwwRequirements" runat="server" CssClass="form-control"></asp:DropDownList></td>
                   </tr>
                   <tr>
                       <th>JavaScript bundling and minification</th>
                       <td><asp:CheckBox ID="cbEnableJsBundling" runat="server" /></td>
                   </tr>
                   <tr>
                       <th>CSS bundling and minification</th>
                       <td><asp:CheckBox ID="cbEnableCssBundling" runat="server" /></td>
                   </tr>
                   <tr>
                       <th>Twitter META tags</th>
                       <td><asp:CheckBox ID="cbTwitterMetaTags" runat="server" /></td>
                   </tr>
                   <tr>
                       <th>Open Graph META tags</th>
                       <td><asp:CheckBox ID="cbOpenGraphMetaTags" runat="server" /></td>
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