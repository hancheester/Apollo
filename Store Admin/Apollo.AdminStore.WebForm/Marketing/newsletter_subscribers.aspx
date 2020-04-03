<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.newsletter_subscribers" Codebehind="newsletter_subscribers.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" Runat="Server">
    <div class="row wrapper border-bottom white-bg page-heading">
        <div class="col-lg-12">
            <h2>Newsletter Subscribers</h2>
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
                                        <asp:LinkButton ID="lbSave" runat="server" Text="Save" OnClick="lbSave_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbSearch" runat="server" Text="Search" OnClick="lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                        <asp:LinkButton ID="lbResetFilter" runat="server" Text="Reset" OnClick="lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <Apollo:CustomGrid ID="gvSubscribers" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="gvSubscribers_PageIndexChanging" 
                                AutoGenerateColumns="false" OnSorting="gvSubscribers_Sorting" OnPreRender="gvSubscribers_PreRender" ShowHeader="true" OnRowDataBound="gvSubscribers_RowDataBound" 
                                CssClass="table table-striped table-bordered table-hover dataTable" 
                                DataKeyNames="Id">
                                <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast"/>
                                <PagerTemplate>
                                    <asp:Panel runat="server" DefaultButton="btnGoPage">
                                        Page 
                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                        <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="btnGoPage_Click" CssClass="hidden" />
                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvSubscribers.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>                      
                                        <asp:ImageButton Visible='<%# (gvSubscribers.CustomPageCount > (gvSubscribers.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />                        
                                        of <%= gvSubscribers.PageCount %> pages | <asp:PlaceHolder ID="phRecordFound" runat="server">Total <%= gvSubscribers.RecordCount %> records found</asp:PlaceHolder><asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false" >No record found</asp:PlaceHolder> 
                                    </asp:Panel>
                                </PagerTemplate>
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <input type="checkbox" onclick="toggle_chosen(this);" />
                                            <br />
                                            <asp:DropDownList ID="ddlFilterChosen" runat="server" OnPreRender="ddlFilterChosen_PreRender" CssClass="form-control">
                                                <asp:ListItem Text="Any" Value="any"></asp:ListItem>
                                                <asp:ListItem Text="Yes" Value="yes"></asp:ListItem>
                                                <asp:ListItem Text="No" Value="no"></asp:ListItem>
                                            </asp:DropDownList>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox runat="server" ID="cbChosen" CssClass="chosen" Checked='<%# Convert.ToBoolean(Eval("IsActive")) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Subscriber ID" SortExpression="SubscriberId">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Subscriber ID</asp:LinkButton>                    
                                        </HeaderTemplate>
                                        <ItemTemplate><%# Eval("Id")%></ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" SortExpression="Email">
                                        <ItemTemplate><%# Eval("Email")%></ItemTemplate>
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Email" runat="server" CommandName="Sort">Email</asp:LinkButton><br />
                                            <asp:TextBox ID="txtFilterEmail" runat="server" CssClass="form-control"></asp:TextBox>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Active" SortExpression="Active">                
                                        <HeaderTemplate>                                
                                            <asp:LinkButton CommandArgument="Active" runat="server" CommandName="Sort">Active</asp:LinkButton><br />
                                            <asp:DropDownList ID="ddlActive" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="- Select -" Value="any"></asp:ListItem>
                                                <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
                                                <asp:ListItem Text="No" Value="false"></asp:ListItem>
                                            </asp:DropDownList>
                                        </HeaderTemplate>
                                        <ItemTemplate><%# (bool)Eval("IsActive") ? "Yes" : "No" %></ItemTemplate>
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