using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Text;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_payment_default : BasePage, ICallbackEventHandler
    {
        public IOrderService OrderService { get; set; }

        protected override void OnInit(EventArgs e)
        {
            if (QueryOrderId <= 0)
                Response.Redirect("/order_default.aspx");
            else
            {
                ltlTitle.Text = string.Format("<h3>Order # {0}</h3>", QueryOrderId.ToString());
                LoadTransactionLogs();
            }

            base.OnInit(e);
        }

        private void LoadTransactionLogs()
        {
            rptLogs.DataSource = OrderService.GetSagePayLogsByOrderId(QueryOrderId);
            rptLogs.DataBind();
        }

        #region ICallbackEventHandler Members

        private string message;
        private const char splitter = '_';

        public string GetCallbackResult()
        {
            return message;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            string[] args = eventArgument.Split(splitter);

            if (args[0] == "sp")
            {
                // sp, tagid, sagepaylogid
                SagePayLog spLog = OrderService.GetSagePayLogById(Convert.ToInt32(args[1]));

                if (spLog != null)
                {
                    message = spLog.NameValue;

                    int charsPerLine = 120;
                    StringBuilder sb = new StringBuilder();

                    while (message.Length / charsPerLine > 0)
                    {
                        sb.AppendFormat("{0}<br/>", message.Substring(0, message.Length > charsPerLine ? charsPerLine : message.Length));
                        message = message.Remove(0, message.Length > charsPerLine ? charsPerLine : message.Length);
                    }

                    sb.AppendFormat("{0}<br/>", message);

                    message = sb.ToString();
                }
            }
        }

        #endregion
    }
}