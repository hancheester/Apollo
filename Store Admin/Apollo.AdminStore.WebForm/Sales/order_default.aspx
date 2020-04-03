<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_default" Codebehind="order_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/order.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/flag-icon-css/2.1.0/css/flag-icon.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="/js/order.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.date').datepicker({
                format: 'dd/mm/yyyy',
                keyboardNavigation: false,
                forceParse: false,
                autoclose: true,
                todayHighlight: true
            });

            $('.activity-red').each(
                function (index, domEle) {
                    $(domEle).parent().parent().addClass('traffic-danger');
                });

            $('.activity-orange').each(
                function (index, domEle) {
                    $(domEle).parent().parent().addClass('traffic-warning');
                });
        });
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">    
    <% 
        if (HasState(ORDER_ADDRESS_FILTER))
        {
            filterby.Text = "<span style='font-size: 15px;'>filtered by address (" + GetStringState(ORDER_ADDRESS_FILTER) + ")</span>";
        }
    %>
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Orders <asp:Literal ID="filterby" runat="server"></asp:Literal></h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                <div class="html5buttons">
                                    <div class="dt-buttons btn-group">
                                        <asp:LinkButton ID="btnExportToCSV" runat="server" Text="Export selected to CSV" CssClass="btn btn-default buttons-copy buttons-html5" OnClientClick="return validateSelection();" onclick="btnExportToCSV_Click" />
                                        <a href="/sales/order_new_select_customer.aspx" class="btn btn-default buttons-copy buttons-html5">Create</a>
                                        <a href="/sales/order_default.aspx" class="btn btn-default buttons-copy buttons-html5">Refresh</a>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <a data-toggle="collapse" href="#advancedSearch" class="btn btn-default buttons-copy buttons-html5">Advanced search</a>
                                    </div>
                                </div>
                            </div>
                            <div class="clearfix"></div>
                            <div class="collapse <%= DisplayAdvancedSearch ? "in" : string.Empty %>" id="advancedSearch">
                                <h4>Advanced Search</h4>
                                <div class="m-b-md">
                                    <label class="col-sm-2 control-label">Address</label>
                                    <div class="col-sm-10">
                                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control"></asp:TextBox>
                                        <span class="help-block m-b-none">It will be used to match address line 1, address line 2, city, county, postcode for both billing and shipping addresses.</span>
                                    </div>
                                    <div class="clearfix"></div>
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-sm-4 col-sm-offset-2">
                                        <a data-toggle="collapse" href="#advancedSearch" class="btn btn-white">Cancel</a>
                                        <asp:LinkButton ID="lbAdvancedSearch" OnClick="lbAdvancedSearch_Click" runat="server" Text="Search" CssClass="btn btn-primary"></asp:LinkButton>                                            
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                            </div>
                            <asp:Panel runat="server" DefaultButton="lbSearch">
                                <Apollo:CustomGrid ID="cgOrders" runat="server" PageSize="20" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable"
                                    AutoGenerateColumns="false" AllowPaging="true" AllowSorting="true"
                                    OnPreRender="cgOrders_PreRender" ShowHeaderWhenEmpty="true"
                                    OnSorting="cgOrders_Sorting"
                                    OnPageIndexChanging="cgOrders_PageIndexChanging">
                                    <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                    <PagerTemplate>                                      
                                        <div style="float: left; width: 50%;">
                                            <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                Page 
                                                <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                                <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# cgOrders.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                                <asp:ImageButton Visible='<%# (cgOrders.CustomPageCount > (cgOrders.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                                of <%= cgOrders.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= cgOrders.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                            </asp:Panel>
                                        </div>            
                                    </PagerTemplate>
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-Width="80px">
                                            <HeaderTemplate>                       
                                                <br />
                                                <asp:DropDownList ID="ddlFilterChosen" CssClass="form-control" runat="server" OnPreRender="ddlFilterChosen_PreRender">
                                                    <asp:ListItem Text="Any" Value="Any"></asp:ListItem>
                                                    <asp:ListItem Text="Yes" Value="Yes"></asp:ListItem>
                                                    <asp:ListItem Text="No" Value="No"></asp:ListItem>
                                                </asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>                        
                                                <asp:CheckBox ID="cbChosen" CssClass="chkExportCSV" runat="server" CausesValidation="false" />                       
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Order ID" HeaderStyle-Width="80px" SortExpression="Id" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>                                
                                                <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Order ID</asp:LinkButton><br />                    
                                                <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate><a href='<%# "/sales/order_info.aspx?orderid=" + Eval("Id") %>' id='<%# Eval("Id") %>' ><%# Eval("Id") %></a>
                                                <asp:Label ID="lblOrderId" runat="server" Style="display:none" Text=' <%# Eval("Id")%>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Placed On" SortExpression="OrderPlaced">                
                                            <HeaderTemplate>                        
                                                <asp:LinkButton CommandArgument="OrderPlaced" runat="server" CommandName="Sort">Placed on</asp:LinkButton>
                                                <br />
                                                <asp:TextBox CssClass="date form-control input-sm" ID="txtFromDate" runat="server"></asp:TextBox>
                                                to <asp:TextBox ID="txtToDate" CssClass="date form-control input-sm" runat="server"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate><%# Eval("OrderPlaced")%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Activity On" SortExpression="LastActivityDate">                
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="LastActivityDate" runat="server" CommandName="Sort">Last activity</asp:LinkButton>
                                                <br /><asp:TextBox CssClass="date form-control input-sm" ID="txtFromActivityDate" runat="server"></asp:TextBox>
                                                to <asp:TextBox ID="txtToActivityDate" CssClass="date form-control input-sm" runat="server"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate><span class='activity-<%# GetTrafficLightColour(Eval("LastAlertDate"), Eval("StatusCode"), Eval("Id")) %>'><%# Eval("LastActivityDate") %></span></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Customer Email" SortExpression="Email">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="Email" runat="server" CommandName="Sort">Email</asp:LinkButton><br />
                                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# Eval("Email") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Grand total">                                                
                                            <ItemTemplate>
                                                <span class="label"><%# AdminStoreUtility.GetFormattedPrice(Eval("GrandTotal"), Eval("CurrencyCode").ToString(), CurrencyType.HtmlEntity) %></span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Shipping" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="ShippingOption" runat="server" CommandName="Sort">Shipping</asp:LinkButton><br />
                                                <asp:DropDownList ID="ddlDelivery" CssClass="form-control" runat="server" OnInit="ddlDelivery_Init"></asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# Eval("ShippingOptionId") != null ? AdminStoreUtility.GetShippingImage((int)Eval("ShippingOptionId")) : string.Empty %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Country" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                Country<br />
                                                <asp:DropDownList ID="ddlCountries" CssClass="form-control" runat="server" OnInit="ddlCountries_Init" DataTextField="Name" DataValueField="Id"></asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# AdminStoreUtility.GetShippingCountryImage(Convert.ToInt32(Eval("ShippingCountryId"))) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="StatusCode" runat="server" CommandName="Sort">Status</asp:LinkButton><br />
                                                <asp:DropDownList ID="ddlStatus" CssClass="form-control" runat="server" OnInit="ddlStatus_Init" DataTextField="Status" DataValueField="StatusCode"></asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <span class="label <%# AdminStoreUtility.GetLabelColour(Eval("StatusCode").ToString()) %>"><%# Eval("OrderStatus") %></span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Issue">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="IssueCode" runat="server" CommandName="Sort">Progress</asp:LinkButton><br />
                                                <asp:DropDownList ID="ddlIssue" CssClass="form-control" runat="server" OnInit="ddlIssue_Init" DataTextField="Issue" DataValueField="IssueCode"></asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# Eval("OrderIssue") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last updated by">
                                            <ItemTemplate>
                                                <span class="label"><%# Eval("LastUpdatedBy") %></span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action">                    
                                            <ItemTemplate>
                                                <a href='<%# "/sales/order_info.aspx?orderid=" + Eval("Id") %>'>View</a>
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