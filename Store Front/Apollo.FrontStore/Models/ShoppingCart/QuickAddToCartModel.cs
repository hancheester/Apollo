using Apollo.Core.Model.OverviewModel;
using Apollo.FrontStore.Models.Product;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.ShoppingCart
{
    public class QuickAddToCartModel
    {
        public IList<ProductPriceModel> Options { get; set; }
        public ProductOverviewModel Product { get; set; }

        public QuickAddToCartModel()
        {
            Options = new List<ProductPriceModel>();
        }
    }
}