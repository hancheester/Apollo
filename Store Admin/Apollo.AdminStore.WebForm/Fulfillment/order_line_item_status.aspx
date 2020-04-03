<%@ Page Language="C#" EnableViewState="true" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.FulFillment.order_line_item_status" Codebehind="order_line_item_status.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">    
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <script type="text/javascript" src="/js/wz_tooltip.js"></script>
    <div class="row wrapper white-bg page-heading hidden-print">
        <div class="col-lg-8">
            <h2>Line Item Status</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight hidden-print">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                <div class="html5buttons">
                                    <div class="dt-buttons btn-group">
                                        <asp:LinkButton ID="lbUpdateSelected" runat="server" Text="Update selected" OnClick="lbUpdateSelected_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbResetProductFilter" runat="server" Text="Reset" OnClick="lbResetProductFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearchProduct" runat="server" Text="Search" OnClick="lbSearchProduct_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <asp:Panel ID="pnlLineItemGrid" runat="server" DefaultButton="lbSearchProduct">
                                <Apollo:CustomGrid ID="gvProducts" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" 
                                    OnPageIndexChanging="gvProducts_PageIndexChanging" OnRowDataBound="gvProducts_RowDataBound" ShowHeaderWhenEmpty="true"
                                    AutoGenerateColumns="false" OnSorting="gvProducts_Sorting" OnPreRender="gvProducts_PreRender" ShowHeader="true" DataKeyNames="Id" 
                                    CssClass="table table-striped table-bordered table-hover dataTable">                   
                                    <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                    <PagerTemplate>                       
                                        <div style="float: left; width: 50%;">
                                            <asp:Panel runat="server" DefaultButton="btnProductGoPage">
                                                Page
                                                <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="/img/pager_arrow_left.gif" />
                                                <asp:Button Width="0" runat="server" ID="btnProductGoPage" OnClick="btnProductGoPage_Click" CssClass="hidden" />
                                                <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                     
                                                <asp:ImageButton Visible='<%# (gvProducts.CustomPageCount > (gvProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="/img/pager_arrow_right.gif" />                       
                                                of <%= gvProducts.PageCount%> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvProducts.RecordCount%> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder>
                                            </asp:Panel>
                                        </div>
                                    </PagerTemplate>
                                    <Columns>
                                        <asp:TemplateField HeaderText="Order ID" SortExpression="OrderId" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">                    
                                            <HeaderTemplate>                               
                                                <asp:LinkButton CommandArgument="OrderId" runat="server" CommandName="Sort">Order ID</asp:LinkButton>
                                                <asp:TextBox ID="txtFilterOrderId" runat="server" CssClass="form-control"/>
                                            </HeaderTemplate>
                                            <ItemTemplate><a href='/sales/order_info.aspx?orderid=<%# Eval("OrderId") %>' target="_blank"><%# Eval("OrderId") %></a></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Order Status">
                                            <ItemTemplate><%# Eval("OrderStatus") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="ProductId" SortExpression="ProductId" HeaderStyle-Width="70px">                    
                                            <HeaderTemplate>                               
                                                <asp:LinkButton CommandArgument="ProductId" runat="server" CommandName="Sort">Product ID</asp:LinkButton>
                                                <asp:TextBox ID="txtFilterProductId" runat="server" CssClass="form-control" />
                                            </HeaderTemplate>
                                            <ItemTemplate><%# Eval("ProductId")%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Name" SortExpression="Name" HeaderStyle-Width="320px">                    
                                            <HeaderTemplate>                               
                                                <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton>
                                                <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control" />
                                            </HeaderTemplate>
                                            <ItemTemplate><a target="_blank" href="/catalog/product_info.aspx?productid=<%# Eval("ProductId") %>" onmouseover="Tip('<img src=/get_image_handler.aspx?type=media&product_id=<%# Eval("ProductId") %> />')" onmouseout="UnTip()"><%# Eval("Name")%></a></ItemTemplate>
                                        </asp:TemplateField>                
                                        <asp:TemplateField HeaderText="Barcode" HeaderStyle-Width="40px">
                                            <HeaderTemplate>
                                                Barcode
                                                <asp:TextBox ID="txtFilterBarcode" runat="server" CssClass="form-control" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# Session["ProductPrices"] = null %>
                                                <%# Eval("Barcode") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>            
                                        <asp:TemplateField HeaderText="Option" HeaderStyle-Width="70px">                    
                                            <HeaderTemplate>                               
                                                Option<br />
                                                <asp:TextBox ID="txtFilterOption" CssClass="form-control" runat="server"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate><%# Eval("Option") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Quantity" SortExpression="Quantity" HeaderStyle-Width="140px">
                                            <ItemTemplate>
                                                <table class="table table-bordered">
                                                    <tr>
                                                        <td>Ordered</td>
                                                        <td><%# Eval("Quantity") %></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Pending</td>
                                                        <td><%# Eval("PendingQuantity") %></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Allocated</td>
                                                        <td><%# Eval("AllocatedQuantity") %></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Shipped</td>
                                                        <td><%# Eval("ShippedQuantity") %></td>
                                                    </tr>
                                                </table>                        
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Comment" HeaderStyle-Width="210px">
                                            <ItemTemplate>                                                
                                                <asp:TextBox ID="txtComment" CssClass="form-control" TextMode="MultiLine" Height="100px" runat="server"></asp:TextBox>
                                                <small class="text-navy">Comment would be added to related order. Once submitted, comment will be emptied.</small>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status" SortExpression="StatusCode" HeaderStyle-Width="200px">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="StatusCode" runat="server" CommandName="Sort">Status</asp:LinkButton><br />
                                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" OnInit="ddlStatus_Init"></asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlItemStatus" runat="server" CssClass="form-control" OnInit="ddlItemStatus_Init"></asp:DropDownList>
                                                <asp:Literal Visible="false" ID="ltlStatusCode" Text='<%# Eval("StatusCode") %>' runat="server"></asp:Literal>
                                                <small class="text-navy">
                                                    Following status will result in being removed from line distribution and quantity reset.<br />
                                                    <ul>
                                                        <li>Ordered</li>
                                                        <li>Pick in Progress</li>
                                                        <li>Cancelled</li>
                                                        <li>Despatch</li>
                                                        <li>Partial Shipping</li>
                                                        <li>Pending</li>
                                                        <li>Scheduled for Cancellation</li>
                                                        <li>Goods Allocated</li>
                                                    </ul>
                                                </small>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Update activity date?" HeaderStyle-Width="40px">
                                            <ItemTemplate><asp:CheckBox runat="server" ID="cbLastActivity" /></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Chosen" HeaderStyle-Width="80px">
                                            <HeaderTemplate>
                                                <input type="checkbox" onclick="toggle_chosen(this);" />
                                                <br />
                                                <asp:DropDownList ID="ddlFilterChosen" CssClass="form-control" runat="server">
                                                    <asp:ListItem Text="Any" Value="Any"></asp:ListItem>
                                                    <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                                    <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                                </asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" />
                                            </ItemTemplate>
                                        </asp:TemplateField>                
                                    </Columns>                   
                                </Apollo:CustomGrid>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>