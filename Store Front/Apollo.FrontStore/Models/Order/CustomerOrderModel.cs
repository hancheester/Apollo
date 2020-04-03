using Apollo.FrontStore.Models.Common;
using Apollo.Web.Framework.Mvc;
using System;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Order
{
    public class CustomerOrderModel : BaseEntityModel
    {
        public string OrderTotal { get; set; }
        public string OrderStatusCode { get; set; }
        public string OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }        
        public IList<LineItemModel> Items { get; set; }
        public CustomerOrderTotalsModel Totals { get; set; }
        public AddressModel Billing { get; set; }
        public AddressModel Shipping { get; set; }
        public bool IsAllowedToDownloadInvoice { get; set; }

        public CustomerOrderModel()
        {
            Items = new List<LineItemModel>();
            Totals = new CustomerOrderTotalsModel();
        }
    }

    #region Nested classes

    public class LineItemModel : BaseEntityModel
    {
        public string Name { get; set; }
        public string Option { get; set; }
        public int Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string Subtotal { get; set; }        
    }

    public class CustomerOrderTotalsModel
    {
        public string Subtotal { get; set; }
        public string Discount { get; set; }
        public string UsedPointsAmount { get; set; }
        public int UsedPoints { get; set; }
        public string ShippingCost { get; set; }
        public string VAT { get; set; }
        public string VATMessage { get; set; }

        public CustomerOrderTotalsModel()
        {
            VATMessage = "included";
        }
    }

    #endregion
}