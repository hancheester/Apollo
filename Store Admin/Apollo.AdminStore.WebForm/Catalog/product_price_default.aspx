<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.product_price_default" Codebehind="product_price_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <script type="text/javascript">
        function getStockLevel(branchId, barcode, id) {
            <%= ClientScript.GetCallbackEventReference(this, "branchId + '_' + barcode", "loadStockLevel", "id", true) %>;
        }

        function loadStockLevel(msg, context) {
            $('#' + context).html(msg);
        }

        function getColumn(styleClass) {
            $('.' + styleClass).each(
                function (index, domEle) {
                    $(domEle).find('.info').attr('class', 'fa fa-spinner fa-spin');
                    getStockLevel($(domEle).find('.branchId').val(), $(domEle).find('.barcode').val(), $(domEle).find('.elementId').val());
                });
        }
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            function toggleChosen(trigger) {
                $('.chosen > input[type=checkbox]').attr('checked', trigger.checked);
            }

            <% 
            var branches = OrderService.GetAllBranches();
            foreach (var branch in branches)
            {
            %>
                $('.<%= branch.Name.ToLower() %>').each(
                    function (index, domEle) {
                        $(domEle).find('.info').click(function () {
                            $(domEle).find('.info').attr('class', 'fa fa-spinner fa-spin');
                            getStockLevel($(domEle).find('.branchId').val(), $(domEle).find('.barcode').val(), $(domEle).find('.elementId').val());
                        });
                    });
            <%
            }
            %>
        });
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Product Price</h2>
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
                                        <asp:LinkButton runat="server" Text="Update selected" OnClick="lbUpdateSelected_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton runat="server" Text="Reset" OnClick="lbResetPriceFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton runat="server" Text="Search" OnClick="lbSearchPrice_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvPrices" runat="server" PageSize="10" OnRowDataBound="gvPrices_RowDataBound" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvPrices_PageIndexChanging" 
                                AutoGenerateColumns="false" OnSorting="gvPrices_Sorting" OnPreRender="gvPrices_PreRender" ShowHeader="true" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>
                                    <div style="float: left; width: 50%;">
                                        <asp:Panel runat="server" DefaultButton="btnPriceGoPage">
                                            Page 
                                            <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                            <asp:Button Width="0" runat="server" ID="btnPriceGoPage" OnClick="btnPriceGoPage_Click" CssClass="hidden" />
                                            <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvPrices.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                            <asp:ImageButton Visible='<%# (gvPrices.CustomPageCount > (gvPrices.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                            of <%= gvPrices.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvPrices.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                        </asp:Panel>
                                    </div>            
                                </PagerTemplate>
                                <Columns>            
                                    <asp:TemplateField HeaderText="ProductId" SortExpression="ProductId" HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center">                
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="ProductId" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterProductId" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <a href="/catalog/product_info.aspx?productid=<%# Eval("ProductId") %>"><%# Convert.ToInt32(Eval("ProductId")) %></a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" SortExpression="Name" HeaderStyle-Width="450px">                
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("ProductName") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Product Status" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Product Status<br />
                                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="lbSearchPrice_Click">
                                                <asp:ListItem Text="- Select -" Value="any"></asp:ListItem>
                                                <asp:ListItem Text="Online" Value="enabled"></asp:ListItem>
                                                <asp:ListItem Text="Offline" Value="disabled"></asp:ListItem>
                                            </asp:DropDownList>
                                        </HeaderTemplate>
                                        <ItemTemplate><i class="fa fa-eye<%# (bool)Eval("ProductEnabled") ? null : "-slash" %>" aria-hidden="true"></i></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Option" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate><%# Eval("Option") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>Status</HeaderTemplate>
                                        <ItemTemplate><i class="fa fa-eye<%# (bool)Eval("Enabled") ? null : "-slash" %>" aria-hidden="true"></i></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Weight" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">                
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="Weight" runat="server" CommandName="Sort">Weight (grams)</asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtWeight" runat="server" Text='<%# Eval("Weight")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Price" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="Price" runat="server" CommandName="Sort">Price</asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtPrice" runat="server" Text='<%# AdminStoreUtility.GetFormattedPrice(Eval("Price"), CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.None, places: 2) %>' CssClass="form-control"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Stock" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="Stock" runat="server" CommandName="Sort">Stock</asp:LinkButton>             
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtStock" runat="server" Text='<%# Eval("Stock")%>' CssClass="form-control"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Barcode" HeaderStyle-Width="200px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Barcode<br />
                                            <asp:TextBox ID="txtFilterBarcode" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtBarcode" runat="server" Text='<%# Eval("Barcode")%>' CssClass="form-control"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Branch Stock">
                                        <HeaderTemplate>
                                            Stock<br />
                                            <i class="info fa fa-search" style="cursor: pointer;" onclick="getColumn('stock');"></i>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <table class="table table-bordered">
                                                <%
                                                    var branches = OrderService.GetAllBranches();
                                                    foreach (var branch in branches)
                                                    {
                                                %>
                                                    <tr>
                                                        <td><%= branch.Name.ToUpper() %></td>
                                                        <td>
                                                            <span id='<%= branch.Name.ToLower() %>_<%# Eval("Id").ToString() %>' class="<%= branch.Name.ToLower() %> stock">
                                                                <i class="info fa fa-question-circle"></i>
                                                                <input type="hidden" value='<%= branch.Id %>' class="branchId" />
                                                                <input type="hidden" value='<%# Eval("Barcode") %>' class="barcode" />
                                                                <input type="hidden" value='<%= branch.Name.ToLower() %>_<%# Eval("Id").ToString() %>' class="elementId" />
                                                            </span>
                                                        </td>
                                                    </tr>
                                                <%
                                                    }
                                                %>
                                            </table>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                        <HeaderTemplate>
                                            <input type="checkbox" onclick="toggle_chosen(this);" />
                                            <br />
                                            <asp:DropDownList ID="ddlFilterChosen" runat="server" OnPreRender="ddlFilterChosen_PreRender" CssClass="form-control">
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
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>