using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_email_payment_default : BasePage
    {
        public IOrderService OrderService { get; set; }
        public IUtilityService UtilityService { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadInfo(QueryOrderId);
        }

        protected void eonNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;

            if (refresh)
                LoadInfo(QueryOrderId);
        }
        
        protected void ddlCurrency_OnInit(object sender, EventArgs e)
        {
            ddlCurrency.DataSource = UtilityService.GetAllCurrency();
            ddlCurrency.DataBind();

            string currencyCode = OrderService.GetCurrencyCodeByOrderId(QueryOrderId);

            if (currencyCode != null)
            {
                ddlCurrency.Items.FindByValue(currencyCode).Selected = true;
            }
        }

        protected void lbSend_Click(object sender, EventArgs e)
        {
            int orderId = QueryOrderId;

            string currencyCode = ddlCurrency.SelectedValue;
            decimal amount = Convert.ToDecimal(txtAmount.Text.Trim());
            string endDate = txtEndDate.Text.Trim();

            OrderService.SendEmailInvoiceRequest(
                orderId,
                txtName.Text.Trim(),
                txtEmail.Text.Trim(),
                txtMessage.Text.Trim(),
                currencyCode,
                txtContactNumber.Text.Trim(),
                amount,
                endDate);

            // Write comment
            OrderService.ProcessOrderCommentInsertion(orderId,
                                                    Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId")),
                                                    Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName")),
                                                    "Send Payment Email",
                                                    "Email was sent successfully.<br/><br/>Amount:<br/>" + currencyCode + " " + amount + "<br/><br/>End Date:<br/>" + endDate,
                                                    string.Empty);
            
            // Display message and refresh
            enbNotice.Message = "Email was sent successfully.";
        }

        private void LoadInfo(int orderId)
        {
            var accountView = OrderService.GetAccountOverviewModelByOrderId(orderId);

            ltlTitle.Text = string.Format("<h3>Order # {0}</h3>", orderId.ToString());

            txtName.Text = accountView.Name;
            txtEmail.Text = accountView.Email;
            txtContactNumber.Text = accountView.ContactNumber;
        }
    }
}