<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_largebanner_info" Codebehind="cms_largebanner_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="CategoryTree" Src="~/UserControls/CategoryTreeControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
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
            <h2>Large Banner</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>        
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary ID="vsBannerSum" runat="server" CssClass="valSummary alert alert-warning" DisplayMode="BulletList" ValidationGroup="vgEditBanner" />                
                <table class="table table-striped">
                    <tr>
                        <th>Title<strong>*</strong></th>
                        <td><asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditBanner" runat="server"
                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Title is required."
                            ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Link<strong>*</strong></th>
                        <td><asp:TextBox ID="txtLink" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditBanner" runat="server"
                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Required.</span>" Display="Dynamic" ErrorMessage="Title is required."
                            ControlToValidate="txtTitle"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Media banner<small class="text-navy clearfix">(size should be 1170x550)</small></th>
                        <td><asp:Literal ID="ltlBanner" runat="server"></asp:Literal>
                        <asp:FileUpload ID="fuBanner" runat="server" CssClass="form-control" /></td>
                    </tr>
                    <tr>
                        <th>From date <small class="text-navy clearfix">(12:00am)</small></th>
                        <td><asp:TextBox ID="txtDateFrom" runat="server" CssClass="date form-control" Width="300"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>To date <small class="text-navy clearfix">(12:00am)</small></th>
                        <td><asp:TextBox ID="txtDateTo" runat="server" CssClass="date form-control" Width="300"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <th>Enabled</th>
                        <td><asp:CheckBox runat="server" ID="cbEnabled" /></td>   
                    </tr>
                    <tr>
                        <th>Priority</th>
                        <td><asp:TextBox ID="txtPriority" runat="server" CssClass="form-control"></asp:TextBox></td>
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
                        <td>
                            <asp:HiddenField ID="hfCategory" runat="server" />
                            <table class="table table-striped">
                                <tr>
                                    <th>Category selection</th>
                                    <td>
                                        <asp:DropDownList ID="ddlCategorySelection" runat="server" CssClass="form-control"></asp:DropDownList>
                                        <asp:LinkButton runat="server" CssClass="btn btn-outline btn-sm btn-warning" Text="Remove" ID="lbRemoveCategory" OnClick="lbRemoveCategory_Click"></asp:LinkButton>
                                    </td>
                                </tr>
                                <tr>
                                    <th>New category</th>
                                    <td>
                                        <asp:Literal ID="ltlCategory" runat="server"></asp:Literal>
                                        <asp:LinkButton ID="lbSearchNewCategory" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Search" OnClick="lbSearchNewCategory_Click"></asp:LinkButton>
                                        <asp:LinkButton ID="lbAddNewCategory" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Add" OnClick="lbAddNewCategory_Click" Visible="false"></asp:LinkButton>
                                        <asp:LinkButton ID="lbCancelCategory" CssClass="btn btn-outline btn-sm btn-primary" runat="server" Text="Cancel" OnClick="lbCancelCategory_Click" Visible="false"></asp:LinkButton>
                                        <Apollo:CategoryTree ID="ectCategory" runat="server" Visible="false" OnTreeChanged="ectCategory_TreeChanged" OnTreeNodeSelected="ectCategory_TreeNodeSelected" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/marketing/cms_largebanner_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <asp:LinkButton ID="lbSaveContinue" runat="server" Text="Update" ValidationGroup="vgEditBanner" OnClick="lbSaveContinue_Click" CssClass="btn btn-sm btn-info"></asp:LinkButton>
                    <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure to delete this banner?');" CssClass="btn btn-sm btn-success"></asp:LinkButton>                                
                </div>
                <div class="col-lg-12"><p></p></div>
            </div>            
        </div>
    </div>
</asp:Content>