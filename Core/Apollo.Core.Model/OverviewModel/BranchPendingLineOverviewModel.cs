using System;
using System.Collections.Generic;

namespace Apollo.Core.Model.OverviewModel
{
    public class BranchPendingLineOverviewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }
        public string Barcode { get; set; }        
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public string BranchName { get; set; }
        public IList<int> RelatedOrders { get; set; }
        public string UrlRewrite { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public string PriceCode { get; set; }
        public decimal Price { get; set; }
        public int BranchItemStatusId { get; set; }
        public int BranchId { get; set; }
        public string Comment { get; set; }
    }
}
