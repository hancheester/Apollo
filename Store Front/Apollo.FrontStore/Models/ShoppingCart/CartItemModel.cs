using System.Collections.Generic;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.ShoppingCart
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public string Name { get; set; }
        public string ThumbnailFilename { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string SavePercentageNote { get; set; }
        public decimal OldPrice { get; set; }
        public string Option { get; set; }
        public string UrlKey { get; set; }
        public int Quantity { get; set; }
        public bool IsEditable { get; set; }        
        public List<SelectListItem> AllowedQuantities { get; set; }

        public CartItemModel()
        {
            AllowedQuantities = new List<SelectListItem>();
        }
    }
}