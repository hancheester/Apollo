<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_settings_blog.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_settings_blog" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Settings</h2>
            <h3>Blog</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Blog enabled</th>
                        <td><asp:CheckBox ID="cbEnabled" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Allow guests to leave comments</th>
                        <td><asp:CheckBox ID="cbAllowNotRegisteredUsersToLeaveComments" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Notify about new blog comments</th>
                        <td><asp:CheckBox ID="cbNotifyAboutNewBlogComments" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>Posts page size</th>
                        <td><asp:TextBox ID="txtPostsPageSize" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Number of tags (cloud)</th>
                        <td><asp:TextBox ID="txtNumberOfTags" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Display blog RSS feed link in the browser address bar</th>
                        <td><asp:CheckBox ID="cbShowHeaderRssUrl" runat="server" /></td>
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