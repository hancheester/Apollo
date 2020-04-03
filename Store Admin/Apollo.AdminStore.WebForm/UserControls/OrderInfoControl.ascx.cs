using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderInfoControl : BaseUserControl
    {
        private const string SUBTRACT_FORM = "-{0}";
        private const string PENDING = "Pending";
        private const string CANCEL = "Cancel";
        private const string WAIT = "Wait";
        private const string UPDATE = "Update";
        private const string DELETE = "Delete";

        public IAccountService AccountService { get; set; }
        public IOrderService OrderService { get; set; }
        public IPaymentService PaymentService { get; set; }
        public IProductService ProductService { get; set; }
        public IShippingService ShippingService { get; set; }
        public IUtilityService UtilityService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public CurrencySettings CurrencySettings { get; set; }
        public MediaSettings MediaSettings { get; set; }

        public delegate void OrderInfoEventHandler(string message, bool refresh);
        public event OrderInfoEventHandler ActionOccurred;

        public bool NewCommentAreaVisible
        {
            set { phNewCommentSection.Visible = value; }
        }
        public bool CommentAreaVisible
        {
            set { phCommentSection.Visible = value; }
        }
        public bool SummaryAreaVisible
        {
            set { phSummarySection.Visible = value; }
        }

        protected int QueryOrderId
        {
            get { return ((BasePage)Page).QueryOrderId; }
        }
        protected int QueryRefundInfoId
        {
            get { return ((BasePage)Page).QueryRefundInfoId; }
        }
        
        protected override void OnInit(EventArgs e)
        {
            var items = OrderService.GetOrderIssueList().OrderBy(x => x.Issue).ToList();
            items.Insert(0, new OrderIssue
            {
                Issue = AppConstant.DEFAULT_SELECT,
                IssueCode = string.Empty
            });

            ddlIssue.DataSource = items;
            ddlIssue.DataBind();

            LoadOrderInfo();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lbSubmitAction_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            var order = OrderService.GetOrderOverviewModelById(orderId);

            string orderStatusCode = string.Empty;
            string orderStatus = string.Empty;
            string issue = string.Empty;
            string message = string.Empty;

            if (ddlOrderStatus.SelectedIndex > 0)
            {
                switch (ddlOrderStatus.SelectedValue)
                {
                    case OrderStatusCode.SHIPPING:
                        if ((order.EarnedPoint + order.AwardedPoint) > 0)
                        {
                            AccountService.InsertRewardPointHistory(
                                order.AccountId,
                                points: order.EarnedPoint + order.AwardedPoint,
                                message: string.Format("Earned points for order ID {0}.", orderId),
                                orderId: orderId);
                        }

                        orderStatusCode = ddlOrderStatus.SelectedValue;
                        break;
                    case OrderStatusCode.SHIPPING_NO_EARNED_POINT:
                        orderStatusCode = OrderStatusCode.SHIPPING;
                        break;
                    default:
                        orderStatusCode = ddlOrderStatus.SelectedValue;
                        break;
                }

                orderStatus = OrderService.GetOrderStatusByCode(orderStatusCode);
                OrderService.UpdateOrderStatusCodeByOrderId(orderId, orderStatusCode);
            }
            else
                orderStatus = OrderService.GetOrderStatusByCode(order.StatusCode);

            if (ddlIssue.SelectedIndex > 0)
            {
                issue = ", " + ddlIssue.SelectedItem.Text;
                OrderService.UpdateOrderIssueCodeByOrderId(orderId, ddlIssue.SelectedValue);
            }
            else
                OrderService.UpdateOrderIssueCodeByOrderId(orderId, ddlIssue.SelectedValue);

            if (cbResetAlert.Checked)
            {
                OrderService.UpdateOrderLastAlertDateByOrderId(orderId, DateTime.Now);

                message = "Alert is reset.";
            }

            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),                                                    
                                                    orderStatus + issue,
                                                    message,
                                                    txtComment.Text.Trim());

            OrderService.UpdateOrderLastActivityDateByOrderId(orderId, DateTime.Now);
            
            txtComment.Text = string.Empty;
            cbResetAlert.Checked = false;

            // Display message
            InvokeNewMessage("Order status / issue / comment was updated successfully.", true);
        }

        protected void lbAuthorise_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;
            var orderView = OrderService.GetOrderOverviewModelById(orderId);
            decimal amount = OrderService.CalculateOrderTotalByOrderId(orderId);

            OrderPayment payment = new OrderPayment
            {
                OrderId = orderId,
                Amount = amount / orderView.ExchangeRate,
                CurrencyCode = orderView.CurrencyCode,
                ExchangeRate = orderView.ExchangeRate,
                TimeStamp = DateTime.Now
            };
            
            var output = PaymentService.ProcessOrderCharging(payment);
            var profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
            var fullName = Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName"));

            if (output.Status)
            {
                var message = " Release amount: " + AdminStoreUtility.GetFormattedPrice(amount, payment.CurrencyCode.ToUpper(), CurrencyType.Code);

                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        profileId,
                                                        fullName,
                                                        "Payment",
                                                        "Payment was authorised successfully. " + message,
                                                        string.Empty);
                
                // Display message and refresh
                InvokeNewMessage("Order was charged successfully.", true);
            }
            else
            {
                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        profileId,
                                                        fullName,
                                                        "Payment",
                                                        "Payment was FAILED to authorise. " + output.Message,
                                                        string.Empty);

                OrderService.UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.ON_HOLD);
                OrderService.UpdateOrderIssueCodeByOrderId(orderId, OrderIssueCode.FAILED_TO_CHARGE);

                InvokeNewMessage("Order was FAILED to charge. " + output.Message, true);
            }
        }

        protected void lbAddItem_Click(object sender, EventArgs e)
        {
            phAddItemGrid.Visible = true;
            phAddItem.Visible = false;
            phFinishAddingItem.Visible = true;

            gvProducts.Visible = true;
            LoadProducts();
        }

        protected void lbFinishAddingItem_Click(object sender, EventArgs e)
        {
            phAddItemGrid.Visible = false;
            phAddItem.Visible = true;
            phFinishAddingItem.Visible = false;
        }

        protected void eavShipping_Changed(string oldAddr, Address newAddr)
        {
            int orderId = QueryOrderId;
            OrderService.UpdateOrderShippingAddressByOrderId(orderId, newAddr);

            OrderService.ProcessOrderCommentInsertion(QueryOrderId,
                                                   Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                   Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                   "Edit Shipping Address",
                                                   "Shipping address was changed successfully.<br/><br/>Old shipping address:<br/>" + oldAddr,
                                                   string.Empty);

            // Display message
            InvokeNewMessage("Shipping address was changed successfully.", true);
        }

        protected void eavBilling_Changed(string oldAddr, Address newAddr)
        {
            OrderService.UpdateOrderBillingAddressByOrderId(QueryOrderId, newAddr);

            OrderService.ProcessOrderCommentInsertion(QueryOrderId,
                                                   Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                   Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                   "Edit Billing Address",
                                                   "Billing address was changed successfully.<br/><br/>Old billing address:<br/>" + oldAddr,
                                                   string.Empty);

            // Display message and refresh
            InvokeNewMessage("Billing address was changed successfully.", true);
        }

        protected void esvShipping_ShippingChanged(string oldNote, string newNote, decimal oldCost, decimal newCost, int oldShippingId, int newShippingId)
        {
            int orderId = QueryOrderId;

            if (oldNote != newNote)
            {
                OrderService.UpdateOrderPackingByOrderId(orderId, newNote);

                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                        Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                        "Edit Packing Note",
                                                        "Packing note was changed successfully.<br/><br/>Old packing note:<br/>" + oldNote,
                                                        string.Empty);

                // Display message and refresh
                InvokeNewMessage("Packing note was changed successfully.", true);
            }

            if (oldCost != newCost)
            {
                var order = OrderService.GetOrderOverviewModelById(orderId);

                if (order != null)
                {
                    // Convert to GBP first as GBP is a base currency in database
                    newCost = newCost / order.ExchangeRate;
                    OrderService.UpdateOrderShippingCostByOrderId(orderId, newCost);

                    OrderService.ProcessOrderCommentInsertion(orderId,
                                                            Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                            Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                            "Edit Shipping Cost",
                                                            "Shipping cost was changed successfully.<br/><br/>Old shipping cost:<br/>" + oldCost.ToString(),
                                                            string.Empty);

                    // Display message and refresh
                    InvokeNewMessage("Shipping cost was changed successfully.", true);
                }
                else
                {
                    // Display message and refresh
                    InvokeNewMessage("Shipping cost was FAILED to change. Order cannot be found.", true);
                }
            }

            if (oldShippingId != newShippingId)
            {
                OrderService.UpdateOrderShippingOptionIdByOrderId(orderId, newShippingId);

                ShippingOption oldShipping = ShippingService.GetShippingOptionById(oldShippingId);

                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                        Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                        "Edit Shipping Option",
                                                        "Shipping option was changed successfully.<br/><br/>Old shipping option:<br/>" + oldShipping.Name,
                                                        string.Empty);

                // Display message and refresh
                InvokeNewMessage("Shipping option was changed successfully.", true);
            }
        }

        protected void eovOffer_OfferChanged(decimal oldDiscount, decimal newDiscount)
        {
            if (oldDiscount != newDiscount)
            {
                int orderId = QueryOrderId;
                var order = OrderService.GetOrderOverviewModelById(orderId);
                
                OrderService.UpdateOrderDiscountByOrderId(orderId, newDiscount / order.ExchangeRate);

                OrderService.ProcessOrderCommentInsertion(orderId,
                                                          Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                          Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                          "Edit Discount",
                                                          "Discount was changed successfully.<br/><br/>Old discount:<br/>" + oldDiscount.ToString(),
                                                          string.Empty);

                // Display message and refresh
                InvokeNewMessage("Discount was changed successfully.", true);
            }
        }

        protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as LineItemOverviewModel;
                
                DropDownList ddlLineStatus = AdminStoreUtility.FindControlRecursive(e.Item, "ddlLineStatus") as DropDownList;
                if (ddlLineStatus != null)
                {
                    ddlLineStatus.DataTextField = "Status";
                    ddlLineStatus.DataValueField = "StatusCode";
                    ddlLineStatus.DataSource = OrderService.GetLineStatusList();                    
                    ddlLineStatus.DataBind();

                    var found = ddlLineStatus.Items.FindByValue(item.StatusCode);
                    if (found != null)
                        found.Selected = true;
                    else
                        InvokeNewMessage("Item contains no status. Please assign appropriate status for this item. Item name is " + item.Name + ".", false);

                }

                DropDownList ddlQuantity = e.Item.FindControl("ddlQuantity") as DropDownList;
                if (ddlQuantity != null)
                {
                    // To get the range, formula is Ordered qty - Shipped qty
                    int pendingRange = item.Quantity - item.ShippedQuantity;

                    for (int i = 1; i <= pendingRange; i++)
                    {
                        ddlQuantity.Items.Add(i.ToString());
                    }
                }

                Repeater rptOptions  = e.Item.FindControl("rptOptions") as Repeater;
                if (rptOptions != null)
                {
                    var prices = ProductService.GetProductPricesByProductId(item.ProductId);
                    rptOptions.DataSource = prices;
                    rptOptions.DataBind();
                }
            }
        }

        protected void rptItems_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            int orderId = QueryOrderId;
            int lineItemId = 0;
            decimal priceInclTax = 0M;
            int orderedQty = 0;
            int pendingQty = 0;
            int allocatedQty = 0;
            LineItem item;
            StringBuilder sb;
            var profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
            var fullName = Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName"));

            switch (e.CommandName)
            {
                case UPDATE:
                    if (!int.TryParse(e.CommandArgument.ToString(), out lineItemId)) break;

                    item = OrderService.GetLineItemById(lineItemId);

                    DropDownList ddlLineStatus = AdminStoreUtility.FindControlRecursive(e.Item, "ddlLineStatus") as DropDownList;
                    TextBox txtRetail = AdminStoreUtility.FindControlRecursive(e.Item, "txtRetail") as TextBox;
                    CheckBox cbFreeWrapped = AdminStoreUtility.FindControlRecursive(e.Item, "cbFreeWrapped") as CheckBox;
                    TextBox txtOrdered = AdminStoreUtility.FindControlRecursive(e.Item, "txtOrdered") as TextBox;
                    TextBox txtPending = AdminStoreUtility.FindControlRecursive(e.Item, "txtPending") as TextBox;
                    TextBox txtInTransit = AdminStoreUtility.FindControlRecursive(e.Item, "txtInTransit") as TextBox;
                    TextBox txtAllocated = AdminStoreUtility.FindControlRecursive(e.Item, "txtAllocated") as TextBox;
                    TextBox txtOption = AdminStoreUtility.FindControlRecursive(e.Item, "txtOption") as TextBox;
                    TextBox txtNote = AdminStoreUtility.FindControlRecursive(e.Item, "txtNote") as TextBox;

                    var sbComment = new StringBuilder();

                    #region Note

                    var note = txtNote.Text.Trim();
                    if (item.Note != note)
                    {
                        sbComment.AppendFormat("Note <i>{0}</i> -> <i>{1}</i><br/>", item.Note, note);
                        OrderService.UpdateLineItemNoteByLineItemId(lineItemId, note);
                    }

                    #endregion

                    #region Line Status

                    if (item.StatusCode != ddlLineStatus.SelectedValue)
                    {
                        sbComment.AppendFormat("Line Status <i>{0}</i> -> <i>{1}</i><br/>", OrderService.GetLineStatusByCode(item.StatusCode), OrderService.GetLineStatusByCode(ddlLineStatus.SelectedValue));
                        OrderService.UpdateLineItemStatusCodeByLineItemId(lineItemId, ddlLineStatus.SelectedValue);
                    }

                    if (ddlLineStatus.SelectedValue == LineStatusCode.CANCELLED)
                    {
                        // Remove from branch allocation
                        OrderService.DeleteBranchItemAllocationByLineItemId(lineItemId);

                        // Remove item (if any) from warehouse allocation
                        OrderService.DeleteWarehouseAllocationByLineItemId(item.Id);

                        sbComment.Append("The item was removed from Line Distribution as it has line status <i>Cancelled</i>.");
                    }

                    #endregion

                    #region Option

                    if (item.Option != txtOption.Text.Trim())
                    {
                        sbComment.AppendFormat("Option <i>{0}</i> -> <i>{1}</i><br/>", item.Option, txtOption.Text.Trim());
                        OrderService.UpdateLineItemOptionByLineItemId(lineItemId, txtOption.Text.Trim());
                    }

                    #endregion

                    #region Retail

                    if (decimal.TryParse(txtRetail.Text.Trim(), out priceInclTax) == false)
                    {
                        InvokeNewMessage("Line item was failed to update. Retail price was invalid.", true);
                        break;
                    }

                    // Convert to GBP
                    priceInclTax = AdminStoreUtility.RoundPrice(priceInclTax / item.ExchangeRate);
                    var priceExclTax = item.PriceExclTax;

                    if (item.Product != null && item.Product.TaxCategory != null && item.Product.TaxCategory.Rate > 0)
                        priceExclTax = priceInclTax / (100 + item.Product.TaxCategory.Rate) * 100;
                    else
                        priceExclTax = priceInclTax;

                    OrderService.UpdateLineItemPriceByLineItemId(lineItemId, priceInclTax, priceExclTax);

                    if (item.PriceInclTax.CompareTo(priceInclTax) != 0)
                        sbComment.AppendFormat("Retail Price <i>{2} {0:0.00}</i> -> <i>{2} {1:0.00}</i><br/>",
                            AdminStoreUtility.RoundPrice(item.PriceInclTax * item.ExchangeRate),
                            AdminStoreUtility.RoundPrice(priceInclTax * item.ExchangeRate),
                            item.CurrencyCode);

                    #endregion

                    #region Giftwrap

                    if (item.Wrapped != cbFreeWrapped.Checked)
                    {
                        sbComment.AppendFormat("Free Wrapped <i>{0}</i> -> <i>{1}</i><br/>", item.Wrapped, cbFreeWrapped.Checked);
                        OrderService.UpdateLineItemFreeWrappedByLineItemId(lineItemId, cbFreeWrapped.Checked);
                    }

                    #endregion
                    
                    #region Quantity

                    #region Ordered
                    if (int.TryParse(txtOrdered.Text.Trim(), out orderedQty) == false)
                    {
                        InvokeNewMessage("Line item was failed to update. Ordered quantity was invalid.", true);
                        break;
                    }
                    OrderService.UpdateLineItemQuantityByLineItemId(lineItemId, orderedQty);

                    if (item.Quantity.CompareTo(orderedQty) != 0)
                        sbComment.AppendFormat("Ordered Quantity <i>{0}</i> -> <i>{1}</i><br/>", item.Quantity, txtOrdered.Text.Trim());
                    #endregion

                    #region Pending
                    if (int.TryParse(txtPending.Text.Trim(), out pendingQty) == false)
                    {
                        InvokeNewMessage("Line item was failed to update. Pending quantity was invalid.", true);
                        break;
                    }
                    OrderService.UpdateLineItemPendingQuantity(lineItemId, pendingQty);

                    if (item.PendingQuantity.CompareTo(pendingQty) != 0)
                        sbComment.AppendFormat("Pending Quantity <i>{0}</i> -> <i>{1}</i><br/>", item.PendingQuantity, txtPending.Text.Trim());
                    #endregion

                    #region In Transit

                    //if (item.IntransitQuantity.ToString() != txtInTransit.Text.Trim())
                    //    sbComment.AppendFormat("In Transit Quantity <i>{0}</i> -> <i>{1}</i><br/>", item.IntransitQuantity, txtInTransit.Text.Trim());
                    //int.TryParse(txtInTransit.Text.Trim(), out inTransitQty);
                    //OrderAgent.UpdateLineItemInTransitQuantity(lineItemId, inTransitQty);

                    #endregion

                    #region Allocated
                    if (int.TryParse(txtAllocated.Text.Trim(), out allocatedQty) == false)
                    {
                        InvokeNewMessage("Line item was failed to update. Allocated quantity was invalid.", true);
                        break;
                    }
                    OrderService.UpdateLineItemAllocatedQuantity(lineItemId, allocatedQty);

                    if (item.AllocatedQuantity.CompareTo(allocatedQty) != 0)
                        sbComment.AppendFormat("Allocated Quantity <i>{0}</i> -> <i>{1}</i><br/>", item.AllocatedQuantity, txtAllocated.Text.Trim());

                    #endregion

                    #endregion

                    OrderService.ResetOrderStatusAccordingToLineStatus(orderId, profileId, fullName);

                    if (string.IsNullOrEmpty(sbComment.ToString()) == false)
                    {
                        // Save to comments
                        sb = new StringBuilder("Line item was updated successfully.<br/><br/>");
                        sb.Append("<u>" + item.Name).Append(AppConstant.SPACE).Append(item.Option).Append("</u>");
                        sb.AppendFormat("<br/>" + sbComment.ToString());

                        OrderService.ProcessOrderCommentInsertion(orderId,
                                                                profileId,
                                                                fullName,
                                                                "Line Item",
                                                                sb.ToString(),
                                                                string.Empty);

                        // Display message
                        InvokeNewMessage("Line item was updated successfully.", true);
                    }
                    break;

                case WAIT:
                    if (!int.TryParse(e.CommandArgument.ToString(), out lineItemId)) break;

                    OrderService.UpdateLineItemStatusCodeByLineItemId(lineItemId, LineStatusCode.PENDING);

                    // Remove from branch allocation
                    OrderService.DeleteBranchItemAllocationByLineItemId(lineItemId);

                    // Save to comments
                    sb = new StringBuilder("Line item was updated successfully.<br/><br/>");

                    item = OrderService.GetLineItemById(lineItemId);
                    sb.Append(item.Name).Append(AppConstant.SPACE).Append(item.Option).Append(AppConstant.SPACE);

                    OrderService.ProcessOrderCommentInsertion(orderId,
                                                            profileId,
                                                            fullName,
                                                            "Wait Line Item",
                                                            sb.ToString(),
                                                            string.Empty);

                    // Display message
                    InvokeNewMessage("Line item was updated successfully.", true);
                    break;

                case CANCEL:
                    if (!int.TryParse(e.CommandArgument.ToString(), out lineItemId)) break;

                    OrderService.UpdateLineItemStatusCodeByLineItemId(lineItemId, LineStatusCode.CANCELLED);

                    // Remove from branch allocation
                    OrderService.DeleteBranchItemAllocationByLineItemId(lineItemId);

                    // Save to comments
                    sb = new StringBuilder("Line item was cancelled successfully.<br/><br/>");

                    item = OrderService.GetLineItemById(lineItemId);
                    sb.Append(item.Name).Append(AppConstant.SPACE).Append(item.Option).Append(AppConstant.SPACE);

                    OrderService.ProcessOrderCommentInsertion(orderId,
                                                            profileId,
                                                            fullName,
                                                            "Cancel Line Item",
                                                            sb.ToString(),
                                                            string.Empty);
                    // Display message
                    InvokeNewMessage("Line item was cancelled successfully.", true);
                    break;

                case DELETE:
                    if (!int.TryParse(e.CommandArgument.ToString(), out lineItemId)) break;

                    // Save to comments
                    sb = new StringBuilder("Line item was removed successfully.<br/><br/>");
                    item = OrderService.GetLineItemById(lineItemId);
                    sb.Append(item.Name).Append(AppConstant.SPACE).Append(item.Option).Append(AppConstant.SPACE);

                    // Remove from branch allocation
                    OrderService.DeleteBranchItemAllocationByLineItemId(lineItemId);

                    // Remove item from the order
                    OrderService.DeleteLineItem(lineItemId);

                    OrderService.ProcessOrderCommentInsertion(orderId,
                                                            profileId,
                                                            fullName,
                                                            "Delete Line Item",
                                                            sb.ToString(),
                                                            string.Empty);

                    // Display message
                    InvokeNewMessage("Line item was removed successfully.", true);
                    break;
            }

            LoadOrderInfo();
        }

        protected void eavAccount_Verified(SysCheckType checkType, bool verified)
        {
            int orderId = QueryOrderId;

            string type = string.Empty;
            string msg = string.Empty;

            var check = OrderService.GetSystemCheckByOrderId(orderId);

            switch (checkType)
            {
                case SysCheckType.Email:
                    type = "Email";
                    check.EmailCheck = true;
                    break;

                case SysCheckType.Name:
                    type = "Name";
                    check.NameCheck = true;
                    break;
                default:
                    break;
            }

            if (verified)
            {
                OrderService.UpdateSystemCheck(check);
                msg = string.Format("{0} was verified.", type);
            }
            else
            {
                msg = string.Format("{0} was FAILED to verify.", type);
            }

            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Verify " + type,
                                                    msg,
                                                    string.Empty);

            OrderService.ProcessSystemCheckVerification(orderId);

            // Display message and refresh
            InvokeNewMessage(msg, true);
        }

        protected void epvPayment_Verified(bool verified)
        {
            int orderId = QueryOrderId;
            string msg = "AVS was FAILED to verify.";

            if (verified)
            {
                SystemCheck check = OrderService.GetSystemCheckByOrderId(orderId);
                check.AvsCheck = true;

                OrderService.UpdateSystemCheck(check);

                msg = "AVS was verified.";
            }

            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Verify AVS",
                                                    msg,
                                                    string.Empty);

            OrderService.ProcessSystemCheckVerification(orderId);

            // Display message and refresh
            InvokeNewMessage(msg, true);
        }

        protected void epvPayment_PaidByPhone(string paymentRef)
        {
            OrderService.ProcessOrderCommentInsertion(QueryOrderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    PaymentType.PAID_BY_PHONE,
                                                    "Order was paid by phone successfully. Transaction ID is " + paymentRef + ".",
                                                    string.Empty);

            // Display message and refresh
            InvokeNewMessage("Order was paid by phone successfully.", true);
        }
        
        protected void eavBilling_Verified(AddressType addrType, SysCheckType checkType, bool verified)
        {
            int orderId = QueryOrderId;
            string message = string.Empty;
            string type = string.Empty;

            SystemCheck check = OrderService.GetSystemCheckByOrderId(orderId);

            switch (checkType)
            {
                case SysCheckType.Name:
                    type = "Billing Name";
                    check.BillingNameCheck = true;
                    break;

                case SysCheckType.Address:
                    type = "Billing Address";
                    check.BillingAddressCheck = true;
                    break;

                case SysCheckType.PostCode:
                    type = "Billing PostCode";
                    check.BillingPostCodeCheck = true;
                    break;

                default:
                    break;
            }

            if (verified)
            {
                OrderService.UpdateSystemCheck(check);
                message = string.Format("{0} was verified.", type);
            }
            else
            {
                message = string.Format("{0} was FAILED to verify.", type);
            }

            OrderService.ProcessSystemCheckVerification(orderId);

            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Verify " + type,
                                                    message,
                                                    string.Empty);

            // Display message and refresh
            InvokeNewMessage(message, true);
        }
        
        protected void eavShipping_Verified(AddressType addrType, SysCheckType checkType, bool verified)
        {
            int orderId = QueryOrderId;
            string message = string.Empty;
            string type = string.Empty;

            SystemCheck check = OrderService.GetSystemCheckByOrderId(orderId);

            switch (checkType)
            {
                case SysCheckType.Name:
                    type = "Shipping Name";
                    check.ShippingNameCheck = true;
                    break;

                case SysCheckType.Address:
                    type = "Shipping Address";
                    check.ShippingAddressCheck = true;
                    break;

                case SysCheckType.PostCode:
                    type = "Shipping PostCode";
                    check.ShippingPostCodeCheck = true;
                    break;

                default:
                    break;
            }

            if (verified)
            {
                OrderService.UpdateSystemCheck(check);
                message = string.Format("{0} was verified.", type);
            }
            else
            {
                message = string.Format("{0} was FAILED to verify.", type);
            }

            OrderService.ProcessSystemCheckVerification(orderId);

            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Verify " + type,
                                                    message,
                                                    string.Empty);

            // Display message and refresh
            InvokeNewMessage(message, true);
        }

        protected void elvLoyalty_LoyaltyChanged(int oldAllocatedPoint, int newAllocatedPoint, int oldEarnedPoint, int newEarnedPoint)
        {
            int orderId = QueryOrderId;
            int profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
            string fullName = Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName"));

            if (oldAllocatedPoint != newAllocatedPoint)
            {
                OrderService.UpdateOrderAllocatedPoint(orderId, newAllocatedPoint);

                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        profileId,
                                                        fullName,
                                                        "Edit Loyalty Information",
                                                        "Loyalty information was changed successfully.<br/><br/>Old allocated points:<br/>" + oldAllocatedPoint.ToString(),
                                                        string.Empty);
            }

            if (oldEarnedPoint != newEarnedPoint)
            {
                OrderService.UpdateOrderEarnedPoint(orderId, newEarnedPoint);

                OrderService.ProcessOrderCommentInsertion(orderId,
                                                        profileId,
                                                        fullName,
                                                        "Edit Loyalty Information",
                                                        "Loyalty information was changed successfully.<br/><br/>Old earned points:<br/>" + oldEarnedPoint.ToString(),
                                                        string.Empty);
            }

            // Display message and refresh
            InvokeNewMessage("Loyalty information was changed successfully.", true);
        }
        
        protected bool HasMoreComments()
        {
            var comments = OrderService.GetOrderCommentOverviewModelListByOrderId(QueryOrderId, AppConstant.SHOW_MAX_COMMENT + 1);

            return AppConstant.SHOW_MAX_COMMENT < comments.Count;
        }

        protected bool HasDiscountOffer()
        {
            var order = OrderService.GetOrderOverviewModelById(QueryOrderId);

            if (order != null)
            {
                return order.DiscountValue > 0;
            }

            return false;
        }

        protected string CheckIfDifferentFromRRP(string currencyCode, decimal exchangeRate, decimal rrp, decimal priceInclTax)
        {
            if (rrp > 0M && rrp != priceInclTax)
                return string.Format("<span class='text-danger clearfix'><strong>was {0}, now {1}</strong></span>",
                    AdminStoreUtility.GetFormattedPrice(rrp, currencyCode, CurrencyType.HtmlEntity, exchangeRate),
                    AdminStoreUtility.GetFormattedPrice(priceInclTax, currencyCode, CurrencyType.HtmlEntity, exchangeRate));

            return string.Empty;
        }

        public void LoadOrderInfo()
        {
            int orderId = QueryOrderId;
            var orderView = OrderService.GetOrderOverviewModelById(orderId);

            if (orderView != null)
            {
                eohHeader.OrderId = orderId;
                eavAccount.ProfileId = orderView.ProfileId;
                eavAccount.OrderId = orderId;
                eavBiling.OrderId = orderId;
                eavShipping.OrderId = orderId;
                epvPayment.OrderId = orderId;
                esvShipping.OrderId = orderId;
                eovOffer.OrderId = orderId;
                elvLoyalty.OrderId = orderId;
                escSystemCheck.Order = orderView;

                var orderItems = OrderService.GetLineItemOverviewModelListByOrderId(orderId);
                rptItems.DataSource = orderItems;
                rptItems.DataBind();

                rptComments.DataSource = OrderService.GetOrderCommentOverviewModelListByOrderId(orderId, AppConstant.SHOW_MAX_COMMENT);
                rptComments.DataBind();

                decimal subTotal = orderItems
                    .Where(i => i.StatusCode != LineStatusCode.CANCELLED)
                    .Select(i => i.Quantity * i.PriceExclTax * i.ExchangeRate)
                    .Sum();                               
                decimal grandTotal = OrderService.CalculateOrderTotalByOrderId(orderId);
                decimal refundTotal = OrderService.CalculateTotalRefundedAmountByOrderId(orderId);
                decimal totalPaid = OrderService.CalculateTotalPaidAmountByOrderId(orderId);
                
                var country = ShippingService.GetCountryById(orderView.ShippingCountryId);

                decimal vat = 0M;
                if (country.IsEC)                
                    vat = OrderService.CalculateVATByOrderId(orderId);
                
                ltlSubtotal.Text = AdminStoreUtility.GetFormattedPrice(subTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity);
                ltlTax.Text = AdminStoreUtility.GetFormattedPrice(vat, orderView.CurrencyCode, CurrencyType.HtmlEntity);
                ltlShipping.Text = AdminStoreUtility.GetFormattedPrice(orderView.ShippingCost, orderView.CurrencyCode, CurrencyType.HtmlEntity, orderView.ExchangeRate);
                ltlDiscount.Text = string.Format(SUBTRACT_FORM, AdminStoreUtility.GetFormattedPrice(orderView.DiscountValue, orderView.CurrencyCode, CurrencyType.HtmlEntity, orderView.ExchangeRate));
                ltlLoyaltyPoint.Text = string.Format(SUBTRACT_FORM, AdminStoreUtility.GetFormattedPrice(AdminStoreUtility.RoundPrice(orderView.AllocatedPoint / 100M), orderView.CurrencyCode, CurrencyType.HtmlEntity, orderView.ExchangeRate));
                ltlGrandTotal.Text = AdminStoreUtility.GetFormattedPrice(grandTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity);
                ltlTotalPaid.Text = AdminStoreUtility.GetFormattedPrice(totalPaid, orderView.CurrencyCode, CurrencyType.HtmlEntity);
                ltlTotalRefund.Text = string.Format(SUBTRACT_FORM, AdminStoreUtility.GetFormattedPrice(refundTotal, orderView.CurrencyCode, CurrencyType.HtmlEntity));

                phAddItemGrid.Visible = false;
                phFinishAddingItem.Visible = false;
                phAddItem.Visible = true;
            }
        }

        public void Refresh()
        {
            LoadOrderInfo();
        }

        #region Line item management

        private const string DT_OPTION_NAME_FORMAT_CURRENCY = "{0}{1:0.00} - {2}, {3} in stock";
        private const string DT_OPTION_NAME_FORMAT_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {5} in stock";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY = "{0}{1:0.00} - {2}, {5} in stock, was {3}{4:0.00}";
        private const string DT_OPTION_NAME_FORMAT_OFFER_CURRENCY_WITH_ORIGINAL = "{0}{1:0.00} ({2}{3:0.00}) - {4}, {9} in stock, was {5}{6:0.00} ({7}{8:0.00})";

        private void LoadProducts()
        {
            int[] productIds = null;
            string name = null;
            ProductSortingType orderBy = ProductSortingType.NameAsc;

            if (((BasePage)this.Page).HasState("product_id"))
            {
                string value = ((BasePage)this.Page).GetStringState("product_id");
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();

                ((TextBox)gvProducts.HeaderRow.FindControl("txtProductIdFilter")).Text = value;
            }

            if (((BasePage)this.Page).HasState("product_name"))
            {
                name = ((BasePage)this.Page).GetStringState("product_name");
                ((TextBox)gvProducts.HeaderRow.FindControl("txtProductNameFilter")).Text = name;
            }

            if (((BasePage)this.Page).HasState("OrderBy")) orderBy = (ProductSortingType)((BasePage)this.Page).GetIntState("OrderBy");
                        
            var result = ProductService.GetPagedProductOverviewModel(
                pageIndex: gvProducts.CustomPageIndex,
                pageSize: gvProducts.PageSize,
                productIds: productIds,
                keywords: name,
                orderBy: orderBy);

            if (result != null)
            {
                gvProducts.DataSource = result.Items;
                gvProducts.RecordCount = result.TotalCount;
                gvProducts.CustomPageCount = result.TotalPages;
            }

            gvProducts.DataBind();
        }
        protected void lbSearchProduct_Click(object sender, EventArgs e)
        {
            ((BasePage)this.Page).SetState("product_id", ((TextBox)gvProducts.HeaderRow.FindControl("txtProductIdFilter")).Text.Trim());
            ((BasePage)this.Page).SetState("product_name", ((TextBox)gvProducts.HeaderRow.FindControl("txtProductNameFilter")).Text.Trim());

            LoadProducts();
        }
        protected void gvProducts_PreRender(object sender, EventArgs e)
        {
            if (gvProducts.TopPagerRow != null)
            {
                gvProducts.TopPagerRow.Visible = true;
                ((TextBox)gvProducts.HeaderRow.FindControl("txtProductIdFilter")).Text = ((BasePage)this.Page).GetStringState("product_id");
                ((TextBox)gvProducts.HeaderRow.FindControl("txtProductNameFilter")).Text = ((BasePage)this.Page).GetStringState("product_name");
            }
        }
        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var product = (ProductOverviewModel)e.Row.DataItem;
                if (product.Id > 0)
                {
                    TextBox txtQty = e.Row.FindControl("txtQty") as TextBox;

                    BuildAddItemRow(e.Row, product.Id);

                    var productPrices = ProductService.GetProductPricesByProductId(product.Id);

                    // If product doesn't have any options, hide it
                    if (productPrices.Count == 0)
                    {
                        var lbAddToOrder = e.Row.FindControl("lbAddToOrder") as LinkButton;

                        lbAddToOrder.Visible = false;
                        txtQty.Visible = false;
                    }
                }
            }
        }
        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.CustomPageIndex = gvProducts.CustomPageIndex + e.NewPageIndex;

            if (gvProducts.CustomPageIndex < 0)
                gvProducts.CustomPageIndex = 0;

            LoadProducts();
        }
        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int orderId = QueryOrderId;

            switch (e.CommandName)
            {
                case "addProduct":

                    GridViewRow row = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                    DropDownList ddlOptions = (DropDownList)row.FindControl("ddlOptions");
                    HiddenField hdnSingleOptionId = (HiddenField)row.FindControl("hdnSingleOptionId");
                    TextBox txtQty = (TextBox)row.FindControl("txtQty");

                    int qtyToAdd;
                    string message = string.Empty;

                    if (int.TryParse(txtQty.Text, out qtyToAdd))
                    {
                        if (qtyToAdd > 0)
                        {
                            int productId = Convert.ToInt32(e.CommandArgument);
                            var currencyCode = OrderService.GetCurrencyCodeByOrderId(orderId);
                            var product = ProductService.GetProductById(productId);
                            int selectedProductPriceId = 0;

                            if (product.ProductPrices.Count > 1)
                                selectedProductPriceId = Convert.ToInt32(ddlOptions.SelectedValue);
                            else if (product.ProductPrices.Count == 1)
                                selectedProductPriceId = Convert.ToInt32(hdnSingleOptionId.Value);

                            var selectedProductPrice = product.ProductPrices.Where(pp => pp.Id == selectedProductPriceId).FirstOrDefault();
                            var currency = UtilityService.GetCurrencyByCurrencyCode(currencyCode);

                            var newItem = new LineItem
                            {
                                OrderId = orderId,
                                ProductPriceId = selectedProductPrice.Id,
                                ProductId = product.Id,
                                Name = product.Name,
                                Option = selectedProductPrice.Option,
                                PriceExclTax = selectedProductPrice.OfferPriceExclTax,
                                PriceInclTax = selectedProductPrice.OfferPriceInclTax,
                                Quantity = qtyToAdd,
                                Wrapped = false,
                                Note = selectedProductPrice.Note,
                                IsPharmaceutical = product.IsPharmaceutical,
                                StatusCode = LineStatusCode.ORDERED,
                                Weight = selectedProductPrice.Weight,
                                CostPrice = selectedProductPrice.CostPrice,
                                CurrencyCode = currency.CurrencyCode,
                                ExchangeRate = currency.ExchangeRate
                            };

                            OrderService.InsertLineItem(newItem);

                            // Save to comments
                            StringBuilder sb = new StringBuilder("List item was added successfully.<br/><br/>");
                            sb.Append(newItem.Name).Append(AppConstant.SPACE).Append(newItem.Option).Append(AppConstant.SPACE);
                            
                            OrderService.ProcessOrderCommentInsertion(orderId,
                                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                                    "Add Line Item",
                                                                    sb.ToString(),
                                                                    string.Empty);

                            message = "Item is added successfully.";

                            gvProducts.Visible = false;

                            phFinishAddingItem.Visible = false;
                            phAddItem.Visible = true;
                        }
                        else
                        {
                            message = "Please enter a quantity to add which is greater than 0.";
                        }
                    }
                    else
                    {
                        message = "Please enter a whole, numerical quanity.";
                    }

                    InvokeNewMessage(message, true);

                    break;
                default:
                    break;
            }
        }
        private void BuildAddItemRow(GridViewRow row, int productId)
        {
            DropDownList ddlOptions = (DropDownList)row.FindControl("ddlOptions");
            Literal litSingleOption = (Literal)row.FindControl("litSingleOption");
            HiddenField hdnSingleOptionId = (HiddenField)row.FindControl("hdnSingleOptionId");

            DataTable dt = new DataTable();
            dt.Columns.Add("ProductPriceId");
            dt.Columns.Add("OptionName");

            var currencyCode = OrderService.GetCurrencyCodeByOrderId(QueryOrderId);
            var product = ProductService.GetProductById(productId);

            for (int i = 0; i < product.ProductPrices.Count; i++)
            {
                DataRow option = dt.NewRow();
                option["ProductPriceId"] = product.ProductPrices[i].Id;                
                option["OptionName"] = BuildOptionName(product.ProductPrices[i], currencyCode, product.ProductPrices[i].Option);

                dt.Rows.Add(option);
            }

            if (dt.Rows.Count > 1)
            {
                ddlOptions.DataSource = dt;
                ddlOptions.DataTextField = "OptionName";
                ddlOptions.DataValueField = "ProductPriceId";
                ddlOptions.DataBind();

                litSingleOption.Visible = false;
                hdnSingleOptionId.Visible = false;
            }
            else if (dt.Rows.Count == 1)
            {
                litSingleOption.Text = Convert.ToString(dt.Rows[0]["OptionName"]);
                hdnSingleOptionId.Value = Convert.ToString(dt.Rows[0]["ProductPriceId"]);
                ddlOptions.Visible = false;
            }
            else
            {
                ddlOptions.Visible = false;
                litSingleOption.Visible = false;
                hdnSingleOptionId.Visible = false;
            }
        }
        private string BuildOptionName(ProductPrice priceOption, string currencyCode, string option)
        {
            string optionName = string.Empty;
            var defaultCurrency = UtilityService.GetCurrency(CurrencySettings.PrimaryStoreCurrencyId);

            if (priceOption.OfferRuleId != 0)
            {
                if (currencyCode == CurrencySettings.PrimaryStoreCurrencyCode)
                {
                    optionName = string.Format(DT_OPTION_NAME_FORMAT_OFFER_CURRENCY,
                                               currencyCode,
                                               priceOption.OfferPriceExclTax,
                                               option,
                                               currencyCode,
                                               priceOption.PriceExclTax,
                                               priceOption.Stock);
                }
                else
                {
                    optionName = string.Format(DT_OPTION_NAME_FORMAT_OFFER_CURRENCY_WITH_ORIGINAL,
                                               currencyCode,
                                               priceOption.OfferPriceExclTax,
                                               CurrencySettings.PrimaryStoreCurrencyCode,
                                               priceOption.OfferPriceExclTax / defaultCurrency.ExchangeRate,
                                               option,
                                               currencyCode,
                                               priceOption.PriceExclTax,
                                               CurrencySettings.PrimaryStoreCurrencyCode,
                                               priceOption.PriceExclTax / defaultCurrency.ExchangeRate,
                                               priceOption.Stock);
                }
            }
            else
            {
                if (currencyCode == CurrencySettings.PrimaryStoreCurrencyCode)
                {
                    optionName = string.Format(DT_OPTION_NAME_FORMAT_CURRENCY,
                                               currencyCode,
                                               priceOption.PriceExclTax,
                                               option,
                                               priceOption.Stock);
                }
                else
                {
                    optionName = string.Format(DT_OPTION_NAME_FORMAT_CURRENCY_WITH_ORIGINAL,
                                               currencyCode,
                                               priceOption.PriceExclTax,
                                               CurrencySettings.PrimaryStoreCurrencyCode,
                                               priceOption.PriceExclTax / defaultCurrency.ExchangeRate,
                                               option,
                                               priceOption.Stock);
                }
            }

            return optionName;
        }

        #endregion

        private void InvokeNewMessage(string message, bool refresh)
        {
            ActionOccurred?.Invoke(message, refresh);
        }        
    }
}