using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class OrderSideMenuControl : BaseUserControl
    {
        public IOrderService OrderService { get; set; }
        public OrderSideMenuType Type { get; set; }

        protected int QueryOrderId
        {
            get { return ((BasePage)Page).QueryOrderId; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadMenu();
        }

        private void LoadMenu()
        {
            int orderId = QueryOrderId;
            //< i class="alert fa fa-exclamation-circle"></i>
            
            ltlInformation.Text = string.Format("<li><a href=\"/sales/order_info.aspx?orderid={0}\">Information</a></li>", orderId);
            ltlShipments.Text = string.Format("<li><a href=\"/sales/order_shipment_default.aspx?orderid={0}\">Shipments</a></li>", orderId);
            ltlComments.Text = string.Format("<li><a href=\"/sales/order_comments_default.aspx?orderid={0}\">Comments history</a></li>", orderId);

            var pharmOrder = OrderService.GetPharmOrderByOrderId(orderId);
            var pharmOrderAlert = string.Empty;
            if (pharmOrder != null) pharmOrderAlert = " <i class=\"fa fa-exclamation-circle\"></i>";
            ltlPharmForm.Text = string.Format("<li><a href=\"/sales/order_pharm_form.aspx?orderid={0}\">Pharmaceutical form{1}</a></li>", orderId, pharmOrderAlert);

            ltlTransaction.Text = string.Format("<li><a href=\"/sales/order_payment_default.aspx?orderid={0}\">Transaction information</a></li>", orderId);
            ltlEmailPayment.Text = string.Format("<li><a href=\"/sales/order_email_payment_default.aspx?orderid={0}\">Email payment</a></li>", orderId);

            switch (Type)
            {
                case OrderSideMenuType.Information:
                    ltlInformation.Text = "<li class=\"active\"><a href=\"#\">Information</a></li>";
                    break;
                case OrderSideMenuType.Shipments:
                    ltlShipments.Text = "<li class=\"active\"><a href=\"#\">Shipments</a></li>";
                    break;
                case OrderSideMenuType.Comments:
                    ltlComments.Text = "<li class=\"active\"><a href=\"#\">Comments history</a></li>";
                    break;
                case OrderSideMenuType.PharmForm:
                    ltlPharmForm.Text = string.Format("<li class=\"active\"><a href=\"#\">Pharmaceutical form{0}</a></li>", pharmOrderAlert);
                    break;
                case OrderSideMenuType.Transaction:
                    ltlTransaction.Text = "<li class=\"active\"><a href=\"#\">Transaction information</a></li>";
                    break;
                case OrderSideMenuType.EmailPayment:
                    ltlEmailPayment.Text = "<li class=\"active\"><a href=\"#\">Email payment</a></li>";
                    break;
                case OrderSideMenuType.None:
                default:
                    break;
            }

        }
    }
}