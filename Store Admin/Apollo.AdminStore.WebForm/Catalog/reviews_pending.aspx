<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="True" Inherits="Apollo.AdminStore.WebForm.Catalog.reviews_pending" Codebehind="reviews_pending.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript">        
        function checkActions(trigger)
        {
            $('.actions').css({'display' : 'none', 'visibility' : 'hidden'});
            
            for (var i = 0; i < trigger.options.length; i++)            
                if (trigger.options[i].selected == true && trigger.options[i].value == 'changestatus')
                {
                    $('.actions').css({'display' : 'inline', 'visibility' : 'visible'});
                    break;
                }
        }
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Pending Reviews</h2>
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server"/>
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="table-responsive">
                            <div class="dataTables_wrapper form-inline dt-bootstrap">
                                <div class="html5buttons">
                                    <div class="dt-buttons btn-group">
                                        <asp:LinkButton ID="lbSubmit" runat="server" CssClass="btn btn-default buttons-copy buttons-html5" Text="Update" OnClick="lbSubmit_Click"></asp:LinkButton>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                                <div class="dataTables_filter">
                                    <label>
                                        Actions: 
                                        <asp:DropDownList ID="ddlActions" runat="server" CssClass="form-control input-sm" OnPreRender="ddlActions_PreRender">
                                            <asp:ListItem Text="- Select -" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Delete" Value="delete"></asp:ListItem>
                                            <asp:ListItem Text="Change Status" Value="changestatus"></asp:ListItem>
                                        </asp:DropDownList>
                                        <span class="actions" style="display: none;">
                                            <b>Status</b>
                                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control input-sm">                                                
                                                <asp:ListItem Text="Approved" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="Not Approved" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </span>
                                    </label>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvReviews" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvReviews_PageIndexChanging" 
                                AutoGenerateColumns="false" OnSorting="gvReviews_Sorting" OnPreRender="gvReviews_PreRender" ShowHeader="true" DataKeyNames="Id"
                                CssClass="table table-striped table-bordered table-hover dataTable">                    
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>
                                    <div style="float: left; width: 50%;">
                                        <asp:Panel runat="server" DefaultButton="btnGoPage">
                                            Page 
                                            <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                            <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                            <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvReviews.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                            <asp:ImageButton Visible='<%# (gvReviews.CustomPageCount > (gvReviews.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                            of <%= gvReviews.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvReviews.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                        </asp:Panel>
                                    </div>                                    
                                </PagerTemplate>
                                <Columns>
                                    <asp:TemplateField HeaderStyle-Width="80">
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
                                    <asp:TemplateField HeaderText="Product Review ID" SortExpression="Id" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="140">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product Review ID</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Created On" SortExpression="TimeStamp" HeaderStyle-Width="120">
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="TimeStamp" runat="server" CommandName="Sort">Created On</asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("TimeStamp")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Alias">                
                                        <HeaderTemplate>
                                            <asp:LinkButton CommandArgument="Alias" runat="server" CommandName="Sort">Alias</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterAlias" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Alias")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Comment">     
                                        <HeaderTemplate>
                                            Comment<br />                                
                                            <asp:TextBox ID="txtFilterComment" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Comment") %></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Product" SortExpression="ProductId" HeaderStyle-Width="180">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Product" runat="server" CommandName="Sort">Product</asp:LinkButton><br />                    
                                        </HeaderTemplate>
                                        <ItemTemplate><a href='<%# "/catalog/product_info.aspx?productid=" + Eval("ProductId") %>'><%# Eval("ProductName") %></a></ItemTemplate>
                                    </asp:TemplateField>            
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-Width="40">
                                        <ItemTemplate>
                                            <a href='<%# "/catalog/review_info.aspx?productreviewid=" + Eval("Id") %>'>Edit</a>
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