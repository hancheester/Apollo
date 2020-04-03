using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Domain.Tax;
using Apollo.Core.Model;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.FulFillment
{
    public partial class order_packing_info : BasePage, ICallbackEventHandler
    {
        private const int SHOW_MAX_COMMENT = 8;
        
        public IAccountService AccountService { get; set; }
        public IOrderService OrderService { get; set; }
        public IShippingService ShippingService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public ShippingSettings ShippingSettings { get; set; }
        public TaxSettings TaxSettings { get; set; }
        public StoreInformationSettings StoreInformationSettings { get; set; }

        protected override void OnInit(EventArgs e)
        {
            int orderId = QueryOrderId;

            var order = OrderService.GetOrderOverviewModelById(orderId);

            if (order != null)
                ltlTitle.Text = string.Format("<h3 class='printHide'>Order # {0}</h3>", order.Id.ToString());
            else
                Response.Redirect("/fulfillment/order_fulfillment.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderNotFound);

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var orderView = OrderService.GetOrderOverviewModelById(QueryOrderId);

                if (orderView != null)
                {
                    bool charged = OrderService.HasFullyPaid(orderView.Id);
                    LoadInfo(charged);
                }
                else
                    Response.Redirect("/fulfillment/order_fulfillment.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderNotFound);                
            }

            ClientScriptManager cm = Page.ClientScript;

            string cbReference = cm.GetCallbackEventReference(this, "arg", "receiveAlert", string.Empty);
            string callbackScript = "function checkAlert(arg, context) {" + cbReference + "; }";

            cm.RegisterClientScriptBlock(this.GetType(), "checkAlert", callbackScript, true);
        }

        protected void ddlIssue_Init(object sender, EventArgs e)
        {
            ddlIssue.DataSource = OrderService.GetOrderIssueList();
            ddlIssue.DataBind();
        }

        /// <summary>
        /// The link button will only be enabled if order has already been charged.
        /// </summary>
        protected void lbSubmit_Click(object sender, EventArgs e)
        {
            OrderService.ProcessOrderShipment(
                QueryOrderId,
                Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                esnShipping.Carrier,
                esnShipping.TrackingNumber,
                esnShipping.NotificationEmail);

            esnShipping.Clear();

            // Display message after process
            enbInfo.Message = "Order was shipped successfully.";
            
            LoadInfo(true);
        }

        protected void lbGo_Click(object sender, EventArgs e)
        {
            Response.Redirect("/fulfillment/order_packing_info.aspx?orderid=" + txtGoOrderId.Text.Trim());
        }

        protected void lbSubmitComment_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            string orderStatus = OrderService.GetOrderStatusCodeByOrderId(orderId);
            string issue = string.Empty;

            if (ddlOrderStatus.SelectedIndex > 0)
            {
                string code = ddlOrderStatus.SelectedValue;
                orderStatus = OrderService.GetOrderStatusByCode(code);
                OrderService.UpdateOrderStatusCodeByOrderId(orderId, code);
            }

            if (ddlIssue.SelectedIndex > 0)
            {
                issue = ", " + ddlIssue.SelectedItem.Text;
                OrderService.UpdateOrderIssueCodeByOrderId(orderId, ddlIssue.SelectedValue);
            }
            else
                OrderService.UpdateOrderIssueCodeByOrderId(orderId, ddlIssue.SelectedValue);

            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    orderStatus + issue,
                                                    string.Empty,
                                                    txtComment.Text.Trim());

            OrderService.UpdateOrderLastActivityDateByOrderId(orderId, DateTime.Now);
            
            txtComment.Text = string.Empty;

            // Display message
            enbInfo.Message = "Order status / issue / comment was updated successfully.";

            LoadInfo(true);
        }

        protected void ProcessPayment(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            int profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
            string fullName = Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName"));
            var message = OrderService.ProcessPaymentForFulfillment(orderId, profileId, fullName);

            bool isCharged;

            if (string.IsNullOrEmpty(message))
            {
                enbInfo.Message = "Order was charged successfully.";
                isCharged = true;
            }
            else
            {
                enbInfo.Message = "Order was FAILED to charge. " + message;
                isCharged = false;
            }
            
            LoadInfo(isCharged);
        }
       
        protected bool HasMoreComments()
        {
            var comments = OrderService.GetOrderCommentOverviewModelListByOrderId(QueryOrderId, AppConstant.SHOW_MAX_COMMENT + 1);
            return AppConstant.SHOW_MAX_COMMENT < comments.Count;
        }
       
        protected string CheckIfDifferentFromRRP(string currencyCode, decimal exchangeRate, decimal rrp, decimal priceInclTax)
        {
            if (rrp > 0M && rrp != priceInclTax)
                return string.Format("was {0}, now {1}",
                    AdminStoreUtility.GetFormattedPrice(rrp, currencyCode, CurrencyType.HtmlEntity, exchangeRate),
                    AdminStoreUtility.GetFormattedPrice(priceInclTax, currencyCode, CurrencyType.HtmlEntity, exchangeRate));

            return string.Empty;
        }

        private void LoadInfo(bool charged)
        {
            int orderId = QueryOrderId;

            var shippingAddress = OrderService.GetShippingAddressViewModelByOrderId(orderId);

            ltlHeaderAddress.Text = "Ship To:" + HtmlElement.BR;
            if (string.IsNullOrEmpty(shippingAddress.Name) == false) ltlHeaderAddress.Text += shippingAddress.Name + HtmlElement.BR;
            if (string.IsNullOrEmpty(shippingAddress.AddressLine1) == false) ltlHeaderAddress.Text += shippingAddress.AddressLine1 + HtmlElement.BR;
            if (string.IsNullOrEmpty(shippingAddress.AddressLine2) == false) ltlHeaderAddress.Text += shippingAddress.AddressLine2 + HtmlElement.BR;
            if (string.IsNullOrEmpty(shippingAddress.City) == false) ltlHeaderAddress.Text += shippingAddress.City + HtmlElement.BR;
            if (string.IsNullOrEmpty(shippingAddress.PostCode) == false) ltlHeaderAddress.Text += shippingAddress.PostCode + HtmlElement.BR;
            if (string.IsNullOrEmpty(shippingAddress.USStateName) == false) ltlHeaderAddress.Text += shippingAddress.USStateName + HtmlElement.BR;
            if (string.IsNullOrEmpty(shippingAddress.CountryName) == false) ltlHeaderAddress.Text += shippingAddress.CountryName + HtmlElement.BR;

            ltlDeliveryAddr.Text = ltlHeaderAddress.Text;

            eohHeader.OrderId = orderId;
            eavShipping.OrderId = orderId;

            var orderView = OrderService.GetOrderOverviewModelById(orderId);
            esnShipping.OrderOverviewModel = orderView;

            // Exclude anonymous order
            if (orderView.ProfileId > 0)
            {
                var account = AccountService.GetAccountByProfileId(orderView.ProfileId);

                if (account != null)
                {
                    eavShipping.Email = string.IsNullOrEmpty(account.Email) ? string.Empty : account.Email;
                    eavShipping.Phone = string.IsNullOrEmpty(account.ContactNumber) ? string.Empty : account.ContactNumber;
                    eavShipping.DisplayPhone = account.DisplayContactNumberInDespatch;

                    ltContactNumber.Text = string.Empty;
                    if (account.DisplayContactNumberInDespatch)
                        ltContactNumber.Text = string.Format("<i class='fa fa-phone' aria-hidden='true'></i> {0}", account.ContactNumber);
                }
            }

            if (orderView.PointValue > 0)
            {
                phEarnPoint.Visible = true;
                ltlPointValue.Text = orderView.PointValue.ToString();
            }
            else
                phEarnPoint.Visible = false;

            var items = OrderService.GetLineItemOverviewModelListByOrderId(orderId);
            rptItems.DataSource = items;
            rptItems.DataBind();

            rptDnoteItems.DataSource = items;
            rptDnoteItems.DataBind();

            rptComments.DataSource = OrderService.GetOrderCommentOverviewModelListByOrderId(orderId, AppConstant.SHOW_MAX_COMMENT);
            rptComments.DataBind();

            if (orderView.StatusCode == OrderStatusCode.SHIPPING
                    || orderView.StatusCode == OrderStatusCode.INVOICED
                    || orderView.StatusCode == OrderStatusCode.CANCELLED
                    || orderView.StatusCode == OrderStatusCode.DISCARDED
                    || orderView.StatusCode == OrderStatusCode.PENDING
                    || orderView.StatusCode == OrderStatusCode.SCHEDULED_FOR_CANCEL
                    || orderView.StatusCode == OrderStatusCode.STOCK_WARNING)
            {
                phItems.Visible = false;
                esnShipping.Visible = false;
            }
            else
            {
                phItems.Visible = true;
                esnShipping.Visible = true;
            }

            var GAItems = items.Where(i => i.StatusCode == LineStatusCode.GOODS_ALLOCATED).ToList();

            int weight = 0;
            decimal netValue = 0M;
            int quantityGA = GAItems.Select(i => i.Quantity).Sum();
            int quantityOrder = items.Select(i => i.Quantity).Sum();

            GAItems.ForEach(delegate (LineItemOverviewModel item)
            {
                weight += item.Weight * item.Quantity;
                netValue += (item.PriceExclTax * item.Quantity);
            });

            // Discount only if quantities match
            if (quantityOrder == quantityGA)
            {
                decimal result = netValue - orderView.DiscountValue;
                if (result > 0M) netValue = result;
            }

            ltlWeightGrams.Text = "<input id=\"cn22Weight\" class=\"form-control\" type=\"text\" value=\"" + weight + "\" onkeyup=\"getWeightInput(event)\" />";
            ltlNetValue.Text = "<input id=\"cn22Value\" class=\"form-control\" type=\"text\" value=\"" + orderView.CurrencyCode + " " + AdminStoreUtility.RoundPrice(netValue) + "\" onkeyup=\"getValueInput(event)\" />";

            if (GAItems.Count == 0)
            {
                lbSubmitTop.Visible = false;
                lbSubmitBottom.Visible = false;
            }

            if (charged)
            {
                lbProcessPaymentTop.Visible = false;
                phAfterPaymentTop.Visible = true;

                lbProcessPaymentBottom.Visible = false;
                phAfterPaymentBottom.Visible = true;
            }
            else
            {
                lbProcessPaymentTop.Visible = true;
                phAfterPaymentTop.Visible = false;

                lbProcessPaymentBottom.Visible = true;
                phAfterPaymentBottom.Visible = false;

                enbInfo.Message = "This order was not fully paid. Please process payment first before proceed to packing.";
            }

            // Shipping Option ID
            // Next Day Shipping Option ID = 2
            // Local Standard Shipping Option ID = 1

            if (shippingAddress.CountryId == ShippingSettings.PrimaryStoreCountryId)
            {
                ltlFirstClass.Text = "<input type=\"radio\" name=\"stampType\" value=\"firstClass\" checked=\"checked\" /> 1st class";
                ltlInternational.Text = "<input type=\"radio\" name=\"stampType\" value=\"international\" /> International";
            }
            else
            {
                ltlFirstClass.Text = "<input type=\"radio\" name=\"stampType\" value=\"firstClass\" /> 1st class";
                ltlInternational.Text = "<input type=\"radio\" name=\"stampType\" value=\"international\" checked=\"checked\" /> International";
            }

            ltlCN22Date.Text = DateTime.Now.ToString(AppConstant.DATE_FORM1);
            ltlSignature.Text = "<img src=\"/img/diana-sig.jpg\" alt=\"diana\" width=\"50\" />"; // Default
            var country = ShippingService.GetCountryById(shippingAddress.CountryId);
            ltlDeminimis.Text = country.DeminimisValue;

            ltOrderNumber.Text = string.Format("Order # {0}", orderId);
            ltOrderNumberReturn.Text = ltOrderNumber.Text;

            int profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
            string email = AccountService.GetEmailByProfileId(profileId);
            if (email.ToLower() == "flory_horgutza@yahoo.com")
            {
                ltlSignature.Text = "<img src=\"/img/flory-sig.jpg\" alt=\"flory\" width=\"50\" />";
            }
        }

        #region ICallbackEventHandler Members

        private string _alert = string.Empty;

        string ICallbackEventHandler.GetCallbackResult()
        {
            return _alert;
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            int count = 0;
            count = OrderService.GetOrderCountForSpecialDelivery();

            if (count > 0)
                _alert = "Alert! Next day delivery order received.";
            else
                _alert = string.Empty;
        }

        #endregion
    }
}