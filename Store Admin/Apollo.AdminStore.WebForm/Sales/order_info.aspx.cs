using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_info : BasePage, ICallbackEventHandler
    {
        public IOrderService OrderService { get; set; }
        
        protected override void OnInit(EventArgs e)
        {
            int orderId = QueryOrderId;

            var order = OrderService.GetOrderOverviewModelById(orderId);

            if (order != null)
                ltlTitle.Text = string.Format("<h3 class='printHide'>Order # {0}</h3>", order.Id.ToString());
            else
                Response.Redirect("/sales/order_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderNotFound);

            base.OnInit(e);
        }
        
        protected void eonNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
            if (refresh) LoadInfo();
        }

        protected void eoiView_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
            if (refresh) LoadInfo();
        }

        #region ICallbackEventHandler Members

        private string _message;

        public string GetCallbackResult()
        {
            return _message;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            const string ERROR = "Error";

            string[] args = eventArgument.Split(new char[] { '_' });

            if (args.Length > 1)
            {
                switch (args[0])
                {
                    case "stock":
                        try
                        {
                            var branchId = Convert.ToInt32(args[1]);
                            _message = string.Empty;

                            if (_message == string.Empty)
                                _message = "n/a";
                        }
                        catch
                        {
                            _message = ERROR;
                        }

                        break;
                }
            }
        }

        #endregion  

        private void LoadInfo()
        {
            eoiView.LoadOrderInfo();
        }
    }
}