<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" ValidateRequest="false" Inherits="Apollo.AdminStore.WebForm.System.system_currency_info" Codebehind="system_currency_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="https://cdnjs.cloudflare.com/ajax/libs/flag-icon-css/2.1.0/css/flag-icon.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Currency</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-6">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary ID="vsItemSum" runat="server" DisplayMode="BulletList" ValidationGroup="vgEditItem" CssClass="alert alert-warning" />
                <table class="table table-striped">
                    <tr>
                        <th>Currency code<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtCurrencyCode" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Currency code is required." ControlToValidate="txtCurrencyCode"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Html entity<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtHtmlEntity" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="HTML entity is required." ControlToValidate="txtHtmlEntity"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Symbol<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtSymbol" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Symbol is required." ControlToValidate="txtSymbol"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Exchange rate<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtExchangeRate" CssClass="form-control" runat="server"> </asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Exchange rate is required." ControlToValidate="txtExchangeRate"></asp:RequiredFieldValidator>
                        </td>
                    </tr>            
                    <tr>
                        <th>Associated countries</th>
                        <td>
                            <table class="table">
                                <asp:Repeater runat="server" ID="rptCountries" onitemcommand="rptCountries_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%# GetCountryImage(Convert.ToInt32(Eval("CountryId")))%>
                                            </td>
                                            <td>
                                                <asp:LinkButton runat="server" CommandName="delete" CommandArgument='<%# Eval("Id") %>' Text="Remove"></asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr>
                                    <td>
                                        <asp:DropDownList runat="server" ID="ddlCountry" CssClass="form-control" OnInit="ddlCountry_Init"></asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:LinkButton runat="server" ID="lbAddCountry" Text="Add" CssClass="btn btn-outline btn-warning" OnClick="lbAddCountry_Click"></asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>                    
                </table> 
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/system/system_currency_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <asp:LinkButton ID="lbSaveContinue" runat="server" ValidationGroup="vgEditItem" Text="Update" OnClick="lbSaveContinue_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>            
                </div>
            </div>
        </div>
    </div>
</asp:Content>
