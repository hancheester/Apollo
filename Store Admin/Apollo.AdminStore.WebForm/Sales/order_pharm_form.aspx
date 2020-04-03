<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_pharm_form" Codebehind="order_pharm_form.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2 class="hidden-print">Order</h2>
            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        </div>
        <Apollo:OrderPrevNext runat="server"></Apollo:OrderPrevNext>
    </div>

    <Apollo:OrderNav runat="server" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:OrderSideMenu runat="server" Type="PharmForm"></Apollo:OrderSideMenu>
                         <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body pharm-form">
                                    <asp:Literal ID="ltlNotFound" runat="server" Text="<p>Form not found.</p>"></asp:Literal>
                                    <asp:PlaceHolder ID="phPharmOrder" runat="server" Visible="false">
                                        <div class="col-lg-5">
                                            <h4>Client Details</h4>
                                            <table class="table table-striped">
                                                <tr>
                                                    <th>Name</th>
                                                    <td><asp:Literal runat="server" ID="ltName"></asp:Literal></td>
                                                </tr>
                                                <tr>
                                                    <th>Email</th>
                                                    <td><asp:Literal runat="server" ID="ltEmail"></asp:Literal></td>
                                                </tr>
                                                <tr>
                                                    <th>Contact number</th>
                                                    <td><asp:Literal runat="server" ID="ltContact"></asp:Literal></td>
                                                </tr>
                                            </table>
                                        </div>
                                        
                                        <div class="col-lg-8">
                                            <h4>Pharmaceutical Form</h4>                                            
                                            <table class="table table-striped">                                                    
                                                <tr>
                                                    <td>Will all the medication in your basket be taken by you?</td>
                                                </tr>
                                                <tr>
                                                    <td><b><asp:Literal runat="server" ID="ltTakenOwner"></asp:Literal></b></td>
                                                </tr>                                                    
                                                <asp:PlaceHolder ID="phAllergy" runat="server">
                                                    <tr>
                                                        <td>Please provide information on any known allergies.</td>
                                                    </tr>
                                                    <tr>
                                                        <td><b><asp:Literal ID="ltlAllergy" runat="server"></asp:Literal></b></td>
                                                    </tr>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="phOwnerAge" runat="server">
                                                    <tr>
                                                        <td>How old are you? (in years)</td>
                                                    </tr>
                                                    <tr>
                                                        <td><b><asp:Literal ID="ltlOwnerAge" runat="server"></asp:Literal></b></td>
                                                    </tr>
                                                </asp:PlaceHolder>                                                    
                                                <tr>
                                                    <td>Do you have any existing conditions or are you taking other medication?</td>
                                                </tr>
                                                <tr>
                                                    <td><b><asp:Literal runat="server" ID="ltHasOtherMed"></asp:Literal></b></td>
                                                </tr>                                                    
                                                <asp:PlaceHolder ID="phOwnerOtherCond" runat="server">
                                                    <tr>
                                                        <td>Please provide information about your conditions or other medication.</td>
                                                    </tr>
                                                    <tr>
                                                        <td><b><asp:Literal ID="ltlOwnerOtherCond" runat="server"></asp:Literal></b></td>
                                                    </tr>
                                                </asp:PlaceHolder>
                                            </table>
                                        </div>

                                        <div class="col-lg-12">
                                            <h4>Pharmaceutical Items</h4>
                                            <asp:Repeater ID="rptPharmItem" runat="server">
                                                <HeaderTemplate>
                                                    <table class="table">
                                                        <tr>
                                                            <th>Quantity</th>
                                                            <th>Name</th>
                                                            <th>Questions</th>
                                                        </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>  
                                                    <tr>
                                                        <td><%# Eval("Quantity") %></td>
                                                        <td><%# Eval("Name") %> <%# Eval("Option") %></td>
                                                        <td>
                                                            <table class="table">
                                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("Symptoms") != null && !string.IsNullOrEmpty(Eval("Symptoms").ToString()) %>'>
                                                                    <tr>
                                                                        <td>What symptoms are going to be treated with this medication?</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td><b><%# Eval("Symptoms") %></b></td>
                                                                    </tr>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("MedForSymptom") != null && !string.IsNullOrEmpty(Eval("MedForSymptom").ToString()) %>'>
                                                                    <tr>
                                                                        <td>What other medicines has the intended user tried for the symptom?</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td><b><%# Eval("MedForSymptom") %></b></td>
                                                                    </tr>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("Age") != null && !string.IsNullOrEmpty(Eval("Age").ToString()) %>'>
                                                                    <tr>
                                                                        <td>What is the age of the person who will be using this product? (in years)</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td><b><%# Eval("Age") %></b></td>
                                                                    </tr>
                                                                </asp:PlaceHolder>
                                                                <tr>
                                                                    <td>Does the intended user have any existing conditions or taking other medication?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><b><%# Convert.ToBoolean(Eval("HasOtherCondMed")) ? "Yes" : "No" %></b></td>
                                                                </tr>
                                                                <asp:PlaceHolder runat="server" Visible='<%# Convert.ToBoolean(Eval("HasOtherCondMed")) %>'>
                                                                    <tr>
                                                                        <td>Please provide information about conditions or other medication.</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td><b><%# Eval("OtherCondMed") %></b></td>
                                                                    </tr>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("PersistedInDays") != null && !string.IsNullOrEmpty(Eval("PersistedInDays").ToString()) %>'>
                                                                    <tr>
                                                                        <td>How long have the symptoms persisted for?</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td><b><%# Eval("PersistedInDays") %> days</b></td>
                                                                    </tr>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("ActionTaken") != null && !string.IsNullOrEmpty(Eval("ActionTaken").ToString()) %>'>
                                                                    <tr>
                                                                        <td>What action has been taken to treat this condition?</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td><b><%# Eval("ActionTaken") %></b></td>
                                                                    </tr>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("HasTaken") != null && !string.IsNullOrEmpty(Eval("HasTaken").ToString()) %>'>
                                                                    <tr><td>Has the intended user taken this medication before?</td></tr>
                                                                    <tr><td><b><%# Convert.ToBoolean(Eval("HasTaken")) ? "Yes" : "No" %></b></td></tr>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("TakenQuantity") != null && !string.IsNullOrEmpty(Eval("TakenQuantity").ToString()) %>'>
                                                                    <tr><td>On how many different occasions has the intended user taken this medication?</td></tr>
                                                                    <tr><td><b><%# Eval("TakenQuantity") %></b></td></tr>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder runat="server" Visible='<%# Eval("LastTimeTaken") != null && !string.IsNullOrEmpty(Eval("LastTimeTaken").ToString()) %>'>
                                                                    <tr><td>When was the last time the intended user took this medication?</td></tr>
                                                                    <tr><td><b><%# Eval("LastTimeTaken") %></b></td></tr>
                                                                </asp:PlaceHolder>
                                                            </table>                            
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>

                                        <asp:PlaceHolder ID="phItemsFromThisClient" runat="server">
                                        <div class="col-lg-12">
                                            <h4>Pharmaceutical Lines Ordered From The Last 60 Days By This Client</h4>
                                            <div class="col-lg-6">
                                                <asp:Repeater ID="rptExistingItems" runat="server">
                                                    <HeaderTemplate>
                                                        <table class="table table-striped">
                                                            <tr>
                                                                <th>Date</th>
                                                                <th>Order ID</th>
                                                                <th>Quantity</th>
                                                                <th>Name</th>
                                                            </tr>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>  
                                                        <tr>
                                                            <td><%# Eval("CreatedOnUtc") %></td>
                                                            <td><%# Eval("OrderId") %></td>
                                                            <td><%# Eval("Quantity") %></td>
                                                            <td><%# Eval("Name") %> <%# Eval("Option") %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                        </asp:PlaceHolder>

                                        <asp:PlaceHolder ID="phItemsFromThisPostCode" runat="server">
                                        <div class="col-lg-12">
                                            <h4>Pharmaceutical Lines Ordered From The Last 60 Days From This Address <small class="text-navy">(address line 1 &amp; postcode)</small></h4>
                                            <div class="col-lg-6">
                                                <asp:Repeater ID="rptExistingItemsFromThisPostcode" runat="server">
                                                    <HeaderTemplate>
                                                        <table class="table table-striped">
                                                            <tr>
                                                                <th>Date</th>
                                                                <th>Order ID</th>
                                                                <th>Quantity</th>
                                                                <th>Name</th>
                                                            </tr>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>  
                                                        <tr>
                                                            <td><%# Eval("CreatedOnUtc") %></td>
                                                            <td><%# Eval("OrderId") %></td>
                                                            <td><%# Eval("Quantity") %></td>
                                                            <td><%# Eval("Name") %> <%# Eval("Option") %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>
                                        </asp:PlaceHolder>

                                        <div class="col-lg-12 hidden-print">
                                            <a href="/sales/order_pharm_form_edit.aspx?orderid=<%= QueryOrderId %>" class="btn btn-primary">Edit</a>                                            
                                        </div>

                                    </asp:PlaceHolder>                                    
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>