using Apollo.Web.Framework.Mvc;
using System;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Order
{
    public class CustomerOrderListModel : BaseEntityModel
    {
        public IList<OrderSummaryModel> Orders { get; set; }
        public CustomerOrderPagingFilteringModel PagingFilteringContext { get; set; }

        public CustomerOrderListModel()
        {
            Orders = new List<OrderSummaryModel>();
            PagingFilteringContext = new CustomerOrderPagingFilteringModel();
        }
    }

    #region Nested classes

    public class OrderSummaryModel : BaseEntityModel
    {
        public string OrderTotal { get; set; }
        //public bool IsReturnRequestAllowed { get; set; }
        public string OrderStatusCode { get; set; }
        public string OrderStatus { get; set; }
        //public string PaymentStatus { get; set; }
        //public string ShippingStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool Chosen { get; set; }
    }

    #endregion
}