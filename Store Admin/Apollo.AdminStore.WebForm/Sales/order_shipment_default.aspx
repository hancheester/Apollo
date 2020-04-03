<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_shipment_default" Codebehind="order_shipment_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" Runat="Server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Order</h2>
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
                        <Apollo:OrderSideMenu runat="server" Type="Shipments"></Apollo:OrderSideMenu>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                            <div class="html5buttons">
                                                <div class="dt-buttons btn-group">
                                                    <asp:LinkButton ID="lbResetFilter" runat="server" CssClass="btn btn-default buttons-copy buttons-html5" Text="Reset" OnClick="lbResetFilter_Click"></asp:LinkButton>
                                                    <asp:LinkButton ID="lbSearch" runat="server" Text="Search" CssClass="btn btn-default buttons-copy buttons-html5" OnClick="lbSearch_Click"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>

                                        <Apollo:CustomGrid ID="gvShipments" CssClass="table table-striped table-bordered table-hover dataTable" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvShipments_PageIndexChanging" 
                                            AutoGenerateColumns="false" OnSorting="gvShipments_Sorting" OnPreRender="gvShipments_PreRender" ShowHeader="true">
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>                                                
                                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                    Page 
                                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvShipments.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                    <asp:ImageButton Visible='<%# (gvShipments.CustomPageCount > (gvShipments.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                    of <%= gvShipments.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvShipments.RecordCount %> records found</asp:PlaceHolder>
                                                                                            <asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                                </asp:Panel>
                                            </PagerTemplate>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Shipment ID">                    
                                                    <HeaderTemplate>Shipment ID</HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Carrier" SortExpression="Carrier">                    
                                                    <HeaderTemplate>                                
                                                        <asp:LinkButton CommandArgument="Carrier" runat="server" CommandName="Sort">Carrier</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterCarrier" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Carrier")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Tracking #" SortExpression="TrackingRef">
                                                    <ItemTemplate><%# Eval("TrackingRef")%></ItemTemplate>                                                        
                                                    <HeaderTemplate>                                
                                                        <asp:LinkButton CommandArgument="TrackingRef" runat="server" CommandName="Sort">Tracking #</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtFilterTrackingRef" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                </asp:TemplateField>  
                                                <asp:TemplateField HeaderText="Date Shipped">                
                                                    <HeaderTemplate>Date</HeaderTemplate>
                                                    <ItemTemplate><%# Eval("TimeStamp")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">                    
                                                <ItemTemplate>
                                                    <a href='<%# "/sales/order_shipment_info.aspx?ordershipmentid=" + Eval("Id") %>'>View</a>
                                                </ItemTemplate>
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