<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Customer.customer_new" Codebehind="customer_new.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerNav" Src="~/UserControls/CustomerNavControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
	<script type="text/javascript">
        $(function () {
            $('#<%= cbSendAutoPwd.ClientID %>').click(function () {
                if ($('#<%= cbSendAutoPwd.ClientID %>').is(':checked')) {
                    $('.newPwd').val('');
                    $('.newPwd').attr('disabled', 'true');
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
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" Runat="Server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Account</h2>            
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <asp:ValidationSummary runat="server" CssClass="valSummary alert alert-danger" DisplayMode="BulletList" ValidationGroup="vgNewCustomer" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:CustomerNav runat="server" DisabledItem="NewCustomer" />
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <table class="table table-striped">
                                            <tr>
                                                <th>Name<strong>*</strong></th>
                                                <td><asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ValidationGroup="vgNewCustomer" runat="server"
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="First name is required."
                                                        ControlToValidate="txtName"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr> 
                                            <tr>
                                                <th>User group</th>
                                                <td><asp:CheckBoxList ID="cblRoles" runat="server"></asp:CheckBoxList></td>
                                            </tr> 
                                            <tr>
                                                <th>Email / username<strong>*</strong></th>
                                                <td><asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ValidationGroup="vgNewCustomer" runat="server"
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Email / username is required."
                                                        ControlToValidate="txtUsername"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ValidationGroup="vgNewCustomer" runat="server" 
                                                        Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Invalid.</span>" Display="Dynamic" ErrorMessage="Email / username is invalid." 
                                                        ControlToValidate="txtUsername" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Contact number </th>
                                                <td><asp:TextBox ID="txtContact" runat="server" CssClass="form-control"></asp:TextBox></td>
                                            </tr>
                                            <tr>
                                                <th>To display contact number on delivery label</th>
                                                <td><asp:CheckBox ID="cbDisplayContactNumber" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>Date of birth <small class="text-navy clearfix">(dd/mm/yyyy)</small></th>
                                                <td><asp:TextBox ID="txtDOB" CssClass="date form-control" runat="server"></asp:TextBox>
                                                    <asp:RegularExpressionValidator ErrorMessage="DOB is invalid."
                                                        runat="server" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Invalid.</span>" 
                                                        ValidationGroup="vgNewCustomer" Display="Dynamic" ControlToValidate="txtDOB" 
                                                        ValidationExpression="(((0[1-9]|[12][0-9]|3[01])(/)(0[13578]|10|12)(/)(\d{4}))|(([0][1-9]|[12][0-9]|30)(/)(0[469]|11)(/)(\d{4}))|((0[1-9]|1[0-9]|2[0-8])(/)(02)(/)(\d{4}))|((29)(\/)(02)(/)([02468][048]00))|((29)(/)(02)(/)([13579][26]00))|((29)(/)(02)(/)([0-9][0-9][0][48]))|((29)(/)(02)(/)([0-9][0-9][2468][048]))|((29)(/)(02)(/)([0-9][0-9][13579][26])))" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Send welcome email</th>
                                                <td><asp:CheckBox ID="cbWelcomeEmail" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>Password<strong>*</strong></th>
                                                <td>
                                                    <asp:TextBox ID="txtPwd" CssClass="newPwd form-control" runat="server"></asp:TextBox><br />
                                                    <small class="text-navy clearfix">Minimum 8 characters.</small>
                                                    <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtPwd" Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Minimum 8 characters required.</span><br/>"
                                                            ValidationExpression="^[\s\S]{8,}$" runat="server" ValidationGroup="vgNewCustomer" ErrorMessage="Minimum 8 characters password is required."></asp:RegularExpressionValidator>
                                                    or<br />
                                                    <asp:CheckBox ID="cbSendAutoPwd" Text="Auto-generated password" CssClass="autoPwd" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                        <div class="hr-line-dashed"></div>
                                        <div class="col-lg-12">
                                            <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewCustomer" Text="Create" 
                                                OnClientClick="if (Page_ClientValidate('vgNewCustomer')) return confirm('Are you sure to save the customer?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
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