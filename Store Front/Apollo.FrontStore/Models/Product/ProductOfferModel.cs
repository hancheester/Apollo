using System;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductOfferModel
    {
        public bool ShowCountDown { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Visible { get; set; }
    }
}