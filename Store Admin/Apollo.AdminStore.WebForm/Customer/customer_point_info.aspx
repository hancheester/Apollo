<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="customer_point_info.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Customer.customer_point_info" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerNav" Src="~/UserControls/CustomerNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="CustomerTopRightNav" Src="~/UserControls/CustomerTopRightNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Account</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>

    <Apollo:CustomerTopRightNav ID="ectTogRightNav" runat="server" OnActionOccurred="ectTogRightNav_ActionOccurred" />

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:CustomerNav runat="server" DisabledItem="Point" />
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <Apollo:CustomGrid ID="gvPoints" runat="server" PageSize="10" AllowPaging="true" AllowSorting="false" OnPageIndexChanging="gvPoints_PageIndexChanging" 
                                            CssClass="table table-striped table-bordered table-hover dataTable" 
                                            AutoGenerateColumns="false" ShowHeader="true">                    
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>                
                                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                    Page 
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvPoints.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                    <asp:ImageButton Visible='<%# (gvPoints.CustomPageCount > (gvPoints.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                    of <%= gvPoints.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvPoints.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                                </asp:Panel>                        
                                            </PagerTemplate>
                                            <Columns>                    
                                                <asp:TemplateField HeaderText="Reward Point History ID">                
                                                    <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Message">                                                    
                                                    <ItemTemplate><%# Eval("Message")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Added Points">
                                                    <ItemTemplate><%# Eval("Points")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Used Points">
                                                    <ItemTemplate><%# Eval("UsedPoints")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Points Balance">
                                                    <ItemTemplate><%# Eval("PointsBalance")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Related Order ID">
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder runat="server" Visible='<%# Eval("UsedWithOrderId") != null %>'>
                                                            <a href='<%# "/sales/order_info.aspx?orderid=" + Eval("UsedWithOrderId") %>'><%# Eval("UsedWithOrderId") %></a>
                                                        </asp:PlaceHolder>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Date">
                                                    <ItemTemplate><%# Eval("CreatedOnDate")%></ItemTemplate>
                                                </asp:TemplateField>                        
                                            </Columns>          
                                        </Apollo:CustomGrid>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
