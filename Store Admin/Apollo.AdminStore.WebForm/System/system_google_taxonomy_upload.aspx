<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="system_google_taxonomy_upload.aspx.cs" Inherits="Apollo.AdminStore.WebForm.System.system_google_taxonomy_upload" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Google Taxonomy Upload</h2>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="alert alert-info">
                    By uploading new Google Taxonomy, all existing assignment for products will be removed.
                </div>
                <table class="table table-striped">
                    <tr>
                        <th>Please select a file <small class="text-navy clearfix">(<a target="_blank" href="https://support.google.com/merchants/answer/1705911">click here</a> to download the latest Google Taxonomy file)</small></th>
                        <td><asp:FileUpload ID="prductFileUpload" runat="server" CssClass="form-control" /></td>
                    </tr>
                 </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12 row">
                    <asp:LinkButton ID="lbFileUpload" runat="server" Text="Upload" OnClick="lbFileUpload_Click" OnClientClick="return confirm('This action will remove all existing Google Taxonomy and existing assignment for products will be removed too.\nAre you sure to upload?');" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>