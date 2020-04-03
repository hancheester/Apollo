using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderShipmentInfoControl : BaseUserControl
    {
        public IOrderService OrderService { get; set; }

        public delegate void ShipmentInfoEventHandler(string message, bool refresh);

        public event ShipmentInfoEventHandler ActionOccurred;

        private int QueryOrderShipmentId
        {
            get { return ((BasePage)Page).QueryOrderShipmentId; }
        }

        protected override void OnInit(EventArgs e)
        {
            LoadShipmentInfo();
            base.OnInit(e);
        }

        protected void lbEditItem_Click(object sender, EventArgs e)
        {
            phEditShipping.Visible = true;
            phShipping.Visible = false;
            var shipment = OrderService.GetOrderShipmentById(QueryOrderShipmentId);

            ddlCarrier.SelectedIndex = -1;
            ListItem found = ddlCarrier.Items.FindByValue(shipment.Carrier);
            if (found != null) found.Selected = true;

            txtTrackingReference.Text = shipment.TrackingRef;
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var shipment = OrderService.GetOrderShipmentById(QueryOrderShipmentId);

            shipment.Carrier = ddlCarrier.SelectedValue;
            shipment.TrackingRef = txtTrackingReference.Text.Trim();

            OrderService.UpdateOrderShipment(shipment);

            phEditShipping.Visible = false;
            phShipping.Visible = true;
            LoadShipmentInfo();
            InvokeNewMessage("Shipment was updated successfully.", true);
        }

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            phEditShipping.Visible = false;
            phShipping.Visible = true;
        }

        private void InvokeNewMessage(string message, bool refresh)
        {
            ShipmentInfoEventHandler handler = ActionOccurred;
            if (handler != null)
                handler(message, refresh);
        }

        public void LoadShipmentInfo()
        {
            var shipment = OrderService.GetOrderShipmentById(QueryOrderShipmentId);

            if (shipment != null)
            {
                var orderView = OrderService.GetOrderOverviewModelById(shipment.OrderId);
                eohHeader.OrderId = orderView.Id;
                eavAccount.ProfileId = orderView.ProfileId;
                eavAccount.OrderId = shipment.OrderId;
                eavShipping.OrderId = shipment.OrderId;

                ltlCarrier.Text = shipment.Carrier;
                ltlTrackingRef.Text = shipment.TrackingRef;

                rptItems.DataSource = shipment.ItemShipmentList;
                rptItems.DataBind();
            }            
        }
    }
}
