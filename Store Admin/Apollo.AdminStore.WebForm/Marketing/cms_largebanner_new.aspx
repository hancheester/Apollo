<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_largebanner_new" Codebehind="cms_largebanner_new.aspx.cs" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet" />    
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">    
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
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
            <h2>New Large Banner</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgNewBanner" CssClass="valSummary alert alert-warning" />
                <table class="table table-striped">        
                    <tr>
                        <th>Title<strong>*</strong></th>
                        <td><asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgNewBanner" runat="server"
                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Title is required."
                                ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
                        </td>
                    </tr> 
                    <tr>
                        <th>Link<strong>*</strong></th>
                        <td><asp:TextBox ID="txtLink" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgNewBanner" runat="server"
                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Link is required."
                                ControlToValidate="txtLink"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Media file <small class="text-navy clearfix">(size should be 790x220)</small></th>
                        <td>                
                            <asp:FileUpload ID="fuMedia" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <th>From date <small class="text-navy clearfix">(12:00am)</small></th>
                        <td><asp:TextBox ID="txtDateFrom" runat="server" CssClass="date form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>To date <small class="text-navy clearfix">(12:00am)</small></th>
                        <td><asp:TextBox ID="txtDateTo" runat="server" CssClass="date form-control"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Enabled</th>
                        <td><asp:CheckBox ID="cbEnabled" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>Priority <small class="text-navy clearfix">(integer only)</small></th>
                        <td><asp:TextBox ID="txtPriority" runat="server" CssClass="form-control" Text="0"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgNewBanner" runat="server"
                                Text="<i class='fa fa-exclamation-circle'></i Required." Display="Dynamic" ErrorMessage="Priority is required."
                                ControlToValidate="txtPriority"></asp:RequiredFieldValidator>
                            <asp:RangeValidator Type="Integer" ControlToValidate="txtPriority" ValidationGroup="vgNewBanner" runat="server" MaximumValue="99999999" MinimumValue="-99999999"
                                Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Invalid.</span>" Display="Dynamic" ErrorMessage="Priority is invalid."></asp:RangeValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Display on home page</th>
                        <td><asp:CheckBox runat="server" ID="cbDisplayOnHomepage" /></td>   
                    </tr>
                    <tr>
                        <th>Display on special offers page</th>
                        <td><asp:CheckBox runat="server" ID="cbDisplayOnOffersPage" /></td>
                    </tr>
                    <tr>
                        <th>Category <small class="text-navy clearfix">(only for first level category)</small></th>
                        <td><small class="text-navy clearfix">(please save to continue)</small></td>
                        
                    </tr>
                </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/marketing/cms_largebanner_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <asp:LinkButton ID="lbSave" runat="server" ValidationGroup="vgNewBanner" Text="Save" 
                        OnClientClick="Page_ClientValidate(); if (Page_IsValid) return confirm('Are you sure to save the banner?');" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>                    
                </div>
            </div>
        </div>
    </div>
</asp:Content>