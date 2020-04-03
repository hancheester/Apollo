<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.promo_catalog_offer_default" Codebehind="promo_catalog_offer_default.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet">
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet">
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
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Catalog Offer Rules</h2>
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
                                        <asp:LinkButton ID="lbCleanBasket" runat="server" Text="Clean basket" OnClientClick="return confirm('Are you sure to remove all items related to offers from customer baskets?');" OnClick="lbCleanBasket_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <a href="/marketing/promo_catalog_offer_new.aspx" class="btn btn-default buttons-copy buttons-html5">Create</a>
                                        <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClick="lbPublish_Click" OnClientClick="return confirm('This action will refresh all offers and products related data on store front and performance could be affected.\nAre you sure to publish?');" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <a href="/marketing/promo_catalog_offer_default.aspx" class="btn btn-default buttons-copy buttons-html5">Refresh</a>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <asp:Panel ID="pnlOfferGrid" runat="server" DefaultButton="lbSearch">            
                                <Apollo:CustomGrid ID="gvRules" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvRules_PageIndexChanging" 
                                    AutoGenerateColumns="false" OnSorting="gvRules_Sorting" OnPreRender="gvRules_PreRender" ShowHeader="true" DataKeyNames="Id" ShowHeaderWhenEmpty="true"
                                    CssClass="table table-striped table-bordered table-hover dataTable">                    
                                    <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                    <PagerTemplate>
                                        <div style="float: left; width: 50%;">
                                        <asp:Panel runat="server" DefaultButton="btnGoPage">
                                            Page 
                                            <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                            <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                            <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvRules.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                            <asp:ImageButton Visible='<%# (gvRules.CustomPageCount > (gvRules.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                            of <%= gvRules.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvRules.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                        </asp:Panel>
                                        </div>            
                                    </PagerTemplate>
                                    <Columns>
                                        <asp:TemplateField HeaderText="ID" SortExpression="Id" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Offer Rule ID</asp:LinkButton><br />
                                                <asp:TextBox ID="txtFilterId" runat="server" CssClass="form-control"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rule Name" SortExpression="Name">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="Name" runat="server" CommandName="Sort">Rule Name</asp:LinkButton><br />
                                                <asp:TextBox ID="txtRuleName" runat="server" CssClass="form-control"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date Start" SortExpression="StartDate">
                                            <HeaderTemplate>
                                                From Date<br />
                                                <asp:TextBox CssClass="date form-control" ID="txtFromDate" runat="server"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate><%# Convert.ToDateTime(Eval("StartDate")) == DateTime.MinValue ? string.Empty : Eval("StartDate") %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date Expire" SortExpression="EndDate">
                                            <HeaderTemplate>
                                                To Date<br />
                                                <asp:TextBox CssClass="date form-control" ID="txtToDate" runat="server"></asp:TextBox>
                                            </HeaderTemplate>
                                            <ItemTemplate><%# Convert.ToDateTime(Eval("EndDate")) == DateTime.MinValue ? string.Empty : Eval("EndDate")%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Proceed For Next" ItemStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>Proceed For Next</HeaderTemplate>
                                            <ItemTemplate><%# Convert.ToBoolean(Eval("ProceedForNext")) ? "Yes" : "No"%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status" SortExpression="IsActive">
                                            <HeaderTemplate>
                                                <asp:LinkButton CommandArgument="IsActive" runat="server" CommandName="Sort">Status</asp:LinkButton><br />
                                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="Active" Value="true"></asp:ListItem>
                                                    <asp:ListItem Text="Inactive" Value="false"></asp:ListItem>
                                                </asp:DropDownList>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive"%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Validity">
                                            <ItemTemplate><%# GetStatus(Eval("Id")) %></ItemTemplate>
                                        </asp:TemplateField>                
                                        <asp:TemplateField SortExpression="Priority" HeaderStyle-Width="80">
                                            <HeaderTemplate><asp:LinkButton CommandArgument="Priority" runat="server" CommandName="Sort">Priority</asp:LinkButton></HeaderTemplate>
                                            <ItemTemplate><%# Eval("Priority")%></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <a href='<%# "/marketing/promo_catalog_offer_info.aspx?offerruleid=" + Eval("Id") %>'>Edit</a>
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