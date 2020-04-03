<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NoticeBoxControl.ascx.cs" Inherits="Apollo.AdminStore.WebForm.UserControls.NoticeBoxControl" %>
<asp:PlaceHolder ID="phBox" runat="server" Visible="false">
    <div class="noticeBox alert alert-info alert-dismissable">
        <button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>
        <asp:Literal ID="ltlMsg" runat="server"></asp:Literal>
    </div>
</asp:PlaceHolder>