<%@ Page Language="C#" MasterPageFile="~/Inspinia.Master" AutoEventWireup="true" CodeBehind="test_search_result.aspx.cs" Inherits="Apollo.AdminStore.WebForm.Catalog.test_search_result" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="phHeaderScript" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
</asp:Content>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Test Search Result</h2>
        </div>
    </div>
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="ibox float-e-margins">
                    <div class="ibox-content">
                        <div class="reportFilter form-inline">
                            <asp:TextBox ID="txtQuery" runat="server" CssClass="form-control" Width="300px" />
                            <asp:LinkButton ID="lbSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="lbSearch_Click" />
                            <asp:LinkButton ID="lbPublish" runat="server" Text="Refresh Cache" OnClientClick="return confirm('This action will refresh all product related data on store front and performance could be affected.\nAre you sure to refresh?');" OnClick="lbPublish_Click" CssClass="btn btn-warning"></asp:LinkButton>
                            <asp:Label runat="server" ID="lbResult"></asp:Label>
                        </div>
                        <br />
                        <p>
                            What is Priority?<br />
                            <small>Search weight by keyword frequency.</small>
                        </p>
                        <p>
                            What is Popularity?<br />
                            <small>How many sold in the last 90 days.</small>
                        </p>
                        <p>
                            What is Display Rank?<br />
                            <small>Rating by stock availability, Apollo rating ,average review rating , popularity, and priority.</small>
                        </p>                        
                    </div>
                </div>
            </div>

            <div class="ibox float-e-margins">
                <div class="ibox-content">
                    <div class="table-responsive">
                        <Apollo:CustomGrid ID="gvResults" runat="server" PageSize="10" AllowPaging="true" OnPageIndexChanging="gvResults_PageIndexChanging" 
                            AutoGenerateColumns="false" OnPreRender="gvResults_PreRender" ShowHeader="true" DataKeyNames="Id"
                            CssClass="table table-striped table-bordered table-hover dataTable">                    
                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                            <PagerTemplate>            
                                <asp:Panel runat="server" DefaultButton="btnGoPage">
                                    Page 
                                    <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                    <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                    <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvResults.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                    <asp:ImageButton Visible='<%# (gvResults.CustomPageCount > (gvResults.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                    of <%= gvResults.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvResults.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                </asp:Panel>            
                            </PagerTemplate>
                            <Columns>
                                <asp:TemplateField HeaderText="Product ID" ItemStyle-HorizontalAlign="Center">                
                                    <HeaderTemplate>                                
                                        Product ID
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Name">
                                    <HeaderTemplate>
                                        Name
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Stock Availability">
                                    <HeaderTemplate>
                                        Stock Availability
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("StockAvailability")%></ItemTemplate>
                                </asp:TemplateField>                                
                                <asp:TemplateField HeaderText="Number of Review">
                                    <HeaderTemplate>
                                        Number of Review
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("ReviewCount")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Average Review Rating">
                                    <HeaderTemplate>
                                        Average Review Rating
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("AverageReviewRating")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Apollo Rating">
                                    <HeaderTemplate>
                                        Apollo Rating
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("ApolloRating")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Priority">
                                    <HeaderTemplate>
                                        Priority
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Priority")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Popularity">
                                    <HeaderTemplate>
                                        Popularity
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("Popularity")%></ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Display Rank">
                                    <HeaderTemplate>
                                        Display Rank
                                    </HeaderTemplate>
                                    <ItemTemplate><%# Eval("DisplayRank")%></ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </Apollo:CustomGrid>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
