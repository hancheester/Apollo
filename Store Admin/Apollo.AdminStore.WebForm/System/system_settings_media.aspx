<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_settings_media.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_settings_media" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Settings</h2>
            <h3>Media</h3>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <table class="table table-striped">
                    <tr>
                        <th>Brand media path</th>
                        <td><asp:TextBox ID="txtBrandMediaPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Brand media local path</th>
                        <td><asp:TextBox ID="txtBrandMediaLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Category media path</th>
                        <td><asp:TextBox ID="txtCategoryMediaPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Category media local path</th>
                        <td><asp:TextBox ID="txtCategoryMediaLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Offer media path</th>
                        <td><asp:TextBox ID="txtOfferMediaPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Offer media local path</th>
                        <td><asp:TextBox ID="txtOfferMediaLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Product media path</th>
                        <td><asp:TextBox ID="txtProductMediaPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Product media local path</th>
                        <td><asp:TextBox ID="txtProductMediaLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Product colour path</th>
                        <td><asp:TextBox ID="txtProductColourPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Product colour local path</th>
                        <td><asp:TextBox ID="txtProductColourLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Medium banner path</th>
                        <td><asp:TextBox ID="txtMediumBannerPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Medium banner local path</th>
                        <td><asp:TextBox ID="txtMediumBannerLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Mini banner path</th>
                        <td><asp:TextBox ID="txtMiniBannerPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Mini banner local path</th>
                        <td><asp:TextBox ID="txtMiniBannerLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Large banner path</th>
                        <td><asp:TextBox ID="txtLargeBannerPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Large banner local path</th>
                        <td><asp:TextBox ID="txtLargeBannerLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Offer banner path</th>
                        <td><asp:TextBox ID="txtOfferBannerPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Offer banner local path</th>
                        <td><asp:TextBox ID="txtOfferBannerLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>No image path</th>
                        <td><asp:TextBox ID="txtNoImagePath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>No image local path</th>
                        <td><asp:TextBox ID="txtNoImageLocalPath" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Large logo link</th>
                        <td><asp:TextBox ID="txtLargeLogoLink" runat="server" CssClass="form-control"></asp:TextBox></td>
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