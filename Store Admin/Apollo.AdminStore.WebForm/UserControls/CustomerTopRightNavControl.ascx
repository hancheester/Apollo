<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_CustomerTopRightNavControl" Codebehind="CustomerTopRightNavControl.ascx.cs" %>
<div class="customer_buttons row wrapper hidden-print">
    <div class="col-lg-12">
        <div class="row"><p></p></div>
        <div class="row">
            <div class="pull-right">
                <asp:LinkButton ID="lbBack" OnClick="lbBack_Click" runat="server" Text="Back" CssClass="btn btn-sm btn-default"></asp:LinkButton>
                <a href="/sales/order_new.aspx?userid=<%= ((BasePage)this.Page).QueryUserId %>" class="btn btn-sm btn-warning">Create order</a>
                <asp:LinkButton ID="lbReset" runat="server" Text="Reset" CssClass="btn btn-sm btn-danger"></asp:LinkButton>
                <asp:LinkButton ID="lbDisapprove" OnClick="lbDisapprove_Click" runat="server" visible="false" text="Disapprove" CssClass="btn btn-sm btn-primary"></asp:LinkButton>
                <asp:LinkButton ID="lbApprove" OnClick="lbApprove_Click" runat="server" visible="false" text="Approve" CssClass="btn btn-sm btn-info"></asp:LinkButton>
                <asp:LinkButton ID="lbUnlock" OnClick="lbUnlock_Click" runat="server" visible="false" text="Unlock" CssClass="btn btn-sm btn-success"></asp:LinkButton>
                <asp:LinkButton ID="lbUnsubscribe" OnClick="lbUnsubscribe_Click" runat="server" Visible="false" Text="Unsubscribe" CssClass="btn btn-sm btn-info"></asp:LinkButton>
                <asp:LinkButton ID="lbSubsribe" OnClick="lbSubscribe_Click" runat="server" Visible="false" Text="Subscribe" CssClass="btn btn-sm btn-success"></asp:LinkButton>
            </div>
        </div>
    </div>
</div>