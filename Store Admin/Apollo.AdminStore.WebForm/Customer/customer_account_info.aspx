<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Customer.customer_account_info" Codebehind="customer_account_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerNav" Src="~/UserControls/CustomerNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerTopRightNav" Src="~/UserControls/CustomerTopRightNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
	<script type="text/javascript">
	    $(function() {	        
	        $('#<%= cbSendAutoPwd.ClientID %>').click(function() {
	            if($('#<%= cbSendAutoPwd.ClientID %>').is(':checked')) {
	                $('.newPwd').val('');
	                $('.newPwd').attr('disabled', 'true');
	                $('#<%= cbNotificationEmail.ClientID %>').attr('checked', 'checked');
	            }
	            else {
	                $('.newPwd').removeAttr('disabled');
	            }
	        });

	        $('.date').datepicker({
	            format: 'dd/mm/yyyy',
	            keyboardNavigation: false,
	            forceParse: false,
	            autoclose: true,
	            todayHighlight: true
	        });
	    });
	</script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Account</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>

    <Apollo:CustomerTopRightNav runat="server" OnActionOccurred="ectTogRightNav_ActionOccurred" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" CssClass="alert alert-danger" ValidationGroup="vgProfile" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:CustomerNav runat="server" DisabledItem="AccountInfo"/>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <div class="panel panel-danger">
                                            <div class="panel-heading">
                                                Account information
                                            </div>
                                            <table class="table">
                                                <tr>
                                                    <th>Name</th>
                                                    <td><asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>Email / Username<strong>*</strong></th>
                                                    <td><asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ValidationGroup="vgProfile" runat="server" 
                                                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is a required field.</span>" ErrorMessage="Email / username is required." 
                                                            ControlToValidate="txtUsername" Display="Dynamic" />
                                                        <asp:RegularExpressionValidator ValidationGroup="vgProfile" runat="server"
                                                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> This is invalid.</span>" Display="Dynamic" ErrorMessage="Email / username is invalid." 
                                                            ControlToValidate="txtUsername" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
                                                        <br />
                                                        <asp:CheckBox ID="cbNewUsername" runat="server" Text="Send email to customer" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>User group<strong>*</strong></th>
                                                    <td><asp:CheckBoxList ID="cblRoles" runat="server"></asp:CheckBoxList></td>
                                                </tr>
                                                <tr>
                                                    <th>Contact number</th>
                                                    <td><asp:TextBox ID="txtPhone" runat="server" CssClass="form-control"></asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <th>To display contact number on delivery label</th>
                                                    <td><asp:CheckBox ID="cbDisplayContactNumber" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <th>Date of birth</th>
                                                    <td><asp:TextBox ID="txtDOB" CssClass="date form-control" runat="server"></asp:TextBox></td>
                                                </tr>
                                                </table>
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="phCredentials" runat="server">
                                    <div class="col-lg-6">
                                        <div class="panel panel-info">
                                            <div class="panel-heading">
                                                Credentials management
                                            </div>
                                            <table class="table">
                                                <asp:Repeater ID="rptCredentials" runat="server" OnItemCommand="rptCredentials_ItemCommand">
                                                    <ItemTemplate>
                                                        <tr>
                                                            <th><%# Eval("Key") %></th>
                                                            <td><asp:LinkButton runat="server" CommandArgument='<%# Eval("Key") + "|" + Eval("Value") %>' CommandName="remove" Text="Remove"></asp:LinkButton></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                             </table>
                                        </div>
                                    </div>
                                    </asp:PlaceHolder>
                                    <div class="col-lg-6">
                                        <div class="panel panel-primary">
                                            <div class="panel-heading">
                                                Loyalty management
                                            </div>
                                            <table class="table">
                                                <tr>
                                                    <th>Loyalty points</th>
                                                    <td>
                                                        <asp:TextBox ID="txtLoyaltyPoint" runat="server" CssClass="form-control"></asp:TextBox>
                                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLoyaltyPoint" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Loyalty point is required.</span>" ErrorMessage="Loyalty point is required." ValidationGroup="vgProfile" Display="Dynamic"></asp:RequiredFieldValidator>                                                        
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="panel panel-info">
                                            <div class="panel-heading">
                                                Note
                                            </div>
                                            <table class="table">
                                                <tr>
                                                    <th>Comment</th>
                                                    <td><asp:TextBox ID="txtNote" TextMode="MultiLine" Height="100px" CssClass="form-control" runat="server"></asp:TextBox></td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="panel panel-warning">
                                            <div class="panel-heading">
                                                Password management
                                            </div>
                                            <table class="table">
                                                <asp:PlaceHolder ID="phHasNoPassword" runat="server">
                                                    <tr>
                                                        <td colspan="2">
                                                            <p>The user doesn't have a password yet.</p>
                                                        </td>
                                                    </tr>
                                                </asp:PlaceHolder>                                                
                                                <tr>
                                                    <th>New password</th>
                                                    <td>
                                                        <asp:TextBox ID="txtPwd" CssClass="newPwd form-control" runat="server"></asp:TextBox><br />                    
                                                        <p class="hint">Minimum 8 characters.</p>
                                                        <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtPwd" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Minimum 8 characters required.</span>" ValidationExpression="^[\s\S]{8,}$" runat="server" ValidationGroup="vgPassword" ErrorMessage="Minimum 8 characters password is required."></asp:RegularExpressionValidator>
                                                        or<br />
                                                        <asp:CheckBox ID="cbSendAutoPwd" Text="Auto-generated password" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th>Notification email</th>
                                                    <td><asp:CheckBox ID="cbNotificationEmail" runat="server" Text="Send email to customer" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <div class="pull-right">
                                                            <asp:LinkButton ID="lbUpdatePassword" ValidationGroup="vgPassword" OnClick="lbUpdatePassword_Click"
                                                                runat="server" Text="Update password" CssClass="btn btn-outline btn-primary"></asp:LinkButton>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>                                            
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <div class="pull-right">
                                            <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgProfile" Text="Update" OnClientClick="if(Page_ClientValidate('vgProfile')) return confirm('Are you sure to save this account?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
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