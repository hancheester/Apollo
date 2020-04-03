<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="cms_blog_post_new.aspx.cs" ValidateRequest="false" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_blog_post_new" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet" />    
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script src="/js/offer.js" type="text/javascript"></script>
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
            <h2>Blog Post</h2>
            <h3>New Blog Post</h3>
        </div>
    </div>

    <div class="row wrapper">
        <div class="col-lg-12">
            <div class="row"><p></p></div>
            <div class="row">
                <div class="pull-right">
                    <asp:LinkButton ID="lbBack" runat="server" Text="Back" OnClick="lbBack_Click" CssClass="btn btn-sm btn-default"></asp:LinkButton>
                    <asp:LinkButton ID="lbSave" runat="server" Text="Save" ValidationGroup="vgNew" OnClick="lbSave_Click" CssClass="btn btn-sm btn-info"></asp:LinkButton>
                    <asp:LinkButton ID="lbReset" runat="server" Text="Reset" OnClick="lbReset_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>                    
                </div>
            </div> 
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgNew" CssClass="alert alert-warning" />
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#info" data-toggle="tab">General information</a></li>
                            <li><a href="#seo" data-toggle="tab">SEO</a></li>
                        </ul>
                        <div id="blog" class="tab-content">
                            <div id="info" class="tab-pane active">
                                <div class="panel-body">
                                    <table class="table" style="margin-bottom: 0;">
                                        <tr>
                                            <th>Title<strong>*</strong></th>
                                            <td>
                                                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle" ValidationGroup="vgNew" ErrorMessage="Title is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Title is required.</span>"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Body<strong>*</strong></th>
                                            <td>
                                                <asp:TextBox ID="txtBody" runat="server" TextMode="MultiLine" Height="200px" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBody" ValidationGroup="vgNew" ErrorMessage="Body is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Body is required.</span>"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Body overview<strong>*</strong></th>
                                            <td>
                                                <asp:TextBox ID="txtBodyOverview" runat="server" TextMode="MultiLine" Height="150px" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtBodyOverview" ValidationGroup="vgNew" ErrorMessage="Body overview is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Body overview is required.</span>"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Tags<strong>*</strong></th>
                                            <td>
                                                <asp:TextBox ID="txtTags" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTags" ValidationGroup="vgNew" ErrorMessage="Tags is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Tags is required.</span>"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Allow comments</th>
                                            <td>
                                                <asp:CheckBox ID="cbAllowComments" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Start date <small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox CssClass="date form-control" ID="txtDateFrom" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>End date <small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox CssClass="date form-control" ID="txtDateTo" runat="server"></asp:TextBox></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div id="seo" class="tab-pane">
                                <div class="panel-body">
                                    <table class="table" style="margin-bottom: 0;">
                                        <tr>
                                            <th>Meta keywords</th>
                                            <td>
                                                <asp:TextBox ID="txtMetaKeywords" runat="server" CssClass="form-control"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Meta description</th>
                                            <td>
                                                <asp:TextBox ID="txtMetaDescription" TextMode="MultiLine" Height="150px" runat="server" CssClass="form-control"></asp:TextBox>                                                
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Meta title</th>
                                            <td>
                                                <asp:TextBox ID="txtMetaTitle" runat="server" CssClass="form-control"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Search engine friendly page name</th>
                                            <td>
                                                <asp:TextBox ID="txtUrlKey" runat="server" CssClass="form-control"></asp:TextBox>
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
</asp:Content>