<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="custom_dict_new.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Catalog.custom_dict_new" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Custom Dictionary</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" CssClass="alert alert-warning" ValidationGroup="vgNewItem" />
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
                        <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewItem" Text="Create" OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to save?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                    </div>
                </div>    
            </div>
        </div>
    </div>
</asp:Content>