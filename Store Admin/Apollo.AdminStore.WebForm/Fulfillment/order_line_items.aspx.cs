using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.FulFillment
{
    public partial class order_line_items : BasePage, ICallbackEventHandler
    {
        public IOrderService OrderService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadList();                
            }

            ClientScriptManager cm = Page.ClientScript;

            string cbReference = cm.GetCallbackEventReference(this, "'nextday'", "receiveAlert", string.Empty);
            string callbackScript = "function checkAlert(arg, context) {" + cbReference + "; }";

            cm.RegisterClientScriptBlock(this.GetType(), "checkAlert", callbackScript, true);
        }
        
        protected string GetProperDateString(object dateString)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dt = DateTime.ParseExact(dateString.ToString(), "yyyy-MM-dd", provider);

            try
            {
                return dt.ToString("dd/MM/yyyy");
            }
            finally
            {
                provider = null;
            }
        }

        protected void lbPrintNote_Click(object sender, EventArgs e)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<DateTime> fromDates = new List<DateTime>();

            // Get all the chosen dates
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                CheckBox cb = rptList.Items[i].FindControl("cbChosen") as CheckBox;

                if (cb != null && cb.Checked)
                {
                    Literal ltl = rptList.Items[i].FindControl("ltlDate") as Literal;
                    DateTime dtStart = DateTime.ParseExact(ltl.Text, AppConstant.DATE_FORM1, provider);

                    fromDates.Add(dtStart);
                }
            }

            // Display next screen if date found
            if (fromDates.Count > 0)
            {
                var profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
                var fullName = Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName"));

                // Process pending line items
                OrderService.ProcessPendingLineItems(profileId, fullName, fromDates.ToArray());

                // Clear session for pick in progress lines (used in order_line_item_pick_in_progress.aspx.cs)
                SessionFacade.PickingLineItems = null;

                Response.Redirect("/fulfillment/order_line_item_pick_in_progress.aspx");
            }
            else
                enbInfo.Message = "Sorry, there were no selection.";
        }

        private void LoadList()
        {
            var inLineStatusXml = AdminStoreUtility.BuildXmlString("status", new string[] { LineStatusCode.ORDERED, LineStatusCode.PENDING });
            var inOrderStatusXml = AdminStoreUtility.BuildXmlString("status", new string[] { OrderStatusCode.ORDER_PLACED, OrderStatusCode.PARTIAL_SHIPPING });
            rptList.DataSource = OrderService.GetOrderCountGroupByDate(inLineStatusXml, inOrderStatusXml);
            rptList.DataBind();
        }

        #region ICallbackEventHandler Members

        private string _alert = string.Empty;

        string ICallbackEventHandler.GetCallbackResult()
        {
            return _alert;
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            switch (eventArgument)
            {
                case "nextday":
                    int count = 0;
                    count = OrderService.GetOrderCountForSpecialDelivery();

                    if (count > 0)
                        _alert = "Alert! Next day delivery order received.";
                    else
                        _alert = string.Empty;

                    break;
            }
        }

        #endregion
    }
}