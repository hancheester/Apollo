<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="order_pharm_form_edit.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Sales.order_pharm_form_edit" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">    
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet" />    
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">    
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.date').datepicker({
                format: 'dd/mm/yyyy',
                keyboardNavigation: false,
                forceParse: false,
                autoclose: true,
                todayHighlight: true
            });
        });
    </script>
</asp:Content>
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
                                            <h4>Edit Pharmaceutical Form</h4>                                            
                                            <table class="table table-striped">                                                    
                                                <tr>
                                                    <td>Will all the medication in your basket be taken by you?</td>
                                                </tr>
                                                <tr>
                                                    <td><asp:CheckBox ID="cbTakenOwner" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td>Please provide information on any known allergies.</td>
                                                </tr>
                                                <tr>
                                                    <td><asp:TextBox ID="txtAllergy" CssClass="form-control" runat="server"></asp:TextBox></td>
                                                </tr>                                                
                                                <tr>
                                                    <td>How old are you? (in years)</td>
                                                </tr>
                                                <tr>
                                                    <td><asp:TextBox ID="txtOwnderAge" CssClass="form-control" runat="server"></asp:TextBox></td>
                                                </tr>                                                
                                                <tr>
                                                    <td>Do you have any existing conditions or are you taking other medication?</td>
                                                </tr>
                                                <tr>
                                                    <td><asp:CheckBox ID="cbHasOtherMed" runat="server" /></td>
                                                </tr>                                                
                                                <tr>
                                                    <td>Please provide information about your conditions or other medication.</td>
                                                </tr>
                                                <tr>
                                                    <td><asp:TextBox ID="txtOwnerOtherCond" CssClass="form-control" runat="server"></asp:TextBox></td>                                                    
                                                </tr>                                                
                                            </table>
                                        </div>

                                        <div class="col-lg-12">
                                            <h4>Edit Pharmaceutical Items</h4>
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
                                                        <asp:HiddenField ID="hfPharmItemId" runat="server" Value='<%# Eval("Id") %>' />
                                                        <td><%# Eval("Quantity") %></td>
                                                        <td><%# Eval("Name") %> <%# Eval("Option") %></td>
                                                        <td>
                                                            <table class="table">                                                                
                                                                <tr>
                                                                    <td>What symptoms are going to be treated with this medication?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><asp:TextBox ID="txtSymptoms" CssClass="form-control" runat="server" Text='<%# Eval("Symptoms") %>'></asp:TextBox></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>What other medicines has the intended user tried for the symptom?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><asp:TextBox ID="txtMedForSymptom" CssClass="form-control" runat="server" Text='<%# Eval("MedForSymptom") %>'></asp:TextBox></td>
                                                                </tr>                                                                
                                                                <tr>
                                                                    <td>What is the age of the person who will be using this product? (in years)</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><asp:TextBox ID="txtAge" CssClass="form-control" runat="server" Text='<%# Eval("Age") %>'></asp:TextBox></td>
                                                                </tr>                                                                
                                                                <tr>
                                                                    <td>Does the intended user have any existing conditions or taking other medication?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><asp:CheckBox ID="cbHasOtherCondMed" runat="server" Checked='<%# Convert.ToBoolean(Eval("HasOtherCondMed")) %>'/></td>
                                                                </tr>                                                                
                                                                <tr>
                                                                    <td>Please provide information about conditions or other medication.</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><asp:TextBox ID="txtOtherCondMed" CssClass="form-control" runat="server" Text='<%# Eval("OtherCondMed") %>'></asp:TextBox></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>How long have the symptoms persisted for?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><asp:TextBox ID="txtPersistedInDays" CssClass="form-control" runat="server" Text='<%# Eval("PersistedInDays") %>'></asp:TextBox> days</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>What action has been taken to treat this condition?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td><asp:TextBox ID="txtActionTaken" CssClass="form-control" runat="server" Text='<%# Eval("ActionTaken") %>'></asp:TextBox></td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Has the intended user taken this medication before?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:CheckBox ID="cbHasTaken" runat="server" Checked='<%# Convert.ToBoolean(Eval("HasTaken")) %>'/>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>On how many different occasions has the intended user taken this medication?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtTakenQuantity" CssClass="form-control" runat="server" Text='<%# Eval("TakenQuantity") %>'></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>When was the last time the intended user took this medication?</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtLastTimeTaken" CssClass="form-control date" runat="server" Text='<%# Eval("LastTimeTaken") %>'></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>                            
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </div>

                                        <div class="col-lg-12">
                                            <asp:LinkButton ID="lbUpdate" runat="server" Text="Update" OnClick="lbUpdate_Click" CssClass="btn btn-primary"></asp:LinkButton>
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