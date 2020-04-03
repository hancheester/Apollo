using Apollo.Web.Framework.Mvc;
using System;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductReviewModel : BaseEntityModel
    {
        public string Title { get; set; }
        public string Comment { get; set; }
        public string Alias { get; set; }
        public DateTime TimeStamp { get; set; }
        public int Score { get; set; }
    }
}