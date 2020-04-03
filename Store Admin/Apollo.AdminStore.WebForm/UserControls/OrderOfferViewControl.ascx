<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderOfferViewControl" Codebehind="OrderOfferViewControl.ascx.cs" %>
<div class="panel panel-default">
    <div class="panel-heading">
        Offer information
        <span class="pull-right label label-plain">
            <a href="javascript:;" class="offerEdit" onclick="$('.offerField').slideToggle(); $('.offerEditField').slideToggle(); $('.offerEdit').text($('.offerEdit').text() == 'Edit' ? 'cancel' : 'edit');">edit</a>
        </span>
    </div>
    <table class="table">
        <tr>
            <th>Name</th>
            <td>
                <asp:Literal ID="ltlName" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <th>Discount</th>
            <td>
                <asp:HiddenField ID="hfDiscount" runat="server" />
                <div class="offerField"><asp:Literal ID="ltlDiscount" runat="server"></asp:Literal></div>
                <div class="offerEditField" style="display:none;">
                    <div class="input-group">
                        <span class="input-group-addon"><asp:Literal ID="ltlCurrencyCode" runat="server"></asp:Literal></span>
                        <asp:TextBox ID="txtDiscount" CssClass="form-control" runat="server"></asp:TextBox>
                    </div>
                </div>
            </td>
          </tr>
          <tr>
              <td colspan="2">
                <div class="offerEditField pull-right" style="display:none;">
                    <asp:LinkButton ID="lbSave" runat="server" CssClass="btn btn-outline btn-warning" Text="Save" OnClick="lbSave_Click"></asp:LinkButton>&nbsp;<a class="btn btn-outline btn-info" href="javascript:;" onclick="$('.offerField').slideToggle(); $('.offerEditField').slideToggle(); $('.offerEdit').text($('.offerEdit').text() == 'Edit' ? 'Cancel' : 'Edit');">Cancel</a>
                </div>
            </td>
        </tr>
    </table>
</div>