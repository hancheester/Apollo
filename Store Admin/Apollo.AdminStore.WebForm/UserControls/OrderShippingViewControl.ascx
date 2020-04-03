<%@ Control Language="C#" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.UserControls.UserControls_OrderShippingViewControl" Codebehind="OrderShippingViewControl.ascx.cs" %>
<div class="panel panel-default">
    <div class="panel-heading">
        Shipping &amp; handling information
        <span class="pull-right label label-plain">
            <a href="javascript:;" class="delEdit" onclick="$('.delField').slideToggle(); $('.delEditField').slideToggle(); $('.delEdit').text($('.delEdit').text() == 'edit' ? 'cancel' : 'edit');">edit</a>
        </span>    
    </div>
    <table class="table">
        <tr>
            <th>Shipping Option</th>
            <td>
                <div class="delField"><asp:Literal ID="ltlShippingInfo" runat="server"></asp:Literal></div>
                <div class="delEditField" style="display:none;">
                    <asp:HiddenField ID="hfCurrentId" runat="server" />
                    <asp:DropDownList ID="ddlOptions" runat="server" CssClass="form-control" DataTextField="Description" DataValueField="Id"></asp:DropDownList>
                </div>
            </td>
        </tr>
        <tr>
            <th>Cost</th>
            <td>
                <asp:HiddenField ID="hfCost" runat="server" />
                <div class="delField"><asp:Literal ID="ltlCost" runat="server"></asp:Literal></div>
                <div class="delEditField" style="display:none;">
                    <div class="input-group">
                        <span class="input-group-addon"><asp:Literal ID="ltlCurrencyCode" runat="server"></asp:Literal></span>
                        <asp:TextBox ID="txtCost" CssClass="form-control" runat="server"></asp:TextBox>
                    </div>                     
                </div>
            </td>
        </tr>
        <tr>
            <th>VAT <asp:Literal ID="ltlVATMessage" runat="server"></asp:Literal></th>
            <td><asp:Literal ID="ltlTaxDiscount" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Weight</th>
            <td><asp:Literal ID="ltlWeight" runat="server"></asp:Literal></td>
        </tr>
        <tr>
            <th>Note</th>
            <td>
                <div class="delField"><asp:Literal ID="ltlPackingInfo" runat="server"></asp:Literal></div>
                <div class="delEditField" style="display:none;">
                    <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" CssClass="form-control" Height="100px" Width="100%"></asp:TextBox>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="delEditField pull-right" style="display:none;">
                    <asp:LinkButton ID="lbSave" runat="server" Text="Save" CssClass="btn btn-outline btn-warning" OnClick="lbSave_Click"></asp:LinkButton>&nbsp;<a href="javascript:;" class="btn btn-outline btn-info" onclick="$('.delField').slideToggle(); $('.delEditField').slideToggle(); $('.delEdit').text($('.delEdit').text() == 'Edit' ? 'Cancel' : 'Edit');">Cancel</a>
                </div>
            </td>
        </tr>
    </table>
</div>