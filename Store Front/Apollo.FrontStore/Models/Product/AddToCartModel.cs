using System.Collections.Generic;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Product
{
    public class AddToCartModel
    {
        public int ProductId { get; set; }
        public bool DisableBuyButton { get; set; }
        public int EnteredQuantity { get; set; }
        public List<SelectListItem> AllowedQuantities { get; set; }
        public bool Visible { get; set; }

        //pre-order
        public bool AvailableForPreOrder { get; set; }
        //public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

        public AddToCartModel()
        {
            AllowedQuantities = new List<SelectListItem>();
        }
    }
}