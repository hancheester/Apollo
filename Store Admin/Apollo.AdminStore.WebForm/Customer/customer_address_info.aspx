<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Customer.customer_address_info" Codebehind="customer_address_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerNav" Src="~/UserControls/CustomerNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerTopRightNav" Src="~/UserControls/CustomerTopRightNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">    
    <script type="text/javascript">
        function setUniqueRadioButton(current, group) {
            $(group + ' > input[type=radio]').attr('checked', false);
            $(current).attr('checked', true);
        }

        function chooseAddress(value, context) {
            $('.addressStatus').html('');
            $('#divType_' + context).html(value);
        }

        function showProcessing(id) {
            $(id).innerHTML = 'updating...';
        }   
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Account</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>

    <Apollo:CustomerTopRightNav ID="ectTogRightNav" runat="server" OnActionOccurred="ectTogRightNav_ActionOccurred" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" CssClass="valSummary alert alert-danger" ValidationGroup="vgNewAddress" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:CustomerNav runat="server" DisabledItem="Address"/>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <div class="col-lg-12">
                                        <asp:Repeater ID="rptAddress" runat="server" OnItemDataBound="rptAddress_ItemDataBound">
                                            <HeaderTemplate>
                                                <div class="panel panel-info">
                                                    <div class="panel-heading">Customer addresses</div>                                                        
                                                        <div class="addressPanel">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <div class="col-lg-3" style="height: 250px;">
                                                    <div class="ibox float-e-margins">
                                                        <div class="ibox-title">
                                                            <h5>Address (ID : <%# Eval("Id") %>)</h5>
                                                            <div class="ibox-tools">
                                                                <asp:LinkButton runat="server" ID="lbEdit" ToolTip="edit" CommandArgument='<%# Eval("Id") %>' OnClick="lbEdit_Click"><i class="fa fa-pencil-square"></i></asp:LinkButton>
                                                                <asp:LinkButton runat="server" ID="lbDelete" ToolTip="remove" CommandArgument='<%# Eval("Id") %>' OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure to delete this address?');"><i class="fa fa-trash"></i></asp:LinkButton>                                                        
                                                            </div>
                                                        </div>
                                                        <div class="ibox-content">
                                                            <asp:Literal ID="ltlAddress" runat="server"></asp:Literal>
                                                            <%# Convert.ToBoolean(Eval("IsBilling")) ? "<br/><span class='label label-danger'>Primary Billing</span>" : null %>
                                                            <%# Convert.ToBoolean(Eval("IsShipping")) ? "<br/><span class='label label-warning'>Primary Shipping</span>" : null %>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                    <div class="clearfix"></div>
                                                    </div>
                                                </div>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                <asp:Literal ID="ltlAddressTitle" runat="server"></asp:Literal>
                                            </div>
                                            <asp:HiddenField ID="hfAddressId" runat="server" />
                                            <table class="table">
                                                <tr>
                                                    <th>Name<strong>*</strong></th>
                                                    <td><asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ValidationGroup="vgNewAddress" runat="server"
                                                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Name is required."
                                                            ControlToValidate="txtName"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>Address line 1<strong>*</strong></th>
                                                    <td><asp:TextBox ID="txtAddr1" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ValidationGroup="vgNewAddress" runat="server"
                                                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Address line 1 is required."
                                                            ControlToValidate="txtAddr1"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>Address line 2</th>
                                                    <td><asp:TextBox ID="txtAddr2" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>City<strong>*</strong></th>
                                                    <td><asp:TextBox ID="txtCity" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ValidationGroup="vgNewAddress" runat="server"
                                                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="City is required."
                                                            ControlToValidate="txtCity"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>County</th>
                                                    <td><asp:TextBox ID="txtCounty" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <asp:PlaceHolder ID="phState" runat="server" Visible="false">
                                                <tr>
                                                    <th>States</th>
                                                    <td><asp:DropDownList ID="ddlState" OnInit="ddlState_Init" runat="server" DataTextField="State" DataValueField="Code" CssClass="form-control"></asp:DropDownList></td>
                                                </tr>
                                                </asp:PlaceHolder>
                                                <tr>
                                                    <th>Post code</th>
                                                    <td><asp:TextBox ID="txtPostCode" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Country<strong>*</strong></th>
                                                    <td><asp:DropDownList ID="ddlCountry" CssClass="form-control" runat="server" OnInit="ddlCountry_Init" OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" AutoPostBack="true" DataTextField="Name" DataValueField="CountryId"></asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <th>Is Billing address?<small class="text-navy clearfix">(if ticked, only this address is a billing address)</small></th>
                                                    <td><asp:CheckBox ID="cbIsBiling" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <th>Is Shipping address?<small class="text-navy clearfix">(if ticked, only this address is a shipping address)</small></th>
                                                    <td><asp:CheckBox ID="cbIsShipping" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <div class="pull-right">
                                                            <asp:LinkButton ID="lbSaveAddress" ValidationGroup="vgNewAddress" Visible="false"
                                                                OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to update the address?');" 
                                                                runat="server" Text="Update" OnClick="lbSaveAddress_Click" CssClass="btn btn-outline btn-warning"></asp:LinkButton>
                                                            <asp:LinkButton ID="lbCancel" Visible="false" runat="server" Text="Cancel" OnClick="lbCancel_Click" CssClass="btn btn-outline btn-default"></asp:LinkButton>
                                                            <asp:LinkButton ID="lbCreateAddress" ValidationGroup="vgNewAddress"
                                                                OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to create the address?');" 
                                                                runat="server" Text="Create" OnClick="lbCreateAddress_Click" CssClass="btn btn-outline btn-primary"></asp:LinkButton>                                                            
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>                                            
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>