using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_invoice_new : BasePage
    {
        public IOrderService OrderService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        private const string SUBTRACT_FORM = "({0})";
        private const string QTY_TO_INVOICE_BY = "qty_to_invoice_by_";

        protected override void OnInit(EventArgs e)
        {
            LoadOrderInfo();
            base.OnInit(e);
        }

        private void LoadOrderInfo()
        {
            var orderView = OrderService.GetOrderOverviewModelById(QueryOrderId);

            if (orderView == null)
                Response.Redirect("/sales/order_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderNotFound);

            ltlTitle.Text = string.Format("<h3>Order # {0}</h3>", orderView.Id);
        }

        protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Literal ltl = e.Item.FindControl("ltlRowTotal") as Literal;
                TextBox txt = e.Item.FindControl("txtQtyInvoiced") as TextBox;

                var item = (LineItem)e.Item.DataItem;

                if (ViewState[QTY_TO_INVOICE_BY + item.ProductPriceId.ToString()] == null)
                    SetState(QTY_TO_INVOICE_BY + item.ProductPriceId.ToString(), item.Quantity);

                txt.Text = GetIntState(QTY_TO_INVOICE_BY + item.ProductPriceId.ToString()).ToString();
                ltl.Text = AdminStoreUtility.GetFormattedPrice(item.PriceInclTax * GetIntState(QTY_TO_INVOICE_BY + item.ProductPriceId.ToString()), item.CurrencyCode, CurrencyType.HtmlEntity);
            }
        }
        
        protected void lbSubmit_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            OrderService.ProcessOrderInvoice(orderId);

            // Save to comments
            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Submit Invoice",
                                                    "Invoice was successfully created.",
                                                    string.Empty);
            
            LoadOrderInfo();
            enbNotice.Message = "Invoice was successfully created.";
        }       
    }
}