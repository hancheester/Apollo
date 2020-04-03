<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.review_info" Codebehind="review_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">    
    <h2><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h2>
    <Apollo:NoticeBox ID="enbInfo" runat="server"/>
    <asp:ValidationSummary ID="vsReviewSum" runat="server" DisplayMode="BulletList" ValidationGroup="vgReview" CssClass="alert alert-warning" />
    <div class="col-lg-6">
        <table class="table table-striped">        
            <tr>
                <th>Product</th>
                <td><asp:Literal ID="ltlProduct" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <th>Posted by</th>
                <td><asp:Literal ID="ltlPostedBy" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <th>Score</th>
                <td><asp:DropDownList ID="ddlScore" runat="server" CssClass="form-control">                    
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th>Status</th>
                <td><asp:CheckBox ID="cbStatus" runat="server" CssClass="form-control" /></td>
            </tr>
            <tr>
                <th>Alias</th>
                <td><asp:TextBox ID="txtAlias" runat="server" CssClass="form-control"></asp:TextBox></td>
            </tr>
            <tr>
                <th>Title</th>
                <td><asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox></td>
            </tr>
            <tr>
                <th>Comment</th>
                <td><asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="8"></asp:TextBox></td>
            </tr>
        </table>
        <div class="hr-line-dashed"></div>
        <div class="col-lg-12">
            <a href="/catalog/reviews_all.aspx" class="btn btn-sm btn-default">Back</a>
            <asp:LinkButton ID="lbSave" runat="server" Text="Save" OnClick="lbSave_Click" CssClass="btn btn-sm btn-primary"></asp:LinkButton>        
            <asp:LinkButton ID="lbDelete" runat="server" Text="Delete" OnClick="lbDelete_Click" OnClientClick="return confirm('Are you sure to delete this review?');" CssClass="btn btn-sm btn-warning"></asp:LinkButton>        
            <asp:LinkButton ID="lbReset" runat="server" Text="Reset" OnClick="lbReset_Click" CssClass="btn btn-sm btn-danger"></asp:LinkButton>            
        </div>
    </div>
</asp:Content>