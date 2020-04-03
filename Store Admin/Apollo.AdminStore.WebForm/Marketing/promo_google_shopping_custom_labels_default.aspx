<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="promo_google_shopping_custom_labels_default.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Marketing.promo_google_shopping_custom_labels_default" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Product Google Shopping Custom Label</h2>
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
                                        <asp:LinkButton ID="lbUpdateSelected" runat="server" OnClick="lbUpdateSelected_Click" Text="Update selected" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbReset" runat="server" Text="Reset" OnClick="lbReset_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvCustomLabels" runat="server" PageSize="10" OnRowDataBound="gvCustomLabels_RowDataBound" 
                                AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvCustomLabels_PageIndexChanging" 
                                AutoGenerateColumns="false" OnPreRender="gvCustomLabels_PreRender" ShowHeader="true" DataKeyNames="ProductId" 
                                CssClass="table table-striped table-bordered table-hover dataTable" OnRowCommand="gvCustomLabels_RowCommand">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>
                                    <div style="float: left; width: 50%;">
                                        <asp:Panel runat="server" DefaultButton="btnGoPage">
                                            Page 
                                            <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                            <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                            <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvCustomLabels.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                            <asp:ImageButton Visible='<%# (gvCustomLabels.CustomPageCount > (gvCustomLabels.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                            of <%= gvCustomLabels.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvCustomLabels.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                        </asp:Panel>
                                    </div>            
                                </PagerTemplate>
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="30px">
                                        <HeaderTemplate>
                                            <input type="checkbox" onclick="toggleChosen(this);" />
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
                                    <asp:TemplateField HeaderText="ProductId" SortExpression="ProductId" HeaderStyle-Width="60px" ItemStyle-HorizontalAlign="Center">                
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="ProductId" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterProductId" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <a href="/catalog/product_info.aspx?productid=<%# Eval("ProductId") %>"><%# Convert.ToInt32(Eval("ProductId")) %></a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name" SortExpression="Name" HeaderStyle-Width="170px">                
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Label 1" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Label 1<br />
                                            <asp:TextBox ID="txtFilterCustomLabel1" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtCustomLabel1" runat="server" Text='<%# Eval("CustomLabel1")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Label 2" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Label 2<br />
                                            <asp:TextBox ID="txtFilterCustomLabel2" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtCustomLabel2" runat="server" Text='<%# Eval("CustomLabel2")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Label 3" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Label 3<br />
                                            <asp:TextBox ID="txtFilterCustomLabel3" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtCustomLabel3" runat="server" Text='<%# Eval("CustomLabel3")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Label 4" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Label 4<br />
                                            <asp:TextBox ID="txtFilterCustomLabel4" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtCustomLabel4" runat="server" Text='<%# Eval("CustomLabel4")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Label 5" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Label 5<br />
                                            <asp:TextBox ID="txtFilterCustomLabel5" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtCustomLabel5" runat="server" Text='<%# Eval("CustomLabel5")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Value 1" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Value 1<br />
                                            <asp:TextBox ID="txtFilterValue1" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtValue1" runat="server" Text='<%# Eval("Value1")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Value 2" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Value 2<br />
                                            <asp:TextBox ID="txtFilterValue2" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtValue2" runat="server" Text='<%# Eval("Value2")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Value 3" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Value 3<br />
                                            <asp:TextBox ID="txtFilterValue3" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtValue3" runat="server" Text='<%# Eval("Value3")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Value 4" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Value 4<br />
                                            <asp:TextBox ID="txtFilterValue4" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtValue4" runat="server" Text='<%# Eval("Value4")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Value 5" HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            Value 5<br />
                                            <asp:TextBox ID="txtFilterValue5" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><asp:TextBox ID="txtValue5" runat="server" Text='<%# Eval("Value5")%>' CssClass="form-control"></asp:TextBox></ItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="40px">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lbUpdate" CommandName="save" CommandArgument='<%# Eval("ProductId") %>' runat="server" Text="Update"></asp:LinkButton> | <asp:LinkButton ID="lbClear" CommandName="clear" CommandArgument='<%# Eval("ProductId") %>' runat="server" Text="Clear"></asp:LinkButton>
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