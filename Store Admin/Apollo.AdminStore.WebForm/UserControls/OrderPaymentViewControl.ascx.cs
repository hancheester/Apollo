using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Text;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderPaymentViewControl : BaseUserControl, ICallbackEventHandler
    {
        public IOrderService OrderService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        public delegate void PaymentEventHandler(bool verified);
        public delegate void PayByPhoneEventHandler(string paymentRef);

        public event PaymentEventHandler Verified;
        public event PayByPhoneEventHandler PaidByPhone;

        private bool _checkPassed;
        private int _orderId;

        public int OrderId
        {
            set
            {
                _orderId = value;
                int orderId = value;
                hfOrderId.Value = orderId.ToString();

                PaymentHeaderOverviewModel payment = OrderService.GetPaymentHeaderOverviewModelByOrderId(orderId);
                ltlPaymentMethod.Text = GetPaymentMethodIcon(payment.PaymentMethod);

                var invoices = OrderService.GetPaidEmailInvoicesByOrderId(orderId);

                rptEmailInvoice.DataSource = invoices;
                rptEmailInvoice.DataBind();

                if (payment.PaymentMethod == PaymentType.GOOGLE_CHECKOUT)
                    ltlPaymentRef.Text = string.Format("<a href='https://checkout.google.com/sell/multiOrder?order={0}'>{0}</a>", payment.PaymentRef);
                else
                    ltlPaymentRef.Text = payment.PaymentRef;

                if (payment.StatusCode.Equals(OrderStatusCode.PENDING)
                    || payment.StatusCode.Equals(OrderStatusCode.DISCARDED)
                    || payment.StatusCode.Equals(OrderStatusCode.CANCELLED))
                {
                    phPayByPhone.Visible = true;
                    phPaymentDetails.Visible = false;
                }
                else if (payment.StatusCode.Equals(OrderStatusCode.ON_HOLD))
                {
                    ltlThirdScoreNow.Text = OrderService.CheckThirdManResultByOrderId(orderId);
                    phThirdScore.Visible = false;

                    ltlPaymentDetailsNow.Text = OrderService.CheckTransactionDetailByOrderId(orderId);
                    phPaymentSagePayDetail.Visible = false;
                }
                else
                {
                    phPayByPhone.Visible = false;
                    phPaymentDetails.Visible = true;
                }

                var entity = OrderService.GetLastSagePayDirectViewModelByOrderId(orderId);
                StringBuilder sb = new StringBuilder();
                sb = GetSagePayPaymentStatus(entity, sb);
                ltPaymentCheck.Text = sb.ToString();

                var systemCheck = OrderService.GetSystemCheckByOrderId(orderId);
                _checkPassed = systemCheck == null ? true : systemCheck.AvsCheck;
                ltAVS.Text = _checkPassed ? "Passed" : "Failed";
                phAVS.Visible = !_checkPassed;                
            }
            get { return _orderId; }
        }

        private StringBuilder GetSagePayPaymentStatus(SagePayDirectOverviewModel spPaymentEntity, StringBuilder sb)
        {
            if (spPaymentEntity != null)
            {
                sb.Append("<table class='table table-bordered'>");
                sb.Append("<tr>");
                sb = CheckSagePayStatus(sb, spPaymentEntity.AVSCV2, "avscv2");
                sb = CheckSagePayStatus(sb, spPaymentEntity.AddressResult, "address");
                sb = CheckSagePayStatus(sb, spPaymentEntity.PostCodeResult, "postcode");
                sb.Append("</tr>");
                sb.Append("<tr>");                
                sb = CheckSagePayStatus(sb, spPaymentEntity.CV2Result, "cv2");
                sb = CheckSagePayStatus(sb, spPaymentEntity.ThreeDSecureStatus, "3d");
                sb.Append("<td></td><td></td></tr>");
                sb.Append("</table>");
            }
            return sb;
        }

        private string GetPaymentMethodIcon(string method)
        {
            if (string.IsNullOrEmpty(method)) return string.Empty;

            switch (method.ToLower())
            {
                case "mastercard":
                    return "<i class='fa fa-cc-mastercard fa-2x'></i>";
                case "visa":
                    return "<i class='fa fa-cc-visa fa-2x'></i>";                
                default:
                    return method;
            }
        }

        private StringBuilder CheckSagePayStatus(StringBuilder sb, string check, string item)
        {
            string status;

            switch (check)
            {
                case "SECURITY CODE MATCH ONLY": status = "adjust"; break;
                case "ADDRESS MATCH ONLY": status = "adjust"; break;
                case "NO DATA MATCHES": status = "times-circle"; break;
                case "DATA NOT CHECKED": status = "ban"; break;
                case "ALL MATCH": status = "check-circle"; break;
                case "NOTPROVIDED": status = "exclamation-circle"; break;
                case "NOTCHECKED": status = "ban"; break;
                case "MATCHED": status = "check-circle"; break;
                case "NOTMATCHED": status = "times-circle"; break;

                case "OK": status = "check-circle"; break;
                case "NOAUTH": status = "exclamation-circle"; break;
                case "CANTAUTH": status = "ban"; break;
                case "NOTAUTH": status = "times-circle"; break;
                case "ATTEMPTONLY": status = "question-circle"; break;
                case "INCOMPLETE": status = "question-circle"; break;
                case "MALFORMED": status = "question-circle"; break;
                case "INVALID": status = "question-circle"; break;
                case "ERROR": status = "question-circle"; break;

                default:
                    if (string.IsNullOrWhiteSpace(check)) check = "no data found";
                    status = "question-circle"; break;
            }

            sb.AppendFormat("<td>{0}</td><td><i class='fa fa-{1}' title='{2}'></i></td>", item, status, check);
            return sb;
        }

        public void lbPayByPhone_Click(object sender, EventArgs e)
        {
            int orderId = Convert.ToInt32(hfOrderId.Value);
            string paymentRef = txtPaymentRef.Text.Trim();

            OrderService.ProcessOrderForPayByPhone(orderId, paymentRef);

            phPayByPhone.Visible = false;
            phPaymentDetails.Visible = true;

            ltlPaymentMethod.Text = PaymentType.PAID_BY_PHONE;
            ltlPaymentRef.Text = txtPaymentRef.Text;

            InvokePaidByPhone(paymentRef);
        }

        protected bool GetAvsCheckStatus()
        {
            return _checkPassed;
        }

        protected void lbVerifyAVS_Click(object sender, EventArgs e)
        {
            InvokeVerified(true);
        }

        private void InvokeVerified(bool verified)
        {
            PaymentEventHandler handler = Verified;
            if (handler != null)
                handler(verified);
        }

        private void InvokePaidByPhone(string paymentRef)
        {
            PayByPhoneEventHandler handler = PaidByPhone;
            if (handler != null)
                handler(paymentRef);
        }

        #region ICallbackEventHandler Members

        private string _message = string.Empty;

        public string GetCallbackResult()
        {
            return _message;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] args = eventArgument.Split(new string[] { "_" }, StringSplitOptions.None);

            switch (args[1])
            {
                case "3rd":
                    _message = OrderService.CheckThirdManResultByOrderId(Convert.ToInt32(args[0]));
                    break;
                case "details":
                default:
                    _message = OrderService.CheckTransactionDetailByOrderId(Convert.ToInt32(args[0]));
                    break;
            }
        }

        #endregion
    }
}