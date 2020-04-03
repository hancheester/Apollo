using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class OrderPrevNextControl : BaseUserControl
    {
        public IOrderService OrderService { get; set; }

        protected int QueryOrderId
        {
            get { return ((BasePage)Page).QueryOrderId; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var prev_next = OrderService.GetPrevNextOrderId(QueryOrderId);

                if (prev_next[0] > 0)
                    hlPrev.NavigateUrl = "/sales/order_info.aspx?orderid=" + prev_next[0].ToString();
                else
                    hlPrev.Enabled = false;

                if (prev_next[1] > 0)
                    hlNext.NavigateUrl = "/sales/order_info.aspx?orderid=" + prev_next[1].ToString();
                else
                    hlNext.Visible = false;
            }
        }

        protected void lbGo_Click(object sender, EventArgs e)
        {
            Response.Redirect("/sales/order_info.aspx?orderid=" + txtGoOrderId.Text.Trim());
        }
    }
}