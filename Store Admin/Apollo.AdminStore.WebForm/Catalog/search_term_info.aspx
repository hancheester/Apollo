<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.search_term_info" Codebehind="search_term_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" Runat="Server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Search Term</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>        
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary ID="vsItemSum" runat="server" CssClass="alert alert-warning" DisplayMode="BulletList" ValidationGroup="vgEditItem" />    
                <div class="col-lg-6">
                    <table class="table table-striped">
                        <tr>
                            <th>Query<strong>*</strong></th>
                            <td>
                                <asp:TextBox ID="txtQuery" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Query is required." ControlToValidate="txtQuery"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <th>Redirect URL<strong>*</strong></th>
                            <td>
                                <asp:TextBox ID="txtRedirectUrl" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Redirect URL is required." ControlToValidate="txtRedirectUrl"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>        
                    <div class="hr-line-dashed"></div>
                    <div class="col-lg-12">
                        <a href="/catalog/search_term_default.aspx" class="btn btn-sm btn-default">Back</a>
                        <asp:LinkButton ID="lbSaveContinue" runat="server" ValidationGroup="vgEditItem" Text="Update" OnClick="lbSaveContinue_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                        <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure to delete this search term?');" CssClass="btn btn-sm btn-danger"></asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>