<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.cms_testimonials_info" Codebehind="cms_testimonials_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Testimonial</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-6">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary ID="vsTestimonialSum" runat="server" DisplayMode="BulletList" ValidationGroup="vgEditTestimonial" CssClass="alert alert-warning" />
                <table class="table table-striped">
                    <tr>
                        <th>Comment<strong>*</strong></th>
                        <td><asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Height="200px" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditTestimonial" runat="server"
                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Comment is required.</span>" Display="Dynamic" ErrorMessage="Comment is required."
                            ControlToValidate="txtComment"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Name of reviewer<strong>*</strong></th>
                        <td><asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ValidationGroup="vgEditTestimonial" runat="server"
                            Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Name is required.</span>" Display="Dynamic" ErrorMessage="Name is required."
                            ControlToValidate="txtName"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th>Priority</th>
                        <td><asp:TextBox ID="txtPriority" runat="server" CssClass="form-control"></asp:TextBox></td>
                    </tr>
                </table>   
                <div class="hr-line-dashed"></div>
                <div class="col-lg-12">
                    <a href="/marketing/cms_testimonials_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <asp:LinkButton ID="lbSaveContinue" runat="server" Text="Update" ValidationGroup="vgEditTestimonial" OnClick="lbSaveContinue_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                    <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure to delete this testimonial?');" CssClass="btn btn-sm btn-danger"></asp:LinkButton>        
                </div>
            </div>
        </div>
    </div>
</asp:Content>