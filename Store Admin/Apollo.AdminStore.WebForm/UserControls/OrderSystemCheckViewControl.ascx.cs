using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Text;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class OrderSystemCheckViewControl : BaseUserControl, ICallbackEventHandler
    {
        public IOrderService OrderService { get; set; }
        
        public int OrderId { get; private set; }
        
        public OrderOverviewModel Order
        {
            set
            {
                OrderId = value.Id;
                phTrigger.Visible = value.StatusCode == OrderStatusCode.ON_HOLD;                
            }
        }

        #region ICallbackEventHandler Members

        private string _message = string.Empty;

        public void RaiseCallbackEvent(string eventArgument)
        {
            var scores = OrderService.CalculateSystemCheckScore(Convert.ToInt32(eventArgument));
            if (scores == null || scores.Count == 0) _message = "<table class=\"table\"><tr><th>Total Score (Lower is better)</th><td>0</td></tr></table>";

            var sb = new StringBuilder("<table class=\"table\">");
            foreach (var item in scores)
            {
                sb.AppendFormat("<tr><th>{0}</th><td>{1}</td></tr>", item.Key, item.Value);
            }
            sb.Append("</table>");

            _message = sb.ToString();            
        }

        public string GetCallbackResult()
        {
            return _message;
        }

        #endregion  
    }
}