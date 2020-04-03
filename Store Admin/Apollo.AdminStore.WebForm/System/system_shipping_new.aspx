<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.System.system_shipping_new" Codebehind="system_shipping_new.aspx.cs" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Shipping Option</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-4">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgNewItem" CssClass="alert alert-warning" />
                <table class="table table-striped">
                    <tr>
                        <th>Name<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfName" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Name is required." ControlToValidate="txtName"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Description<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfDescription" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Description is required." ControlToValidate="txtDescription"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Value<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtValue" CssClass="form-control" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfValue" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Value is required." ControlToValidate="txtValue"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Free threshold<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtFreeThreshold" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfFreeThreshold" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Free Threshold is required." ControlToValidate="txtFreeThreshold"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Single item value<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtSingleItemValue" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfSingleItemValue" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Single Item Value is required." ControlToValidate="txtSingleItemValue"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Up To 1 KG<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtUptoOneKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoOneKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto One KG value is required." ControlToValidate="txtUptoOneKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>
                           Up To 1 1/2 KG<strong>*</strong>
                        </th>
                        <td>
                            <asp:TextBox ID="txtUptoOneAndHalfKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoOneAndHalfKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto One and a half KG value is required." ControlToValidate="txtUptoOneAndHalfKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Up to 2 KG<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtUptoTwoKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoTwoKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto Two KG value is required." ControlToValidate="txtUptoTwoKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Up To 2 1/2 KG<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtUptoTwoAndHalfKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoTwoAndHalfKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto Two and a half KG value is required." ControlToValidate="txtUptoTwoAndHalfKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Up To 3 KG<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtUptoThreeKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoThreeKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto Three KG value is required." ControlToValidate="txtUptoThreeKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Up To 3 1/2 KG<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtUptoThreeAndHalfKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoThreeAndHalfKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto Three and a half KG value is required." ControlToValidate="txtUptoThreeAndHalfKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Up to 4 KG<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtUptoFourKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoFourKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto Four KG value is required." ControlToValidate="txtUptoFourKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Up To 4 1/2 KG<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtUptoFourAndHalfKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoFourAndHalfKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto Four and a half KG value is required." ControlToValidate="txtUptoFourAndHalfKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Up to 5 KG<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtUptoFiveKG" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfUptoFiveKG" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Upto Five KG value is required." ControlToValidate="txtUptoFiveKG"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Half KG rate<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtHalfKGRate" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfHalfKGRate" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Half KG Rate value is required." ControlToValidate="txtHalfKGRate"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                     <tr>
                        <th>Enabled</th>
                        <td><asp:CheckBox ID="cbEnabled" runat="server"/></td>
                    </tr>
                     <tr>
                        <th>Priority<strong>*</strong></th>
                        <td>
                            <asp:TextBox ID="txtPriority" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfPriority" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Priority value is required." ControlToValidate="txtPriority"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Timeline</th>
                        <td><asp:TextBox ID="txtTimeline" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Country</th>
                        <td>
                            <asp:DropDownList ID="ddlCountry" runat="server" CssClass="form-control" OnInit="ddlCountry_Init" DataTextField="Name" DataValueField="CountryId"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfCountry" ValidationGroup="vgEditItem" runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Country value is required." ControlToValidate="ddlCountry"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>  
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/system/system_shipping_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewItem" Text="Create" OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to save the Shipping Option?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>                                
                </div>
            </div>
        </div>
    </div>
</asp:Content>