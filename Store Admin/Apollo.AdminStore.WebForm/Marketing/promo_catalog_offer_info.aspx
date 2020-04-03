<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Marketing.promo_catalog_offer_info" ValidateRequest="false" Codebehind="promo_catalog_offer_info.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<asp:Content ContentPlaceHolderID="phHeaderStyle" runat="server">
    <link href="/css/inspinia/plugins/dataTables/datatables.min.css" rel="stylesheet" />
    <link href="/css/inspinia/plugins/datepicker/datepicker3.css" rel="stylesheet" />
    <link href="/css/inspinia/offer-rule-tree.css" rel="stylesheet" />
</asp:Content>
<asp:Content ContentPlaceHolderID="phFooterScript" runat="server">
    <script type="text/javascript" src="/js/offer.js"></script>
    <!-- Data picker -->
    <script src="/js/inspinia/plugins/datepicker/bootstrap-datepicker.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            var panel = $('#<%= hfCurrentPanel.ClientID %>').val();

            if (panel) {
                $('.nav-tabs a[href=#' + panel + ']').tab('show');
            }

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
    <asp:HiddenField ID="hfCurrentPanel" runat="server" />
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>Catalog Offer</h2>
            <h3><asp:Literal ID="ltlTitle" runat="server"></asp:Literal></h3>
        </div>
    </div>

    <div class="row wrapper">
        <div class="col-lg-12">
            <div class="row"><p></p></div>
            <div class="row">
                <div class="pull-right">
                    <a href="/marketing/promo_catalog_offer_default.aspx" class="btn btn-sm btn-default">Back</a>
                    <a href="/marketing/promo_catalog_offer_info.aspx?offerruleid=<%= QueryOfferRuleId %>" class="btn btn-sm btn-primary">Reset</a>                    
                    <asp:LinkButton ID="lbClean" runat="server" Text="Clean basket" OnClientClick="return confirm('Are you sure to remove all items related to this offer from customers basket?');" CssClass="btn btn-sm btn-danger" OnClick="lbClean_Click"></asp:LinkButton>
                    <asp:LinkButton ID="lbPublish" runat="server" Text="Publish" OnClick="lbPublish_Click" OnClientClick="return confirm('This action will refresh all offers and products related data on store front and performance could be affected.\nAre you sure to publish?');" CssClass="btn btn-sm btn-success"></asp:LinkButton>
                    <asp:LinkButton ID="lbSave" runat="server" Text="Save" CssClass="btn btn-sm btn-info" ValidationGroup="vgEdit" OnClick="lbSave_Click"></asp:LinkButton>
                </div>
            </div> 
        </div>
    </div>

    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgEdit" CssClass="alert alert-warning" />
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#info" data-toggle="tab">General information</a></li>
                            <li><a href="#conditions" data-toggle="tab">Conditions</a></li>
                            <li><a href="#action" data-toggle="tab">Action</a></li>
                            <li><a href="#contentMgmt" data-toggle="tab">Offer page management</a></li>                            
                            <li><a href="#relatedProducts" data-toggle="tab">Related products</a></li>
                            <li><a href="#test" data-toggle="tab">Test offer</a></li>
                        </ul>
                        <div id="offer" class="tab-content">
                            <div id="info" class="tab-pane active">
                                <div class="panel-body">
                                    <table class="table">
                                        <tr>
                                            <th>Rule name<strong>*</strong></th>
                                            <td>
                                                <asp:TextBox ID="txtRuleName" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRuleName" ValidationGroup="vgEdit" ErrorMessage="Rule name is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Rule name is required.</span>"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Rule alias<strong>*</strong></th>
                                            <td>
                                                <asp:TextBox ID="txtRuleAlias" runat="server" CssClass="form-control"></asp:TextBox>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtRuleAlias" ValidationGroup="vgEdit" ErrorMessage="Rule alias is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Rule alias is required.</span>"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Status<strong>*</strong></th>
                                            <td>
                                                <asp:RadioButtonList ID="rblStatus" runat="server">
                                                    <asp:ListItem Text="Active" Value="true"></asp:ListItem>
                                                    <asp:ListItem Text="Inactive" Value="false"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>From date <small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox ID="txtDateFrom" CssClass="date form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>To date <small class="text-navy clearfix">(12:00am)</small></th>
                                            <td><asp:TextBox ID="txtDateTo" CssClass="date form-control" runat="server"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>Priority</th>
                                            <td><asp:TextBox ID="txtPriority" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>URL key<br/><small class="text-navy">(auto generated if left blank)</small></th>
                                            <td><asp:TextBox ID="txtUrlKey" runat="server" CssClass="form-control"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <th>
                                                Show offer tag
                                                <br/><small class="text-navy">(only visible on product grid)</small>
                                                <br/><small class="text-navy">(could be overwritten by product mark)</small>
                                            </th>
                                            <td><asp:CheckBox ID="chkShowOfferTag" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Show RRP / price reduction</th>
                                            <td><asp:CheckBox ID="chkShowRRP" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Point spendable</th>
                                            <td><asp:CheckBox ID="chkPointSpendable" runat="server" Checked="true" /></td>
                                        </tr>
                                        <tr>
                                            <th>Offer type</th>
                                            <td><asp:DropDownList ID="ddlOfferTypes" runat="server" CssClass="form-control"></asp:DropDownList></td>
                                        </tr>
                                        <tr>
                                            <th>Show count down timer
                                                <br/><small class="text-navy">(only visible on product details page)</small>
                                                <br/><small class="text-navy">(only works with valid 'to date')</small>
                                            </th>
                                            <td><asp:CheckBox ID="cbShowCountDownTimer" runat="server" Checked="true" /></td>
                                        </tr>
                                        <tr>
                                            <th>Display on product page</th>
                                            <td><asp:CheckBox ID="cbDisplayOnProductPage" runat="server" /></td>
                                        </tr>
                                        <tr>
                                            <th>Related items</th>
                                            <td>
                                                <asp:DropDownList ID="ddlRelatedTypes" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="Please select" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="Products" Value="products"></asp:ListItem>
                                                    <asp:ListItem Text="Brands" Value="brands"></asp:ListItem>
                                                    <asp:ListItem Text="Category" Value="category"></asp:ListItem>
                                                </asp:DropDownList>
                                                <br />
                                                <asp:TextBox ID="txtRelatedItems" runat="server" CssClass="form-control"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div id="conditions" class="tab-pane">
                                <div class="panel-body">
                                    <h4>Conditions (leave blank for all products)</h4>
                                    <div class="rule-root" data-ruleid="<%= QueryOfferRuleId %>">
                                        <ul class="rule-tree">
                                            <asp:Literal ID="ltlConditions" runat="server"></asp:Literal>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                            <div id="action" class="tab-pane">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <h4>Update prices using the following information</h4>
                                        <table class="table">
                                            <tr>
                                                <th>Apply</th>
                                                <td>
                                                    <asp:DropDownList ID="ddlAction" runat="server" CssClass="form-control"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Discount amount<strong>*</strong></th>
                                                <td>
                                                    <asp:TextBox ID="txtDiscountAmount" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDiscountAmount" ValidationGroup="vgEdit" ErrorMessage="Discount amount is required." Display="Dynamic"
                                                    Text="<span class='text-danger'><i class='fa fa-exclamation-circle'></i> Discount amount is required.</span>"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Discount target option
                                                    <small class="text-navy clearfix">(size, colour)</small>
                                                </th>
                                                <td>
                                                    <div class="form-inline">
                                                        <asp:DropDownList ID="ddlOptionOperator" runat="server" CssClass="form-control"></asp:DropDownList>
                                                        <asp:TextBox ID="txtOption" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>Stop further rules processing</th>
                                                <td>
                                                    <asp:RadioButtonList ID="rblProceed" runat="server">
                                                        <asp:ListItem Text="Yes" Value="false"></asp:ListItem>
                                                        <asp:ListItem Text="No" Value="true" Selected="True"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </div>
                            <div id="contentMgmt" class="tab-pane">
                                <div class="panel-body">
                                    <div class="col-lg-9">
                                        <table class="table">
                                            <tr>
                                                <th>Show this offer on special offer page</th>
                                                <td><asp:CheckBox ID="chkShowOnOfferPage" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Show this offer on header strip
                                                    <small class="text-navy clearfix">(appears only if "Show this offer on special offer page" is enabled)</small>
                                                </th>
                                                <td><asp:CheckBox ID="chkDisplayOnHeaderStrip" runat="server" /></td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Offer label                                                    
                                                    <small class="text-navy clearfix">(it will be used in product grid)</small>
                                                    <small class="text-navy clearfix">(if left blank, discount in percentage will be displayed instead)</small>
                                                    <img class="img-thumbnail" src="/img/offer-label.png" style="width: 400px"/>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtOfferLabel" runat="server" CssClass="form-control"></asp:TextBox>
                                                    <asp:CheckBox ID="cbDisableOfferLabel" runat="server" Text="Disable?" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    &quot;View offer&quot; URL
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtViewOfferURL" runat="server" CssClass="form-control"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Short description
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="txtShortDescription" runat="server" TextMode="MultiLine" Height="200px" CssClass="form-control"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Short description image
                                                    <small class="text-navy clearfix">(size should be 400x110)</small>
                                                </th>
                                                <td>
                                                    <asp:Literal ID="ltlSmallImage" runat="server"></asp:Literal>
                                                    <asp:FileUpload ID="fuSmallImage" runat="server" CssClass="form-control"/>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Long description
                                                    <small class="text-navy clearfix">(please use standard button html script below)</small>
                                                    <code>&lt;div class=&quot;row col-sm-4&quot;&gt;<br />
                                                        &lt;a href=&quot;[Link]&quot; class=&quot;btn btn-primary standard&quot;&gt;<br />
                                                            [Name for the link]<br />
                                                        &lt;/a&gt;<br />
                                                        &lt;/div&gt;
                                                    </code>
                                                </th>
                                                <td class="ftb">
                                                    <FTB:FreeTextBox ID="ftbLongDesc" ToolbarLayout="bold,italic,underline;bulletedlist,numberedlist" runat="Server" BreakMode="LineBreak" FormatHtmlTagsToXhtml="true" PasteMode="Text" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <th>
                                                    Long description image
                                                    <small class="text-navy clearfix">(size should be 825x220)</small>
                                                </th>
                                                <td>
                                                    <asp:Literal ID="ltlLargeImage" runat="server"></asp:Literal>
                                                    <asp:FileUpload ID="fuLargeImage" runat="server" CssClass="form-control"/>
                                                </td>
                                            </tr>                      
                                        </table>
                                    </div>
                                </div>
                            </div>                            
                            <div id="relatedProducts" class="tab-pane">
                                <div class="panel-body">
                                    <table class="table">
                                        <tr>
                                            <th>Selected products</th>
                                            <td>
                                                <asp:GridView ID="gvRelatedProducts" runat="server" AutoGenerateColumns="false" 
                                                    CssClass="table table-striped table-bordered table-hover dataTable" OnRowCommand="gvRelatedProducts_RowCommand">
                                                    <Columns>
                                                        <asp:TemplateField ShowHeader="true" HeaderText="Product ID">
                                                            <ItemTemplate>
                                                                <%# Eval("ProductId") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ShowHeader="true" HeaderText="Name">
                                                            <ItemTemplate>
                                                                <%# Eval("ProductName") %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Priority">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtPriority" runat="server" CssClass="form-control" Text='<%# Eval("Priority").ToString() %>'></asp:TextBox>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Enabled">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkEnabled" runat="server" Checked='<%# Convert.ToBoolean(Eval("Enabled")) %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Action">
                                                            <ItemTemplate>
                                                                <asp:LinkButton runat="server" Text="Delete" CommandArgument='<%# Eval("Id") %>' CommandName="deleteRelated"></asp:LinkButton> | 
                                                                <asp:LinkButton runat="server" Text="Update" CommandArgument='<%# Eval("Id") %>' CommandName="updateRelated"></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>Available products</th>
                                            <td>
                                                <div class="table-responsive">
                                                    <div class="dataTables_wrapper form-inline dt-bootstrap">
                                                        <div class="html5buttons">
                                                            <div class="dt-buttons btn-group">
                                                                <asp:LinkButton runat="server" Text="Reset" OnClick="gvRelatedProductSelector_lbResetFilter_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                                <asp:LinkButton runat="server" Text="Search" OnClick="gvRelatedProductSelector_lbSearch_Click" CssClass="btn btn-default buttons-copy buttons-html5"></asp:LinkButton>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <Apollo:CustomGrid ID="gvRelatedProductSelector" runat="server" PageSize="10" AllowPaging="true" AllowSorting="true" 
                                                        OnPageIndexChanging="gvRelatedProductSelector_PageIndexChanging" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" 
                                                        OnSorting="gvRelatedProductSelector_Sorting" OnPreRender="gvRelatedProductSelector_PreRender" ShowHeader="true" 
                                                        OnRowCommand="gvRelatedProductSelector_OnRowCommand" DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable">
                                                        <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                                        <PagerTemplate>                                
                                                            <asp:Panel runat="server" DefaultButton="btnGoPage">
                                                                Page
                                                                <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                                <asp:Button Width="0" runat="server" ID="btnGoPage" OnClick="gvRelatedProductSelector_btnGoPage_Click" CssClass="hidden" />
                                                                <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvRelatedProductSelector.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                                                <asp:ImageButton Visible='<%# (gvRelatedProductSelector.CustomPageCount > (gvRelatedProductSelector.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                                of
                                                                <%= gvRelatedProductSelector.PageCount %>
                                                                pages |
                                                                <asp:PlaceHolder ID="phRecordFound" runat="server">Total
                                                                    <%= gvRelatedProductSelector.RecordCount %>
                                                                    records found</asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                            </asp:Panel>
                                                        </PagerTemplate>
                                                        <Columns>
                                                            <asp:TemplateField HeaderStyle-Width="80px" HeaderStyle-CssClass="header" HeaderText="Product ID" ItemStyle-HorizontalAlign="Center" SortExpression="ProductId">                                    
                                                                <ItemTemplate><%# Eval("Id") %></ItemTemplate>
                                                                <HeaderTemplate>
                                                                    <asp:LinkButton CommandArgument="ProductId" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                                    <asp:TextBox ID="txtFilterId" CssClass="form-control" runat="server"></asp:TextBox>
                                                                </HeaderTemplate>                                    
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderStyle-CssClass="header" HeaderText="Product name">
                                                                <ItemTemplate><%# Eval("Name") %></ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Action">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton runat="server" Text="Add" CommandArgument='<%# Eval("Id") %>' CommandName="select"></asp:LinkButton>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </Apollo:CustomGrid>
                                                </div>                                                
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div id="test" class="tab-pane">
                                <div class="panel-body">
                                    <div class="col-lg-6">
                                        <Apollo:NoticeBox ID="enbTestProducts" runat="server" />
                                        <p>Please make sure you have saved the offer before testing.</p>
                                        <p>This is only an offer simulation. You have to make sure that the offer is active and not expired.</p>
                                        <div class="dataTables_wrapper form-inline dt-bootstrap">
                                            <div class="html5buttons">
                                                <div class="dt-buttons btn-group">
                                                    <asp:LinkButton runat="server" Text="Reset" CssClass="btn btn-default buttons-copy buttons-html5" OnClick="lbTestProductReset_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" Text="Search" CssClass="btn btn-default buttons-copy buttons-html5" OnClick="lbTestProductSearch_Click"></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <Apollo:CustomGrid ID="gvTestProducts" runat="server" PageSize="10" OnRowCommand="gvTestProducts_RowCommand"
                                            AllowPaging="true" AllowSorting="false" AutoGenerateColumns="false" ShowHeader="true"
                                            DataKeyNames="Id" CssClass="table table-striped table-bordered table-hover dataTable" 
                                            OnPageIndexChanging="gvTestProducts_PageIndexChanging" OnPreRender="gvTestProducts_PreRender">
                                            <PagerSettings Visible="true" Position="Top" Mode="NextPreviousFirstLast" />
                                            <PagerTemplate>
                                                <div style="float: left; width: 70%;">
                                                    <asp:Panel runat="server" DefaultButton="btnProductGoPage">
                                                        Page
                                                        <asp:ImageButton runat="server" CommandName="Page" CommandArgument="Prev" ImageUrl="~/img/pager_arrow_left.gif" />
                                                        <asp:Button Width="0" runat="server" ID="btnProductGoPage" CssClass="hidden" />
                                                        <asp:TextBox ID="txtPageIndex" Width="25" Text='<%# gvTestProducts.CustomPageIndex + 1 %>' runat="server"></asp:TextBox>
                                                        <asp:ImageButton Visible='<%# (gvTestProducts.CustomPageCount > (gvTestProducts.CustomPageIndex + 1)) %>' runat="server" CommandName="Page" CommandArgument="Next" ImageUrl="~/img/pager_arrow_right.gif" />
                                                        of
                                                        <%= gvTestProducts.PageCount %>
                                                        pages |
                                                        <asp:PlaceHolder ID="phRecordFound" runat="server">Total
                                                            <%= gvTestProducts.RecordCount %>
                                                            records found</asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="phRecordNotFound" runat="server" Visible="false">No record found</asp:PlaceHolder>
                                                    </asp:Panel>
                                                </div>
                                            </PagerTemplate>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Product ID">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Product ID</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtTestProductId" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%# Eval("Id")%>
                                                        <asp:HiddenField ID="hfProductId" runat="server" Value='<%# Eval("Id") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Product name">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton CommandArgument="Id" runat="server" CommandName="Sort">Name</asp:LinkButton><br />
                                                        <asp:TextBox ID="txtTestProductName" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </HeaderTemplate>
                                                    <ItemTemplate><%# Eval("Name")%></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Open for offer">                                                    
                                                    <ItemTemplate><%# Eval("OpenForOffer") %></ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lbPreview" runat="server" CommandArgument='<%# Eval("Id")%>' CommandName="preview">Preview</asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </Apollo:CustomGrid>
                                    </div>
                                    <asp:PlaceHolder ID="phPreviewProduct" runat="server" Visible="false">
                                        <div class="col-lg-6">
                                            <p>Product below is only for preview.</p>
                                            <p>If offer price is zero, this product has no discount.</p>
                                            <p>If there is an offer price, it will display related offer.</p>
                                            <table class="table table-striped">
                                                <tr>
                                                    <th>Product ID</th>
                                                    <td><asp:Literal ID="ltTestProductId" runat="server"></asp:Literal></td>
                                                </tr>
                                                <tr>
                                                    <th>Name</th>
                                                    <td><asp:Literal ID="ltTestProductName" runat="server"></asp:Literal></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Repeater ID="rptTestProductPrices" runat="server">
                                                            <HeaderTemplate>
                                                                <table class="table">
                                                                    <tr>
                                                                        <th>Option</th>
                                                                        <th>Original Price (incl tax)</th>
                                                                        <th>Offer Price (incl tax)</th>
                                                                        <th>Related Offer</th>
                                                                    </tr>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td><%# Eval("Option") %></td>
                                                                    <td><%= CurrencySettings.PrimaryStoreCurrencyCode %> <%# Eval("PriceInclTax", "{0:f2}") %></td>
                                                                    <td><%= CurrencySettings.PrimaryStoreCurrencyCode %> <%# Eval("OfferPriceInclTax", "{0:f2}") %></td>
                                                                    <td><%# GetOfferName(Convert.ToInt32(Eval("OfferRuleId"))) %></td>
                                                                </tr>                                                                
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                </table>
                                                            </FooterTemplate>
                                                        </asp:Repeater>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>