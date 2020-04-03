using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_shipment_info : BasePage
    {
        public IOrderService OrderService { get; set; }

        protected int OrderId
        {
            get { return OrderService.GetOrderShipmentById(QueryOrderShipmentId).OrderId; }
        }

        protected override void OnInit(EventArgs e)
        {
            OrderShipment shipment = OrderService.GetOrderShipmentById(QueryOrderShipmentId);
            OrderOverviewModel orderView = null;
            if (shipment != null) orderView = OrderService.GetOrderOverviewModelById(shipment.OrderId);

            if (shipment != null && orderView != null)
            {
                ltlTitle.Text = string.Format("<h3>Order # {0}</h3>", orderView.Id.ToString());
            }
            else
                Response.Redirect("/sales/order_shipment_default.aspx?orderid=" + shipment.OrderId.ToString() + "&" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderShipmentNotFound);

            base.OnInit(e);
        }
        
        protected void eosShipment_ActionOccurred(string message, bool refresh)
        {
            SetMessage(message);
            if (refresh) LoadInfo();
        }

        private void LoadInfo()
        {
            eosShipment.LoadShipmentInfo();
        }

        private void SetMessage(string message)
        {
            enbNotice.Message = message;
        }
    }
}