<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.search_term_new" Codebehind="search_term_new.aspx.cs" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Search Term</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" CssClass="alert alert-warning" ValidationGroup="vgNewItem" />
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
                        <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewItem" Text="Create" OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to save the search term?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                    </div>
                </div>    
            </div>
        </div>
    </div>    
</asp:Content>