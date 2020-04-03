<%@ Page Language="C#" MasterPageFile="~/Inspinia.master" AutoEventWireup="true" Inherits="Apollo.AdminStore.WebForm.Sales.order_invoice_new" Codebehind="order_invoice_new.aspx.cs" %>
<%@ Register TagPrefix="Apollo" TagName="OrderInfo" Src="~/UserControls/OrderInfoControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderNav" Src="~/UserControls/OrderNavControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="NoticeBox" Src="~/UserControls/NoticeBoxControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderSideMenu" Src="~/UserControls/OrderSideMenuControl.ascx" %>
<%@ Register TagPrefix="Apollo" TagName="OrderPrevNext" Src="~/UserControls/OrderPrevNextControl.ascx" %>
<asp:Content ContentPlaceHolderID="primaryPlaceHolder" runat="server">
    <div class="row wrapper white-bg page-heading">
        <div class="col-lg-8">
            <h2>New Order Invoice</h2>
            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        </div>
        <Apollo:OrderPrevNext runat="server"></Apollo:OrderPrevNext>
    </div>

    <Apollo:OrderNav runat="server"/>
  
    <div class="wrapper wrapper-content animated fadeInRight">
        <div class="row">
            <div class="col-lg-12">
                <Apollo:NoticeBox ID="enbNotice" runat="server" />
                <asp:ValidationSummary runat="server" DisplayMode="BulletList" ValidationGroup="vgInvoice" CssClass="alert alert-warning" />
                <div class="tabs-container">
                    <div class="tabs-left">
                        <Apollo:OrderSideMenu runat="server"></Apollo:OrderSideMenu>
                        <div class="tab-content">
                            <div class="tab-pane active">
                                <div class="panel-body">
                                    <Apollo:OrderInfo ID="eoiView" NewCommentAreaVisible="false" CommentAreaVisible="false" runat="server" />
                                    <div class="hr-line-dashed"></div>
                                    <div class="col-lg-12">
                                        <asp:LinkButton ID="lbSubmit" ValidationGroup="vgInvoice" runat="server" CssClass="btn btn-sm btn-primary" Text="Submit invoice" OnClick="lbSubmit_Click"></asp:LinkButton>
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