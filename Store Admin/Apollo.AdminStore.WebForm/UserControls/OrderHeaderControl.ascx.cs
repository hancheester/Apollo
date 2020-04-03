using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderHeaderControl : BaseUserControl, ICallbackEventHandler
    {
        public IOrderService OrderService { get; set; }

        private bool _hidePrint;
        private int _orderId;

        public bool HidePrint
        {
            set { _hidePrint = value; }
        }

        public int OrderId
        {
            set
            {
                _orderId = value;
                OrderHeaderOverviewModel header = OrderService.GetOrderHeaderOverviewModelByOrderId(value);

                ltlOrderTitle.Text = string.Format("Order # {0}", value.ToString());
                ltlOrderDate.Text = header.OrderPlaced.Value.ToLongDateString() + ", " + header.OrderPlaced.Value.ToLongTimeString();
                ltlStatus.Text = OrderService.GetOrderStatusByCode(header.StatusCode);
                
                if (header.IssueCode != string.Empty)
                    ltlIssue.Text = OrderService.GetOrderIssueByCode(header.IssueCode);
                else
                    ltlIssue.Text = string.Empty;
            }
            get { return _orderId; }
        }

        public bool ShowExtraInfo
        {
            set { phExtraInfo.Visible = value; }
        }

        protected string PrintHideFlag
        {
            get
            {
                if (_hidePrint) return "printHide"; else return string.Empty;
            }
        }

        #region ICallbackEventHandler Members

        private string _message = string.Empty;
        private const char splitter = '_';

        public string GetCallbackResult()
        {
            return _message;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            _message = string.Empty;

            string[] args = eventArgument.Split(splitter);

            string type = args[0];

            switch (type)
            {
                case "paymentstatus":
                    _message = OrderService.GetLatestStatusFromLog(Convert.ToInt32(args[1]));
                    break;

                case "iplocation":
                    _message = OrderService.CheckIPLocationByOrderId(Convert.ToInt32(args[1]));
                    break;
            }
        }

        #endregion
    }
}