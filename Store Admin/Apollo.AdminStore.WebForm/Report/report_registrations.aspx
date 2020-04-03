<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Report.report_registrations" Codebehind="report_registrations.aspx.cs" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
	<script type="text/javascript">
	    $(document).ready(function () {
	        $('.date').datepicker({
	            format: 'dd/mm/yyyy',	            
	            duration: '',
	            changeMonth: true,
	            changeYear: true
	        });
	    });
	</script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" Runat="Server">
    <h2>Registration Report</h2>    
    <div class="reportFilter">
        <asp:Label runat="server" ID="lblDateFrom" Text="From:" AssociatedControlID="txtDateFrom" />
        <asp:TextBox ID="txtDateFrom" runat="server" CssClass="date" />
        
        <asp:Label runat="server" ID="lblDateTo" Text="To:" AssociatedControlID="txtDateTo" />
        <asp:TextBox ID="txtDateTo" runat="server" CssClass="date" />
        
        <asp:Label runat="server" ID="lblShowBy" Text="Show By:" AssociatedControlID="ddlShowBy" />
        <asp:DropDownList ID="ddlShowBy" runat="server" Enabled="false">
            <asp:ListItem>Day</asp:ListItem>
            <asp:ListItem>Week</asp:ListItem>
            <asp:ListItem>Month</asp:ListItem>
        </asp:DropDownList>
        
        <asp:LinkButton ID="btnRefresh" runat="server" Text="Refresh" CssClass="ABtn" OnClick="btnRefresh_Click" />
    </div>
    
    <div style="width: 20%; float: left; margin-right: 1%;">
    <table class="grid" width="100%">
        <tr>
            <th style="background: #CFE4F4;color:#003366;">Register-Order:</th>
            <td style="background: #CFE4F4;font-weight: bold;color:#003366;"><asp:Literal ID="ltlRegisterOrder" runat="server" /></td>
        </tr>
        <tr>
            <th>Total Registered:</th>
            <td><asp:Literal ID="ltlTotalRegistered" runat="server" /></td>
        </tr>
        <tr>
            <th>Total Orders:</th>
            <td><asp:Literal ID="ltlTotalOrders" runat="server" /></td>
        </tr>       
    </table>
    </div>
    
    <div style="width: 79%; float: left;">
    <asp:GridView ID="gvOrdersByDay" runat="server" CssClass="customGrid" AutoGenerateColumns="false" HeaderStyle-Height="50" GridLines="None" CellPadding="0" CellSpacing="0">
        <Columns>
            <asp:TemplateField HeaderText="Day">
                <ItemTemplate>
	                <asp:Literal Text='<%# Convert.ToDateTime(Eval("TheDay")).DayOfWeek %>' Runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Date">
                <ItemTemplate>
	                <asp:Literal Text='<%# Convert.ToDateTime(Eval("TheDay")).ToLongDateString() %>' Runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TotalRegistered" HeaderText="Registrations" />
            <asp:BoundField DataField="TotalOrders" HeaderText="Orders" />
            <asp:TemplateField HeaderText="Register-Order %">
                <ItemTemplate>
	                 <b><asp:Literal Text='<%# Convert.ToInt32(((Convert.ToDouble(Eval("TotalOrders")) / Convert.ToDouble(Eval("TotalRegistered"))) * 100)).ToString() %>' Runat="server" />%</b>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <AlternatingRowStyle BackColor="#FAFAFA" />
    </asp:GridView>
    </div>
    
    
</asp:Content>