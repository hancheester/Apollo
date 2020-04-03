<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderLoyaltyViewControl" Codebehind="OrderLoyaltyViewControl.ascx.cs" %>
<div class="panel panel-default">
    <div class="panel-heading">
        Loyalty information
        <span class="pull-right label label-plain">
            <a href="javascript:;" class="loyalEdit" onclick="$('.loyalField').slideToggle(); $('.loyalEditField').slideToggle(); $('.loyalEdit').text($('.loyalEdit').text() == 'Edit' ? 'cancel' : 'edit');">edit</a>
        </span>
    </div>
    <table class="table">
        <tr>
            <th>Allocated Points</th>
            <td>
                <asp:HiddenField ID="hfAllocatedPoint" runat="server" />
                <div class="loyalField"><asp:Literal ID="ltlLoyalty" runat="server"></asp:Literal></div>
                <div class="loyalEditField" style="display:none;">
                    <asp:TextBox ID="txtLoyalty" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
            </td>
        </tr>
        <tr>
            <th>Earned Points</th>
            <td>
                <div class="loyalField"><asp:Literal ID="lblEarned" runat="server"></asp:Literal></div>
                <div class="loyalEditField" style="display:none;">
                    <asp:TextBox ID="txtEarned" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="loyalEditField pull-right" style="display:none;">
                    <asp:LinkButton ID="lbSave" runat="server" Text="Save" CssClass="btn btn-outline btn-warning" OnClick="lbSave_Click"></asp:LinkButton>&nbsp;<a href="javascript:;" class="btn btn-outline btn-info" onclick="$('.loyalField').slideToggle(); $('.loyalEditField').slideToggle(); $('.loyalEdit').text($('.loyalEdit').text() == 'Edit' ? 'Cancel' : 'Edit');">Cancel</a>
                </div>
            </td>
        </tr>
    </table>
</div>