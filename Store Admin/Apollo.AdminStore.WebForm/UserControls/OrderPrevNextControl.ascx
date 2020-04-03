<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderPrevNextControl.ascx.cs" Inherits="Apollo.AdminStore.WebForm.UserControls.OrderPrevNextControl" %>
<div class="col-lg-4 hidden-print">
    <div class="row"><p></p></div>
    <div class="pull-right">
        <span class="form-inline">
            Order ID: <asp:TextBox ID="txtGoOrderId" CssClass="form-control input-sm" runat="server"></asp:TextBox>
            <asp:LinkButton ID="lbGo" runat="server" Text="Go" OnClick="lbGo_Click" CssClass="btn btn-outline btn-sm btn-primary"></asp:LinkButton>
        </span>        
        <asp:HyperLink ID="hlPrev" runat="server" Text="Prev" CssClass="btn btn-outline btn-sm btn-danger"></asp:HyperLink>
        <asp:HyperLink ID="hlNext" runat="server" Text="Next" CssClass="btn btn-outline btn-sm btn-warning"></asp:HyperLink>
    </div>
</div>