<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_sitemap_default.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_sitemap_default" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Sitemap</h2>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="alert alert-info">
                    A sitemap XML file (sitemap.xml) will be created under the store front's root directory.
                </div>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12 row">
                    <asp:LinkButton ID="lbGenerate" runat="server" Text="Generate" OnClick="lbGenerate_Click" CssClass="btn btn-sm btn-danger"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>