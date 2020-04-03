using System;

namespace Apollo.Core.Model.OverviewModel
{
    public class OrderHeaderOverviewModel
    {
        public DateTime? OrderPlaced { get; set; }
        public string StatusCode { get; set; }
        public string IssueCode { get; set; }
        public int InvoiceNumber { get; set; }
    }
}
