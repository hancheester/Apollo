<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="custom_dict_info.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Catalog.custom_dict_info" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" Runat="Server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Custom Dictionary</h2>
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
                            <th>Word<strong>*</strong></th>
                            <td>
                                <asp:TextBox ID="txtWord" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Word is required." ControlToValidate="txtWord"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>        
                    <div class="hr-line-dashed"></div>
                    <div class="col-lg-12">
                        <a href="/catalog/custom_dict_default.aspx" class="btn btn-sm btn-default">Back</a>
                        <asp:LinkButton ID="lbUpdate" runat="server" ValidationGroup="vgEditItem" Text="Update" OnClick="lbUpdate_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                        <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure to delete?');" CssClass="btn btn-sm btn-danger"></asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>